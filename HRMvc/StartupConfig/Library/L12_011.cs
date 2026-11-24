using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._11_AMS;
using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.OPay;
using HRApiLibrary.Modules._11003O;
using HRApiLibrary.Modules._13003O;

namespace HRMvc.StartupConfig.Library;

public class AttendanceProcessResult
{
    public Dictionary<int, List<AttDailyRow>> DailyByEmp { get; set; } = [];
    public List<AttSummaryRow> Summary { get; set; } = [];
    public bool NdMissing { get; set; }
    public Dictionary<int, AtttemplateModel> Templates { get; set; } = [];
    public AtttemplateModel? DefaultTemplate { get; set; }

    public string ScheduledDuty(int systemId, DateTime d)
    {
        var t = Templates.TryGetValue(systemId, out var x) ? x : DefaultTemplate;
        return t is null ? string.Empty : AttendanceProcessor.DutyOf(t, d);
    }
}

public class AttendanceProcessor
{
    private readonly IAttpunches1DataAccess     _punchDa;
    private readonly IOHolidayDataAccess        _holidayDa;
    private readonly IOtreqdtlDataAccess        _otDtlDa;
    private readonly ISettingsDataAccess        _settingsDa;
    private readonly IAms_otsettingsDataAccess  _otSetDa;
    private readonly IAtttemplateDataAccess     _tplDa;
    private readonly IDA_11003O                 _daO;

    public AttendanceProcessor(IAttpunches1DataAccess punchDa, IOHolidayDataAccess holidayDa, IOtreqdtlDataAccess otDtlDa, ISettingsDataAccess settingsDa, IAms_otsettingsDataAccess otSetDa, IAtttemplateDataAccess tplDa, IDA_11003O daO)
    {
        _punchDa = punchDa; _holidayDa = holidayDa; _otDtlDa = otDtlDa; _settingsDa = settingsDa; _otSetDa = otSetDa; _tplDa = tplDa; _daO = daO;
    }

    // -- The rules of AMS 12011 Attendance Processing; every attendance screen and report uses this one --
    public async Task<AttendanceProcessResult> Process(UserClaimsModel Uc, int payrollGrpId, List<AttEmpRow> emps, DateTime start, DateTime end)
    {
        var result      = new AttendanceProcessResult();
        var _dailyByEmp = result.DailyByEmp;
        var _summary    = result.Summary;
        var _start      = start;
        var _end        = end;
        var _emps       = emps;

        var dstart  = DateOnly.FromDateTime(_start);
        var dend    = DateOnly.FromDateTime(_end);
        var holidays = await LoadHolidays(Uc, payrollGrpId, _start, _end);
        var pay      = await _settingsDa._02(1, Uc.SchemaUserPay, Uc.Conn);
        var nd       = pay is null || pay.NdStart is null || pay.NdEnd is null ? ((int, int)?)null : (pay.NdStart.Value, pay.NdEnd.Value);
        result.NdMissing = nd is null;
        var otSettings = (await _otSetDa._02sByPayrollgrpid(payrollGrpId, Uc.SchemaUserPis!, Uc.OpisDb!, Uc.Conn!) ?? [])
                            .Where(x => x is not null)
                            .GroupBy(x => x!.Empnumber.Trim())
                            .ToDictionary(g => g.Key, g => g.First()!);

            var ids       = _emps.Where(x => x.SystemId > 0).Select(x => x.SystemId).Distinct().ToList();
            var allPunch  = (await _punchDa._02ByIdsByRange(ids, dstart.AddDays(-14), dend, Uc.SchemaUserPis, Uc.Conn) ?? [])
                                .Where(p => p is not null).GroupBy(p => p!.EmpmasId ?? 0).ToDictionary(g => g.Key, g => g.ToList());

            var needIds   = _emps.Where(x => x.SystemId > 0 && otSettings.TryGetValue(x.EmpNumber.Trim(), out var nn) && nn.NeedsotfilingBool).Select(x => x.SystemId).Distinct().ToList();
            var allFiled  = (await _otDtlDa._02ApprovedByEmpmasIds_RangeByPunchIn(needIds, _start, _end, Uc.SchemaUserPis, Uc.Conn) ?? [])
                                .Where(o => o is not null).GroupBy(o => o!.EmpmasId).ToDictionary(g => g.Key, g => g.ToList());
        var tpls       = (await _tplDa._02ByEmpmasIds(ids, Uc.SchemaUserPis, Uc.Conn) ?? []).Where(t => t is not null).ToDictionary(t => t!.EmpmasId ?? 0, t => t!);
        var defaultTpl = await _tplDa._02NoSchedule(0, Uc.SchemaUserPis, Uc.Conn);
        result.Templates = tpls; result.DefaultTemplate = defaultTpl;

        foreach (var e in _emps)
            {
                var rows = new List<AttDailyRow>();
                var empTpl = e.SystemId > 0 && tpls.TryGetValue(e.SystemId, out var et) ? et : defaultTpl;
                Func<DateTime, bool> isRdDay = d => empTpl is not null && DutyOf(empTpl, d) == "RD";
                if (e.SystemId > 0 && allPunch.TryGetValue(e.SystemId, out var punches))
                {
                    otSettings.TryGetValue(e.EmpNumber.Trim(), out var n);
                    bool needsOt = n?.NeedsotfilingBool ?? false;
                    double maxOt = n?.Maxothour ?? 0;
                    var filed = new Dictionary<DateTime, double>();
                    if (needsOt && allFiled.TryGetValue(e.SystemId, out var ots))
                        filed = ots.GroupBy(o => o!.PunchIn.Date).ToDictionary(g => g.Key, g => g.Sum(o => o!.TotHrs));
                    rows = punches.Select(p => ToDaily(p!, holidays, needsOt, maxOt, filed, nd, isRdDay)).ToList();
                }
                if (e.SystemId > 0)
                {
                    var tpl = tpls.TryGetValue(e.SystemId, out var t) ? t : defaultTpl;
                    if (tpl is not null)
                    {
                        var seen = rows.Select(r => r.Date).ToHashSet();
                        var last = _end.Date < DateTime.Today ? _end.Date : DateTime.Today;
                        for (var d = _start.Date; d <= last; d = d.AddDays(1))
                        {
                            if (seen.Contains(d) || holidays.ContainsKey(d) || DutyOf(tpl, d) != "R") continue;
                            rows.Add(new AttDailyRow
                            {
                                Date = d, Day = d.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture), DayType = "R", Kind = "AB",
                                Remarks = "Absent", Credit = 0, AbsentHrs = OPayrollConstants.HoursPerDay
                            });
                        }
                        var byDate = rows.GroupBy(r => r.Date).ToDictionary(g => g.Key, g => g.First());
                        bool PresentBefore(DateTime d)
                        {
                            for (var p = d.AddDays(-1); p >= d.AddDays(-14); p = p.AddDays(-1))
                            {
                                if (byDate.TryGetValue(p, out var pr)) return pr.Kind != "AB";
                                if (holidays.TryGetValue(p, out var ph)) { if (ph == "SH") continue; return false; }
                                if (DutyOf(tpl, p) == "RD") continue;
                                return false;
                            }
                            return false;
                        }
                        for (var d = _start.Date; d <= last; d = d.AddDays(1))
                        {
                            if (!holidays.TryGetValue(d, out var hc) || (hc != "LH" && hc != "DH")) continue;
                            if (byDate.ContainsKey(d) || DutyOf(tpl, d) == "RD") continue;
                            if (!PresentBefore(d))
                            {
                                var ab = new AttDailyRow
                                {
                                    Date = d, Day = d.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture), DayType = "R", Kind = "AB",
                                    Remarks = "Absent (no holiday pay)", Credit = 0, AbsentHrs = OPayrollConstants.HoursPerDay
                                };
                                rows.Add(ab);
                                byDate[d] = ab;
                                continue;
                            }
                            var hp = new AttDailyRow
                            {
                                Date = d, Day = d.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture), DayType = "R",
                                Holiday = hc, Remarks = "Holiday pay (not worked)", Credit = 1
                            };
                            rows.Add(hp);
                            byDate[d] = hp;
                        }
                        rows = rows.OrderBy(r => r.Date).ToList();
                    }
                    rows = rows.Where(r => r.Date >= _start.Date).ToList();
                }
                _dailyByEmp[e.SystemId] = rows;
                _summary.Add(ToSummary(e, rows));
            }

        return result;
    }


    public async Task<List<M13003_Emp>> Employees(UserClaimsModel uc, int groupId)
    {
        var list = await _daO._02Deprec_ByFieldId("PayrollgrpId", groupId, uc.SchemaUserPis, uc.OpisDb, uc.Conn) ?? [];
        return list.Select(x => new M13003_Emp { SystemId = x.EmpmasId, EmpNumber = (x.EmpNumber ?? string.Empty).Trim(), EmpName = (x.EmpName ?? string.Empty).Trim() }).ToList();
    }

    public static List<AttEmpRow> ToEmpRows(IEnumerable<M13003_Emp> emps) =>
        emps.Select(x => new AttEmpRow { SystemId = x.SystemId, EmpNumber = x.EmpNumber, EmpName = x.EmpName }).ToList();

    // -- Late and Absent Summary : totals of the 12011 daily rows of each employee --
    public async Task<List<M13003_Sum>> LateAbsent(UserClaimsModel uc, int groupId, string groupName, IEnumerable<M13003_Emp> emps, DateTime from, DateTime to)
    {
        var list = emps.ToList();
        var res  = await Process(uc, groupId, ToEmpRows(list), from, to);
        var sums = new List<M13003_Sum>();
        foreach (var e in list)
        {
            var rows = res.DailyByEmp.TryGetValue(e.SystemId, out var r) ? r : [];
            sums.Add(new M13003_Sum
            {
                EmpNumber = e.EmpNumber, EmpName = e.EmpName, Client = groupName,
                ScheduledDays = rows.Count(x => x.DayType != "RD"),
                DaysPresent   = rows.Count(x => x.Kind == "" && x.TimeIn != "" && x.DayType != "RD"),
                DaysAbsent    = (int)Math.Round(rows.Sum(x => x.AbsDays)),
                DaysLate      = rows.Count(x => x.Tardiness > 0),
                LateMinutes   = (int)rows.Sum(x => x.Tardiness),
                DaysLeaveOb   = rows.Count(x => x.Kind == "OB" || x.Kind == "LV"),
                DaysRestWork  = rows.Count(x => x.DayType == "RD" && x.TimeIn != "")
            });
        }
        return sums;
    }

    // -- Daily Attendance : the 12011 daily row of each employee on one date --
    public async Task<List<M13004_Row>> Daily(UserClaimsModel uc, int groupId, string groupName, IEnumerable<M13003_Emp> emps, DateTime date)
    {
        var list = emps.ToList();
        var res  = await Process(uc, groupId, ToEmpRows(list), date, date);
        var outp = new List<M13004_Row>();
        DateTime? At(string s) => s != string.Empty && TimeSpan.TryParse(s, out var t) ? date.Date + t : null;
        foreach (var e in list)
        {
            var row = (res.DailyByEmp.TryGetValue(e.SystemId, out var r) ? r : []).FirstOrDefault(x => x.Date == date.Date);
            string status; DateTime? tin = null, tout = null; string sched = string.Empty; int late = 0;
            if (row is null)
                status = res.ScheduledDuty(e.SystemId, date) == "RD" ? "Rest Day" : "No record";
            else
            {
                tin = At(row.TimeIn); tout = At(row.TimeOut); late = (int)row.Tardiness;
                sched = row.DayType == "RD" ? string.Empty : row.DutyStart;
                status = row.Kind switch
                {
                    "AB" => "Absent",
                    "OB" => "OB",
                    "LV" => "On Leave",
                    _    => row.TimeIn == string.Empty ? (row.Holiday != string.Empty ? "Holiday" : "No record") : late > 0 ? "Late" : "Present"
                };
            }
            outp.Add(new M13004_Row { EmpNumber = e.EmpNumber, EmpName = e.EmpName, Client = groupName, Status = status, TimeIn = tin, TimeOut = tout, SchedIn = sched, LateMinutes = late });
        }
        return outp;
    }

    public static List<(DateTime Date, DateTime Start, DateTime End)> SplitByMidnight(DateTime s, DateTime e)
    {
        var list = new List<(DateTime, DateTime, DateTime)>();
        var cur = s;
        while (cur < e)
        {
            var next = cur.Date.AddDays(1);
            var end  = next < e ? next : e;
            list.Add((cur.Date, cur, end));
            cur = end;
        }
        return list;
    }

    public static AttDailyRow ToDaily(Attpunches1Model p, Dictionary<DateTime, string> holidays, bool needsOt, double maxOt, Dictionary<DateTime, double> filed, (int Start, int End)? nd, Func<DateTime, bool> isRdDay)
    {
        var date      = (p.PunchInDate ?? DateTime.MinValue).Date;
        int sched     = p.PunchT ?? 0;
        var dutyStart = date.AddHours(sched / 100).AddMinutes(sched % 100);
        var dutyEnd   = dutyStart.AddMinutes((p.SchedDuration ?? 0) / 100 * 60 + (p.SchedDuration ?? 0) % 100);

        var st      = (p.Status ?? string.Empty).Trim().ToUpper();
        var kind    = st.StartsWith("OB") ? "OB" : st.StartsWith("LV") ? "LV" : string.Empty;
        var half    = st.EndsWith("1") ? " - 1st Half" : st.EndsWith("2") ? " - 2nd Half" : string.Empty;
        double tard = kind == "" && p.PunchInDate is not null && p.PunchInDate > dutyStart ? Math.Floor((p.PunchInDate.Value - dutyStart).TotalMinutes) : 0;
        double ot   = kind == "" && p.PunchOutDate is not null && p.PunchOutDate > dutyEnd ? Math.Round((p.PunchOutDate.Value - dutyEnd).TotalHours, 2) : 0;
        string otNote = string.Empty;
        if (needsOt)
        {
            double fh = kind == "" && filed.TryGetValue(date, out var f) ? f : 0;
            if (fh > ot) otNote = $"Filed OT {fh:N2} > actual {ot:N2} (capped)";
            ot = Math.Min(fh, ot);
        }
        if (maxOt > 0 && ot > maxOt)
        {
            otNote = (otNote == string.Empty ? string.Empty : otNote + "; ") + $"OT capped at {maxOt:N2} (limit)";
            ot = maxOt;
        }

        double ndReg = 0, ndOt = 0, regHrs = 0, under = 0;
        var segs = new Dictionary<DateTime, AttSeg>();
        AttSeg Seg(DateTime d)
        {
            if (!segs.TryGetValue(d, out var sg))
                segs[d] = sg = new AttSeg
                {
                    Date = d, Holiday = holidays.TryGetValue(d, out var hh) ? hh : string.Empty,
                    IsRd = (d == date && p.DutyTypeId == 2) || isRdDay(d)
                };
            return sg;
        }

        if (kind == "" && p.PunchInDate is not null && p.PunchOutDate is not null)
        {
            var ws  = p.PunchInDate.Value > dutyStart ? p.PunchInDate.Value : dutyStart;
            var we0 = p.PunchOutDate.Value < dutyEnd ? p.PunchOutDate.Value : dutyEnd;
            if (p.PunchOutDate.Value < dutyEnd) under = Math.Floor((dutyEnd - p.PunchOutDate.Value).TotalMinutes);

            // break : the on-site duty above the 8 working hours; it is taken out of the longest part of the duty, not out of the night differential
            double breakHrs = Math.Max(0, (dutyEnd - dutyStart).TotalHours - OPayrollConstants.HoursPerDay);
            var parts = SplitByMidnight(ws, we0).Select(x => (x.Date, Hours: (x.End - x.Start).TotalHours, Nd: nd is null ? 0 : NdOverlap(x.Start, x.End, nd.Value.Start, nd.Value.End))).ToList();
            if (parts.Count > 0 && breakHrs > 0)
            {
                int longest = 0;
                for (int i = 1; i < parts.Count; i++) if (parts[i].Hours > parts[longest].Hours) longest = i;
                parts[longest] = (parts[longest].Date, Math.Max(0, parts[longest].Hours - breakHrs), parts[longest].Nd);
            }
            foreach (var x in parts)
            {
                var sg = Seg(x.Date);
                sg.Hours += x.Hours;
                sg.Nd    += x.Nd;
            }
            regHrs = Math.Round(Math.Min(parts.Sum(x => x.Hours), OPayrollConstants.HoursPerDay), 2);

            if (ot > 0)
                foreach (var x in SplitByMidnight(dutyEnd, dutyEnd.AddHours(ot)))
                {
                    var sg = Seg(x.Date);
                    sg.OtHrs += (x.End - x.Start).TotalHours;
                    sg.NdOt  += nd is null ? 0 : NdOverlap(x.Start, x.End, nd.Value.Start, nd.Value.End);
                }

            foreach (var sg in segs.Values)
            {
                sg.Hours = Math.Round(sg.Hours, 2); sg.Nd = Math.Round(sg.Nd, 2);
                sg.OtHrs = Math.Round(sg.OtHrs, 2); sg.NdOt = Math.Round(sg.NdOt, 2);
            }
            ndReg = segs.Values.Sum(x => x.Nd);
            ndOt  = segs.Values.Sum(x => x.NdOt);
        }
        double ndHrs = Math.Round(ndReg + ndOt, 2);

        return new AttDailyRow
        {
            Date      = date,
            Day       = date.ToString("ddd", System.Globalization.CultureInfo.InvariantCulture),
            DayType   = p.DutyTypeId switch { 2 => "RD", 3 => "RN", _ => "R" },
            Kind      = kind,
            Remarks   = kind == "OB" ? "OB" + half : kind == "LV" ? "Paid Leave" + half : otNote != string.Empty ? otNote : segs.Count > 1 ? "Crosses midnight" : string.Empty,
            Credit    = kind != "" && half != string.Empty ? 0.5 : 1,
            Holiday   = holidays.TryGetValue(date, out var h) ? h : string.Empty,
            TimeIn    = p.PunchInDate?.ToString("HH:mm") ?? string.Empty,
            TimeOut   = p.PunchOutDate?.ToString("HH:mm") ?? string.Empty,
            Tardiness = tard,
            Overtime  = ot,
            ND        = ndHrs,
            NdReg     = ndReg,
            NdOt      = ndOt,
            RegHrs    = regHrs,
            Undertime = under,
            DutyStart = dutyStart.ToString("HH:mm"),
            DutyEnd   = dutyEnd.ToString("HH:mm"),
            Segs      = segs.Values.OrderBy(x => x.Date).ToList()
        };
    }

    public static string DutyOf(AtttemplateModel t, DateTime d)
    {
        var x = ((int)d.DayOfWeek + 1) switch
        {
            1 => t.D1_dutytype, 2 => t.D2_dutytype, 3 => t.D3_dutytype, 4 => t.D4_dutytype,
            5 => t.D5_dutytype, 6 => t.D6_dutytype, _ => t.D7_dutytype
        };
        return (x ?? "RD").Trim();
    }

    private static double NdOverlap(DateTime s, DateTime e, int ndStart, int ndEnd)
    {
        if (e <= s) return 0;
        double total = 0;
        for (var d = s.Date.AddDays(-1); d <= e.Date; d = d.AddDays(1))
        {
            var ws = d.AddHours(ndStart / 100).AddMinutes(ndStart % 100);
            var we = (ndEnd <= ndStart ? d.AddDays(1) : d).AddHours(ndEnd / 100).AddMinutes(ndEnd % 100);
            var a = s > ws ? s : ws;
            var b = e < we ? e : we;
            if (b > a) total += (b - a).TotalHours;
        }
        return Math.Round(total, 2);
    }

    private async Task<Dictionary<DateTime, string>> LoadHolidays(UserClaimsModel Uc, int _payrollGrpId, DateTime _start, DateTime _end)
    {
        var found = new Dictionary<DateTime, List<string>>();
        void Add(DateTime d, string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return;
            if (!found.TryGetValue(d.Date, out var l)) found[d.Date] = l = [];
            l.Add(code.Trim().ToUpper());
        }

        for (int year = _start.AddDays(-14).Year; year <= _end.Year; year++)
        {
            foreach (var g in await _holidayDa._02Global(year, Uc!.OpayDb!, Uc.Conn!))
                Add(g.HDate, g.TypeCode);
            foreach (var l in await _holidayDa._02Local(year, Uc.OpayDb!, Uc.Conn!))
            {
                var ids = (l.GroupIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (ids.Contains(_payrollGrpId.ToString())) Add(l.HDate, l.TypeCode);
            }
        }

        return found.ToDictionary(x => x.Key, x => x.Value.Contains("DH") || x.Value.Distinct().Count() > 1 ? "DH" : x.Value[0]);
    }

    public static AttSummaryRow ToSummary(AttEmpRow e, List<AttDailyRow> rows)
    {
        var r = new AttSummaryRow { EmpNumber = e.EmpNumber, EmpName = e.EmpName };
        foreach (var x in rows)
        {
            bool rd = x.DayType == "RD";
            switch (x.Holiday)
            {
                case "LH": if (rd) r.LegRD += x.Credit; else r.LegDays += x.Credit; r.LegTard += x.TotTardHrs; break;
                case "SH": if (rd) r.SpRD += x.Credit;  else r.SpDays += x.Credit;  r.SpTard  += x.TotTardHrs; break;
                case "DH": if (rd) r.DbRD += x.Credit;  else r.DbDays += x.Credit;  r.DbTard  += x.TotTardHrs; break;
                default:   if (rd) r.RegRD += x.Credit; else r.RegDays += x.Credit; r.RegTard += x.TotTardHrs; break;
            }

            var parts = x.Segs.Count > 0 ? x.Segs.Select(g => (g.Holiday, g.OtHrs, g.Nd + g.NdOt)).ToList()
                                         : [(x.Holiday, x.Overtime, x.ND)];
            foreach (var (h, ot, ndh) in parts)
            {
                switch (h)
                {
                    case "LH": r.LegOT += ot; r.LegND += ndh; break;
                    case "SH": r.SpOT  += ot; r.SpND  += ndh; break;
                    case "DH": r.DbOT  += ot; r.DbND  += ndh; break;
                    default:   r.RegOT += ot; r.RegND += ndh; break;
                }
            }
        }
        return r;
    }
}

public class AttEmpRow
{
    public int    SystemId  { get; set; }
    public string EmpNumber { get; set; } = string.Empty;
    public string EmpName   { get; set; } = string.Empty;
}

public class AttDailyRow
{
    public DateTime Date      { get; set; }
    public string   Day       { get; set; } = string.Empty;
    public string   DayType   { get; set; } = string.Empty;
    public string   Holiday   { get; set; } = string.Empty;
    public string   Kind      { get; set; } = string.Empty;
    public string   Remarks   { get; set; } = string.Empty;
    public double   Credit    { get; set; } = 1;
    public string   TimeIn    { get; set; } = string.Empty;
    public string   TimeOut   { get; set; } = string.Empty;
    public double   Tardiness { get; set; }
    public double   Undertime { get; set; }
    public double   Overtime  { get; set; }
    public double   RegHrs    { get; set; }
    public double   NdReg     { get; set; }
    public double   NdOt      { get; set; }
    public double   AbsentHrs { get; set; }
    public double   LateHrs    => Tardiness / 60.0;
    public double   UnderHrs   => Undertime / 60.0;
    public double   AbsDays    => AbsentHrs / OPayrollConstants.HoursPerDay;
    public double   TotTardHrs => LateHrs + UnderHrs + AbsentHrs;
    public double   ND        { get; set; }
    public string   DutyStart { get; set; } = string.Empty;
    public string   DutyEnd   { get; set; } = string.Empty;
    public List<AttSeg> Segs  { get; set; } = [];
}

public class AttSeg
{
    public DateTime Date    { get; set; }
    public string   Holiday { get; set; } = string.Empty;
    public bool     IsRd    { get; set; }
    public double   Hours   { get; set; }
    public double   Nd      { get; set; }
    public double   OtHrs   { get; set; }
    public double   NdOt    { get; set; }
}

public class AttSummaryRow
{
    public string EmpNumber { get; set; } = string.Empty;
    public string EmpName   { get; set; } = string.Empty;
    public double RegDays { get; set; } public double RegRD { get; set; } public double RegTard { get; set; } public double RegOT { get; set; } public double RegND { get; set; }
    public double LegDays { get; set; } public double LegRD { get; set; } public double LegTard { get; set; } public double LegOT { get; set; } public double LegND { get; set; }
    public double SpDays  { get; set; } public double SpRD  { get; set; } public double SpTard  { get; set; } public double SpOT  { get; set; } public double SpND  { get; set; }
    public double DbDays  { get; set; } public double DbRD  { get; set; } public double DbTard  { get; set; } public double DbOT  { get; set; } public double DbND  { get; set; }
}
