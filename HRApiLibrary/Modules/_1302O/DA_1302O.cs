using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._1302O;

public class DA_1302O : IDA_1302O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_1302O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    class TaxRow { public string EmpNumber { get; set; } = ""; public string Mm { get; set; } = ""; public decimal Amount { get; set; } }

    public async Task<M1302_Fetch> _02Fetch(string year4, string opis, string opay, string conn)
    {
        string yy = year4[^2..];
        var res = new M1302_Fetch();

        string esql = $@"select trim(x.empnumber) EmpNumber,
                                concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', trim(e.empmidnm)) EmpName,
                                trim(e.client_) PayGrpCd, trim(c.clname) PayGrp, trim(s.name) EmpStatus, trim(e.taxcode) TaxCode, ifnull(ex.exemption, 0) Exemption
                         from (select distinct empnumber from {opay}.tbltran where left(trn, 2) = @Yy and acctnumber = 'D001') x
                              left join {opis}.empmas e on e.empnumber = x.empnumber
                              left join {opis}.client c on c.clnumber = e.client_
                              left join {opis}.empstat s on s.code = e.empstat_
                              left join {opay}.tblatexemption ex on ex.taxcode = e.taxcode
                         order by c.clname, EmpName";
        res.Employees = await _sql.FetchLegacy<M1302_Emp, dynamic>(esql, new { Yy = yy }, conn) ?? [];

        string rsql = $@"select trim(empnumber) EmpNumber, max(nval) Rate from {opay}.tbltrandtl where left(trn, 2) = @Yy and dtlcd = 'RATE' group by empnumber";
        var rates = (await _sql.FetchLegacy<M1302_Rate, dynamic>(rsql, new { Yy = yy }, conn) ?? []).ToDictionary(r => r.EmpNumber, r => r.Rate);

        string tsql = $@"select trim(empnumber) EmpNumber, substring(trn, 3, 2) Mm, sum(amount) Amount
                         from {opay}.tbltran where left(trn, 2) = @Yy and acctnumber = 'D001' group by empnumber, substring(trn, 3, 2)";
        var tax = await _sql.FetchLegacy<TaxRow, dynamic>(tsql, new { Yy = yy }, conn) ?? [];
        var byEmp = tax.GroupBy(t => t.EmpNumber).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var e in res.Employees)
        {
            e.Rate = rates.TryGetValue(e.EmpNumber, out var r) ? r : null;
            if (byEmp.TryGetValue(e.EmpNumber, out var list))
                foreach (var t in list) if (int.TryParse(t.Mm, out var m) && m is >= 1 and <= 12) e.Tax[m - 1] += t.Amount;
        }

        res.Groups = res.Employees.GroupBy(e => e.PayGrpCd ?? "").Select(g => new M1302_Group
        {
            PayGrpCd = g.Key, PayGrp = string.IsNullOrWhiteSpace(g.First().PayGrp) ? "---No Current Deployment--" : g.First().PayGrp!
        }).OrderBy(g => g.PayGrp).ToList();

        string coSql = $"select trim(ifnull(coname, '')) CoName, trim(ifnull(coadd, '')) CoAdd from {opis}.coinfo limit 1";
        var co = (await _sql.FetchLegacy<M1302_Co, dynamic>(coSql, new { }, conn))?.FirstOrDefault();
        res.CoName = co?.CoName ?? ""; res.CoAdd = co?.CoAdd ?? "";
        return res;
    }

    class Sum2 { public string EmpNumber { get; set; } = ""; public string Mm { get; set; } = ""; public int Type { get; set; } public decimal Amount { get; set; } }

    public async Task<Dictionary<string, decimal[][]>> _02Income(string year4, string opay, string conn)
    {
        string yy = year4[^2..];
        string sql = $@"select trim(t.empnumber) EmpNumber, substring(t.trn, 3, 2) Mm, if(c.taxable_type = 1, 1, 2) Type, sum(t.amount) Amount
                        from {opay}.tbltran t join {opay}.chartofacct c on c.acctnumber = t.acctnumber
                        where left(t.trn, 2) = @Yy and left(trim(t.acctnumber), 1) = 'E'
                        group by t.empnumber, substring(t.trn, 3, 2), if(c.taxable_type = 1, 1, 2)";
        var rows = await _sql.FetchLegacy<Sum2, dynamic>(sql, new { Yy = yy }, conn) ?? [];
        var res = new Dictionary<string, decimal[][]>();
        foreach (var r in rows)
        {
            if (!int.TryParse(r.Mm, out var m) || m < 1 || m > 12) continue;
            if (!res.TryGetValue(r.EmpNumber, out var a)) res[r.EmpNumber] = a = new[] { new decimal[12], new decimal[12] };
            a[r.Type == 1 ? 0 : 1][m - 1] += r.Amount;
        }
        return res;
    }

    class Det { public string EmpNumber { get; set; } = ""; public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; public string Mm { get; set; } = ""; public int Type { get; set; } public decimal Amount { get; set; } }

    public async Task<Dictionary<string, M1302_Detail>> _02Detailed(string year4, string opay, string conn)
    {
        string yy = year4[^2..];
        string sql = $@"select trim(t.empnumber) EmpNumber, trim(t.acctnumber) AcctNumber, trim(ifnull(c.acctname, '')) AcctName, substring(t.trn, 3, 2) Mm,
                               case when left(trim(t.acctnumber), 1) = 'E' and c.taxable_type = 1 then 1
                                    when left(trim(t.acctnumber), 1) = 'E' then 2
                                    when t.acctnumber in ('D001', 'D002', 'D003', 'D004') then 3 else 0 end Type,
                               sum(t.amount) Amount
                        from {opay}.tbltran t left join {opay}.chartofacct c on c.acctnumber = t.acctnumber
                        where left(t.trn, 2) = @Yy
                        group by t.empnumber, t.acctnumber, substring(t.trn, 3, 2)";
        var rows = (await _sql.FetchLegacy<Det, dynamic>(sql, new { Yy = yy }, conn) ?? []).Where(r => r.Type != 0).ToList();
        var res = new Dictionary<string, M1302_Detail>();
        foreach (var g in rows.GroupBy(r => (r.EmpNumber, r.AcctNumber, r.Type)))
        {
            if (!res.TryGetValue(g.Key.EmpNumber, out var d)) res[g.Key.EmpNumber] = d = new M1302_Detail();
            var line = new M1302_DetLine { AcctNumber = g.Key.AcctNumber, AcctName = g.First().AcctName };
            foreach (var r in g) if (int.TryParse(r.Mm, out var m) && m is >= 1 and <= 12) line.M[m - 1] += r.Amount;
            (g.Key.Type == 1 ? d.Taxable : g.Key.Type == 2 ? d.NonTaxable : d.Premiums).Add(line);
        }
        foreach (var d in res.Values)
        {
            d.Taxable = d.Taxable.OrderBy(x => x.AcctNumber).ToList();
            d.NonTaxable = d.NonTaxable.OrderBy(x => x.AcctNumber).ToList();
            d.Premiums = d.Premiums.OrderBy(x => x.AcctNumber).ToList();
        }
        return res;
    }
}

public interface IDA_1302O
{
    Task<M1302_Fetch> _02Fetch(string year4, string opis, string opay, string conn);
    Task<Dictionary<string, decimal[][]>> _02Income(string year4, string opay, string conn);
    Task<Dictionary<string, M1302_Detail>> _02Detailed(string year4, string opay, string conn);
}

public class M1302_Rate { public string EmpNumber { get; set; } = ""; public decimal Rate { get; set; } }
public class M1302_Co { public string CoName { get; set; } = ""; public string CoAdd { get; set; } = ""; }
public class M1302_Group { public string PayGrpCd { get; set; } = ""; public string PayGrp { get; set; } = ""; public bool Mrk { get; set; } = true; }
public class M1302_Emp
{
    public string EmpNumber { get; set; } = ""; public string? EmpName { get; set; }
    public string? PayGrpCd { get; set; } public string? PayGrp { get; set; } public string? EmpStatus { get; set; }
    public string? TaxCode { get; set; } public decimal Exemption { get; set; } public decimal? Rate { get; set; }
    public decimal[] Tax { get; set; } = new decimal[12];
    public decimal T(int i) => Tax[i];
    public decimal Total => Tax.Sum();
}
public class M1302_Fetch
{
    public List<M1302_Emp> Employees { get; set; } = new();
    public List<M1302_Group> Groups { get; set; } = new();
    public string CoName { get; set; } = ""; public string CoAdd { get; set; } = "";
}
public class M1302_DetLine { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; public decimal[] M { get; set; } = new decimal[12]; }
public class M1302_Detail
{
    public List<M1302_DetLine> Taxable { get; set; } = new();
    public List<M1302_DetLine> NonTaxable { get; set; } = new();
    public List<M1302_DetLine> Premiums { get; set; } = new();
}
