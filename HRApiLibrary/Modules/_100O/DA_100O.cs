using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._100O;

public class DA_100O : IDA_100O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_100O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    private async Task<long> Count(string sql, string conn)
    {
        try { return (await _sql.FetchLegacy<long, dynamic>(sql, new { }, conn)).FirstOrDefault(); }
        catch { return 0; }
    }

    private async Task<int> Filled(string table, string idColumn, string opay, string conn)
    {
        try
        {
            var rows = await _sql.FetchLegacy<dynamic, dynamic>($"select * from {opay}.{table} limit 1", new { }, conn);
            if (rows.FirstOrDefault() is not IDictionary<string, object> d) return 0;
            return d.Count(kv => !string.Equals(kv.Key, idColumn, StringComparison.OrdinalIgnoreCase) && kv.Value is string s && s.Trim() != "" && s.Trim() != "0");
        }
        catch { return 0; }
    }

    public async Task<List<M100_Step>> _02Steps(string opis, string opay, string conn)
    {
        long groups = await Count($"select count(*) from {opay}.payrollgrp where Status = 'A'", conn);
        long deployed = await Count($"select count(*) from {opis}.deprec where ifnull(PayrollgrpId, 0) > 0", conn);
        long stat = await Count($"select count(*) from {opis}.empstat where ifnull(isInPayroll, 0) = 1", conn);
        long statAll = await Count($"select count(*) from {opis}.empstat", conn);
        long grades = await Count($"select count(*) from {opay}.salgrade", conn);
        int payMap = await Filled("paymaindtlsetup", "idpaymaindtlsetup", opay, conn);
        int cfgMap = await Filled("acctconfig", "", opay, conn);
        long grpRate = await Count($"select count(*) from {opay}.payrollgrp where Status = 'A' and ifnull(RatePerDay, 0) > 0", conn);
        long empRate = await Count($"select count(*) from {opis}.empmas where ifnull(EmpBasicRate, 0) > 0", conn);
        long mand = await Count($"select count(*) from {opay}.dedmandatory where status = 'A'", conn);
        long sss = await Count($"select count(*) from {opay}.ssstbl", conn);
        long phic = await Count($"select count(*) from {opay}.medtbl", conn);
        long pagibig = await Count($"select count(*) from {opay}.pagibigtbl", conn);
        long tax = await Count($"select count(*) from {opay}.semimonthlytax", conn);

        var done = new Dictionary<string, M100_Step>();
        try
        {
            foreach (var r in await _sql.FetchLegacy<M100_Review, dynamic>($"select StepKey `Key`, ConfirmedBy, ConfirmedAt from {opay}.setupreview", new { }, conn) ?? [])
                done[r.Key] = new M100_Step { ConfirmedBy = r.ConfirmedBy, ConfirmedAt = r.ConfirmedAt };
        }
        catch { }

        M100_Step S(string key, string title, string detail, string link, string linkText, bool missing)
        {
            var s = new M100_Step { Key = key, Title = title, Detail = detail, Link = link, LinkText = linkText };
            if (missing) s.Status = "Missing";
            else if (done.TryGetValue(key, out var c)) { s.Status = "Confirmed"; s.ConfirmedBy = c.ConfirmedBy; s.ConfirmedAt = c.ConfirmedAt; }
            else s.Status = "Review";
            return s;
        }

        return
        [
            S("groups", "Payroll Groups", $"{groups} active group(s), {deployed} employee(s) deployed", "13T/102", "Payroll Group Management", groups == 0),
            S("status", "Employee Status Visible in Payroll", $"{stat} of {statAll} employee status(es) included", "13T/108", "Employee Status", stat == 0),
            S("salgrade", "Salary Grade", $"{grades} salary grade(s)", "13T/104", "Salary Grade", grades == 0),
            S("mapping", "Payroll Account Mapping", $"{payMap} payroll account(s) and {cfgMap} allowance account(s) mapped", "13T/105", "Payroll Account Mapping", payMap == 0),
            S("rates", "Rate Setup", $"{grpRate} of {groups} payroll group(s) have a rate, {empRate} employee(s) have their own rate", "13T/106", "Rate Setup", grpRate == 0 && empRate == 0),
            S("mandatory", "Mandatory Deductions", $"{mand} active mandatory deduction(s)", "13T/605", "Mandatory Deduction", mand == 0),
            S("contrib", "SSS, PhilHealth and Pag-IBIG Tables", $"SSS {sss}, PhilHealth {phic}, Pag-IBIG {pagibig} bracket(s)", "13T/109", "Contribution Tables", sss == 0 || phic == 0 || pagibig == 0),
            S("tax", "Withholding Tax Table", $"{tax} bracket(s)", "13T/111", "Withholding Tax Table", tax == 0),
        ];
    }

    public async Task _03Confirm(string key, string user, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($@"insert into {opay}.setupreview (StepKey, ConfirmedBy, ConfirmedAt) values (@Key, @User, now())
                                       on duplicate key update ConfirmedBy = @User, ConfirmedAt = now()", new { Key = key, User = user }, conn);
    }

    public async Task _03Unconfirm(string key, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {opay}.setupreview where StepKey = @Key", new { Key = key }, conn);
    }
}

public interface IDA_100O
{
    Task<List<M100_Step>> _02Steps(string opis, string opay, string conn);
    Task _03Confirm(string key, string user, string opay, string conn);
    Task _03Unconfirm(string key, string opay, string conn);
}

public class M100_Review
{
    public string Key { get; set; } = "";
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}

public class M100_Step
{
    public string Key { get; set; } = "";
    public string Title { get; set; } = "";
    public string Detail { get; set; } = "";
    public string Link { get; set; } = "";
    public string LinkText { get; set; } = "";
    public string Status { get; set; } = "Review";
    public string? ConfirmedBy { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}
