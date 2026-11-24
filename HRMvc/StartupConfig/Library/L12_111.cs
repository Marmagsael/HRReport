using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRMvc.StartupConfig.Library;

public class L12_111
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public L12_111(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    static string Months(string col, bool minusOne) =>
        $"(((year(now()) - year({col})) * 12) + month(now()) - month({col}) - {(minusOne ? "if(day(now()) < day(" + col + "), 0, 1)" : "0")})";

    static string Valid(string col) => $"({col} is not null and {col} > '1900-01-01')";

    static string LastLeaveEnd => "(select l.`END` from {0}.leave1 l where l.EMPNUMBER = e.EMPNUMBER order by l.LVNUMBER desc limit 1)";

    public List<V12_111Card> Cards(string opis)
    {
        var onLeave   = $"e.EMPSTAT_ in (select CODE from {opis}.empstat where isonleaved = '1')";
        var floating  = $"e.EMPSTAT_ in (select CODE from {opis}.empstat where isfloating = '1')";
        var suspended = $"e.EMPSTAT_ in (select CODE from {opis}.empstat where issuspended = '1')";
        var leaveEnd  = string.Format(LastLeaveEnd, opis);
        var floatMo   = Months("e.MOVDATE", false);
        var floatNotice = $"{Valid("e.MOVDATE")} and {floating} and {floatMo} between 3 and 6";
        string Need(int n) => $"{floatNotice} and (f.date{n} is null or f.date{n} <= '1900-01-01') and {floatMo} >= {n + 2}";

        return new()
        {
            new("Employees", "fordeploy",   "For Deployment",                           "fa-user-clock",        "#1f6fb2", "e.EMPSTAT_ = '0'", "e.MOVDATE"),
            new("Employees", "undeployed",  "Undeployed Employees",                     "fa-user-xmark",        "#b36b00", $"e.EMPSTAT_ in (select CODE from {opis}.empstat where ISRESIGNED = '0') and e.EMPSTAT_ <> 'A'", "e.MOVDATE"),
            new("Employees", "deployed",    "Deployed Employees",                       "fa-user-shield",       "#157347", "e.EMPSTAT_ = 'A'", "e.MOVDATE"),
            new("Employees", "emp5mo",      "Employed five months and above",           "fa-calendar-check",    "#157347", $"{Valid("e.DATEHIRED")} and e.EMPSTAT_ = 'A' and {Months("e.DATEHIRED", true)} >= 5", "e.DATEHIRED"),
            new("Employees", "newhired",    "Newly hired for the month",                "fa-user-plus",         "#1f6fb2", "year(now()) = year(e.DATEHIRED) and month(now()) = month(e.DATEHIRED)", "e.DATEHIRED"),
            new("Employees", "floating",    "Floating Employees",                       "fa-user-clock",        "#b02a37", floating, "e.MOVDATE"),
            new("Employees", "onleave",     "On-Leave Employees",                       "fa-umbrella-beach",    "#8e5bd0", onLeave, leaveEnd),
            new("Employees", "leavedone",   "Finished Leave Duration",                  "fa-calendar-xmark",    "#8e5bd0", $"{onLeave} and {leaveEnd} < curdate()", leaveEnd),
            new("Employees", "awol",        "Awol Employee",                            "fa-person-walking-arrow-right", "#b02a37", "e.EMPSTAT_ = 'W'", "e.MOVDATE"),
            new("Employees", "clearance",   "Under-Clearance",                          "fa-file-circle-check", "#b36b00", "e.EMPSTAT_ = 'U'", "e.SEPARATE"),
            new("Employees", "clearance30", "Under Clearance Over 30 days",             "fa-file-circle-exclamation", "#b02a37", "e.EMPSTAT_ = 'U' and not coalesce(e.SEPARATE > date_sub(curdate(), interval 30 day), 0)", "e.SEPARATE"),
            new("Employees", "training",    "For Training",                             "fa-chalkboard-user",   "#1f6fb2", "e.EMPSTAT_ = 'I'", "e.DATEHIRED"),
            new("Employees", "suspended",   "Suspended Employee",                       "fa-user-lock",         "#b02a37", suspended, "e.MOVEND"),

            new("Notices", "need1",         "Need for 1st Notice",                      "fa-envelope",          "#e08a00", Need(1), "e.MOVDATE", true),
            new("Notices", "need2",         "Need for 2nd Notice",                      "fa-envelope",          "#e08a00", Need(2), "e.MOVDATE", true),
            new("Notices", "need3",         "Need for 3rd Notice",                      "fa-envelope",          "#d9480f", Need(3), "e.MOVDATE", true),
            new("Notices", "need4",         "Need for 4th Notice",                      "fa-envelope",          "#b02a37", Need(4), "e.MOVDATE", true),
            new("Notices", "candfloat",     "Candidate for Floating Status",            "fa-user-clock",        "#d9480f", $"{suspended} and e.MOVEND < curdate()", "e.MOVEND"),
            new("Notices", "candawol",      "Candidate for AWOL Status",                "fa-user-slash",        "#b02a37", $"{Valid("e.MOVDATE")} and {floating} and {Months("e.MOVDATE", true)} >= 6", "e.MOVDATE"),
            new("Notices", "dormant",       "Dormant Employee (floating for more than 365 days)", "fa-bed",     "#5b6b7d", $"{Valid("e.MOVDATE")} and {floating} and e.MOVDATE < date_sub(curdate(), interval 365 day)", "e.MOVDATE"),

            new("Clients and License", "clients",   "Active Clients",                   "fa-building",          "#157347", "", "", false, true),
            new("Clients and License", "explic",    "Expired License",                  "fa-id-card",           "#b02a37", $"{Valid("e.LICEXPIRE")} and e.LICEXPIRE < now() and coalesce(e.SECLICENSE, '') <> '' and e.EMPSTAT_ in (select CODE from {opis}.empstat where ISRESIGNED = '0')", "e.LICEXPIRE"),

            new("Issued this month", "assign",  "Assignment Issued for the curr. month","fa-location-dot",     "#1f6fb2", "", "m.`DATE`", false, false, "A"),
            new("Issued this month", "recall",  "Recall Issued for the curr. month",    "fa-rotate-left",      "#e08a00", "", "m.`DATE`", false, false, "R"),
            new("Issued this month", "rtu",     "RTU Issued for the curr. month",       "fa-arrow-right-to-bracket", "#8e5bd0", "", "m.`DATE`", false, false, "T"),
        };
    }

    string FromWhere(V12_111Card c, string opis)
    {
        if (c.IsClient) return $"from {opis}.client cl where cl.STATUS = 'Active'";
        if (c.MoveMode != null)
            return $@"from {opis}.movedtl m
                      join {opis}.empmas e on e.EMPNUMBER = m.EMPNUMBER
                      left join {opis}.client c on c.CLNUMBER = e.CLIENT_
                      left join {opis}.empstat s on s.CODE = e.EMPSTAT_
                      where m.`MODE` = '{c.MoveMode}' and year(now()) = year(m.`DATE`) and month(now()) = month(m.`DATE`)";
        return $@"from {opis}.empmas e
                  left join {opis}.client c on c.CLNUMBER = e.CLIENT_
                  left join {opis}.empstat s on s.CODE = e.EMPSTAT_
                  {(c.NeedsNotice ? $"left join {opis}.floatnotice f on f.empnumber = e.EMPNUMBER" : "")}
                  where {c.Where}";
    }

    public async Task Count(List<V12_111Card> cards, string opis, string? conn)
    {
        foreach (var c in cards)
        {
            try
            {
                var n = await _sql.FetchLegacy<int, dynamic>($"select count(*) {FromWhere(c, opis)}", new { }, conn);
                c.Total = n.FirstOrDefault();
            }
            catch { c.Total = 0; c.Failed = true; }
        }
    }

    public async Task<List<V12_111Row>> Rows(V12_111Card c, string opis, string? conn)
    {
        var dateCol = string.IsNullOrEmpty(c.DateColumn) ? "null" : $"case when {c.DateColumn} > '1900-01-01' then {c.DateColumn} end";
        var select = c.IsClient
            ? "select cl.CLNUMBER as EmpNumber, cl.CLNAME as Name, '' as Client, cl.STATUS as Status, null as OnDate, '' as Note"
            : $@"select e.EMPNUMBER as EmpNumber,
                        concat(trim(e.EMPLASTNM), ', ', trim(e.EMPFIRSTNM), ' ', trim(coalesce(e.EMPMIDNM, ''))) as Name,
                        coalesce(c.CLNAME, '') as Client, coalesce(s.NAME, '') as Status,
                        {dateCol} as OnDate, {(c.MoveMode != null ? "coalesce(m.REASON, '')" : "''")} as Note";
        var order = c.IsClient ? "order by Name" : "order by e.EMPLASTNM, e.EMPFIRSTNM";
        return await _sql.FetchLegacy<V12_111Row, dynamic>($"{select} {FromWhere(c, opis)} {order}", new { }, conn);
    }
}

public class V12_111Card
{
    public string   Group       { get; }
    public string   Key         { get; }
    public string   Title       { get; }
    public string   Icon        { get; }
    public string   Color       { get; }
    public string   Where       { get; }
    public string   DateColumn  { get; }
    public bool     NeedsNotice { get; }
    public bool     IsClient    { get; }
    public string?  MoveMode    { get; }
    public int      Total       { get; set; }
    public bool     Failed      { get; set; }

    public V12_111Card(string group, string key, string title, string icon, string color, string where, string dateColumn, bool needsNotice = false, bool isClient = false, string? moveMode = null)
    {
        Group = group; Key = key; Title = title; Icon = icon; Color = color; Where = where; DateColumn = dateColumn;
        NeedsNotice = needsNotice; IsClient = isClient; MoveMode = moveMode;
    }
}

public class V12_111Row
{
    public string?      EmpNumber   { get; set; }
    public string?      Name        { get; set; }
    public string?      Client      { get; set; }
    public string?      Status      { get; set; }
    public DateTime?    OnDate      { get; set; }
    public string?      Note        { get; set; }
}
