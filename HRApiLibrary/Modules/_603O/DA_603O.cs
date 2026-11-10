using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._603O;

public class DA_603O : IDA_603O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_603O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M603_Client>> _02Clients(string opis, string conn)
    {
        string sql = $@"select trim(a.clNumber) ClNumber, concat(trim(a.clName), ' - ', trim(ifnull(b.areaname, ''))) ClName
                        from {opis}.client a left join {opis}.area b on a.areacode = b.areacode
                        where a.status = 'Active' order by ClName";
        return await _sql.FetchLegacy<M603_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M603_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(a.empNumber) EmpNumber, trim(ifnull(a.client_, '')) ClNumber,
                               concat(trim(ifnull(a.empLastNm, '')), ', ', trim(ifnull(a.EmpFirstNm, '')), ' ', left(trim(ifnull(a.EmpMidNm, '')), 1)) EmpName,
                               trim(ifnull(b.name, '')) Position, trim(ifnull(a.EmpStat_, '')) EmpStat, trim(ifnull(c.name, '')) Status,
                               if(ifnull(c.isInPayroll, 0) = 1, 1, 0) InPayroll,
                               date_add(a.datehired, interval 5 month) RegDate
                        from {opis}.empmas a
                             left join {opis}.position b on a.position_ = b.code
                             left join {opis}.empstat c on a.empStat_ = c.code
                        where a.empnumber is not null
                        order by a.empLastNm, a.empFirstNm";
        var r = await _sql.FetchLegacy<M603_Emp, dynamic>(sql, new { }, conn) ?? [];
        foreach (var e in r) { e.EmpName = (e.EmpName ?? "").Trim(); if (e.RegDate is { } d && d.Year < 2) e.RegDate = null; }
        return r;
    }

    public async Task<List<M603_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where AcctType = 'D' and isOthers = '1' order by AcctName";
        return await _sql.FetchLegacy<M603_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M603_Period>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) PdNumber, trim(PdName) PdName from {opay}.period where PdNumber != '00' order by PdNumber";
        return await _sql.FetchLegacy<M603_Period, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<(string? Error, int Rows)> _01Save(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string remarks,
                                                         bool p1, bool p2, bool p3, bool p4, bool p5, string opay, string conn)
    {
        if (emps.Count < 1) return ("No employee selected.", 0);
        var used = deds.Where(d => !string.IsNullOrWhiteSpace(d.AcctNumber)).ToList();
        if (used.Count < 1) return ("No deductions selected.", 0);
        if (used.Any(d => d.Amount == 0)) return ("ZERO VALUE deduction(s) are not accepted.", 0);
        if (!(p1 || p2 || p3 || p4 || p5)) return ("Payment period is invalid!", 0);
        var dup = used.GroupBy(d => d.AcctNumber.Trim()).FirstOrDefault(g => g.Count() > 1);
        if (dup != null) return ("Duplicate deduction exists. [DESCRIPTION -> " + dup.First().AcctName + "]", 0);

        int n = 0;
        var pairs = emps.SelectMany(e => used.Select(d => (Emp: e.Trim(), Acct: d.AcctNumber.Trim(), d.Amount))).ToList();
        foreach (var chunk in pairs.Chunk(200))
        {
            var dp = new DynamicParameters();
            dp.Add("Y", year); dp.Add("M", month); dp.Add("Pd", period); dp.Add("Rem", remarks);
            dp.Add("P1", p1 ? 1 : 0); dp.Add("P2", p2 ? 1 : 0); dp.Add("P3", p3 ? 1 : 0); dp.Add("P4", p4 ? 1 : 0); dp.Add("P5", p5 ? 1 : 0);
            var vals = new List<string>();
            for (int i = 0; i < chunk.Length; i++)
            {
                dp.Add("E" + i, chunk[i].Emp); dp.Add("A" + i, chunk[i].Acct); dp.Add("V" + i, Math.Round(chunk[i].Amount, 2));
                vals.Add($"(@Y, @M, @Pd, @E{i}, @A{i}, @V{i}, @Rem, 0, @P1, @P2, @P3, @P4, @P5)");
            }
            string sql = $@"insert into {opay}.tblotherdeductions (year, month, periodID, empNumber, acctNumber, amount, remarks, contproc, p1, p2, p3, p4, p5)
                            values {string.Join(", ", vals)}
                            on duplicate key update amount = values(amount), contproc = 0, p1 = values(p1), p2 = values(p2), p3 = values(p3), p4 = values(p4), p5 = values(p5)";
            await _sql.ExecuteLegacyCmd(sql, dp, conn);
            n += chunk.Length;
        }
        return (null, n);
    }

    private static object DelParams(List<string> emps, List<M603_Ded> deds, int year, int month, string period)
    {
        string pd = (period ?? "").Trim();
        return new { Y = year, M = month, Pd = pd, Pd2 = pd.TrimStart('0'), Emps = emps.Select(e => e.Trim()).ToList(),
                     Accts = deds.Where(d => !string.IsNullOrWhiteSpace(d.AcctNumber)).Select(d => d.AcctNumber.Trim()).Distinct().ToList() };
    }

    private const string DelWhere = @"year = @Y and month = @M and periodID in (@Pd, @Pd2) and processed = 0
                                      and trim(empNumber) in @Emps and trim(acctNumber) in @Accts";

    public async Task<int> _02DeleteCount(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn)
    {
        var p = DelParams(emps, deds, year, month, period);
        if (emps.Count < 1 || !deds.Any(d => !string.IsNullOrWhiteSpace(d.AcctNumber))) return 0;
        var r = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.tblotherdeductions where {DelWhere}", p, conn);
        return (int)(r?.FirstOrDefault() ?? 0);
    }

    public async Task _04Delete(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn)
    {
        var p = DelParams(emps, deds, year, month, period);
        if (emps.Count < 1 || !deds.Any(d => !string.IsNullOrWhiteSpace(d.AcctNumber))) return;
        await _sql.ExecuteLegacyCmd($"delete from {opay}.tblotherdeductions where {DelWhere}", p, conn);
    }

    public async Task<List<R603_Print>> _02Print(int year, int month, string period, int group, string opis, string opay, string conn)
    {
        string pd = (period ?? "").Trim();
        string pd2 = pd.TrimStart('0');
        string order = group switch { 2 => "ClName2, a.EmpLastNm, a.EmpFirstNm", 3 => "AcctName, a.EmpLastNm, a.EmpFirstNm", _ => "a.EmpLastNm, a.EmpFirstNm" };
        string sql = $@"select trim(d.empNumber) Empnumber, trim(ifnull(a.EmpLastNm, '')) Emplastnm, trim(ifnull(a.EmpFirstNm, '')) Empfirstnm, trim(ifnull(a.EmpMidNm, '')) Empmidnm,
                               trim(ifnull(c.AcctName, d.acctNumber)) Acctname, d.amount Amount, ifnull(d.remarks, '') Remarks,
                               d.year Year, d.month Month, @Pd Periodid,
                               concat(trim(ifnull(a.EmpLastNm, '')), ', ', trim(ifnull(a.EmpFirstNm, '')), ' ', left(trim(ifnull(a.EmpMidNm, '')), 1), '.') Empname,
                               concat(trim(ifnull(cl.clName, '')), ' - ', trim(ifnull(ar.areaname, ''))) Clname2
                        from {opay}.tblotherdeductions d
                             left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(d.acctNumber)
                             join {opis}.empmas a on trim(a.empNumber) = trim(d.empNumber)
                             left join {opis}.client cl on a.client_ = cl.clNumber
                             left join {opis}.area ar on cl.areacode = ar.areacode
                        where d.year = @Y and d.month = @M and d.periodID in (@Pd, @Pd2)
                        order by {order.Replace("ClName2", "Clname2").Replace("AcctName", "Acctname")}";
        sql = sql.Replace("a.EmpLastNm, a.EmpFirstNm", "Emplastnm, Empfirstnm");
        return await _sql.FetchLegacy<R603_Print, dynamic>(sql, new { Y = year, M = month, Pd = pd, Pd2 = pd2 }, conn) ?? [];
    }
}

public interface IDA_603O
{
    Task<List<M603_Client>> _02Clients(string opis, string conn);
    Task<List<M603_Emp>> _02Employees(string opis, string conn);
    Task<List<M603_Acct>> _02Accounts(string opay, string conn);
    Task<List<M603_Period>> _02Periods(string opay, string conn);
    Task<(string? Error, int Rows)> _01Save(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string remarks,
                                            bool p1, bool p2, bool p3, bool p4, bool p5, string opay, string conn);
    Task<List<R603_Print>> _02Print(int year, int month, string period, int group, string opis, string opay, string conn);
    Task<int> _02DeleteCount(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn);
    Task _04Delete(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn);
}

public class M603_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }
public class M603_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }
public class M603_Period { public string PdNumber { get; set; } = ""; public string PdName { get; set; } = ""; }
public class M603_Ded { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; public decimal Amount { get; set; } }

public class M603_Emp
{
    public string EmpNumber { get; set; } = "";
    public string ClNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string Position { get; set; } = "";
    public string EmpStat { get; set; } = "";
    public string Status { get; set; } = "";
    public bool InPayroll { get; set; }
    public DateTime? RegDate { get; set; }
}

public class R603_Print
{
    public string Empnumber { get; set; } = "";
    public string Emplastnm { get; set; } = "";
    public string Empfirstnm { get; set; } = "";
    public string Empmidnm { get; set; } = "";
    public string Empname { get; set; } = "";
    public string Acctname { get; set; } = "";
    public decimal Amount { get; set; }
    public string Remarks { get; set; } = "";
    public int Year { get; set; }
    public int Month { get; set; }
    public string Periodid { get; set; } = "";
    public string Clname2 { get; set; } = "";
}
