using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._1306O;

public class DA_1306O : IDA_1306O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_1306O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<string>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(pdNumber) from {opay}.period where pdNumber != '00' order by pdNumber";
        return await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M1306_Group>> _02Groups(string prd, string opis, string opay, string conn)
    {
        string sql = $@"select distinct trim(c.clnumber) ClNumber, trim(c.clname) ClName
                        from {opay}.tbltran t left join {opis}.client c on c.clnumber = right(t.trn, 5)
                        where left(t.trn, 6) = @Prd
                        order by ClName";
        var r = await _sql.FetchLegacy<M1306_Group, dynamic>(sql, new { Prd = prd }, conn) ?? [];
        return r.Where(x => !string.IsNullOrEmpty(x.ClNumber)).ToList();
    }

    class Tran
    {
        public string Trn { get; set; } = ""; public string? EmpNumber { get; set; }
        public string? ClientCur { get; set; } public string? EmpStatCur { get; set; }
        public string? TrnClNumber { get; set; } public string? TrnClName { get; set; }
        public string? EmpName { get; set; } public decimal GrossPay { get; set; } public decimal BasicPay { get; set; }
    }

    class Dep
    {
        public string Trn { get; set; } = ""; public string EmpNumber { get; set; } = "";
        public string? Client_ { get; set; } public string? EmpStat_ { get; set; }
        public string? EmpStatus { get; set; } public string? ClName_ { get; set; } public string? EmpName { get; set; }
    }

    async Task<List<Dep>> Deployment(string prd, string opis, string opay, string conn)
    {
        string sql = $@"select h.trn Trn, trim(h.empnumber) EmpNumber, trim(h.client_) Client_, h.empstat_ EmpStat_, trim(s.name) EmpStatus, trim(c.clname) ClName_,
                               concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', left(trim(ifnull(e.empmidnm, '')), 1)) EmpName
                        from {opay}.pisval_empdeploymenthist h
                             left join {opis}.empstat s on s.code = h.empstat_
                             left join {opis}.client c on c.clnumber = h.client_
                             left join {opis}.empmas e on e.empnumber = h.empnumber
                        where left(h.trn, 6) = @Prd and length(h.trn) > 10";
        return await _sql.FetchLegacy<Dep, dynamic>(sql, new { Prd = prd }, conn) ?? [];
    }

    public async Task<List<M1306_Row>> _02Fetch(string prd, List<string> clNumbers, string user, string opis, string opay, string conn)
    {
        string tsql = $@"select t.trn Trn, trim(e.empnumber) EmpNumber, trim(e.client_) ClientCur, e.empstat_ EmpStatCur,
                                trim(c.clnumber) TrnClNumber, trim(c.clname) TrnClName,
                                concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', left(trim(ifnull(e.empmidnm, '')), 1)) EmpName,
                                sum(t.amount) GrossPay, sum(if(t.acctnumber = 'E001', t.amount, 0)) BasicPay
                         from {opay}.tbltran t
                              left join {opis}.empmas e on e.empnumber = t.empnumber
                              left join {opis}.client c on c.clnumber = right(t.trn, 5)
                         where left(t.trn, 6) = @Prd and upper(left(t.acctnumber, 1)) = 'E' and length(t.trn) > 10
                         group by t.trn, t.empnumber
                         order by c.clname, EmpName";
        var tran = await _sql.FetchLegacy<Tran, dynamic>(tsql, new { Prd = prd }, conn) ?? [];

        var stat = await _sql.FetchLegacy<string, dynamic>($"select trim(code) from {opay}.empstatpisval", new { }, conn) ?? [];
        var dep = await Deployment(prd, opis, opay, conn);

        var missing = tran.Where(t => !string.IsNullOrEmpty(t.EmpNumber) && dep.All(d => d.EmpNumber != t.EmpNumber))
                          .GroupBy(t => (t.ClientCur ?? "", t.EmpNumber!, t.EmpStatCur ?? "")).ToList();
        if (missing.Count > 0)
        {
            foreach (var g in missing)
            {
                await _sql.ExecuteLegacyCmd($@"insert ignore into {opay}.pisval_empdeploymenthist (TRN, EmpNumber, Client_, EmpStat_, lockBy, TimeStamp)
                                               values (@Trn, @Emp, @Cl, @St, @By, now())",
                    new { Trn = prd + g.Key.Item1, Emp = g.Key.Item2, Cl = g.Key.Item1, St = g.Key.Item3, By = (user ?? "").Trim() }, conn);
            }
            dep = await Deployment(prd, opis, opay, conn);
        }

        var rows = new List<M1306_Row>();
        foreach (var t in tran.Where(t => clNumbers.Contains(t.Trn.Length >= 5 ? t.Trn[^5..] : "")))
        {
            var ds = dep.Where(d => d.EmpNumber == t.EmpNumber).DefaultIfEmpty(null);
            foreach (var d in ds)
                rows.Add(new M1306_Row
                {
                    PayrollGrp = d?.ClName_ ?? "", EmpName = t.EmpName ?? "", CurrDeployment = t.TrnClName ?? "",
                    StatusOnPayroll = d?.EmpStatus ?? "",
                    TransStatus = d != null && (d.Client_ ?? "") != (t.TrnClNumber ?? "") ? "Added" : "",
                    BasicPay = t.BasicPay, GrossPay = t.GrossPay
                });
        }

        foreach (var cl in clNumbers)
        {
            string clName = (await _sql.FetchLegacy<string, dynamic>($"select trim(clname) from {opis}.client where clnumber = @Cl", new { Cl = cl }, conn))?.FirstOrDefault() ?? "";
            var onTran = tran.Where(t => t.Trn.EndsWith(cl)).Select(t => t.EmpNumber).ToHashSet();
            foreach (var d in dep.Where(d => d.Client_ == cl && !onTran.Contains(d.EmpNumber) && stat.Contains((d.EmpStat_ ?? "").Trim())))
                rows.Add(new M1306_Row { PayrollGrp = clName, EmpName = d.EmpName ?? "", CurrDeployment = "", StatusOnPayroll = d.EmpStatus ?? "", TransStatus = "Removed" });
        }

        return rows.Where(r => !string.IsNullOrEmpty(r.PayrollGrp)).OrderBy(r => r.PayrollGrp).ThenBy(r => r.EmpName).ToList();
    }
}

public interface IDA_1306O
{
    Task<List<string>> _02Periods(string opay, string conn);
    Task<List<M1306_Group>> _02Groups(string prd, string opis, string opay, string conn);
    Task<List<M1306_Row>> _02Fetch(string prd, List<string> clNumbers, string user, string opis, string opay, string conn);
}

public class M1306_Group { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; public bool Mrk { get; set; } }

public class M1306_Row
{
    public string PayrollGrp { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string CurrDeployment { get; set; } = "";
    public string StatusOnPayroll { get; set; } = "";
    public string TransStatus { get; set; } = "";
    public decimal? BasicPay { get; set; }
    public decimal? GrossPay { get; set; }
}
