using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._1305O;

public class DA_1305O : IDA_1305O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_1305O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    class Combo { public string Prd { get; set; } = ""; public string ClNumber { get; set; } = ""; }
    class Dep { public string Trn { get; set; } = ""; public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }

    async Task<List<Dep>> Deployment(string y1, string y2, string opis, string opay, string conn)
    {
        string sql = $@"select trim(t.trn) Trn, trim(t.clnumber) ClNumber, trim(ifnull(c.clname, '')) ClName
                        from {opay}.tbltrandeployment t left join {opis}.client c on c.clnumber = t.clnumber
                        where left(t.trn, 4) between @A and @B";
        return await _sql.FetchLegacy<Dep, dynamic>(sql, new { A = y1, B = y2 }, conn) ?? [];
    }

    public async Task<M1305_FetchResult> _02Fetch(string yymm1, string yymm2, string opis, string opay, string conn)
    {
        var res = new M1305_FetchResult();
        string csql = $@"select distinct concat(left(trn, 4), '-', trim(empnumber)) Prd, right(trim(trn), 5) ClNumber
                         from {opay}.tbltran where left(trn, 4) between @A and @B";
        var combos = await _sql.FetchLegacy<Combo, dynamic>(csql, new { A = yymm1, B = yymm2 }, conn) ?? [];
        if (combos.Count == 0) { res.Error = "No records found in the database"; return res; }

        var dep = await Deployment(yymm1, yymm2, opis, opay, conn);
        var have = dep.Select(d => d.Trn + d.ClNumber).ToHashSet();
        var missing = combos.Where(c => !have.Contains(c.Prd + c.ClNumber)).ToList();
        foreach (var m in missing)
            await _sql.ExecuteLegacyCmd($"insert ignore into {opay}.tbltrandeployment (trn, clnumber) values (@T, @C)", new { T = m.Prd, C = m.ClNumber }, conn);
        if (missing.Count > 0)
        {
            dep = await Deployment(yymm1, yymm2, opis, opay, conn);
            var trns = dep.Select(d => d.Trn).ToHashSet();
            if (combos.Any(c => !trns.Contains(c.Prd)))
            {
                res.Error = "Error found, some personnel have lost their deployment. Try to click Fetch Data again to solve the problem!";
                return res;
            }
        }
        res.Groups = dep.GroupBy(d => d.ClNumber).Select(g => new M1305_Group { ClNumber = g.Key, ClName = g.First().ClName })
                        .OrderBy(g => g.ClName).ToList();
        return res;
    }

    class Tran { public string Prd { get; set; } = ""; public string Trn { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string AcctNumber { get; set; } = ""; public decimal Amount { get; set; } }
    class Emp
    {
        public string EmpNumber { get; set; } = ""; public string? EmpLastNm { get; set; } public string? EmpFirstNm { get; set; } public string? EmpMidNm { get; set; }
        public DateTime? DateHired { get; set; } public DateTime? Separate { get; set; } public DateTime? EmpBirth { get; set; }
        public string? EmpStat_ { get; set; } public string? Tin { get; set; } public string? Sss { get; set; } public string? Hdmf { get; set; }
        public string? PagibigNo { get; set; } public string? Phic { get; set; } public string? Sex_ { get; set; } public string EmpName { get; set; } = "";
    }

    public async Task<List<M1305_Row>> _02Report(int type, string yymm1, string yymm2, List<string> selected, string opis, string opay, string conn)
    {
        string tsql = $@"select concat(left(t.trn, 4), '-', trim(t.empnumber)) Prd, trim(t.trn) Trn, trim(t.empnumber) EmpNumber, trim(t.acctnumber) AcctNumber, t.amount Amount
                         from {opay}.tbltran t where left(t.trn, 4) between @A and @B";
        var tran = await _sql.FetchLegacy<Tran, dynamic>(tsql, new { A = yymm1, B = yymm2 }, conn) ?? [];

        string esel = @"e.empnumber EmpNumber, e.emplastnm EmpLastNm, e.empfirstnm EmpFirstNm, e.empmidnm EmpMidNm, e.datehired DateHired, e.separate Separate,
                        e.empbirth EmpBirth, e.empstat_ EmpStat_, e.tin Tin, e.sss Sss, e.hdmf Hdmf, e.pagibigno PagibigNo, e.phic Phic, e.sex_ Sex_,
                        concat(ifnull(trim(e.emplastnm), ''), ', ', ifnull(trim(e.empfirstnm), ''), ' ', ifnull(left(trim(e.empmidnm), 1), '')) EmpName";
        string esql = $@"select {esel} from {opis}.empmas e
                         union
                         select {esel} from {opis}.empmasarchieved e where e.empnumber not in (select empnumber from {opis}.empmas)";
        var emps = (await _sql.FetchLegacy<Emp, dynamic>(esql, new { }, conn) ?? []).GroupBy(e => e.EmpNumber.Trim()).ToDictionary(g => g.Key, g => g.First());

        var clients = (await _sql.FetchLegacy<Dep, dynamic>($"select '' Trn, trim(clnumber) ClNumber, trim(ifnull(clname, '')) ClName from {opis}.client", new { }, conn) ?? [])
                        .GroupBy(c => c.ClNumber).ToDictionary(g => g.Key, g => g.First().ClName);

        var lines = new List<(string Cl, string ClName, Tran T)>();
        if (type == 3)
        {
            foreach (var t in tran)
            {
                string cl = t.Trn.Length >= 5 ? t.Trn[^5..] : "";
                if (selected.Contains(cl)) lines.Add((cl, clients.GetValueOrDefault(cl) ?? "", t));
            }
        }
        else
        {
            var dep = (await Deployment(yymm1, yymm2, opis, opay, conn)).GroupBy(d => d.Trn).ToDictionary(g => g.Key, g => g.First());
            foreach (var t in tran)
            {
                if (string.IsNullOrEmpty(t.EmpNumber) || !dep.TryGetValue(t.Prd, out var d)) continue;
                if (selected.Contains(d.ClNumber)) lines.Add((d.ClNumber, d.ClName, t));
            }
        }

        var rows = new List<M1305_Row>();
        foreach (var g in lines.Where(l => l.T.AcctNumber.StartsWith("E", StringComparison.OrdinalIgnoreCase)).GroupBy(l => (l.Cl, l.T.EmpNumber)))
        {
            var emp = emps.GetValueOrDefault(g.Key.EmpNumber);
            decimal Sum(string acct) => lines.Where(l => l.Cl == g.Key.Cl && l.T.EmpNumber == g.Key.EmpNumber && l.T.AcctNumber == acct).Sum(l => l.T.Amount);
            rows.Add(new M1305_Row
            {
                ClNumber = g.Key.Cl, ClName = g.First().ClName, EmpNumber = g.Key.EmpNumber, EmpName = emp?.EmpName ?? "",
                LastName = emp?.EmpLastNm?.Trim() ?? "", FirstName = emp?.EmpFirstNm?.Trim() ?? "", MidName = emp?.EmpMidNm?.Trim() ?? "",
                Tin = emp?.Tin?.Trim() ?? "", DateHired = Clean(emp?.DateHired), Separate = Clean(emp?.Separate), Birth = Clean(emp?.EmpBirth),
                EmpStat = emp?.EmpStat_?.Trim() ?? "", SssNo = emp?.Sss?.Trim() ?? "", HdmfNo = emp?.Hdmf?.Trim() ?? "",
                PagibigNo = emp?.PagibigNo?.Trim() ?? "", PhicNo = emp?.Phic?.Trim() ?? "", Sex = emp?.Sex_?.Trim() ?? "",
                Gross = g.Sum(l => l.T.Amount), Basic = Sum("E001"), Tax = Sum("D001"), Sss = Sum("D002"), Phic = Sum("D003"), Pagibig = Sum("D004")
            });
        }
        return type == 2
            ? rows.OrderBy(r => r.EmpName).ThenBy(r => r.ClNumber).ToList()
            : rows.OrderBy(r => r.ClNumber).ThenBy(r => r.EmpName).ToList();
    }

    static DateTime? Clean(DateTime? d) => d is { } v && v.Year >= 2 ? v : null;
}

public interface IDA_1305O
{
    Task<M1305_FetchResult> _02Fetch(string yymm1, string yymm2, string opis, string opay, string conn);
    Task<List<M1305_Row>> _02Report(int type, string yymm1, string yymm2, List<string> selected, string opis, string opay, string conn);
}

public class M1305_Group { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }
public class M1305_FetchResult { public string? Error { get; set; } public List<M1305_Group> Groups { get; set; } = new(); }
public class M1305_Row
{
    public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = "";
    public string LastName { get; set; } = ""; public string FirstName { get; set; } = ""; public string MidName { get; set; } = "";
    public string Tin { get; set; } = ""; public DateTime? DateHired { get; set; } public DateTime? Separate { get; set; } public DateTime? Birth { get; set; }
    public string EmpStat { get; set; } = ""; public string SssNo { get; set; } = ""; public string HdmfNo { get; set; } = "";
    public string PagibigNo { get; set; } = ""; public string PhicNo { get; set; } = ""; public string Sex { get; set; } = "";
    public decimal Gross { get; set; } public decimal Basic { get; set; } public decimal Tax { get; set; } public decimal Sss { get; set; } public decimal Phic { get; set; } public decimal Pagibig { get; set; }
}
