using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._606O;

public class DA_606O : IDA_606O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_606O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M606_Period>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) PdNumber, trim(PdName) PdName from {opay}.period where PdNumber != '00' order by PdNumber";
        return await _sql.FetchLegacy<M606_Period, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M606_Acct>> _02DeductionAccounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where left(AcctNumber, 1) = 'D' order by AcctName";
        return await _sql.FetchLegacy<M606_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M606_HAcct>> _02Accounts(string opay, string conn)
    {
        string sql = $@"select h.id Id, trim(h.acctnumber) AcctNumber, ifnull(trim(c.AcctName), '') AcctName, trim(ifnull(h.savingsacct, '')) SavingsAcct,
                               ifnull(trim(c1.AcctName), '') SavingsAcctName, ifnull(h.fund, 0) Fund, ifnull(h.amort, 0) Amort,
                               ifnull(h.isdefault, 0) IsDefault, ifnull(h.remarks, '') Remarks
                        from {opay}.hospitalizationacct h
                             left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(h.acctnumber)
                             left join {opay}.chartofacct c1 on trim(c1.AcctNumber) = trim(h.savingsacct)
                        order by c.AcctName";
        return await _sql.FetchLegacy<M606_HAcct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<string?> _01SaveAccount(M606_HAcct a, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(a.AcctNumber)) return "Please select account to proceed!";
        var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.hospitalizationacct where trim(acctnumber) = @A and id <> @Id", new { A = a.AcctNumber.Trim(), a.Id }, conn);
        if ((dup?.FirstOrDefault() ?? 0) > 0) return "Unable to save data, account already exists.";
        if (a.IsDefault == 1) await _sql.ExecuteLegacyCmd($"update {opay}.hospitalizationacct set isdefault = 0", new { }, conn);
        var p = new { A = a.AcctNumber.Trim(), S = (a.SavingsAcct ?? "").Trim(), a.Fund, a.Amort, D = a.IsDefault, R = a.Remarks ?? "", a.Id };
        if (a.Id == 0)
            await _sql.ExecuteLegacyCmd($"insert into {opay}.hospitalizationacct (acctnumber, savingsacct, fund, amort, isdefault, remarks) values (@A, @S, @Fund, @Amort, @D, @R)", p, conn);
        else
            await _sql.ExecuteLegacyCmd($"update {opay}.hospitalizationacct set acctnumber = @A, savingsacct = @S, fund = @Fund, amort = @Amort, isdefault = @D, remarks = @R where id = @Id", p, conn);
        return null;
    }

    public async Task _04DeleteAccount(int id, string opay, string conn)
        => await _sql.ExecuteLegacyCmd($"delete from {opay}.hospitalizationacct where id = @Id", new { Id = id }, conn);

    public async Task<List<M606_Emp>> _02Employees(string opis, string opay, string conn)
    {
        string sql = $@"select trim(e.empNumber) EmpNumber,
                               concat(trim(ifnull(e.empLastNm, '')), ', ', trim(ifnull(e.empFirstNm, '')), ' ', trim(ifnull(e.empMidNm, ''))) EmpName,
                               ifnull(trim(s.name), '') EmpStatus,
                               ifnull((select trim(g.Name) from {opis}.deprec d join {opay}.payrollgrp g on g.Id = d.PayrollgrpId where d.EmpNumber = e.EmpNumber limit 1), '') PayrollGroup,
                               ifnull(trim(h.acctnumber), '') AcctNumber
                        from {opis}.empmas e
                             left join {opis}.empstat s on s.code = e.empStat_
                             left join {opay}.hospitalizationgrp h on trim(h.empnumber) = trim(e.empNumber)
                        where e.empNumber is not null and e.empStat_ in (select code from {opay}.hospitalizationstatus)
                        order by e.empLastNm, e.empFirstNm";
        var r = await _sql.FetchLegacy<M606_Emp, dynamic>(sql, new { }, conn) ?? [];
        foreach (var e in r) e.EmpName = (e.EmpName ?? "").Trim();
        return r;
    }

    public async Task<List<M606_Hdr>> _02Header(int year, string mo, string prd, string opay, string conn)
    {
        string sql = $"select trim(acctnumber) AcctNumber, ifnull(ded, 0) Ded, ifnull(totded, 0) TotDed, ifnull(isdefault, 0) IsDefault from {opay}.hospitalizationhdr where year = @Y and mo = @M and prd = @P";
        return await _sql.FetchLegacy<M606_Hdr, dynamic>(sql, new { Y = year, M = mo, P = prd }, conn) ?? [];
    }

    public async Task _01SaveHeader(int year, string mo, string prd, List<M606_Hdr> rows, string opay, string conn)
    {
        foreach (var r in rows)
            await _sql.ExecuteLegacyCmd($@"insert into {opay}.hospitalizationhdr (year, mo, prd, acctnumber, ded, totded, isdefault) values (@Y, @M, @P, @A, @D, @T, @I)
                                           on duplicate key update ded = @D, totded = @T, isdefault = @I",
                new { Y = year, M = mo, P = prd, A = r.AcctNumber.Trim(), D = r.Ded, T = r.TotDed, I = r.IsDefault }, conn);
    }

    public async Task<int> _01PostDeductions(int year, string mo, string prd, List<M606_Post> rows, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"insert into {opay}.hospitalizationdtlrchieved select * from {opay}.hospitalizationdtl where length(trnposted) > 0", new { }, conn);
        await _sql.ExecuteLegacyCmd($"delete from {opay}.hospitalizationdtl where length(trnposted) > 0", new { }, conn);
        await _sql.ExecuteLegacyCmd($"delete from {opay}.hospitalizationdtl where year = @Y and mo = @M and prd = @P", new { Y = year, M = mo, P = prd }, conn);
        int n = 0;
        foreach (var chunk in rows.Where(r => r.Ded > 0).Chunk(200))
        {
            var dp = new DynamicParameters();
            dp.Add("Y", year); dp.Add("M", mo); dp.Add("P", prd);
            var vals = new List<string>();
            for (int i = 0; i < chunk.Length; i++)
            {
                dp.Add("A" + i, chunk[i].AcctNumber.Trim()); dp.Add("E" + i, chunk[i].EmpNumber.Trim()); dp.Add("D" + i, Math.Round(chunk[i].Ded, 2));
                vals.Add($"(@Y, @M, @P, @A{i}, @E{i}, @D{i})");
            }
            await _sql.ExecuteLegacyCmd($"insert into {opay}.hospitalizationdtl (year, mo, prd, acctnumber, empnumber, ded) values {string.Join(", ", vals)}", dp, conn);
            n += chunk.Length;
        }
        return n;
    }

    public async Task _01Assign(List<string> emps, string acct, string opay, string conn)
    {
        foreach (var chunk in emps.Chunk(200))
        {
            var dp = new DynamicParameters();
            dp.Add("A", acct.Trim());
            var vals = new List<string>();
            for (int i = 0; i < chunk.Length; i++) { dp.Add("E" + i, chunk[i].Trim()); vals.Add($"(@E{i}, @A)"); }
            await _sql.ExecuteLegacyCmd($"insert into {opay}.hospitalizationgrp (empnumber, acctnumber) values {string.Join(", ", vals)} on duplicate key update acctnumber = @A", dp, conn);
        }
    }

    public async Task _04Remove(List<string> emps, string opay, string conn)
    {
        foreach (var chunk in emps.Chunk(200))
            await _sql.ExecuteLegacyCmd($"delete from {opay}.hospitalizationgrp where trim(empnumber) in @E", new { E = chunk.Select(x => x.Trim()).ToList() }, conn);
    }

    public async Task<List<M606_Status>> _02Statuses(string opis, string opay, string conn)
    {
        string sql = $@"select trim(s.code) Code, trim(s.name) Name, if(h.code is null, 0, 1) Included
                        from {opis}.empstat s left join {opay}.hospitalizationstatus h on trim(h.code) = trim(s.code) order by s.name";
        return await _sql.FetchLegacy<M606_Status, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task _01SetStatus(string code, bool on, string opay, string conn)
    {
        if (on) await _sql.ExecuteLegacyCmd($"insert ignore into {opay}.hospitalizationstatus (code) values (@C)", new { C = code.Trim() }, conn);
        else await _sql.ExecuteLegacyCmd($"delete from {opay}.hospitalizationstatus where trim(code) = @C", new { C = code.Trim() }, conn);
    }

    public async Task<List<M606_QEmp>> _02QueryEmployees(string opis, string opay, string conn)
    {
        string sql = $@"select trim(e.empNumber) EmpNumber,
                               concat(trim(ifnull(e.empLastNm, '')), ', ', trim(ifnull(e.empFirstNm, '')), ' ', trim(ifnull(e.empMidNm, ''))) EmpName,
                               ifnull(trim(s.name), '') EmpStatus,
                               ifnull((select trim(g.Name) from {opis}.deprec d join {opay}.payrollgrp g on g.Id = d.PayrollgrpId where d.EmpNumber = e.EmpNumber limit 1), '') PayrollGroup
                        from {opis}.empmas e left join {opis}.empstat s on s.code = e.empStat_
                        where e.empNumber is not null and ifnull(s.isInPayroll, 0) = 1
                        order by e.empLastNm, e.empFirstNm";
        return await _sql.FetchLegacy<M606_QEmp, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M606_Tran>> _02Savings(string empNumber, string opay, string conn)
    {
        string sql = $@"select trim(t.trn) Trn, trim(t.acctNumber) AcctNumber, ifnull(trim(c.AcctName), '') AcctName, ifnull(t.amount, 0) Amount
                        from {opay}.tbltran t left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(t.acctNumber)
                        where trim(t.empNumber) = @E and trim(t.acctNumber) in (select trim(savingsacct) from {opay}.hospitalizationacct)
                        order by t.trn";
        return await _sql.FetchLegacy<M606_Tran, dynamic>(sql, new { E = empNumber.Trim() }, conn) ?? [];
    }

    public async Task<string?> _01Transfer(string empNumber, string from, string to, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(to)) return "Please select account from the list.";
        if (from.Trim() == to.Trim()) return "Origin and destination account is the same.";
        var amt = await _sql.FetchLegacy<double, dynamic>($"select ifnull(sum(amount), 0) from {opay}.tbltran where trim(empNumber) = @E and trim(acctNumber) = @F", new { E = empNumber.Trim(), F = from.Trim() }, conn);
        if ((amt?.FirstOrDefault() ?? 0) == 0) return "Origin amount is zero.";
        await _sql.ExecuteLegacyCmd($"update {opay}.tbltran set acctNumber = @T where trim(empNumber) = @E and trim(acctNumber) = @F", new { E = empNumber.Trim(), F = from.Trim(), T = to.Trim() }, conn);
        await _sql.ExecuteLegacyCmd($@"insert into {opay}.hospitalizationsavings (empnumber, saving)
                                       select @E, ifnull(sum(amount), 0) from {opay}.tbltran where trim(empNumber) = @E and trim(acctNumber) in (select trim(savingsacct) from {opay}.hospitalizationacct)
                                       on duplicate key update saving = values(saving)", new { E = empNumber.Trim() }, conn);
        return null;
    }
}

public interface IDA_606O
{
    Task<List<M606_Period>> _02Periods(string opay, string conn);
    Task<List<M606_Acct>> _02DeductionAccounts(string opay, string conn);
    Task<List<M606_HAcct>> _02Accounts(string opay, string conn);
    Task<string?> _01SaveAccount(M606_HAcct a, string opay, string conn);
    Task _04DeleteAccount(int id, string opay, string conn);
    Task<List<M606_Emp>> _02Employees(string opis, string opay, string conn);
    Task<List<M606_Hdr>> _02Header(int year, string mo, string prd, string opay, string conn);
    Task _01SaveHeader(int year, string mo, string prd, List<M606_Hdr> rows, string opay, string conn);
    Task<int> _01PostDeductions(int year, string mo, string prd, List<M606_Post> rows, string opay, string conn);
    Task _01Assign(List<string> emps, string acct, string opay, string conn);
    Task _04Remove(List<string> emps, string opay, string conn);
    Task<List<M606_Status>> _02Statuses(string opis, string opay, string conn);
    Task _01SetStatus(string code, bool on, string opay, string conn);
    Task<List<M606_QEmp>> _02QueryEmployees(string opis, string opay, string conn);
    Task<List<M606_Tran>> _02Savings(string empNumber, string opay, string conn);
    Task<string?> _01Transfer(string empNumber, string from, string to, string opay, string conn);
}

public class M606_Period { public string PdNumber { get; set; } = ""; public string PdName { get; set; } = ""; }
public class M606_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }

public class M606_HAcct
{
    public int Id { get; set; }
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public string SavingsAcct { get; set; } = "";
    public string SavingsAcctName { get; set; } = "";
    public decimal Fund { get; set; }
    public decimal Amort { get; set; }
    public int IsDefault { get; set; }
    public string Remarks { get; set; } = "";
}

public class M606_Emp
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string EmpStatus { get; set; } = "";
    public string PayrollGroup { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal Ded { get; set; }
}

public class M606_Hdr
{
    public string AcctNumber { get; set; } = "";
    public decimal Ded { get; set; }
    public decimal TotDed { get; set; }
    public int IsDefault { get; set; }
}

public class M606_Post { public string EmpNumber { get; set; } = ""; public string AcctNumber { get; set; } = ""; public decimal Ded { get; set; } }
public class M606_Status { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public int Included { get; set; } }

public class M606_QEmp
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string EmpStatus { get; set; } = "";
    public string PayrollGroup { get; set; } = "";
}

public class M606_Tran
{
    public string Trn { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal Amount { get; set; }
}
