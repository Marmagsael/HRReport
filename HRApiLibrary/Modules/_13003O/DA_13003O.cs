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

    public async Task<List<M13003_Client>> _02Clients(string opis, string conn)
    {
        string sql = $"select trim(clnumber) ClNumber, trim(clname) ClName from {opis}.client where status = 'Active' order by clname";
        return await _sql.FetchLegacy<M13003_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    class Emp
    {
        public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
        public int EmpmasId { get; set; } public DateTime? DateHired { get; set; } public DateTime? Separate { get; set; }
    }
    class Punch { public int EmpmasId { get; set; } public DateTime PunchDate { get; set; } public DateTime? TimeIn { get; set; } public DateTime? TimeOut { get; set; } }
    class Req { public string EmpNumber { get; set; } = ""; public DateTime CovStart { get; set; } public DateTime CovEnd { get; set; } public string Category { get; set; } = ""; }

    class Ctx
    {
        public List<Emp> Emps = new();
        public Dictionary<int, AtttemplateModel> Tpl = new();
        public AtttemplateModel NoSched = new();
        public ILookup<(int, DateTime), Punch> Punches = null!;
        public List<Req> Reqs = new();
    }

    async Task<Ctx> Load(string clNumber, DateTime from, DateTime to, string pisdb, string opis, string conn)
    {
        var c = new Ctx();
        string esql = $@"select trim(o.EmpNumber) EmpNumber,
                                concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName,
                                trim(ifnull(cl.clname, '')) Client, p.Id EmpmasId, o.DateHired DateHired, o.Separate Separate
                         from {opis}.Empmas o
                              join {opis}.empstat s on s.code = o.empstat_ and s.IsInPayroll = 1
                              left join {opis}.client cl on cl.clnumber = o.client_
                              left join {pisdb}.Empmas p on trim(p.EmpNumber) = trim(o.EmpNumber)
                         where @Cl = '' or trim(o.client_) = @Cl
                         order by o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm";
        c.Emps = await _sql.FetchLegacy<Emp, dynamic>(esql, new { Cl = clNumber ?? "" }, conn) ?? [];
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

            var punches = await _sql.FetchLegacy<Punch, dynamic>($@"select EmpmasId, PunchDate, TimeIn, TimeOut from {pisdb}.AttDaily
                                                                    where PunchDate between @From and @To and EmpmasId in @Ids", new { From = from.Date, To = to.Date, Ids = ids }, conn) ?? [];
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
            int late = work ? (int)tin.TimeOfDay.TotalMinutes - Minutes(inHhmm) : 0;
            return (late > 0 ? "Late" : "Present", tin, rec.TimeOut, inHhmm, work, late > 0 ? late : 0);
        }
        var req = c.Reqs.FirstOrDefault(r => r.EmpNumber == e.EmpNumber && r.CovStart.Date <= d.Date && r.CovEnd.Date >= d.Date);
        if (req != null) return (req.Category == "OB" ? "OB" : "On Leave", null, null, inHhmm, work, 0);
        return (work ? "Absent" : "Rest Day", null, null, inHhmm, work, 0);
    }

    static bool InService(Emp e, DateTime d) => (e.DateHired == null || e.DateHired.Value.Date <= d.Date) && (e.Separate == null || e.Separate.Value.Date >= d.Date);

    public async Task<List<M13003_Sum>> _02Summary(string clNumber, DateTime from, DateTime to, string pisdb, string opis, string conn)
    {
        var c = await Load(clNumber, from, to, pisdb, opis, conn);
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
                    case "Present": s.DaysPresent++; break;
                    case "Late": s.DaysPresent++; s.DaysLate++; s.LateMinutes += r.LateMin; break;
                    case "Absent": s.DaysAbsent++; break;
                    case "On Leave": case "OB": s.DaysLeaveOb++; break;
                }
            }
            res.Add(s);
        }
        return res;
    }

    public async Task<List<M13004_Row>> _02Daily(string clNumber, DateTime date, string pisdb, string opis, string conn)
    {
        var c = await Load(clNumber, date, date, pisdb, opis, conn);
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
    Task<List<M13003_Client>> _02Clients(string opis, string conn);
    Task<List<M13003_Sum>> _02Summary(string clNumber, DateTime from, DateTime to, string pisdb, string opis, string conn);
    Task<List<M13004_Row>> _02Daily(string clNumber, DateTime date, string pisdb, string opis, string conn);
}

public class M13003_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }

public class M13003_Sum
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
    public int ScheduledDays { get; set; } public int DaysPresent { get; set; } public int DaysAbsent { get; set; }
    public int DaysLate { get; set; } public int LateMinutes { get; set; } public int DaysLeaveOb { get; set; }
}

public class M13004_Row
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Client { get; set; } = "";
    public string Status { get; set; } = ""; public DateTime? TimeIn { get; set; } public DateTime? TimeOut { get; set; }
    public string SchedIn { get; set; } = ""; public int LateMinutes { get; set; }
}
