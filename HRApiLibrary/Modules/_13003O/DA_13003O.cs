using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.Modules._13003O;

// -- AMS attendance reports (Late and Absent Summary, Daily Attendance). The schedule of an employee is the one the Punch Capturing page uses
// -- (Atttemplate of the employee, else the Attscheddaily default). Late = the time in is after the scheduled time in (no grace period, no holiday).
public class DA_13003O : IDA_13003O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IAtttemplateDataAccess _tpl;

    public DA_13003O(I_90_001_MySqlDataAccess sql, IAtttemplateDataAccess tpl) { _sql = sql; _tpl = tpl; }

    public async Task<List<M13003_Client>> _02Clients(string opay, string conn)
    {
        string sql = $"select cast(Id as char) ClNumber, trim(Name) ClName from {opay}.payrollgrp where ifnull(Status, 'A') = 'A' order by Name";
        return await _sql.FetchLegacy<M13003_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M13003_Emp>> _02Employees(string clNumber, string opis, string conn)
    {
        string sql = $@"select trim(o.EmpNumber) EmpNumber, concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName
                        from {opis}.Empmas o
                             join {opis}.empstat s on s.code = o.empstat_ and s.IsInPayroll = 1
                             left join {opis}.deprec dp on trim(dp.EmpNumber) = trim(o.EmpNumber)
                        where @Cl = '' or cast(dp.PayrollgrpId as char) = @Cl
                        order by o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm";
        return await _sql.FetchLegacy<M13003_Emp, dynamic>(sql, new { Cl = clNumber ?? "" }, conn) ?? [];
    }

    class Emp
    {
        public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
        public int EmpmasId { get; set; } public DateTime? DateHired { get; set; } public DateTime? Separate { get; set; }
    }
    class Punch { public int? PunchT { get; set; } public string? DutyCode { get; set; } public int EmpmasId { get; set; } public DateTime PunchDate { get; set; } public DateTime? TimeIn { get; set; } public DateTime? TimeOut { get; set; } }
    class Req { public string EmpNumber { get; set; } = ""; public DateTime CovStart { get; set; } public DateTime CovEnd { get; set; } public string Category { get; set; } = ""; }

    class Ctx
    {
        public List<Emp> Emps = new();
        public Dictionary<int, AtttemplateModel> Tpl = new();
        public AtttemplateModel NoSched = new();
        public ILookup<(int, DateTime), Punch> Punches = null!;
        public List<Req> Reqs = new();
    }

    async Task<Ctx> Load(string clNumber, IEnumerable<string>? empNumbers, DateTime from, DateTime to, string pisdb, string opis, string opay, string conn)
    {
        var c = new Ctx();
        string esql = $@"select trim(o.EmpNumber) EmpNumber,
                                concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName,
                                trim(ifnull(pg.Name, '')) Client, p.Id EmpmasId, o.DateHired DateHired, o.Separate Separate
                         from {opis}.Empmas o
                              join {opis}.empstat s on s.code = o.empstat_ and s.IsInPayroll = 1
                              left join {opis}.deprec dp on trim(dp.EmpNumber) = trim(o.EmpNumber)
                              left join {opay}.payrollgrp pg on pg.Id = dp.PayrollgrpId
                              left join {pisdb}.Empmas p on trim(p.EmpNumber) = trim(o.EmpNumber)
                         where (@Cl = '' or cast(dp.PayrollgrpId as char) = @Cl) and (@NoEmp = 1 or trim(o.EmpNumber) in @Emps)
                         order by o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm";
        c.Emps = await _sql.FetchLegacy<Emp, dynamic>(esql, new { Cl = clNumber ?? "", NoEmp = empNumbers == null ? 1 : 0, Emps = (empNumbers ?? new List<string>()).Select(x => x.Trim()).DefaultIfEmpty("").ToList() }, conn) ?? [];
        foreach (var e in c.Emps)
        {
            if (e.DateHired is { } h && h.Year < 2) e.DateHired = null;
            if (e.Separate is { } s && s.Year < 2) e.Separate = null;
        }
        var ids = c.Emps.Where(e => e.EmpmasId > 0).Select(e => e.EmpmasId).Distinct().ToList();

        if (ids.Count > 0)
        {
            var tpls = await _sql.FetchData<AtttemplateModel, dynamic>($"select * from {pisdb}.Atttemplate where EmpmasId in @Ids", new { Ids = ids }, conn) ?? [];
            foreach (var t in tpls) if (t.EmpmasId is int id && !c.Tpl.ContainsKey(id)) c.Tpl[id] = t;

            var punches = await _sql.FetchLegacy<Punch, dynamic>($@"select p.EmpmasId, p.PunchT, date(p.PunchInDate) PunchDate, p.PunchInDate TimeIn, p.PunchOutDate TimeOut, t.Code DutyCode from {pisdb}.AttPunches1 p left join {pisdb}.AttDutyType t on t.Id = p.DutyTypeId
                                                                    where p.PunchInDate >= @From and p.PunchInDate < date_add(@To, interval 1 day) and p.EmpmasId in @Ids", new { From = from.Date, To = to.Date, Ids = ids }, conn) ?? [];
            foreach (var p in punches)
            {
                if (p.TimeIn is { } a && a.Year < 2) p.TimeIn = null;
                if (p.TimeOut is { } b && b.Year < 2) p.TimeOut = null;
            }
            c.Punches = punches.ToLookup(p => (p.EmpmasId, p.PunchDate.Date));
        }
        else c.Punches = Array.Empty<Punch>().ToLookup(p => (p.EmpmasId, p.PunchDate.Date));

        c.NoSched = (await _tpl._02NoSchedule(null, pisdb, conn)) ?? new AtttemplateModel();

        string rsql = $@"select trim(h.EmpNumber) EmpNumber, h.CovStart CovStart, h.CovEnd CovEnd, t.Category Category
                         from {pisdb}.AttReqHdr h join {pisdb}.AttReqType t on t.Id = h.AttReqTypeId
                         where h.Status = 'A' and t.Category in ('Leave', 'OB') and h.CovStart <= @To and h.CovEnd >= @From";
        c.Reqs = await _sql.FetchLegacy<Req, dynamic>(rsql, new { From = from.Date, To = to.Date.AddDays(1) }, conn) ?? [];
        return c;
    }

    static (int In, string Duty) Sched(AtttemplateModel a, DateTime d) => (int)d.DayOfWeek switch
    {
        0 => (a.D1_in ?? 0, a.D1_dutytype ?? "RD"),
        1 => (a.D2_in ?? 0, a.D2_dutytype ?? "RD"),
        2 => (a.D3_in ?? 0, a.D3_dutytype ?? "RD"),
        3 => (a.D4_in ?? 0, a.D4_dutytype ?? "RD"),
        4 => (a.D5_in ?? 0, a.D5_dutytype ?? "RD"),
        5 => (a.D6_in ?? 0, a.D6_dutytype ?? "RD"),
        _ => (a.D7_in ?? 0, a.D7_dutytype ?? "RD"),
    };

    static int Minutes(int hhmm) => hhmm / 100 * 60 + hhmm % 100;

    // -- The status of one employee on one date --
    (string Status, DateTime? In, DateTime? Out, int InSched, bool Work, int LateMin) Day(Ctx c, Emp e, DateTime d)
    {
        var tpl = e.EmpmasId > 0 && c.Tpl.TryGetValue(e.EmpmasId, out var t) ? t : c.NoSched;
        var (inHhmm, duty) = Sched(tpl, d);
        bool work = (duty ?? "").Trim() == "R";
        var rec = e.EmpmasId > 0 ? c.Punches[(e.EmpmasId, d.Date)].FirstOrDefault(p => p.TimeIn != null) : null;

        if (rec?.TimeIn is { } tin)
        {
            // -- Same rule as Attendance Processing (12011): the schedule is the one stored on the punch (PunchT), late = time in after PunchT --
            bool punchWork = (rec.DutyCode ?? "R").Trim() != "RD";
            int sched = rec.PunchT ?? inHhmm;
            var dutyStart = tin.Date.AddHours(sched / 100).AddMinutes(sched % 100);
            int late = punchWork && tin > dutyStart ? (int)Math.Floor((tin - dutyStart).TotalMinutes) : 0;
            return (late > 0 ? "Late" : "Present", tin, rec.TimeOut, sched, punchWork, late);
        }
        var req = c.Reqs.FirstOrDefault(r => r.EmpNumber == e.EmpNumber && r.CovStart.Date <= d.Date && r.CovEnd.Date >= d.Date);
        if (req != null) return (req.Category == "OB" ? "OB" : "On Leave", null, null, inHhmm, work, 0);
        return (work ? "Absent" : "Rest Day", null, null, inHhmm, work, 0);
    }

    static bool InService(Emp e, DateTime d) => (e.DateHired == null || e.DateHired.Value.Date <= d.Date) && (e.Separate == null || e.Separate.Value.Date >= d.Date);

    public async Task<List<M13003_Sum>> _02Summary(string clNumber, IEnumerable<string>? empNumbers, DateTime from, DateTime to, string pisdb, string opis, string opay, string conn)
    {
        var c = await Load(clNumber, empNumbers, from, to, pisdb, opis, opay, conn);
        var res = new List<M13003_Sum>();
        foreach (var e in c.Emps)
        {
            var s = new M13003_Sum { EmpNumber = e.EmpNumber, EmpName = e.EmpName, Client = e.Client };
            for (var d = from.Date; d <= to.Date; d = d.AddDays(1))
            {
                if (!InService(e, d)) continue;
                var r = Day(c, e, d);
                if (r.Work) s.ScheduledDays++;
                switch (r.Status)
                {
                    case "Present": if (r.Work) s.DaysPresent++; else s.DaysRestWork++; break;
                    case "Late": s.DaysPresent++; s.DaysLate++; s.LateMinutes += r.LateMin; break;
                    case "Absent": s.DaysAbsent++; break;
                    case "On Leave": case "OB": s.DaysLeaveOb++; break;
                }
            }
            res.Add(s);
        }
        return res;
    }

    public async Task<List<M13004_Row>> _02Daily(string clNumber, IEnumerable<string>? empNumbers, DateTime date, string pisdb, string opis, string opay, string conn)
    {
        var c = await Load(clNumber, empNumbers, date, date, pisdb, opis, opay, conn);
        var res = new List<M13004_Row>();
        foreach (var e in c.Emps)
        {
            if (!InService(e, date)) continue;
            var r = Day(c, e, date);
            res.Add(new M13004_Row
            {
                EmpNumber = e.EmpNumber, EmpName = e.EmpName, Client = e.Client, Status = r.Status, TimeIn = r.In, TimeOut = r.Out,
                SchedIn = r.Work ? $"{r.InSched / 100:00}:{r.InSched % 100:00}" : "", LateMinutes = r.LateMin
            });
        }
        return res;
    }
}

public interface IDA_13003O
{
    Task<List<M13003_Client>> _02Clients(string opay, string conn);
    Task<List<M13003_Emp>> _02Employees(string clNumber, string opis, string conn);
    Task<List<M13003_Sum>> _02Summary(string clNumber, IEnumerable<string>? empNumbers, DateTime from, DateTime to, string pisdb, string opis, string opay, string conn);
    Task<List<M13004_Row>> _02Daily(string clNumber, IEnumerable<string>? empNumbers, DateTime date, string pisdb, string opis, string opay, string conn);
}

public class M13003_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }

public class M13003_Emp { public int SystemId { get; set; } public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; }

public class M13003_Sum
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
    public int ScheduledDays { get; set; } public int DaysPresent { get; set; } public int DaysAbsent { get; set; }
    public int DaysLate { get; set; } public int LateMinutes { get; set; } public int DaysLeaveOb { get; set; } public int DaysRestWork { get; set; }
}

public class M13004_Row
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
    public string Status { get; set; } = ""; public DateTime? TimeIn { get; set; } public DateTime? TimeOut { get; set; }
    public string SchedIn { get; set; } = ""; public int LateMinutes { get; set; }
}
