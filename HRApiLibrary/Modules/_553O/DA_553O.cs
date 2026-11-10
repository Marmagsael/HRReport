using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Modules._603O;

namespace HRApiLibrary.Modules._553O;

public class DA_553O : IDA_553O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_553O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M603_Client>> _02Clients(string opis, string conn)
    {
        string sql = $@"select trim(a.clNumber) ClNumber, concat(trim(a.clName), ' - ', trim(ifnull(b.areaname, ''))) ClName
                        from {opis}.client a left join {opis}.area b on a.areacode = b.areacode
                        where a.status = 'Active' order by ClName";
        return await _sql.FetchLegacy<M603_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M553_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(a.empNumber) EmpNumber, trim(ifnull(a.client_, '')) ClNumber,
                               concat(trim(ifnull(a.empLastNm, '')), ', ', trim(ifnull(a.EmpFirstNm, '')), ' ', left(trim(ifnull(a.EmpMidNm, '')), 1)) EmpName,
                               trim(ifnull(b.name, '')) Position, trim(ifnull(a.EmpStat_, '')) EmpStat, trim(ifnull(c.name, '')) Status,
                               if(ifnull(c.isInPayroll, 0) = 1, 1, 0) InPayroll,
                               concat(trim(ifnull(d.clName, '')), ' - ', trim(ifnull(ar.areaname, ''))) ClName
                        from {opis}.empmas a
                             left join {opis}.position b on a.position_ = b.code
                             left join {opis}.empstat c on a.empStat_ = c.code
                             left join {opis}.client d on a.client_ = d.clNumber
                             left join {opis}.area ar on d.areacode = ar.areacode
                        where c.inOE = 1
                        order by a.empLastNm, a.empFirstNm";
        var r = await _sql.FetchLegacy<M553_Emp, dynamic>(sql, new { }, conn) ?? [];
        foreach (var e in r) e.EmpName = (e.EmpName ?? "").Trim();
        return r;
    }

    public async Task<List<M603_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where AcctType = 'E' and isOthers = '1' order by AcctName";
        return await _sql.FetchLegacy<M603_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M603_Period>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) PdNumber, trim(PdName) PdName from {opay}.period where PdNumber != '00' order by PdNumber";
        return await _sql.FetchLegacy<M603_Period, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- The six checks of the FoxPro form, in the same order --
    public string? _02Validate(List<string> emps, List<M603_Ded> deds, bool contProc, bool p1, bool p2, bool p3, bool p4, bool p5, DateTime? effective)
    {
        if (emps.Count < 1) return "No employee selected.";
        var used = deds.Where(d => !string.IsNullOrWhiteSpace(d.AcctNumber)).ToList();
        if (used.Count < 1) return "No earnings selected.";
        if (used.Any(d => d.Amount == 0)) return "ZERO VALUED EARNINGS are not accepted.";
        var dup = used.GroupBy(d => d.AcctNumber.Trim()).FirstOrDefault(g => g.Count() > 1);
        if (dup != null) return "Unable to process double account entry. [EARNING DESCRIPTION -> " + dup.First().AcctName + "]";
        if (contProc && !(p1 || p2 || p3 || p4 || p5)) return "Please select earning period to proceed!";
        if (effective == null) return "Effectivity date is invalid.";
        return null;
    }

    // -- Employee x earning, with the record already saved for it (same period one time, or continuous started on or before the effective date) --
    public async Task<List<M553_Prev>> _02Preview(List<M553_Emp> emps, List<M603_Ded> deds, int year, int month, string period, DateTime effective, string opay, string conn)
    {
        string pd = (period ?? "").Trim();
        var used = deds.Where(d => !string.IsNullOrWhiteSpace(d.AcctNumber)).ToList();
        var accts = used.Select(d => d.AcctNumber.Trim()).Distinct().ToList();
        var nums = emps.Select(e => e.EmpNumber.Trim()).ToList();

        var existing = await _sql.FetchLegacy<M553_Exist, dynamic>($@"select e.CONTROL_NO ControlNo, trim(e.empNumber) EmpNumber, trim(e.acctNumber) AcctNumber,
                        e.amount Amount, e.processed Processed, ifnull(e.contproc, 0) ContProc
                        from {opay}.tblotherearnings e
                        where ((e.contproc = 0 and e.year = @Y and e.month = @M and e.periodid in (@Pd, @Pd2)) or (e.contproc = 1 and e.effective <= @Eff))
                          and trim(e.acctNumber) in @Accts and trim(e.empNumber) in @Nums",
                        new { Y = year, M = month, Pd = pd, Pd2 = pd.TrimStart('0'), Eff = effective, Accts = accts, Nums = nums }, conn) ?? [];

        var rows = new List<M553_Prev>();
        foreach (var e in emps.OrderBy(x => x.EmpName))
            foreach (var d in used.OrderBy(x => x.AcctName))
            {
                var ex = existing.FirstOrDefault(x => x.EmpNumber == e.EmpNumber.Trim() && x.AcctNumber == d.AcctNumber.Trim());
                rows.Add(new M553_Prev
                {
                    EmpNumber = e.EmpNumber.Trim(), EmpName = e.EmpName, ClName = e.ClName,
                    AcctNumber = d.AcctNumber.Trim(), AcctName = d.AcctName, NewAmt = d.Amount,
                    ControlNo = ex?.ControlNo ?? 0, ExistingAmt = ex?.Amount ?? 0, Processed = ex?.Processed ?? 0, ContProc = ex?.ContProc ?? 0,
                    Apply = ex == null || (ex.Processed == 0 && ex.ContProc == 0)
                });
            }
        return rows;
    }

    // -- New entries are inserted; existing ones only get the new amount when they are ticked, not processed and not continuous --
    public async Task<(int Inserted, int Updated)> _01Commit(List<M553_Prev> rows, int year, int month, string period, string remarks, DateTime effective, bool contProc,
                                                            bool p1, bool p2, bool p3, bool p4, bool p5, string opay, string conn)
    {
        int ins = 0, upd = 0;
        var news = rows.Where(r => r.ControlNo == 0 && r.Apply).ToList();
        foreach (var chunk in news.Chunk(200))
        {
            var dp = new DynamicParameters();
            dp.Add("Y", year); dp.Add("M", month); dp.Add("Pd", period); dp.Add("Rem", remarks ?? ""); dp.Add("Eff", effective); dp.Add("Cp", contProc ? 1 : 0);
            dp.Add("P1", p1 ? 1 : 0); dp.Add("P2", p2 ? 1 : 0); dp.Add("P3", p3 ? 1 : 0); dp.Add("P4", p4 ? 1 : 0); dp.Add("P5", p5 ? 1 : 0);
            var vals = new List<string>();
            for (int i = 0; i < chunk.Length; i++)
            {
                dp.Add("E" + i, chunk[i].EmpNumber); dp.Add("A" + i, chunk[i].AcctNumber); dp.Add("V" + i, Math.Round(chunk[i].NewAmt, 2));
                vals.Add($"(@Y, @M, @Pd, @E{i}, @A{i}, @V{i}, 0, @Rem, @Eff, @Cp, @P1, @P2, @P3, @P4, @P5)");
            }
            await _sql.ExecuteLegacyCmd($@"insert into {opay}.tblotherearnings (year, month, periodID, empNumber, acctNumber, amount, processed, Remarks, effective, contproc, p1, p2, p3, p4, p5)
                                          values {string.Join(", ", vals)}", dp, conn);
            ins += chunk.Length;
        }

        foreach (var r in rows.Where(x => x.ControlNo > 0 && x.Apply && x.ContProc == 0 && x.Processed == 0))
        {
            await _sql.ExecuteLegacyCmd($"update {opay}.tblotherearnings set amount = @A where CONTROL_NO = @C and processed = 0", new { A = Math.Round(r.NewAmt, 2), C = r.ControlNo }, conn);
            upd++;
        }
        return (ins, upd);
    }

    private static object DelParams(List<string> emps, List<M603_Ded> deds, int year, int month, string period)
    {
        string pd = (period ?? "").Trim();
        return new { Y = year, M = month, Pd = pd, Pd2 = pd.TrimStart('0'), Emps = emps.Select(e => e.Trim()).ToList(),
                     Accts = deds.Where(d => !string.IsNullOrWhiteSpace(d.AcctNumber)).Select(d => d.AcctNumber.Trim()).Distinct().ToList() };
    }

    private const string DelWhere = @"year = @Y and month = @M and periodID in (@Pd, @Pd2) and processed = 0 and contproc = 0
                                      and trim(empNumber) in @Emps and trim(acctNumber) in @Accts";

    public async Task<int> _02DeleteCount(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn)
    {
        if (emps.Count < 1 || !deds.Any(d => !string.IsNullOrWhiteSpace(d.AcctNumber))) return 0;
        var r = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.tblotherearnings where {DelWhere}", DelParams(emps, deds, year, month, period), conn);
        return (int)(r?.FirstOrDefault() ?? 0);
    }

    public async Task _04Delete(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn)
    {
        if (emps.Count < 1 || !deds.Any(d => !string.IsNullOrWhiteSpace(d.AcctNumber))) return;
        await _sql.ExecuteLegacyCmd($"delete from {opay}.tblotherearnings where {DelWhere}", DelParams(emps, deds, year, month, period), conn);
    }

    public async Task<List<R603_Print>> _02Print(int year, int month, string period, int group, string opis, string opay, string conn)
    {
        string pd = (period ?? "").Trim();
        string order = group switch { 2 => "Clname2, Emplastnm, Empfirstnm", 3 => "Acctname, Emplastnm, Empfirstnm", _ => "Emplastnm, Empfirstnm" };
        string sql = $@"select trim(d.empNumber) Empnumber, trim(ifnull(a.EmpLastNm, '')) Emplastnm, trim(ifnull(a.EmpFirstNm, '')) Empfirstnm, trim(ifnull(a.EmpMidNm, '')) Empmidnm,
                               trim(ifnull(c.AcctName, d.acctNumber)) Acctname, d.amount Amount, ifnull(d.remarks, '') Remarks,
                               d.year Year, d.month Month, @Pd Periodid,
                               concat(trim(ifnull(a.EmpLastNm, '')), ', ', trim(ifnull(a.EmpFirstNm, '')), ' ', left(trim(ifnull(a.EmpMidNm, '')), 1), '.') Empname,
                               concat(trim(ifnull(cl.clName, '')), ' - ', trim(ifnull(ar.areaname, ''))) Clname2
                        from {opay}.tblotherearnings d
                             left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(d.acctNumber)
                             join {opis}.empmas a on trim(a.empNumber) = trim(d.empNumber)
                             left join {opis}.client cl on a.client_ = cl.clNumber
                             left join {opis}.area ar on cl.areacode = ar.areacode
                        where d.year = @Y and d.month = @M and d.periodID in (@Pd, @Pd2)
                        order by {order}";
        return await _sql.FetchLegacy<R603_Print, dynamic>(sql, new { Y = year, M = month, Pd = pd, Pd2 = pd.TrimStart('0') }, conn) ?? [];
    }
}

public interface IDA_553O
{
    Task<List<M603_Client>> _02Clients(string opis, string conn);
    Task<List<M553_Emp>> _02Employees(string opis, string conn);
    Task<List<M603_Acct>> _02Accounts(string opay, string conn);
    Task<List<M603_Period>> _02Periods(string opay, string conn);
    string? _02Validate(List<string> emps, List<M603_Ded> deds, bool contProc, bool p1, bool p2, bool p3, bool p4, bool p5, DateTime? effective);
    Task<List<M553_Prev>> _02Preview(List<M553_Emp> emps, List<M603_Ded> deds, int year, int month, string period, DateTime effective, string opay, string conn);
    Task<(int Inserted, int Updated)> _01Commit(List<M553_Prev> rows, int year, int month, string period, string remarks, DateTime effective, bool contProc,
                                                bool p1, bool p2, bool p3, bool p4, bool p5, string opay, string conn);
    Task<int> _02DeleteCount(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn);
    Task _04Delete(List<string> emps, List<M603_Ded> deds, int year, int month, string period, string opay, string conn);
    Task<List<R603_Print>> _02Print(int year, int month, string period, int group, string opis, string opay, string conn);
}

public class M553_Emp
{
    public string EmpNumber { get; set; } = "";
    public string ClNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string Position { get; set; } = "";
    public string EmpStat { get; set; } = "";
    public string Status { get; set; } = "";
    public bool InPayroll { get; set; }
    public string ClName { get; set; } = "";
}

public class M553_Exist
{
    public int ControlNo { get; set; }
    public string EmpNumber { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public decimal Amount { get; set; }
    public int Processed { get; set; }
    public int ContProc { get; set; }
}

public class M553_Prev
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string ClName { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal NewAmt { get; set; }
    public int ControlNo { get; set; }
    public decimal ExistingAmt { get; set; }
    public int Processed { get; set; }
    public int ContProc { get; set; }
    public bool Apply { get; set; }
}
