using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OManhourPushDataAccess : IOManhourPushDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OManhourPushDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public static readonly string[] Columns =
    [
        "daywrk", "absent", "late", "utime", "rn", "rnot", "rot", "rd", "rdot", "lh", "lhot", "rdlh", "rdlhot", "sh", "shot", "rdsh", "rdshot",
        "dh", "dhot", "rddh", "rddhot", "nddh", "nddhot", "ndrddh", "ndrddhot",
        "nd", "ndot", "ndrd", "ndrdot", "ndlh", "ndlhot", "ndrdlh", "ndrdlhot", "ndsh", "ndshot", "ndrdsh", "ndrdshot"
    ];

    public async Task<OManhourTrnInfo?> _02TrnInfo(string trn, string paydb, string conn)
    {
        var sql = $@"select 'Lock' Status, attStart AttStart, attEnd AttEnd from {paydb}.paymainhdr where trn = @Trn and status = 'Lock'
                     union all
                     select status, attStart, attEnd from {paydb}.tmppaymainhdr where trn = @Trn";
        var r = await _sql.FetchLegacy<OManhourTrnInfo, dynamic>(sql, new { Trn = (trn ?? "").Trim() }, conn);
        return r.FirstOrDefault();
    }

    public async Task<Dictionary<string, int>> _02RateIds(List<string> empNumbers, string opis, string conn)
    {
        var nums = empNumbers.Select(x => x.Trim()).ToList();
        var r = await _sql.FetchLegacy<OEmpRateRow, dynamic>($"select trim(empnumber) EmpNumber, ifnull(rateID, 2) RateId from {opis}.empmas where trim(empnumber) in @Nums", new { Nums = nums }, conn);
        return r.GroupBy(x => x.EmpNumber, StringComparer.OrdinalIgnoreCase).ToDictionary(g => g.Key, g => g.First().RateId, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<(int Updated, List<string> Skipped)> _03Push(string trn, Dictionary<string, Dictionary<string, double>> byEmp, string paydb, string conn)
    {
        var nums     = byEmp.Keys.Select(x => x.Trim()).ToList();
        var existing = (await _sql.FetchLegacy<string, dynamic>($"select trim(empnumber) from {paydb}.tmppaymaindtl where trn = @Trn and trim(empnumber) in @Nums", new { Trn = trn, Nums = nums }, conn))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var setList = string.Join(", ", Columns.Select(c => $"{c} = @{c}"));
        int updated = 0;
        var skipped = new List<string>();

        foreach (var (emp, vals) in byEmp)
        {
            var key = emp.Trim();
            if (!existing.Contains(key)) { skipped.Add(key); continue; }

            var p = new DynamicParameters();
            p.Add("Trn", trn);
            p.Add("Emp", key);
            foreach (var c in Columns) p.Add(c, Math.Round(vals.TryGetValue(c, out var v) ? v : 0, 4));

            await _sql.ExecuteLegacyCmd($"update {paydb}.tmppaymaindtl set {setList} where trn = @Trn and trim(empnumber) = @Emp", p, conn);
            updated++;
        }
        return (updated, skipped);
    }
}

public interface IOManhourPushDataAccess
{
    Task<OManhourTrnInfo?> _02TrnInfo(string trn, string paydb, string conn);
    Task<Dictionary<string, int>> _02RateIds(List<string> empNumbers, string opis, string conn);
    Task<(int Updated, List<string> Skipped)> _03Push(string trn, Dictionary<string, Dictionary<string, double>> byEmp, string paydb, string conn);
}

public class OEmpRateRow
{
    public string EmpNumber { get; set; } = string.Empty;
    public int    RateId    { get; set; } = 2;
}

public class OManhourTrnInfo
{
    public string?   Status   { get; set; }
    public DateTime  AttStart { get; set; }
    public DateTime  AttEnd   { get; set; }
}
