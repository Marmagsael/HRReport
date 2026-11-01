using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRMvc.StartupConfig.Library;

// -- 12/101 Employee Summary : counts from the old PIS (source of truth); department and employment type names from the company PIS masters --
public class L12_101
{
    private const int   WatchDays   = 30;       // -- regularization due within this many days --

    private readonly I_90_001_MySqlDataAccess _sql;

    public L12_101(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<V12_101> _02Summary(UserClaimsModel uc)
    {
        var v = new V12_101();
        if (string.IsNullOrWhiteSpace(uc.OpisDb) || string.IsNullOrWhiteSpace(uc.SchemaUserPis)) return v;

        var opis    = uc.OpisDb!.Trim();
        var pis     = uc.SchemaUserPis!.Trim();
        var conn    = uc.Conn;

        // -- Totals --
        var totals = await _sql.FetchLegacy<V12_101Totals, dynamic>($@"
            select  count(*)                                                                    as Total,
                    cast(coalesce(sum(e.EMPSTAT_ = 'A'), 0) as signed)                          as Active,
                    cast(coalesce(sum(s.ISRESIGNED = 1), 0) as signed)                          as Resigned,
                    cast(coalesce(sum(e.DATEHIRED >= @MonthStart and e.DATEHIRED < @NextMonth), 0) as signed) as NewHires
            from    {opis}.empmas e
            left join {opis}.empstat s on s.CODE = e.EMPSTAT_",
            new { MonthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), NextMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1) }, conn);
        v.Totals = totals.FirstOrDefault() ?? new();

        // -- Active employees per department (no deployment record = "Not assigned") --
        v.ByDepartment = await _sql.FetchLegacy<V12_101Slice, dynamic>($@"
            select  coalesce(r.Name, 'Not assigned') as Label, count(*) as Total
            from    {opis}.empmas e
            left join {opis}.deprec d       on d.EmpNumber = e.EMPNUMBER
            left join {pis}.rdepartment r   on r.Id = d.DepId
            where   e.EMPSTAT_ = 'A'
            group by coalesce(r.Name, 'Not assigned')
            order by Total desc, Label", new { }, conn);

        // -- Active employees per employment type --
        v.ByEmploymentType = await _sql.FetchLegacy<V12_101Slice, dynamic>($@"
            select  coalesce(t.Name, 'Not assigned') as Label, count(*) as Total
            from    {opis}.empmas e
            left join {opis}.deprec d       on d.EmpNumber = e.EMPNUMBER
            left join {pis}.employmenttype t on t.Id = d.EmploymentTypeId
            where   e.EMPSTAT_ = 'A'
            group by coalesce(t.Name, 'Not assigned')
            order by Total desc, Label", new { }, conn);

        // -- Probationary employees whose probation ends soon (due for regularization) --
        v.ForRegularization = await _sql.FetchLegacy<V12_101Person, dynamic>($@"
            select  e.EMPNUMBER as EmpNumber,
                    concat(trim(e.EMPLASTNM), ', ', trim(e.EMPFIRSTNM)) as Name,
                    d.DProbationaryEnd as OnDate
            from    {opis}.empmas e
            join    {opis}.deprec d on d.EmpNumber = e.EMPNUMBER
            where   e.EMPSTAT_ = 'A' and d.EmploymentTypeId = 3
                    and d.DProbationaryEnd >= @Today and d.DProbationaryEnd <= @Until
            order by d.DProbationaryEnd, Name
            limit 20", new { Today = DateTime.Today, Until = DateTime.Today.AddDays(WatchDays) }, conn);

        // -- Birthdays this month --
        v.Birthdays = await _sql.FetchLegacy<V12_101Person, dynamic>($@"
            select  e.EMPNUMBER as EmpNumber,
                    concat(trim(e.EMPLASTNM), ', ', trim(e.EMPFIRSTNM)) as Name,
                    e.EMPBIRTH as OnDate
            from    {opis}.empmas e
            where   e.EMPSTAT_ = 'A' and e.EMPBIRTH >= '1900-01-01' and month(e.EMPBIRTH) = @Month
            order by day(e.EMPBIRTH), Name
            limit 50", new { Month = DateTime.Today.Month }, conn);

        v.WatchDays = WatchDays;
        return v;
    }
}

public class V12_101
{
    public V12_101Totals        Totals              { get; set; } = new();
    public List<V12_101Slice>   ByDepartment        { get; set; } = new();
    public List<V12_101Slice>   ByEmploymentType    { get; set; } = new();
    public List<V12_101Person>  ForRegularization   { get; set; } = new();
    public List<V12_101Person>  Birthdays           { get; set; } = new();
    public int                  WatchDays           { get; set; }
}

public class V12_101Totals
{
    public int Total    { get; set; }
    public int Active   { get; set; }
    public int Resigned { get; set; }
    public int NewHires { get; set; }
}

public class V12_101Slice
{
    public string?  Label   { get; set; }
    public int      Total   { get; set; }
}

public class V12_101Person
{
    public string?      EmpNumber   { get; set; }
    public string?      Name        { get; set; }
    public DateTime?    OnDate      { get; set; }
}
