using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.DataAccess._90_Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Dapper;

namespace HRApiLibrary.Tests._20_Pay;

//--- Recomputes locked TRNs of the local MBPay database (pay / secpis) and compares with what FoxPro saved in tbltran. ----
//--- Runs only when OPAY_PARITY_CONN is set, e.g. "server=127.0.0.1;Port=3306;uid=root;pwd=...;"
sealed class OneConn(string conn) : IConfiguration
{
    public string? this[string key] { get => key == "ConnectionStrings:legacy" ? conn : null; set { } }
    public IEnumerable<IConfigurationSection> GetChildren() => [];
    public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken() => throw new NotSupportedException();
    public IConfigurationSection GetSection(string key) => new OneSection(conn);
}

sealed class OneSection(string conn) : IConfigurationSection
{
    public string? this[string key] { get => conn; set { } }
    public string Key => "ConnectionStrings";
    public string Path => "ConnectionStrings";
    public string? Value { get => conn; set { } }
    public IEnumerable<IConfigurationSection> GetChildren() => [];
    public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken() => throw new NotSupportedException();
    public IConfigurationSection GetSection(string key) => this;
}

[TestClass]
public class OPayrollParityTests
{
    [TestMethod]
    public async Task LockedTrns_MatchFoxProTbltran()
    {
        var conn = Environment.GetEnvironmentVariable("OPAY_PARITY_CONN");
        if (string.IsNullOrWhiteSpace(conn)) { Assert.Inconclusive("OPAY_PARITY_CONN is not set."); return; }

        var cfg = new OneConn(conn);
        var da = new OPayrollDataAccess(new _90_001_MySqlDataAccess(cfg));

        using var db = new MySqlConnection(conn + ";database=pay;Convert Zero Datetime=True");
        var take = int.TryParse(Environment.GetEnvironmentVariable("OPAY_PARITY_TAKE"), out var n) ? n : 40;
        var trns = (await db.QueryAsync<(string Trn, DateTime AttStart, DateTime AttEnd)>(
            $"select trn, attStart, attEnd from paymainhdr where status = 'Lock' order by dateposted desc limit {take}")).ToList();

        var log = new System.Text.StringBuilder();
        int emps = 0, exact = 0, trnOk = 0;
        foreach (var t in trns)
        {
            var code = t.Trn[7..];
            var s = await da._01Load("20" + t.Trn[..2], t.Trn[2..4], t.Trn[4..6], code, t.AttStart, t.AttEnd, "pay", "secpis", "legacy");
            if (s.Client.Id == 0 || s.Results.Count == 0) continue;

            var saved = (await db.QueryAsync<(string Emp, string Acct, double Amt)>(
                "select empnumber, trim(acctnumber), amount from tbltran where trn = @t", new { t = t.Trn }))
                .GroupBy(x => x.Emp).ToDictionary(g => g.Key.Trim(), g => g.ToDictionary(x => x.Acct, x => x.Amt));

            //--- earnings the TRN already posted : bring them back as if they were still open ---
            var nums = s.Results.Select(x => x.EmpNumber!).ToList();
            s.FixedEarnings.AddRange(await db.QueryAsync<HRApiLibrary.Models._20_Pay.OPay.OPayFixedEarning>(
                "select idfixed_earnings Id, trim(empnumber) EmpNumber, trim(acctNumber) AcctNumber, amount Amount, ifnull(dayspara,0) DaysPara from fixed_earnings where trnPosted = @t and empnumber in @n", new { t = t.Trn, n = nums }));
            s.OtherEarnings.AddRange(await db.QueryAsync<HRApiLibrary.Models._20_Pay.OPay.OPayOtherEarning>(
                "select CONTROL_NO ControlNo, trim(empNumber) EmpNumber, trim(acctNumber) AcctNumber, amount Amount from tblotherearnings where trnposted = @t and empnumber in @n", new { t = t.Trn, n = nums }));

            s.Status = "New";
            s.SavedMonth.RemoveAll(x => x.Trn == s.Trn);
            OPayrollCompute.Compute(s);

            int bad = 0;
            foreach (var r in s.Results)
            {
                emps++;
                saved.TryGetValue(r.EmpNumber!, out var exp); exp ??= [];
                var accts = r.Amounts.Where(a => a.Value > 0).Select(a => a.Key).Concat(exp.Keys)
                             .Where(a => a.StartsWith("E") || a is "D001" or "D002" or "D003" or "D004").Distinct().ToList();
                var diffs = accts.Where(a => Math.Abs((r.Amounts.TryGetValue(a, out var v) && v > 0 ? v : 0) - (exp.TryGetValue(a, out var e) ? e : 0)) > 0.05).ToList();
                if (diffs.Count == 0) { exact++; continue; }
                bad++;
                log.AppendLine($"{t.Trn} {r.EmpNumber} {r.Name}: " + string.Join("; ", diffs.Select(a =>
                    $"{a} calc={(r.Amounts.TryGetValue(a, out var v2) ? v2 : 0):N2} saved={(exp.TryGetValue(a, out var e2) ? e2 : 0):N2}")));
            }
            if (bad == 0) trnOk++;
        }

        log.AppendLine($"TRNs {trns.Count}, clean {trnOk}; employees {emps}, exact {exact}");
        var path = Environment.GetEnvironmentVariable("OPAY_PARITY_LOG");
        if (!string.IsNullOrWhiteSpace(path)) File.WriteAllText(path, log.ToString());
        Console.WriteLine(log.ToString());
        Assert.IsTrue(emps > 0, "No locked employees were compared.");
    }
}
