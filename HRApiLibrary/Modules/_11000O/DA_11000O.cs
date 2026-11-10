using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._11000O;

public class DA_11000O : IDA_11000O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_11000O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    private async Task<int> Count(string sql, object p, bool legacy, string conn)
    {
        try
        {
            var r = legacy ? await _sql.FetchLegacy<long, dynamic>(sql, p, conn) : await _sql.FetchData<long, dynamic>(sql, p, conn);
            return (int)(r?.FirstOrDefault() ?? 0);
        }
        catch { return 0; }
    }


    private async Task ScheduleCounts(M11000_Home h, string pisdb, string opisdb, string opaydb, string paydb, string conn)
    {
        try
        {
            var now = DateTime.Now;
            var today = now.Date;
            var dn = (int)today.DayOfWeek + 1;
            int nowT = now.Hour * 60 + now.Minute;
            var p = new { Today = today, Dn = dn };

            var emps = await _sql.FetchLegacy<M11000_SchedEmp, dynamic>($@"select e.SystemId EmpmasId, trim(o.EmpNumber) EmpNumber
                    from {opisdb}.Empmas o
                    left join {opisdb}.empstat s on s.CODE = o.EmpStat_
                    join {pisdb}.Empmas e on trim(e.EmpNumber) = trim(o.EmpNumber) and e.SystemId > 0
                    where ifnull(s.ISRESIGNED, '0') <> '1'", p, conn) ?? [];

            var tpl = (await _sql.FetchData<M11000_Tpl, dynamic>($@"select EmpmasId, AttendanceTypeId Att,
                    elt(@Dn, D1_In, D2_In, D3_In, D4_In, D5_In, D6_In, D7_In) SchedIn,
                    elt(@Dn, D1_DutyType, D2_DutyType, D3_DutyType, D4_DutyType, D5_DutyType, D6_DutyType, D7_DutyType) Duty
                    from {pisdb}.Atttemplate where EmpmasId > 0", p, conn) ?? []).ToDictionary(x => x.EmpmasId, x => x);

            var punch = (await _sql.FetchData<M11000_FirstIn, dynamic>($@"select EmpmasId, min(PunchInDate) FirstIn from {pisdb}.Attpunches1
                    where date(PunchInDate) = @Today group by EmpmasId", p, conn) ?? []).ToDictionary(x => x.EmpmasId, x => x.FirstIn);

            var away = new HashSet<string>(((await _sql.FetchLegacy<string, dynamic>($@"select trim(EMPNUMBER) from {opisdb}.ob1 where `START` <= @Today and `END` >= @Today
                    union select trim(EMPNUMBER) from {opisdb}.leave1 where `START` <= @Today and `END` >= @Today", p, conn)) ?? []));

            var grace = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(paydb) && !string.IsNullOrEmpty(opaydb))
            {
                try
                {
                    var g = await _sql.FetchData<M11000_Grace, dynamic>($@"select r.EmpmasId, g.GracePrd Minutes from {paydb}.EmpRates r
                            join {opaydb}.payrollgrp g on g.Id = r.PayrollgrpId", p, conn) ?? [];
                    foreach (var x in g) grace[x.EmpmasId] = x.Minutes;
                }
                catch { }
            }

            bool holiday = false;
            try
            {
                var hc = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opaydb}.holidaysetting where Scope = 'G' and HDate = @Today", p, conn);
                holiday = (hc?.FirstOrDefault() ?? 0) > 0;
            }
            catch { }

            int late = 0, none = 0;
            foreach (var e in emps)
            {
                if (!tpl.TryGetValue(e.EmpmasId, out var t)) continue;
                if (t.Att == 3 || (t.Duty ?? "") != "R") continue;
                if (away.Contains(e.EmpNumber ?? "")) continue;

                int inT = t.SchedIn ?? 0;
                bool validIn = inT > 0 && inT / 100 < 24 && inT % 100 < 60;
                int due = validIn ? (inT / 100) * 60 + inT % 100 + (grace.TryGetValue(e.EmpmasId, out var gm) ? gm : 0) : 12 * 60;

                if (punch.TryGetValue(e.EmpmasId, out var first))
                {
                    if (t.Att == 1 && validIn && first.Hour * 60 + first.Minute > due) late++;
                }
                else if (!holiday && nowT > due) none++;
            }
            h.Late = late;
            h.NoPunch = none;
        }
        catch { }
    }

    public async Task<M11000_Home> _02Home(string pisdb, string opisdb, string opaydb, string paydb, string mainSchema, string conn)
    {
        var today = DateTime.Today;
        var p = new { Today = today };
        var h = new M11000_Home { Today = today };

        h.Active = await Count($@"select count(*) from {opisdb}.Empmas e left join {opisdb}.empstat s on s.CODE = e.EmpStat_
                                  where ifnull(s.ISRESIGNED, '0') <> '1'", p, true, conn);
        h.EmployeeCount = await Count($"select count(*) from {opisdb}.Empmas", p, true, conn);

        try
        {
            var lim = await _sql.FetchData<int, dynamic>($"select EmployeeCnt from {mainSchema}.UsersCompany where PISSchema = @Pis limit 1", new { Pis = pisdb }, conn);
            h.EmployeeLimit = lim?.FirstOrDefault() ?? 0;
        }
        catch { }

        h.Present   = await Count($"select count(distinct EmpmasId) from {pisdb}.Attpunches1 where date(PunchInDate) = @Today", p, false, conn);
        h.InNoOut   = await Count($"select count(*) from {pisdb}.Attpunches1 where date(PunchInDate) = @Today and PunchOutDate is null", p, false, conn);
        h.Incomplete = await Count($"select count(*) from {pisdb}.Attpunches1 where date(PunchInDate) < @Today and PunchOutDate is null", p, false, conn);

        h.OnObLeave = await Count($@"select count(distinct EMPNUMBER) from (
                                        select trim(EMPNUMBER) EMPNUMBER from {opisdb}.ob1    where `START` <= @Today and `END` >= @Today
                                        union
                                        select trim(EMPNUMBER) EMPNUMBER from {opisdb}.leave1 where `START` <= @Today and `END` >= @Today) x", p, true, conn);
        await ScheduleCounts(h, pisdb, opisdb, opaydb, paydb, conn);

        h.PendingOb     = await Count($"select count(*) from {pisdb}.ObApplication where Status in ('S','F')", p, false, conn);
        h.PendingLeave  = await Count($"select count(*) from {pisdb}.LeaveApplication where Status in ('S','F')", p, false, conn);
        h.PendingOt     = await Count($"select count(*) from {pisdb}.OtReqHdr where Status in ('S','F')", p, false, conn);
        h.PendingChange = await Count($"select count(*) from {pisdb}.AttTemplateReqHdr where Status in ('S','F')", p, false, conn);

        h.Enrolled = await Count($"select count(distinct EmpmasId) from {pisdb}.FaceTemplate", p, false, conn);

        try
        {
            var punches = await _sql.FetchData<M11000_Punch, dynamic>($@"select * from (
                    select trim(e.EmpNumber) EmpNumber, p.PunchInDate `At`, 'IN' `Type` from {pisdb}.Attpunches1 p join {pisdb}.Empmas e on e.SystemId = p.EmpmasId
                     where date(p.PunchInDate) = @Today
                    union all
                    select trim(e.EmpNumber), p.PunchOutDate, 'OUT' from {pisdb}.Attpunches1 p join {pisdb}.Empmas e on e.SystemId = p.EmpmasId
                     where p.PunchOutDate is not null and date(p.PunchOutDate) = @Today) x
                    order by `At` desc limit 7", p, conn);
            h.Latest = punches ?? [];

            var nums = h.Latest.Select(x => x.EmpNumber ?? "").Distinct().ToList();
            if (nums.Count > 0)
            {
                var names = await _sql.FetchLegacy<M11000_Name, dynamic>($@"select trim(EmpNumber) EmpNumber,
                        concat(trim(ifnull(EmpLastNm, '')), ', ', trim(ifnull(EmpFirstNm, '')), ' ', trim(ifnull(EmpMidNm, ''))) EmpName
                        from {opisdb}.Empmas where trim(EmpNumber) in @Nums", new { Nums = nums }, conn);
                var map = (names ?? []).ToDictionary(x => x.EmpNumber ?? "", x => (x.EmpName ?? "").Trim());
                foreach (var x in h.Latest) x.EmpName = map.TryGetValue(x.EmpNumber ?? "", out var n) && n.Length > 0 ? n : x.EmpNumber;
            }
        }
        catch { }

        try
        {
            var hol = await _sql.FetchLegacy<M11000_Holiday, dynamic>($@"select HDate, Name, TypeCode from {opaydb}.holidaysetting
                                                                        where Scope = 'G' and HDate >= @Today order by HDate limit 1", p, conn);
            h.Holiday = hol?.FirstOrDefault();
        }
        catch { }

        return h;
    }
}

public interface IDA_11000O
{
    Task<M11000_Home> _02Home(string pisdb, string opisdb, string opaydb, string paydb, string mainSchema, string conn);
}

public class M11000_Home
{
    public DateTime Today { get; set; }
    public int Present { get; set; }
    public int NoPunch { get; set; }
    public int Late { get; set; }
    public int OnObLeave { get; set; }
    public int InNoOut { get; set; }
    public int Incomplete { get; set; }
    public int PendingOb { get; set; }
    public int PendingLeave { get; set; }
    public int PendingOt { get; set; }
    public int PendingChange { get; set; }
    public int Active { get; set; }
    public int Enrolled { get; set; }
    public int EmployeeCount { get; set; }
    public int EmployeeLimit { get; set; }
    public List<M11000_Punch> Latest { get; set; } = new();
    public M11000_Holiday? Holiday { get; set; }
}

public class M11000_Punch
{
    public string? EmpNumber { get; set; }
    public string? EmpName { get; set; }
    public DateTime At { get; set; }
    public string? Type { get; set; }
}

public class M11000_Name
{
    public string? EmpNumber { get; set; }
    public string? EmpName { get; set; }
}

public class M11000_Holiday
{
    public DateTime HDate { get; set; }
    public string? Name { get; set; }
    public string? TypeCode { get; set; }
}

public class M11000_SchedEmp { public int EmpmasId { get; set; } public string? EmpNumber { get; set; } }
public class M11000_Tpl { public int EmpmasId { get; set; } public int? Att { get; set; } public int? SchedIn { get; set; } public string? Duty { get; set; } }
public class M11000_FirstIn { public int EmpmasId { get; set; } public DateTime FirstIn { get; set; } }
public class M11000_Grace { public int EmpmasId { get; set; } public int Minutes { get; set; } }
