using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OSalgradeDataAccess : IOSalgradeDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OSalgradeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<SalgradeModel>> _02(string schema, string conn)
    {
        var sql = $@"select trim(sgcode) SgCode, trim(sgrade) SGrade, monthlyRate MonthlyRate, dailyRate DailyRate,
                            wtax WTax, wSSS WSss, wGSIS WGsis, wPHIC WPhic, wPagibig WPagibig, isLock IsLock
                     from {schema}.salgrade order by sgrade";
        return await _sql.FetchLegacy<SalgradeModel, dynamic>(sql, new { }, conn) ?? new();
    }

    public async Task<string?> _01(SalgradeModel m, string schema, string conn)
    {
        var msg = Validate(m);
        if (msg != null) return msg;

        var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.salgrade where trim(sgcode) = @SgCode", new { SgCode = m.SgCode!.Trim() }, conn);
        if (dup.FirstOrDefault() > 0) return $"Salary grade code {m.SgCode!.Trim()} already exists.";

        await _sql.ExecuteLegacyCmd($@"insert into {schema}.salgrade (sgcode, sgrade, monthlyRate, dailyRate, wtax, wSSS, wGSIS, wPHIC, wPagibig, isLock)
                                       values (@SgCode, @SGrade, @MonthlyRate, @DailyRate, @WTax, @WSSS, @WGSIS, @WPHIC, @WPagibig, 0)",
                                    new { SgCode = m.SgCode!.Trim(), SGrade = m.SGrade!.Trim(), m.MonthlyRate, m.DailyRate, WTax = m.WTax ?? 0, WSSS = m.WSss ?? 0, WGSIS = m.WGsis ?? 0, WPHIC = m.WPhic ?? 0, WPagibig = m.WPagibig ?? 0 }, conn);
        return null;
    }

    public async Task<string?> _03(SalgradeModel m, string schema, string conn)
    {
        var msg = Validate(m);
        if (msg != null) return msg;

        var cur = await _sql.FetchLegacy<int, dynamic>($"select isLock from {schema}.salgrade where trim(sgcode) = @SgCode", new { SgCode = m.SgCode!.Trim() }, conn);
        if (cur.Count == 0) return "Salary grade was not found.";
        if (cur[0] == 1) return "This salary grade is locked. Unlock it first to change it.";

        await _sql.ExecuteLegacyCmd($@"update {schema}.salgrade set sgrade = @SGrade, monthlyRate = @MonthlyRate, dailyRate = @DailyRate,
                                              wtax = @WTax, wSSS = @WSSS, wGSIS = @WGSIS, wPHIC = @WPHIC, wPagibig = @WPagibig
                                       where trim(sgcode) = @SgCode and isLock = 0",
                                    new { SgCode = m.SgCode!.Trim(), SGrade = m.SGrade!.Trim(), m.MonthlyRate, m.DailyRate, WTax = m.WTax ?? 0, WSSS = m.WSss ?? 0, WGSIS = m.WGsis ?? 0, WPHIC = m.WPhic ?? 0, WPagibig = m.WPagibig ?? 0 }, conn);
        return null;
    }

    public async Task _03Lock(string sgCode, bool locked, string schema, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {schema}.salgrade set isLock = @IsLock where trim(sgcode) = @SgCode",
                                    new { SgCode = sgCode.Trim(), IsLock = locked ? 1 : 0 }, conn);
    }

    public async Task<string?> _04(string sgCode, string paySchema, string pisSchema, string conn)
    {
        var cur = await _sql.FetchLegacy<int, dynamic>($"select isLock from {paySchema}.salgrade where trim(sgcode) = @SgCode", new { SgCode = sgCode.Trim() }, conn);
        if (cur.Count == 0) return "Salary grade was not found.";
        if (cur[0] == 1) return "This salary grade is locked. Unlock it first to delete it.";

        var used = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {pisSchema}.empmas where trim(sgcode) = @SgCode", new { SgCode = sgCode.Trim() }, conn);
        if (used.FirstOrDefault() > 0) return $"{used.First()} employee(s) use this salary grade. It cannot be deleted.";

        await _sql.ExecuteLegacyCmd($"delete from {paySchema}.salgrade where trim(sgcode) = @SgCode and isLock = 0", new { SgCode = sgCode.Trim() }, conn);
        return null;
    }

    private static string? Validate(SalgradeModel m)
    {
        if (string.IsNullOrWhiteSpace(m.SgCode))    return "Enter the salary grade code.";
        if (m.SgCode.Trim().Length > 10)            return "The code can have 10 characters at most.";
        if (string.IsNullOrWhiteSpace(m.SGrade))    return "Enter the salary grade name.";
        if (m.SGrade.Trim().Length > 45)            return "The name can have 45 characters at most.";
        if (m.MonthlyRate <= 0)                     return "Monthly rate is not valid.";
        if (m.DailyRate <= 0)                       return "Daily rate is not valid.";
        return null;
    }
}

public interface IOSalgradeDataAccess
{
    Task<List<SalgradeModel>>   _02(string schema, string conn);
    Task<string?>               _01(SalgradeModel m, string schema, string conn);
    Task<string?>               _03(SalgradeModel m, string schema, string conn);
    Task                        _03Lock(string sgCode, bool locked, string schema, string conn);
    Task<string?>               _04(string sgCode, string paySchema, string pisSchema, string conn);
}
