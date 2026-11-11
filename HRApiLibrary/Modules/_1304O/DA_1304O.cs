using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._1304O;

public class DA_1304O : IDA_1304O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_1304O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M1304_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber,
                               concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', trim(ifnull(e.empmidnm, ''))) EmpName,
                               concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', trim(ifnull(e.empmidnm, '')), ' - [', trim(e.empnumber), ']  [', trim(ifnull(s.name, '')), ']') Label,
                               trim(ifnull(c.clname, '')) ClName, e.datehired DateHired, e.separate Separate, trim(ifnull(s.name, '')) EmpStatus,
                               ifnull(e.suretybondquota, 0) SuretyBondQuota
                        from {opis}.empmas e
                             left join {opis}.client c on c.clnumber = e.client_
                             left join {opis}.empstat s on s.code = e.empstat_
                        order by e.emplastnm, e.empfirstnm, e.empmidnm";
        var r = await _sql.FetchLegacy<M1304_Emp, dynamic>(sql, new { }, conn) ?? [];
        foreach (var x in r)
        {
            if (x.DateHired is { } h && h.Year < 2) x.DateHired = null;
            if (x.Separate is { } s && s.Year < 2) x.Separate = null;
        }
        return r;
    }

    public async Task<M1304_Result> _02Generate(M1304_Emp emp, List<string> yymm, string opay, string conn)
    {
        var res = new M1304_Result();
        string emn = emp.EmpNumber;

        // 13th month
        string s13 = $@"select t.trn Trn, ifnull(c.acctname, '') AcctName, t.amount Amount
                        from {opay}.tbltran t left join {opay}.chartofacct c on c.acctnumber = t.acctnumber
                        where t.empnumber = @Emp and left(t.trn, 4) in @Yymm
                          and t.acctnumber in (select acctnumber from {opay}.quitclaimcoa where grp = '13MoAcct')";
        var t13 = yymm.Count == 0 ? [] : await _sql.FetchLegacy<M1304_Tran, dynamic>(s13, new { Emp = emn, Yymm = yymm }, conn) ?? [];
        decimal tot13 = t13.Sum(x => x.Amount) / 12;
        if (tot13 > 0) res.Earnings.Add(new M1304_Line { Descr = "13th Month Pay", Amount = tot13 });
        foreach (var x in t13) res.Print.Add(new M1304_Prn { Grp = "13Mo", GrpName = "13th Month pay", Dtls = $"{x.AcctName.Trim()}   [{Left6(x.Trn)}] Amt P{x.Amount:N2}", Amt = x.Amount / 12 });

        // suretybond
        if (emp.SuretyBondQuota > 0)
        {
            res.Earnings.Add(new M1304_Line { Descr = "Suretybond Fund", Amount = emp.SuretyBondQuota });
            res.Print.Add(new M1304_Prn { Grp = "SB", GrpName = "Suretybond", Dtls = "Suretybond Fund", Amt = emp.SuretyBondQuota });
        }
        string sbd = $@"select a.trn Trn, ifnull(b.acctname, '') AcctName, a.amount Amount
                        from {opay}.tbltran a left join {opay}.chartofacct b on a.acctnumber = b.acctnumber
                        where a.empnumber = @Emp and left(a.trn, 4) >= 1801
                          and a.acctnumber in (select acctnumber from {opay}.quitclaimcoa where grp = 'SB')";
        foreach (var x in await _sql.FetchLegacy<M1304_Tran, dynamic>(sbd, new { Emp = emn }, conn) ?? [])
            res.Print.Add(new M1304_Prn { Grp = "SB", GrpName = "Suretybond", Dtls = $"{x.AcctName.Trim()}   [{Left6(x.Trn)}]", Amt = x.Amount });
        string sbs = $@"select ifnull(b.acctname, '') AcctName, sum(a.amount) Amount
                        from {opay}.tbltran a left join {opay}.chartofacct b on a.acctnumber = b.acctnumber
                        where a.empnumber = @Emp and a.acctnumber in (select acctnumber from {opay}.quitclaimcoa where grp = 'SB')
                        group by a.empnumber, b.acctname";
        foreach (var x in await _sql.FetchLegacy<M1304_Tran, dynamic>(sbs, new { Emp = emn }, conn) ?? [])
            res.Earnings.Add(new M1304_Line { Descr = x.AcctName.Trim(), Amount = x.Amount });

        // other earnings
        string oes = $@"select concat(b.acctname, '(', a.remarks, ')') AcctName, a.amount Amount,
                               ifnull(a.p1, 0) P1, ifnull(a.p2, 0) P2, ifnull(a.p3, 0) P3, ifnull(a.p4, 0) P4, ifnull(a.p5, 0) P5
                        from {opay}.tblotherearnings a left join {opay}.chartofacct b on a.acctnumber = b.acctnumber
                        where a.processed = 0 and a.empnumber = @Emp
                          and a.acctnumber in (select acctnumber from {opay}.quitclaimcoa where grp = 'OE')";
        foreach (var x in await _sql.FetchLegacy<M1304_Sched, dynamic>(oes, new { Emp = emn }, conn) ?? [])
        {
            var ps = x.Periods();
            string desc = $"OE-{(x.AcctName ?? "").Trim()} Prd : [{string.Join(",", ps.Select(p => "P" + p))}]";
            decimal amt = x.Amount * ps.Count;
            res.Earnings.Add(new M1304_Line { Descr = desc, Amount = amt });
            res.Print.Add(new M1304_Prn { Grp = "OE", GrpName = "Other Earnings", Dtls = desc, Amt = amt });
        }

        // partial deductions (loans)
        string lns = $@"select ifnull(b.acctname, '') AcctName, a.amount Amount, a.amort Amort, a.balance Balance, ifnull(a.remarks, '') Remarks
                        from {opay}.loans a left join {opay}.chartofacct b on a.dedncode = b.acctnumber
                        where a.status = 'A' and a.dedncode not in (select acctnumber from {opay}.quitclaimcoa where grp = 'PDed')
                          and a.balance > 0 and a.empnumber = @Emp";
        foreach (var x in await _sql.FetchLegacy<M1304_Loan, dynamic>(lns, new { Emp = emn }, conn) ?? [])
        {
            string d = x.AcctName.Trim();
            if (!string.IsNullOrWhiteSpace(x.Remarks)) d += $" [{x.Remarks.Trim()}]";
            res.Deductions.Add(new M1304_Line { Descr = d, Amount = x.Balance });
            string pd = $"{x.AcctName.Trim()} [Amt : {x.Amount:N2} | Amort:{x.Amort:N2} | REM: {x.Remarks.Trim()}]";
            res.Print.Add(new M1304_Prn { Grp = "PD", GrpName = "Partial Ded.", Dtls = pd.Length > 200 ? pd[..200] : pd, Amt = -x.Balance });
        }

        // other deductions
        string ods = $@"select trim(c.acctname) AcctName, concat('YR:', trim(d.year), ' Mo:', trim(d.month)) Prd, d.amount Amount, ifnull(d.remarks, '') Remarks,
                               ifnull(d.p1, 0) P1, ifnull(d.p2, 0) P2, ifnull(d.p3, 0) P3, ifnull(d.p4, 0) P4, ifnull(d.p5, 0) P5
                        from {opay}.tblotherdeductions d left join {opay}.chartofacct c on c.acctnumber = d.acctnumber
                        where d.processed = 0 and d.empnumber = @Emp
                          and (ifnull(d.p1, 0) + ifnull(d.p2, 0) + ifnull(d.p3, 0) + ifnull(d.p4, 0) + ifnull(d.p5, 0)) > 0
                          and d.acctnumber not in (select acctnumber from {opay}.quitclaimcoa where grp = 'PDed')";
        foreach (var x in await _sql.FetchLegacy<M1304_Sched, dynamic>(ods, new { Emp = emn }, conn) ?? [])
        {
            string sched = string.Join(",", x.Periods().Select(p => "P" + p));
            res.Deductions.Add(new M1304_Line { Descr = $"{(x.AcctName ?? "").Trim()} [{x.Prd}]  SCHED: {sched}", Amount = x.Amount });
            string pd = $"{(x.AcctName ?? "").Trim()} [SCHED: {x.Prd} PRD ({string.Concat(x.Periods().Select(p => " P" + p))}) | REM: {x.Remarks.Trim()}]";
            res.Print.Add(new M1304_Prn { Grp = "OD", GrpName = "Other Deductions", Dtls = pd.Length > 200 ? pd[..200] : pd, Amt = -x.Amount });
        }
        return res;
    }

    static string Left6(string trn) => trn.Trim().Length >= 6 ? trn.Trim()[..6] : trn.Trim();

    public async Task<List<M1304_Acct>> _02Accounts(string grp, string opay, string conn)
    {
        string type = grp is "13MoAcct" or "OE" ? "E" : "D";
        string where = grp == "OE" ? $"and acctnumber in (select distinct acctnumber from {opay}.tblotherearnings)" : "";
        string sql = $@"select trim(acctnumber) AcctNumber, trim(acctname) AcctName,
                               if(acctnumber in (select acctnumber from {opay}.quitclaimcoa where grp = @Grp), 1, 0) Mrk
                        from {opay}.chartofacct where accttype = '{type}' {where}
                        order by Mrk desc, acctname";
        return await _sql.FetchLegacy<M1304_Acct, dynamic>(sql, new { Grp = grp }, conn) ?? [];
    }

    public async Task _01Toggle(string grp, string acctNumber, bool on, string opay, string conn)
    {
        if (on)
            await _sql.ExecuteLegacyCmd($"insert into {opay}.quitclaimcoa (grp, acctnumber) values (@Grp, @A)", new { Grp = grp, A = acctNumber }, conn);
        else
            await _sql.ExecuteLegacyCmd($"delete from {opay}.quitclaimcoa where grp = @Grp and acctnumber = @A", new { Grp = grp, A = acctNumber }, conn);
    }
}

public interface IDA_1304O
{
    Task<List<M1304_Emp>> _02Employees(string opis, string conn);
    Task<M1304_Result> _02Generate(M1304_Emp emp, List<string> yymm, string opay, string conn);
    Task<List<M1304_Acct>> _02Accounts(string grp, string opay, string conn);
    Task _01Toggle(string grp, string acctNumber, bool on, string opay, string conn);
}

public class M1304_Emp
{
    public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Label { get; set; } = "";
    public string ClName { get; set; } = ""; public DateTime? DateHired { get; set; } public DateTime? Separate { get; set; }
    public string EmpStatus { get; set; } = ""; public decimal SuretyBondQuota { get; set; }
}
public class M1304_Tran { public string Trn { get; set; } = ""; public string AcctName { get; set; } = ""; public decimal Amount { get; set; } }
public class M1304_Loan { public string AcctName { get; set; } = ""; public decimal Amount { get; set; } public decimal Amort { get; set; } public decimal Balance { get; set; } public string Remarks { get; set; } = ""; }
public class M1304_Sched
{
    public string? AcctName { get; set; } public string Prd { get; set; } = ""; public decimal Amount { get; set; } public string Remarks { get; set; } = "";
    public int P1 { get; set; } public int P2 { get; set; } public int P3 { get; set; } public int P4 { get; set; } public int P5 { get; set; }
    public List<int> Periods() { var l = new List<int>(); if (P1 == 1) l.Add(1); if (P2 == 1) l.Add(2); if (P3 == 1) l.Add(3); if (P4 == 1) l.Add(4); if (P5 == 1) l.Add(5); return l; }
}
public class M1304_Line { public bool Mrk { get; set; } = true; public string Descr { get; set; } = ""; public decimal Amount { get; set; } }
public class M1304_Prn { public string Grp { get; set; } = ""; public string GrpName { get; set; } = ""; public string Dtls { get; set; } = ""; public decimal Amt { get; set; } }
public class M1304_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; public bool Mrk { get; set; } }
public class M1304_Result
{
    public List<M1304_Line> Earnings { get; set; } = new();
    public List<M1304_Line> Deductions { get; set; } = new();
    public List<M1304_Prn> Print { get; set; } = new();
}
