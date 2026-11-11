using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._1303O;

public class DA_1303O : IDA_1303O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_1303O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M1303_Client>> _02Clients(string opis, string conn)
    {
        string sql = $"select trim(clnumber) ClNumber, trim(clname) ClName from {opis}.client where status = 'Active' order by clname";
        return await _sql.FetchLegacy<M1303_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<string>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(pdNumber) from {opay}.period where pdNumber != '00' order by pdNumber";
        return await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<M1303_Result> _02Fetch(string clNumber, string trnStart, string trnEnd, bool savedTransaction, string opis, string opay, string conn)
    {
        string dtl = savedTransaction ? "tmptbltrandtl" : "tbltrandtl";
        string tran = savedTransaction ? "tmptbltran" : "tbltran";

        string sql = $@"select trim(t.empnumber) EmpNumber,
                               concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', left(trim(ifnull(e.empmidnm, '')), 1)) EmpName,
                               left(trim(t.trn), 6) Prd, trim(t.dtlcd) DtlCd, ifnull(t.nval, 0) NVal,
                               ifnull(c.acctnumber, '') AcctNumber, ifnull(c.acctname, '') AcctName, ifnull(c.shortdesc, '') ShortDesc
                        from {opay}.{dtl} t
                             left join {opay}.chartofacct c on c.acctnumber = left(t.dtlcd, 4)
                             left join {opis}.empmas e on e.empnumber = t.empnumber
                        where left(t.trn, 6) between @S and @E and right(trim(t.trn), 5) = @Cl
                        order by t.trn, e.emplastnm, e.empfirstnm, c.acctnumber, t.dtlcd";
        var d = await _sql.FetchLegacy<M1303_Dtl, dynamic>(sql, new { S = trnStart, E = trnEnd, Cl = clNumber }, conn) ?? [];
        var res = new M1303_Result();
        if (d.Count == 0) return res;

        res.Accts = d.Where(x => x.AcctNumber != "")
                     .GroupBy(x => x.AcctNumber).OrderBy(g => g.Key)
                     .Select(g => new M1303_Acct { AcctNumber = g.Key, Caption = !string.IsNullOrWhiteSpace(g.First().ShortDesc) ? g.First().ShortDesc.Trim() : g.First().AcctName.Trim() })
                     .ToList();

        foreach (var g in d.GroupBy(x => (x.Prd, x.EmpNumber)).OrderBy(g => g.Key.Prd).ThenBy(g => g.First().EmpName))
        {
            var r = new M1303_Row { EmpNumber = g.Key.EmpNumber, EmpName = g.First().EmpName, Prd = g.Key.Prd };
            foreach (var x in g)
            {
                if (x.DtlCd == "RATE") r.Rate = x.NVal;
                else if (x.DtlCd == "TH") r.Th = x.NVal;
                if (x.AcctNumber != "" && x.DtlCd.StartsWith(x.AcctNumber)) r.Amt[x.AcctNumber] = x.NVal;
            }
            res.Rows.Add(r);
        }

        string gsql = $@"select left(trn, 6) Prd, trim(empnumber) EmpNumber, sum(amount) Amount
                         from {opay}.{tran}
                         where left(trn, 6) between @S and @E and right(trim(trn), 5) = @Cl and left(acctnumber, 1) = 'E'
                         group by left(trn, 6), empnumber";
        var gross = await _sql.FetchLegacy<M1303_Gross, dynamic>(gsql, new { S = trnStart, E = trnEnd, Cl = clNumber }, conn) ?? [];
        foreach (var r in res.Rows)
            r.Gross = gross.FirstOrDefault(x => x.Prd == r.Prd && x.EmpNumber == r.EmpNumber)?.Amount;
        return res;
    }
}

public interface IDA_1303O
{
    Task<List<M1303_Client>> _02Clients(string opis, string conn);
    Task<List<string>> _02Periods(string opay, string conn);
    Task<M1303_Result> _02Fetch(string clNumber, string trnStart, string trnEnd, bool savedTransaction, string opis, string opay, string conn);
}

public class M1303_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }
public class M1303_Acct { public string AcctNumber { get; set; } = ""; public string Caption { get; set; } = ""; }
public class M1303_Dtl
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Prd { get; set; } = "";
    public string DtlCd { get; set; } = ""; public decimal NVal { get; set; }
    public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; public string ShortDesc { get; set; } = "";
}
public class M1303_Gross { public string Prd { get; set; } = ""; public string EmpNumber { get; set; } = ""; public decimal Amount { get; set; } }
public class M1303_Row
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Prd { get; set; } = "";
    public decimal Rate { get; set; } public decimal Th { get; set; }
    public Dictionary<string, decimal> Amt { get; set; } = new();
    public decimal? Gross { get; set; }
}
public class M1303_Result { public List<M1303_Acct> Accts { get; set; } = new(); public List<M1303_Row> Rows { get; set; } = new(); }
