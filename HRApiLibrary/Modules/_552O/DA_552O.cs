using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._552O;

public class DA_552O : IDA_552O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_552O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M552_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', left(trim(ifnull(e.EmpMidNm, '')), 1), '.') EmpName,
                               trim(ifnull(s.name, '')) Status
                        from {opis}.EmpMas e left join {opis}.empstat s on s.code = e.empstat_
                        where s.inOE = 1
                        order by e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";
        return await _sql.FetchLegacy<M552_Emp, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M552_Earn>> _02Earnings(string emp, string opay, string conn)
    {
        string sql = $@"select a.CONTROL_NO ControlNo, a.Year, a.month Month, a.periodID PeriodId, trim(a.AcctNumber) AcctNumber, trim(ifnull(b.AcctName, a.AcctNumber)) AcctName,
                               a.Amount, a.processed Processed, ifnull(a.remarks, '') Remarks,
                               ifnull(a.p1, 0) P1, ifnull(a.p2, 0) P2, ifnull(a.p3, 0) P3, ifnull(a.p4, 0) P4, ifnull(a.p5, 0) P5,
                               ifnull(a.contproc, 0) ContProc, a.effective Effective
                        from {opay}.TblOtherEarnings a left join {opay}.CHartOfAcct b on a.AcctNumber = b.AcctNumber
                        where trim(a.EmpNumber) = @Emp
                        order by a.Year desc, a.Month desc, a.PeriodID desc, b.AcctName";
        var r = await _sql.FetchLegacy<M552_Earn, dynamic>(sql, new { Emp = emp }, conn) ?? [];
        foreach (var x in r) if (x.Effective is { } d && d.Year < 2) x.Effective = null;
        return r;
    }

    public async Task<List<M552_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where acctType = 'E' and isOthers = '1' order by acctName";
        return await _sql.FetchLegacy<M552_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M552_Period>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) PdNumber, trim(PdName) PdName from {opay}.period where pdNumber != '00' order by pdNumber";
        return await _sql.FetchLegacy<M552_Period, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<string?> _01Save(M552_Earn f, bool isNew, string emp, string opay, string conn)
    {
        if (string.IsNullOrWhiteSpace(f.AcctNumber)) return "Description should not be empty.";
        if (f.Effective == null) return "Please set effectivity date.";
        if (!(f.P1 == 1 || f.P2 == 1 || f.P3 == 1 || f.P4 == 1 || f.P5 == 1)) return "Please select period of deduction.";
        if (f.Amount == 0) return "Amount is invalid.";
        f.PeriodId = f.P1 == 1 ? "01" : f.P2 == 1 ? "02" : f.P3 == 1 ? "03" : f.P4 == 1 ? "04" : "05";

        var p = new { f.ControlNo, Emp = emp, Acct = f.AcctNumber.Trim(), f.Year, f.Month, Pd = f.PeriodId, Eff = f.Effective, f.ContProc, f.Amount, f.Processed,
                      Rem = f.Remarks ?? "", f.P1, f.P2, f.P3, f.P4, f.P5 };

        if (isNew)
        {
            var dup = await _sql.FetchLegacy<long, dynamic>($@"select count(*) from {opay}.tblotherearnings
                        where trim(empnumber) = @Emp and trim(AcctNumber) = @Acct
                          and ((contproc = 0 and year = @Year and month = @Month and periodid = @Pd) or (contproc = 1))", p, conn);
            if ((dup?.FirstOrDefault() ?? 0) > 0) return "Unable to save data.  Existing account found!";

            await _sql.ExecuteLegacyCmd($@"insert into {opay}.tblOtherEarnings (year, month, periodID, effective, contproc, EmpNumber, AcctNumber, Amount, Processed, Remarks, p1, p2, p3, p4, p5)
                        values (@Year, @Month, @Pd, @Eff, @ContProc, @Emp, @Acct, @Amount, 0, @Rem, @P1, @P2, @P3, @P4, @P5)", p, conn);
        }
        else
        {
            await _sql.ExecuteLegacyCmd($@"update {opay}.tblOtherEarnings set Year = @Year, month = @Month, periodID = @Pd, effective = @Eff, contproc = @ContProc,
                        EmpNumber = @Emp, AcctNumber = @Acct, Amount = @Amount, Remarks = @Rem, p1 = @P1, p2 = @P2, p3 = @P3, p4 = @P4, p5 = @P5
                        where CONTROL_NO = @ControlNo", p, conn);
        }
        return null;
    }

    public async Task _03Terminate(int controlNo, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.tblOtherEarnings set contproc = 0 where CONTROL_NO = @C", new { C = controlNo }, conn);
    }

    public async Task<string?> _04Delete(int controlNo, string opay, string conn)
    {
        var pr = await _sql.FetchLegacy<long, dynamic>($"select ifnull(processed, 0) from {opay}.tblOtherEarnings where CONTROL_NO = @C", new { C = controlNo }, conn);
        if ((pr?.FirstOrDefault() ?? 0) > 0) return "Unable to delete posted transaction.";
        await _sql.ExecuteLegacyCmd($"delete from {opay}.tblOtherEarnings where CONTROL_NO = @C", new { C = controlNo }, conn);
        return null;
    }

    public async Task<List<M552_Flag>> _02StatusFlags(string opis, string conn)
    {
        string sql = $@"select trim(code) `Key`, trim(name) Name, if(ifnull(inOE, 0) = 1, 1, 0) `On`
                        from {opis}.empstat where length(code) > 0 and length(trim(name)) > 0 order by isInPayroll desc, name";
        return await _sql.FetchLegacy<M552_Flag, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task _03StatusFlag(string code, bool on, string opis, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opis}.empstat set inOE = @V where code = @C", new { V = on ? 1 : 0, C = code }, conn);
    }

    public async Task<List<M552_Flag>> _02CoaFlags(string opay, string conn)
    {
        string sql = $@"select trim(AcctNumber) `Key`, trim(AcctName) Name, if(ifnull(isOthers, 0) = 1, 1, 0) `On`
                        from {opay}.chartofacct where AcctType = 'E' order by AcctName";
        return await _sql.FetchLegacy<M552_Flag, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task _03CoaFlag(string acct, bool on, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.chartofacct set isOthers = @V where trim(AcctNumber) = @A", new { V = on ? 1 : 0, A = acct }, conn);
    }
}

public interface IDA_552O
{
    Task<List<M552_Emp>> _02Employees(string opis, string conn);
    Task<List<M552_Earn>> _02Earnings(string emp, string opay, string conn);
    Task<List<M552_Acct>> _02Accounts(string opay, string conn);
    Task<List<M552_Period>> _02Periods(string opay, string conn);
    Task<string?> _01Save(M552_Earn f, bool isNew, string emp, string opay, string conn);
    Task _03Terminate(int controlNo, string opay, string conn);
    Task<string?> _04Delete(int controlNo, string opay, string conn);
    Task<List<M552_Flag>> _02StatusFlags(string opis, string conn);
    Task _03StatusFlag(string code, bool on, string opis, string conn);
    Task<List<M552_Flag>> _02CoaFlags(string opay, string conn);
    Task _03CoaFlag(string acct, bool on, string opay, string conn);
}

public class M552_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Status { get; set; } = ""; }
public class M552_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }
public class M552_Period { public string PdNumber { get; set; } = ""; public string PdName { get; set; } = ""; }

public class M552_Flag
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public bool On { get; set; }
}

public class M552_Earn
{
    public int ControlNo { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string PeriodId { get; set; } = "";
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public decimal Amount { get; set; }
    public int Processed { get; set; }
    public string Remarks { get; set; } = "";
    public int P1 { get; set; }
    public int P2 { get; set; }
    public int P3 { get; set; }
    public int P4 { get; set; }
    public int P5 { get; set; }
    public int ContProc { get; set; }
    public DateTime? Effective { get; set; }
    public M552_Earn Copy() => (M552_Earn)MemberwiseClone();
}
