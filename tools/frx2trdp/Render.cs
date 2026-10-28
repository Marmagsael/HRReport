// Renders a .trdp with data from the real HRApiLibrary DA to PDF.
// usage: frx2trdp <report> <trdp> <out.pdf> [args...]   (PNG_PAGES=1,2 also writes those pages as PNG)
//   register <trdp> <out.pdf> <yymmpp> [trn,trn,...]
using HRApiLibrary.DataAccess._20_Pay_Report;
using HRApiLibrary.DataAccess._90_Utils;
using Microsoft.Extensions.Configuration;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

static class Render
{
    public static async Task Run(string[] args)
    {

        var config = new ConfigurationBuilder().AddJsonFile(Path.Combine(Repo.Root, "HRMvc", "appsettings.json")).Build();
        var sql = new _90_001_MySqlDataAccess(config);
        var rep = new GPayrollReportDataAccess(sql);
        string conn = "MySqlConn", paydb = "pay", pisdb = "secpis";
        var set = new Lazy<Task<Dictionary<string, string>>>(() => rep._02ReportSettings(paydb, conn));

        IEnumerable<object> data;
        var prm = new Dictionary<string, object> { ["CoName"] = "Morpheusbox Inc.", ["CoAddress"] = "GF Suntrade Bldg. 577 Alabang-Zapote Rd., Almanza Uno, Las Pinas City", ["RunDate"] = new DateTime(2026, 10, 28, 9, 0, 0) };
        switch (args[0])
        {
            case "register":
                {
                    var prd = args[3];
                    var trns = args.Length > 4 ? args[4].Split(',').ToList() : (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                    var rows = await rep._02PayrollRegister(trns, prd, (string)prm["CoName"], paydb, pisdb, conn);
                    Console.WriteLine($"trns={trns.Count} rows={rows.Count} gross={rows.Sum(r => r.GrossPay):N2} ded={rows.Sum(r => r.GrossDed):N2} net={rows.Sum(r => r.Netpay):N2}");
                    data = rows;
                    break;
                }
            case "regcheck":
                {
                    // per TRN: DA gross/ded vs plain SQL sums of tbltran (active accounts), every period in the DB
                    var prds = await sql.FetchData<string, dynamic>($"select distinct left(TRN,6) from {paydb}.tbltran order by 1", new { }, conn);
                    int bad = 0, nT = 0, nR = 0;
                    foreach (var prd in prds)
                    {
                        var trns = (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                        var rows = await rep._02PayrollRegister(trns, prd, "x", paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Trn, double E, double D, long Emps), dynamic>(
                            $"select t.TRN, sum(if(left(t.AcctNumber,1)='E',round(cast(t.Amount as decimal(14,4)),2),0)) + 0e0, sum(if(left(t.AcctNumber,1)='D',round(cast(t.Amount as decimal(14,4)),2),0)) + 0e0, count(distinct t.EmpNumber) from {paydb}.tbltran t join {paydb}.chartofacct c on c.AcctNumber=t.AcctNumber and c.status_=1 where left(t.TRN,6)=@P group by t.TRN", new { P = prd }, conn);
                        foreach (var e in exp)
                        {
                            nT++;
                            var g = rows.Where(r => r.Trn == e.Trn).ToList(); nR += g.Count;
                            double ge = Math.Round(g.Sum(r => r.GrossPay), 2), gd = Math.Round(g.Sum(r => r.GrossDed), 2), gn = Math.Round(g.Sum(r => r.Netpay), 2);
                            if (Math.Abs(ge - e.E) > 0.011 || Math.Abs(gd - e.D) > 0.011 || g.Count != e.Emps || Math.Abs(gn - (ge - gd)) > 0.011) { bad++; Console.WriteLine($"DIFF {e.Trn} gross {ge} vs {e.E} ded {gd} vs {e.D} emps {g.Count} vs {e.Emps}"); }
                        }
                    }
                    Console.WriteLine($"periods={prds.Count} trns={nT} rows={nR} diffs={bad}");
                    return;
                }
            case "peracct":
                {
                    var prd = args[3];
                    var trns = args.Length > 4 ? args[4].Split(',').ToList() : (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                    var rows = await rep._02PayrollRegisterPerAcct(trns, prd, (string)prm["CoName"], paydb, pisdb, conn);
                    Console.WriteLine($"trns={trns.Count} rows={rows.Count} gross={rows.Sum(r => r.Gross):N2} net={rows.Sum(r => r.Net):N2} basic hrs={rows.Sum(r => r.E001H):N2}");
                    data = rows;
                    break;
                }
            case "peracctcheck":
                {
                    // per TRN: gross/net/employees vs plain SQL over tbltran (all accounts), hours E001-E014 vs tbltrandtl, every period
                    var prds = await sql.FetchData<string, dynamic>($"select distinct left(TRN,6) from {paydb}.tbltran order by 1", new { }, conn);
                    int bad = 0, nT = 0, nR = 0;
                    foreach (var prd in prds)
                    {
                        var trns = (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                        var rows = await rep._02PayrollRegisterPerAcct(trns, prd, "x", paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Trn, double E, double D, long Emps), dynamic>(
                            $"select t.TRN, sum(if(left(t.AcctNumber,1)='E',round(cast(t.Amount as decimal(14,4)),2),0)) + 0e0, sum(if(left(t.AcctNumber,1)='D',round(cast(t.Amount as decimal(14,4)),2),0)) + 0e0, count(distinct t.EmpNumber) from {paydb}.tbltran t join {paydb}.chartofacct c on c.AcctNumber=t.AcctNumber where left(t.TRN,6)=@P group by t.TRN", new { P = prd }, conn);
                        var hrs = (await sql.FetchData<(string Trn, double H), dynamic>(
                            $"select TRN, sum(nVal) + 0e0 from {paydb}.tbltrandtl where left(TRN,6)=@P and left(DtlCd,4) between 'E001' and 'E014' group by TRN", new { P = prd }, conn)).ToDictionary(x => x.Trn, x => x.H);
                        foreach (var e in exp)
                        {
                            nT++;
                            var g = rows.Where(r => r.Trn == e.Trn).ToList(); nR += g.Count;
                            double ge = Math.Round(g.Sum(r => r.Gross), 2), gn = Math.Round(g.Sum(r => r.Net), 2);
                            double gh = Math.Round(g.Sum(r => r.E001H + r.E002H + r.E003H + r.E004H + r.E005H + r.E006H + r.E007H + r.E008H + r.E009H + r.E010H + r.E011H + r.E012H + r.E013H + r.E014H), 2);
                            double eh = Math.Round(hrs.GetValueOrDefault(e.Trn), 2);
                            if (Math.Abs(ge - e.E) > 0.011 || Math.Abs(gn - (e.E - e.D)) > 0.011 || g.Count != e.Emps || Math.Abs(gh - eh) > 0.011)
                            { bad++; if (bad <= 10) Console.WriteLine($"DIFF {e.Trn} gross {ge} vs {e.E} net {gn} vs {e.E - e.D} emps {g.Count} vs {e.Emps} hrs {gh} vs {eh}"); }
                        }
                    }
                    Console.WriteLine($"periods={prds.Count} trns={nT} rows={nR} diffs={bad}");
                    return;
                }
            case "accttotal":
            case "accttotalcash":
                {
                    // <trdp> <out.pdf> <yymmpp>
                    var prd = args[3];
                    var rows = await rep._02AccountTotal(prd, args[0] == "accttotal", paydb, pisdb, conn);
                    int y = 2000 + int.Parse(prd[..2]), m = int.Parse(prd.Substring(2, 2));
                    prm["Period"] = $"{new DateTime(y, m, 1):MMMM} {(prd.EndsWith("01") ? "1-15" : $"16-{DateTime.DaysInMonth(y, m)}")}, {y}";
                    prm["EmpLabel"] = (await set.Value)["ACCTTOTAL_EMPLABEL"];
                    Console.WriteLine($"accounts={rows.Count} emps={rows.FirstOrDefault()?.TotalEmp} net={rows.FirstOrDefault()?.NetPay:N2} er={rows.Sum(r => r.Sss_ershare):N2} hrs={rows.Sum(r => r.Hrs):N2}");
                    data = rows;
                    break;
                }
            case "accttotalcheck":
                {
                    // per period: amounts per account (ATM + cash) vs plain SQL, employee counts, net pay
                    var prds = await sql.FetchData<string, dynamic>($"select distinct left(TRN,6) from {paydb}.tbltran order by 1", new { }, conn);
                    int bad = 0;
                    foreach (var prd in prds)
                    {
                        var rows = (await rep._02AccountTotal(prd, true, paydb, pisdb, conn)).Concat(await rep._02AccountTotal(prd, false, paydb, pisdb, conn)).ToList();
                        var exp = await sql.FetchData<(string Atm, string Acct, double Amt), dynamic>(
                            $@"select if(trim(ifnull(e.AcctCode,''))='','N','Y'), trim(t.AcctNumber), sum(t.Amount) + 0e0 from {paydb}.tbltran t left join {pisdb}.empmas e on e.EmpNumber = t.EmpNumber
                               where left(t.TRN,6)=@P group by 1, 2", new { P = prd }, conn);
                        var emps = await sql.FetchData<(string Atm, long N), dynamic>(
                            $@"select if(trim(ifnull(e.AcctCode,''))='','N','Y'), count(distinct t.EmpNumber) from {paydb}.tbltran t left join {pisdb}.empmas e on e.EmpNumber = t.EmpNumber where left(t.TRN,6)=@P group by 1", new { P = prd }, conn);
                        bool ok = exp.Count == rows.Count;
                        foreach (var e in exp)
                        {
                            var r = rows.FirstOrDefault(x => x.IsAtm == (e.Atm == "Y") && x.Acctnumber == e.Acct);
                            if (r == null || Math.Abs(r.Amount - e.Amt) > 0.011) { ok = false; Console.WriteLine($"  {prd} {e.Atm} {e.Acct} {r?.Amount} vs {e.Amt:0.00}"); }
                        }
                        foreach (var n in emps) ok &= rows.Where(x => x.IsAtm == (n.Atm == "Y")).All(x => x.TotalEmp == n.N);
                        foreach (var g in rows.GroupBy(x => x.IsAtm))
                        {
                            double net = g.Where(x => x.Section == "EARNINGS").Sum(x => x.Amount) - g.Where(x => x.Section == "DEDUCTIONS").Sum(x => x.Amount);
                            ok &= Math.Abs(net - g.First().NetPay) < 0.01 * g.Count();
                        }
                        if (!ok) { bad++; Console.WriteLine($"DIFF {prd}"); }
                    }
                    Console.WriteLine($"periods={prds.Count} diffs={bad}");
                    return;
                }
            case "gt":
            case "gtcl":
                {
                    // <trdp> <out.pdf> <fromPrd> <toPrd>: every account and client
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct", new { }, conn);
                    var cls = (await rep._02GrandTotalClients(args[3], args[4], accts, paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                    var rows = await rep._02GrandTotal(args[3], args[4], accts, cls, args[0] == "gtcl", paydb, pisdb, conn);
                    prm["From"] = args[3]; prm["To"] = args[4];
                    Console.WriteLine($"clients={cls.Count} rows={rows.Count} net={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "gtcheck":
                {
                    // each period, and the whole range: grand total per account vs plain SQL; by client sums to the same
                    var prds = await sql.FetchData<string, dynamic>($"select distinct left(TRN,6) from {paydb}.tbltran order by 1", new { }, conn);
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct", new { }, conn);
                    int bad = 0;
                    foreach (var (f, t) in prds.Select(p => (p, p)).Append((prds[0], prds[^1])))
                    {
                        var cls = (await rep._02GrandTotalClients(f, t, accts, paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                        var tt = await rep._02GrandTotal(f, t, accts, cls, false, paydb, pisdb, conn);
                        var cl = await rep._02GrandTotal(f, t, accts, cls, true, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Acct, double Amt), dynamic>(
                            $"select trim(t.AcctNumber), sum(if(left(t.AcctNumber,1)='D',-t.Amount,t.Amount)) + 0e0 from {paydb}.tbltran t join {paydb}.chartofacct c on trim(c.AcctNumber) = trim(t.AcctNumber) where left(t.TRN,6) between @F and @T group by 1", new { F = f, T = t }, conn);
                        bool ok = exp.Count == tt.Count && exp.All(e => Math.Abs(tt.First(x => x.Acctnumber == e.Acct).Amount - e.Amt) < 0.011)
                                  && tt.All(x => Math.Abs(cl.Where(c => c.Acctnumber == x.Acctnumber).Sum(c => c.Amount) - x.Amount) < 0.011 * Math.Max(1, cls.Count));
                        if (!ok) { bad++; Console.WriteLine($"DIFF {f}-{t} accounts {tt.Count} vs {exp.Count}"); }
                        else if (f != t) Console.WriteLine($"ok {f}-{t} accounts {tt.Count} clients {cls.Count} net {tt.Sum(x => x.Amount):N2}");
                    }
                    Console.WriteLine($"ranges={prds.Count + 1} diffs={bad}");
                    return;
                }
            case "earnsum":
            case "dedsum":
                {
                    // <trdp> <out.pdf> <yymm>: every earning / deduction account
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct where left(AcctNumber,1) = @T", new { T = args[0] == "earnsum" ? "E" : "D" }, conn);
                    var rows = await rep._02EarnDedSummary(args[3], accts, paydb, pisdb, conn);
                    prm["MM"] = args[3]; prm["YY"] = "20" + args[3][..2];
                    Console.WriteLine($"accounts={accts.Count} rows={rows.Count} total={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "earndedcheck":
                {
                    // each month, every account of both kinds: rows and total vs plain SQL
                    var mons = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.tbltran order by 1", new { }, conn);
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct", new { }, conn);
                    int bad = 0;
                    foreach (var m in mons)
                    {
                        var rows = await rep._02EarnDedSummary(m, accts, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(*), sum(round(Amount,2)) + 0e0 from {paydb}.tbltran t join {paydb}.chartofacct c on trim(c.AcctNumber) = trim(t.AcctNumber) where left(t.TRN,4) = @M", new { M = m }, conn);
                        if (rows.Count != exp[0].N || Math.Abs(rows.Sum(r => r.Amount) - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {m} {rows.Count} vs {exp[0].N} {rows.Sum(r => r.Amount):N2} vs {exp[0].Amt:N2}"); }
                    }
                    Console.WriteLine($"months={mons.Count} diffs={bad}");
                    return;
                }
            case "earnhist":
            case "dedhist":
                {
                    // <trdp> <out.pdf> <empNumber> <fromDate> <toDate>
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct where left(AcctNumber,1) = @T", new { T = args[0] == "earnhist" ? "E" : "D" }, conn);
                    DateTime f = DateTime.Parse(args[4]), t = DateTime.Parse(args[5]);
                    var rows = await rep._02EarnDedHistory(args[3], f, t, accts, paydb, conn);
                    prm["EmpName"] = args[3]; prm["From"] = f.ToString("MMMM d, yyyy"); prm["To"] = t.ToString("MMMM d, yyyy");
                    Console.WriteLine($"rows={rows.Count} total={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "histcheck":
                {
                    // every employee with TRNs, whole range: rows + total vs plain SQL; a one-month range keeps only that month's periods
                    var emps = await sql.FetchData<string, dynamic>($"select distinct trim(EmpNumber) from {paydb}.tbltran", new { }, conn);
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct", new { }, conn);
                    int bad = 0;
                    foreach (var e in emps)
                    {
                        var rows = await rep._02EarnDedHistory(e, new DateTime(2000, 1, 1), new DateTime(2099, 12, 31), accts, paydb, conn);
                        var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(*), ifnull(sum(round(t.Amount,2)),0) + 0e0 from {paydb}.tbltran t join {paydb}.chartofacct c on trim(c.AcctNumber) = trim(t.AcctNumber) where trim(t.EmpNumber) = @E", new { E = e }, conn);
                        if (rows.Count != exp[0].N || Math.Abs(rows.Sum(r => r.Amount) - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {e} {rows.Count} vs {exp[0].N}"); }
                        var one = await rep._02EarnDedHistory(e, new DateTime(2026, 5, 1), new DateTime(2026, 5, 15), accts, paydb, conn);
                        if (one.Any(r => r.Period!.StartsWith("2605") == false || r.Pp != "01")) { bad++; Console.WriteLine($"RANGE {e}"); }
                    }
                    Console.WriteLine($"employees={emps.Count} diffs={bad}");
                    return;
                }
            case "monthly":
                {
                    // <trdp> <out.pdf> <yymm> <acct>
                    var rows = await rep._02EarnDedMonthly(args[3], args[4], paydb, pisdb, conn);
                    prm["P1"] = "May 1-15"; prm["P2"] = "May 16-31"; prm["MM"] = "May"; prm["YY"] = "2026";
                    Console.WriteLine($"rows={rows.Count} p1={rows.Sum(r => r.Amount01):N2} p2={rows.Sum(r => r.Amount02):N2}");
                    data = rows;
                    break;
                }
            case "monthlycheck":
                {
                    // each month, every account of both kinds: totals per period and employee count vs plain SQL
                    var mons = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.tbltran order by 1", new { }, conn);
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct", new { }, conn);
                    int bad = 0, n = 0;
                    foreach (var m in mons) foreach (var a in accts)
                    {
                        var rows = await rep._02EarnDedMonthly(m, a, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(long Emps, double P1, double P2), dynamic>($"select count(distinct EmpNumber), ifnull(sum(if(substring(trim(TRN),5,2)='01', round(Amount,2), 0)),0) + 0e0, ifnull(sum(if(substring(trim(TRN),5,2)='02', round(Amount,2), 0)),0) + 0e0 from {paydb}.tbltran where left(TRN,4) = @M and trim(AcctNumber) = @A", new { M = m, A = a }, conn);
                        if (exp[0].Emps == 0 && rows.Count == 0) continue;
                        n++;
                        if (rows.Count != exp[0].Emps || Math.Abs(rows.Sum(r => r.Amount01) - exp[0].P1) > 0.01 * Math.Max(1, rows.Count) || Math.Abs(rows.Sum(r => r.Amount02) - exp[0].P2) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {m} {a} {rows.Count} vs {exp[0].Emps}"); }
                    }
                    Console.WriteLine($"month+account sets={n} diffs={bad}");
                    return;
                }
            case "consolcheck":
                {
                    // all statuses + every deduction account: total and employee count vs plain SQL
                    var st = (await rep._02EmpStatuses(pisdb, conn)).Select(x => x.Code!).ToList();
                    var accts = await sql.FetchData<string, dynamic>($"select trim(AcctNumber) from {paydb}.chartofacct where AcctType = 'D'", new { }, conn);
                    var rows = await rep._02ConsolidatedDeduction(st, accts, paydb, pisdb, conn);
                    var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(distinct t.EmpNumber), sum(round(t.Amount,4)) + 0e0 from {paydb}.tbltran t join {pisdb}.empmas e on e.EmpNumber = t.EmpNumber where trim(t.AcctNumber) in @A and trim(e.EmpStat_) in @S", new { A = accts, S = st }, conn);
                    Console.WriteLine($"statuses={st.Count} accts={accts.Count} rows={rows.Count} vs {exp[0].N}  total={rows.Sum(r => r.Amount):N2} vs {exp[0].Amt:N2}");
                    return;
                }
            case "loancheck":
                {
                    // every loan type, every employee (resigned too): one latest loan per employee; payments vs plain SQL
                    var types = await rep._02LoanTypes(paydb, conn);
                    var emps = (await rep._02LoanEmployees(true, pisdb, conn)).Select(e => e.Empnumber!).ToList();
                    int bad = 0, total = 0;
                    foreach (var t in types)
                    {
                        var rows = await rep._02LoanSummary(t.Code!, emps, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Emp, double Pay), dynamic>(
                            $@"select trim(l.EmpNumber), ifnull((select sum(d.Amort) from {paydb}.loandtl d where d.AcctNumber = l.DEDNCODE and d.EmpNumber = l.EmpNumber and d.LoanDate = l.Date),0) + 0e0
                               from {paydb}.loans l where l.DEDNCODE = @C and l.Number in (select max(Number) from {paydb}.loans where DEDNCODE = @C group by EmpNumber)", new { C = t.Code }, conn);
                        total += rows.Count;
                        if (rows.Count != exp.Count || exp.Any(e => Math.Abs((rows.FirstOrDefault(r => r.Empnumber == e.Emp)?.Pymt ?? -1) - e.Pay) > 0.011)) { bad++; Console.WriteLine($"DIFF {t.Code} {rows.Count} vs {exp.Count}"); }
                    }
                    Console.WriteLine($"loan types={types.Count} employees={emps.Count} rows={total} diffs={bad}");
                    return;
                }
            case "tmcheck":
                {
                    // each half of each year with E001 data: every employee; total and slot totals vs plain SQL
                    var yrs = await sql.FetchData<int, dynamic>($"select distinct 2000 + cast(left(TRN,2) as unsigned) from {paydb}.tbltran where trim(AcctNumber) = 'E001' order by 1", new { }, conn);
                    int bad = 0, n = 0;
                    foreach (var y in yrs) foreach (var half in new[] { "01", "02" })
                    {
                        var rows = await rep._02ThirteenthMonth(half, y, null, null, true, null, paydb, pisdb, conn);
                        string yy = (y % 100).ToString("00"), py = ((y + 99) % 100).ToString("00");
                        string from = half == "02" ? py + "12" : yy + "06", to = half == "02" ? yy + "05" : yy + "11";
                        var exp = await sql.FetchData<(long Emps, double Amt), dynamic>($"select count(distinct trim(EmpNumber)), ifnull(sum(round(Amount,2)),0) + 0e0 from {paydb}.tbltran where trim(AcctNumber) = 'E001' and left(TRN,4) between @F and @T", new { F = from, T = to }, conn);
                        if (rows.Count == 0 && exp[0].Emps == 0) continue;
                        n++;
                        double tot = rows.Sum(r => r.Total);
                        if (rows.Count != exp[0].Emps || Math.Abs(tot - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {y}-{half}: {rows.Count} vs {exp[0].Emps}  {tot:N2} vs {exp[0].Amt:N2}"); }
                        else Console.WriteLine($"ok {y}-{half}: employees {rows.Count} total {tot:N2}");
                    }
                    Console.WriteLine($"halves={n} diffs={bad}");
                    return;
                }
            case "tm":
                {
                    // <trdp> <out.pdf> <half> <year> [client=<code>|emp=<no>|atm|cash]
                    string half = args[3]; int year = int.Parse(args[4]);
                    string opt = args.Length > 5 ? args[5] : "";
                    var rows = await rep._02ThirteenthMonth(half, year, null, opt.StartsWith("client=") ? [opt[7..]] : null, !opt.StartsWith("client="), opt.StartsWith("emp=") ? opt[4..] : null, paydb, pisdb, conn);
                    if (opt == "atm") rows = rows.Where(r => r.Acctcode != "" && r.Amount > 0).ToList();
                    if (opt == "cash") rows = rows.Where(r => r.Acctcode == "" && r.Amount > 0).OrderBy(r => r.Clname).ThenBy(r => r.Emplastnm).ToList();
                    prm["PP"] = half == "02" ? $"December, {year - 1} - May, {year}" : $"June, {year} - November, {year}";
                    prm["Particulars"] = "In payment of 13th month pay for the period " + (half == "02" ? $"December 1, {year - 1}, to May 31, {year}" : $"June 1, {year}, to November 30, {year}");
                    Console.WriteLine($"rows={rows.Count} total={rows.Sum(r => r.Total):N2}");
                    data = rows;
                    break;
                }
            case "sssremit":
            case "sssremitadj":
                {
                    // <trdp> <out.pdf> <yymm>
                    var rows = await rep._02SssRemit(args[3], args[0] == "sssremitadj", paydb, pisdb, conn);
                    prm["MM"] = "May"; prm["YY"] = "2026"; prm["EmployerSss"] = "03-0000000-0";
                    Console.WriteLine($"rows={rows.Count} ee={rows.Sum(r => r.Amount):N2} er={rows.Sum(r => r.Fssser):N2} ec={rows.Sum(r => r.Fecc):N2}");
                    data = rows;
                    break;
                }
            case "sssremitcheck":
                {
                    // each month: employee share total and employee count vs plain SQL; the adjusted amounts are all ssstbl shares
                    var mons = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.tbltran where trim(AcctNumber) = 'D002' order by 1", new { }, conn);
                    var shares = (await sql.FetchData<double, dynamic>($"select FSSSEE + 0e0 from {paydb}.ssstbl", new { }, conn)).Select(x => Math.Round(x, 2)).ToHashSet();
                    int bad = 0, unmatched = 0;
                    foreach (var m in mons)
                    {
                        var rows = await rep._02SssRemit(m, false, paydb, pisdb, conn);
                        var adj  = await rep._02SssRemit(m, true, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(distinct EmpNumber), sum(round(Amount,2)) + 0e0 from {paydb}.tbltran where trim(AcctNumber) = 'D002' and left(TRN,4) = @M", new { M = m }, conn);
                        if (rows.Count != exp[0].N || Math.Abs(rows.Sum(r => r.Amount) - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {m} {rows.Count} vs {exp[0].N}"); }
                        unmatched += rows.Count(r => r.Fssser == 0);
                        if (adj.Any(r => !shares.Contains(r.Amount) && r.Amount != rows.First(x => x.Empnumber == r.Empnumber).Amount)) { bad++; Console.WriteLine($"ADJ {m}"); }
                    }
                    Console.WriteLine($"months={mons.Count} diffs={bad} rows without ssstbl match={unmatched}");
                    return;
                }
            case "sssloan":
                {
                    // <trdp> <out.pdf> <yymm> [acct]: every client of the month; acct = another loan account (the local DB has no SSS loans)
                    var cls = (await rep._02MonthClients(args[3], paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                    var rows = args.Length > 4 ? await rep._02LoanRefRemit(args[4], args[3], cls, paydb, pisdb, conn) : await rep._02SssLoanRemit(args[3], cls, paydb, pisdb, conn);
                    prm["MonthYear"] = "May, 2026"; prm["EmployerSss"] = "03-0000000-0";
                    Console.WriteLine($"clients={cls.Count} rows={rows.Count} amort={rows.Sum(r => r.Amort):N2}");
                    data = rows;
                    break;
                }
            case "sssloancheck":
                {
                    // each month, every client: employees and amortization vs plain SQL (loansref joined to tbltran)
                    var mons = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.loansref where trim(DednCode) = 'D078' order by 1", new { }, conn);   // D005 has no loansref rows locally
                    int bad = 0;
                    foreach (var m in mons)
                    {
                        var cls = (await rep._02MonthClients(m, paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                        var rows = await rep._02LoanRefRemit("D078", m, cls, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(distinct a.EmpNumber), ifnull(sum(round(a.Amort,2)),0) + 0e0 from {paydb}.loansref a where trim(a.DednCode) = 'D078' and left(a.TRN,4) = @M and exists (select 1 from {paydb}.tbltran x where x.TRN = a.TRN and x.EmpNumber = a.EmpNumber and x.AcctNumber = a.DednCode)", new { M = m }, conn);
                        if (rows.Count != exp[0].N || Math.Abs(rows.Sum(r => r.Amort) - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {m} {rows.Count} vs {exp[0].N}  {rows.Sum(r => r.Amort):N2} vs {exp[0].Amt:N2}"); }
                    }
                    Console.WriteLine($"months={mons.Count} diffs={bad}");
                    return;
                }
            case "sssloancounts":
                {
                    foreach (var t in new[] { "loansref", "loandtl", "loans", "sssloandtl" })
                    {
                        try { var n = await sql.FetchData<long, dynamic>($"select count(*) from {paydb}.{t}", new { }, conn); Console.WriteLine($"{t}: {n[0]}"); }
                        catch (Exception e) { Console.WriteLine($"{t}: {e.Message[..Math.Min(80, e.Message.Length)]}"); }
                    }
                    foreach (var r in await sql.FetchData<(string C, long N, string Mn, string Mx), dynamic>($"select trim(DednCode), count(*), min(left(TRN,6)), max(left(TRN,6)) from {paydb}.loansref group by 1 order by 2 desc limit 8", new { }, conn)) Console.WriteLine($"loansref {r.C}: {r.N} trn {r.Mn}..{r.Mx}");
                    foreach (var r in await sql.FetchData<(string T, string E), dynamic>($"select trim(TRN), trim(EmpNumber) from {paydb}.tbltran where trim(AcctNumber) = 'D005'", new { }, conn)) Console.WriteLine($"tbltran D005 {r.T} {r.E}");
                    var d5 = await sql.FetchData<long, dynamic>($"select count(*) from {paydb}.tbltran where trim(AcctNumber) = 'D005'", new { }, conn);
                    Console.WriteLine($"tbltran D005: {d5[0]}");
                    return;
                }
            case "sysmenu":
                {
                    // every row of {paydb}.sysmenu, columns as they come
                    var cols = await sql.FetchData<string, dynamic>($"select column_name from information_schema.columns where table_schema = @S and table_name = 'sysmenu' order by ordinal_position", new { S = paydb }, conn);
                    Console.WriteLine("columns: " + string.Join(", ", cols));
                    var rows = await sql.FetchData<string, dynamic>($"select concat_ws(' | ', {string.Join(", ", cols.Select(c => $"ifnull(cast(`{c}` as char),'')"))}) from {paydb}.sysmenu", new { }, conn);
                    Console.WriteLine($"rows={rows.Count}");
                    foreach (var r in rows) Console.WriteLine(r);
                    return;
                }
            case "phmo":
                {
                    // <trdp> <out.pdf> <yymm>: every client of the month
                    var cls = (await rep._02MonthClients(args[3], paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                    var rows = await rep._02PhicMonthly(args[3], cls, paydb, pisdb, conn);
                    prm["MM"] = "May"; prm["YY"] = "2026";
                    Console.WriteLine($"clients={cls.Count} rows={rows.Count} amount={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "phmocheck":
                {
                    // each month, every client: employees and total vs plain SQL; one client alone still gives the employee's whole month
                    var mons = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.tbltran where trim(AcctNumber) = 'D003' order by 1", new { }, conn);
                    int bad = 0;
                    foreach (var m in mons)
                    {
                        var cls = (await rep._02MonthClients(m, paydb, pisdb, conn)).Select(c => c.Clnumber!).ToList();
                        var rows = await rep._02PhicMonthly(m, cls, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(long N, double Amt), dynamic>($"select count(distinct EmpNumber), sum(round(Amount,2)) + 0e0 from {paydb}.tbltran where trim(AcctNumber) = 'D003' and left(TRN,4) = @M", new { M = m }, conn);
                        if (rows.Count != exp[0].N || Math.Abs(rows.Sum(r => r.Amount) - exp[0].Amt) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {m} {rows.Count} vs {exp[0].N}"); }
                        var one = await rep._02PhicMonthly(m, [cls[0]], paydb, pisdb, conn);
                        if (one.Any(o => Math.Abs(o.Amount - rows.First(r => r.Empnumber == o.Empnumber).Amount) > 0.001)) { bad++; Console.WriteLine($"ONE {m}"); }
                    }
                    Console.WriteLine($"months={mons.Count} diffs={bad}");
                    return;
                }
            case "phquarter":
                {
                    // <trdp> <out.pdf> <quarter> <year>
                    int q = int.Parse(args[3]), y = int.Parse(args[4]);
                    var rows = await rep._02PhicQuarter(q, y, paydb, pisdb, conn);
                    var names = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                    prm["M1"] = names[(q - 1) * 3]; prm["M2"] = names[(q - 1) * 3 + 1]; prm["M3"] = names[(q - 1) * 3 + 2];
                    Console.WriteLine($"rows={rows.Count} m1={rows.Sum(r => r.P1ee):N2} m2={rows.Sum(r => r.P2ee):N2} m3={rows.Sum(r => r.P3ee):N2}");
                    data = rows;
                    break;
                }
            case "phquartercheck":
                {
                    // each quarter of each year with D003: employees and the three month totals vs plain SQL
                    var yrs = await sql.FetchData<int, dynamic>($"select distinct 2000 + cast(left(TRN,2) as unsigned) from {paydb}.tbltran where trim(AcctNumber) = 'D003' order by 1", new { }, conn);
                    int bad = 0, n = 0;
                    foreach (var y in yrs) for (int q = 1; q <= 4; q++)
                    {
                        var rows = await rep._02PhicQuarter(q, y, paydb, pisdb, conn);
                        string yy = (y % 100).ToString("00");
                        double[] exp = new double[3]; long emps = 0;
                        for (int k = 0; k < 3; k++)
                        {
                            var e = await sql.FetchData<(long N, double A), dynamic>($"select count(distinct EmpNumber), ifnull(sum(round(Amount,2)),0) + 0e0 from {paydb}.tbltran where trim(AcctNumber) = 'D003' and left(TRN,4) = @M", new { M = yy + ((q - 1) * 3 + 1 + k).ToString("00") }, conn);
                            exp[k] = e[0].A;
                        }
                        var allEmps = await sql.FetchData<long, dynamic>($"select count(distinct EmpNumber) from {paydb}.tbltran where trim(AcctNumber) = 'D003' and left(TRN,4) in @M", new { M = new[] { 1, 2, 3 }.Select(k => yy + ((q - 1) * 3 + k).ToString("00")).ToList() }, conn);
                        emps = allEmps[0];
                        if (rows.Count == 0 && emps == 0) continue;
                        n++;
                        if (rows.Count != emps || Math.Abs(rows.Sum(r => r.P1ee) - exp[0]) > 0.01 * Math.Max(1, rows.Count) || Math.Abs(rows.Sum(r => r.P2ee) - exp[1]) > 0.01 * Math.Max(1, rows.Count) || Math.Abs(rows.Sum(r => r.P3ee) - exp[2]) > 0.01 * Math.Max(1, rows.Count)) { bad++; Console.WriteLine($"DIFF {y} Q{q}"); }
                    }
                    Console.WriteLine($"quarters={n} diffs={bad}");
                    return;
                }
            case "psemps":
                {
                    // <yymmpp>: the TRN + employee list, and each payslip of the first TRN alone equals its rows of the whole list
                    var list = await rep._02PayslipEmployees(args[1], paydb, pisdb, conn);
                    var trn = list.First().Trn!;
                    var all = await rep._02Payslip([trn], paydb, pisdb, conn);
                    int bad = 0;
                    foreach (var e in list.Where(x => x.Trn == trn)) if (!all.Any(r => r.Empnumber == e.Empnumber)) bad++;
                    Console.WriteLine($"list={list.Count} firstTrn={trn} employees={list.Count(x => x.Trn == trn)} slips={all.Select(r => r.Empnumber).Distinct().Count()} missing={bad} sample={list.First().Empname} / {list.First().Clname2}");
                    return;
                }
            case "hdmfcert":
            case "phiccert":
                {
                    // <trdp> <out.pdf> <fromYymm> <toYymm> [emp,emp]: every employee when none given
                    bool phic = args[0] == "phiccert";
                    var emps = args.Length > 5 ? args[5].Split(',').ToList() : (await rep._02ReportEmployees(pisdb, conn)).Select(e => e.Empnumber!.Trim()).ToList();
                    var rows = await rep._02ContributionSummary(phic ? "ACCT_PHIC" : "ACCT_HDMF", args[3], args[4], emps, phic, paydb, pisdb, conn);
                    var s = await set.Value; string k = phic ? "CERT_PHIC_" : "CERT_HDMF_";
                    prm["CertText1"] = s[k + "TEXT1"]; prm["CertText2"] = s[k + "TEXT2"]; prm["Signatory"] = s[k + "SIGNATORY"]; prm["SignatoryTitle"] = s[k + "SIGNATORYTITLE"];
                    Console.WriteLine($"emps={rows.Select(r => r.Empnumber).Distinct().Count()} rows={rows.Count} total={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "certcheck":
                {
                    // whole range, every employee: total per employee vs plain SQL (PhilHealth per month, Pag-IBIG per TRN)
                    var s = await set.Value; int bad = 0;
                    var emps = (await rep._02ReportEmployees(pisdb, conn)).Select(e => e.Empnumber!.Trim()).ToList();
                    foreach (var (code, perMonth) in new[] { ("ACCT_HDMF", false), ("ACCT_PHIC", true) })
                    {
                        var rows = await rep._02ContributionSummary(code, "0000", "9999", emps, perMonth, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Emp, double Amt, long N), dynamic>(
                            $"select trim(EmpNumber), sum(round(Amount,4)) + 0e0, count(distinct {(perMonth ? "left(TRN,4)" : "TRN")}) from {paydb}.tbltran where trim(AcctNumber) = @A group by 1", new { A = s[code] }, conn);
                        foreach (var e in exp)
                        {
                            var g = rows.Where(r => r.Empnumber == e.Emp).ToList();
                            if (g.Count != e.N || Math.Abs(g.Sum(r => r.Amount) - e.Amt) > 0.01 * g.Count) { bad++; if (bad <= 5) Console.WriteLine($"DIFF {code} {e.Emp} {g.Sum(r => r.Amount):N2} vs {e.Amt:N2} rows {g.Count} vs {e.N}"); }
                        }
                        Console.WriteLine($"{code}: employees={exp.Count} rows={rows.Count}");
                    }
                    Console.WriteLine($"diffs={bad}");
                    return;
                }
            case "hdmfloan":
            case "pagibigloan":
                {
                    // <trdp> <out.pdf> <yymm>: HDMF = ACCT_HDMFLOAN, every client; Pag-IBIG = HDMF + Pag-IBIG loan, every client
                    var accts = args[0] == "hdmfloan" ? new List<string> { "ACCT_HDMFLOAN" } : ["ACCT_HDMFLOAN", "ACCT_PAGIBIGLOAN"];
                    var rows = await rep._02Remittance(accts, args[3], args[0] == "hdmfloan" ? null : await sql.FetchData<string, dynamic>($"select ClNumber from {pisdb}.client", new { }, conn), paydb, pisdb, conn);
                    int y = 2000 + int.Parse(args[3][..2]);
                    prm["Month"] = new DateTime(y, int.Parse(args[3][2..4]), 1).ToString("MMMM"); prm["Year"] = y.ToString(); prm["EmployerSss"] = "03-0000000-0";
                    Console.WriteLine($"rows={rows.Count} total={rows.Sum(r => r.Amount):N2}");
                    data = rows;
                    break;
                }
            case "pagibigcont":
                {
                    // <trdp> <out.pdf> <yymm>: every client
                    var rows = await rep._02PagibigContRemit(args[3], null, paydb, pisdb, conn);
                    int y = 2000 + int.Parse(args[3][..2]);
                    prm["Month"] = new DateTime(y, int.Parse(args[3][2..4]), 1).ToString("MMMM"); prm["Year"] = y.ToString(); prm["EmployerSss"] = "03-0000000-0";
                    double cap = double.Parse((await set.Value)["PAGIBIG_ER_MAX"]);
                    Console.WriteLine($"rows={rows.Count} ee={rows.Sum(r => r.Amount):N2} er={rows.Sum(r => r.Er):N2} capOk={rows.All(r => r.Er == Math.Min(r.Amount, cap))}");
                    data = rows;
                    break;
                }
            case "remitcheck":
                {
                    // every month: per employee totals of the loan accounts vs plain SQL
                    var s = await set.Value; int bad = 0;
                    var months = await sql.FetchData<string, dynamic>($"select distinct left(TRN,4) from {paydb}.tbltran order by 1", new { }, conn);
                    foreach (var mo in months)
                        foreach (var accts in new[] { new List<string> { "ACCT_HDMFLOAN" }, ["ACCT_HDMFLOAN", "ACCT_PAGIBIGLOAN"], ["ACCT_SSSLOAN"], ["ACCT_HDMF"] })
                        {
                            var rows = await rep._02Remittance(accts, mo, null, paydb, pisdb, conn);
                            var exp = await sql.FetchData<(string Emp, double Amt), dynamic>(
                                $"select trim(EmpNumber), sum(Amount) + 0e0 from {paydb}.tbltran where left(TRN,4)=@M and trim(AcctNumber) in @A group by 1", new { M = mo, A = accts.Select(a => s[a]).ToList() }, conn);
                            if (exp.Count != rows.Count || exp.Any(e => Math.Abs(rows.First(r => r.Empnumber == e.Emp).Amount - e.Amt) > 0.006)) { bad++; Console.WriteLine($"DIFF {mo} {string.Join("+", accts)}"); }
                        }
                    Console.WriteLine($"months={months.Count} diffs={bad}");
                    return;
                }
            case "settings":
                {
                    // makes {paydb}.reportsettings with the real migration code, then lists it
                    await new HRApiLibrary.DataAccess._00_Main.OldPay_TblMakerDataAccess(sql)._01ReportSettings(paydb, conn);
                    foreach (var s in await rep._02ReportSettings(paydb, conn)) Console.WriteLine($"{s.Key,-16} {s.Value}");
                    return;
                }
            case "advise":
            case "cash":
            case "charge":
                {
                    // <trdp> <out.pdf> <yymmpp> [trns]
                    var prd = args[3];
                    var trns = args.Length > 4 ? args[4].Split(',').ToList() : (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                    var rows = args[0] == "charge" ? await rep._02BankCharge(trns, paydb, pisdb, conn) : await rep._02BankAdvise(trns, args[0] == "cash", paydb, pisdb, conn);
                    prm["Period"] = rows.FirstOrDefault()?.Period ?? "";
                    prm["Batch"]  = string.Join(", ", rows.Select(r => r.Batch).Distinct());
                    prm["BankCharge"] = double.Parse((await set.Value)["BANKCHARGE"]);
                    Console.WriteLine($"trns={trns.Count} rows={rows.Count} amt={rows.Sum(r => r.Amt):N2}");
                    data = rows;
                    break;
                }
            case "textfile":
                {
                    // <out.xlsx> <yymmpp> <motherAcct> [trns]: HRMvc BankTextFileSheet, read back
                    var prd = args[2];
                    var trns = args.Length > 4 ? args[4].Split(',').ToList() : (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                    var rows = await rep._02BankTextFile(trns, paydb, pisdb, conn);
                    var s = await set.Value;
                    File.WriteAllBytes(args[1], HRMvc.Applications.PayrollReport.Class.BankTextFileSheet.Build(rows, args[3], s["BANKCURRENCY"], s[prd.EndsWith("01") ? "BANKPAYCODE01" : "BANKPAYCODE02"], int.Parse(s["BANKACCTLEN"])));
                    using var pkg = new OfficeOpenXml.ExcelPackage(new FileInfo(args[1]));
                    var ws = pkg.Workbook.Worksheets.First();
                    for (int r = 1; r <= Math.Min(5, ws.Dimension.End.Row); r++)
                        Console.WriteLine(string.Join(" | ", Enumerable.Range(1, 7).Select(c => ws.Cells[r, c].Formula is { Length: > 0 } f ? "=" + f : ws.Cells[r, c].Value?.ToString())));
                    double sheet = Enumerable.Range(3, ws.Dimension.End.Row - 2).Sum(r => Convert.ToDouble(ws.Cells[r, 5].Value));
                    Console.WriteLine($"rows={ws.Dimension.End.Row - 2} sheet total={sheet:N2} DA total={rows.Sum(r => r.Amt):N2}");
                    return;
                }
            case "bankcheck":
                {
                    // per period (all TRNs): ATM + cash totals vs plain SQL net of employees with earnings; one charge line per ATM employee
                    var prds = await sql.FetchData<string, dynamic>($"select distinct left(TRN,6) from {paydb}.tbltran order by 1", new { }, conn);
                    int bad = 0; double tot = 0;
                    foreach (var prd in prds)
                    {
                        var trns = (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                        var atm  = await rep._02BankAdvise(trns, false, paydb, pisdb, conn);
                        var cash = await rep._02BankAdvise(trns, true, paydb, pisdb, conn);
                        var chg  = await rep._02BankCharge(trns, paydb, pisdb, conn);
                        var txt  = await rep._02BankTextFile(trns, paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Atm, double Net), dynamic>(
                            $@"select if(trim(ifnull(e.AcctCode,''))='','N','Y'), sum(x.Net) + 0e0 from
                                 (select TRN, EmpNumber, sum(if(left(AcctNumber,1)='E',Amount,-Amount)) Net from {paydb}.tbltran where left(TRN,6)=@P
                                  group by TRN, EmpNumber having sum(left(AcctNumber,1)='E') > 0) x
                               left join {pisdb}.empmas e on e.EmpNumber = x.EmpNumber group by 1", new { P = prd }, conn);
                        double eAtm = exp.Where(x => x.Atm == "Y").Sum(x => x.Net), eCash = exp.Where(x => x.Atm == "N").Sum(x => x.Net);
                        bool ok = Math.Abs(atm.Sum(r => r.Amt) - eAtm) < 0.01 * Math.Max(1, atm.Count) && Math.Abs(cash.Sum(r => r.Amt) - eCash) < 0.01 * Math.Max(1, cash.Count)
                                  && chg.Select(r => r.Empnumber).Distinct().Count() == chg.Count && chg.Count <= atm.Count
                                  && txt.Count == atm.Count && Math.Abs(txt.Sum(r => r.Amt) - atm.Sum(r => r.Amt)) < 0.001;
                        tot += atm.Sum(r => r.Amt) + cash.Sum(r => r.Amt);
                        if (!ok) { bad++; Console.WriteLine($"DIFF {prd} atm {atm.Sum(r => r.Amt):N2} vs {eAtm:N2} ({atm.Count}) cash {cash.Sum(r => r.Amt):N2} vs {eCash:N2} ({cash.Count}) charge {chg.Count}"); }
                        else if (prd == prds.Last() || cash.Count > 0) Console.WriteLine($"ok {prd} atm {atm.Count} cash {cash.Count} charge {chg.Count}");
                    }
                    Console.WriteLine($"periods={prds.Count} net={tot:N2} diffs={bad}");
                    return;
                }
            case "payslip":
                {
                    var prd = args[3];
                    var trns = args.Length > 4 ? args[4].Split(',').ToList() : (await rep._02PayrollRegisterTrns(prd, paydb, pisdb, conn)).Select(t => t.Trn!).ToList();
                    var rows = await rep._02Payslip(trns, paydb, pisdb, conn);
                    prm["CoNameShort"] = "MBI";
                    prm["PayslipFont"] = (await set.Value)["PAYSLIPFONT"];
                    Console.WriteLine($"trns={trns.Count} slips={rows.Select(r => (r.Trn, r.Empnumber)).Distinct().Count()} lines={rows.Count} gross={rows.Sum(r => r.Amount):N2} ded={rows.Sum(r => r.Amount2):N2}");
                    data = rows;
                    break;
                }
            case "payslipcheck":
                {
                    // per TRN + employee: payslip earnings/deductions vs plain SQL sums of tbltran (all accounts), every TRN in the DB
                    var trns = await sql.FetchData<string, dynamic>($"select distinct TRN from {paydb}.tbltran order by 1", new { }, conn);
                    int bad = 0, nS = 0, nL = 0;
                    foreach (var chunk in trns.Chunk(20))
                    {
                        var rows = await rep._02Payslip(chunk.ToList(), paydb, pisdb, conn);
                        var exp = await sql.FetchData<(string Trn, string Emp, double E, double D, long NE, long ND), dynamic>(
                            $"select TRN, trim(EmpNumber), sum(if(left(AcctNumber,1)='E',round(cast(Amount as decimal(14,4)),2),0)) + 0e0, sum(if(left(AcctNumber,1)='D',round(cast(Amount as decimal(14,4)),2),0)) + 0e0, sum(left(AcctNumber,1)='E'), sum(left(AcctNumber,1)='D') from {paydb}.tbltran where TRN in @T group by 1,2", new { T = chunk }, conn);
                        foreach (var e in exp)
                        {
                            nS++;
                            var g = rows.Where(r => r.Trn == e.Trn && r.Empnumber!.Trim() == e.Emp).ToList(); nL += g.Count;
                            double ge = Math.Round(g.Sum(r => r.Amount), 2), gd = Math.Round(g.Sum(r => r.Amount2), 2);
                            long ne = g.Count(r => !string.IsNullOrEmpty(r.Acctnumber)), nd = g.Count(r => !string.IsNullOrEmpty(r.Acctnumber2));
                            if (Math.Abs(ge - e.E) > 0.011 || Math.Abs(gd - e.D) > 0.011 || ne != e.NE || nd != e.ND || g.Count != Math.Max(13, Math.Max(e.NE, e.ND)))
                            { bad++; if (bad <= 10) Console.WriteLine($"DIFF {e.Trn} {e.Emp} earn {ge} vs {e.E} ded {gd} vs {e.D} lines {g.Count} E {ne}/{e.NE} D {nd}/{e.ND}"); }
                        }
                    }
                    Console.WriteLine($"trns={trns.Count} payslips={nS} lines={nL} diffs={bad}");
                    return;
                }
            default: throw new ArgumentException(args[0]);
        }

        var report = (Telerik.Reporting.Report)new ReportPackager().UnpackageDocument(File.OpenRead(args[1]));
        foreach (var p in prm) if (report.ReportParameters[p.Key] != null) report.ReportParameters[p.Key].Value = p.Value;
        report.DataSource = new ObjectDataSource { DataSource = data };
        var fmt = Environment.GetEnvironmentVariable("FORMAT") ?? "PDF";
        var result = new ReportProcessor().RenderReport(fmt, new InstanceReportSource { ReportDocument = report }, null);
        if (result.Errors?.Length > 0) foreach (var e in result.Errors) Console.WriteLine("ERR " + e.Message);
        File.WriteAllBytes(args[2], result.DocumentBytes);
        Console.WriteLine($"pdf {result.DocumentBytes.Length} bytes, pages={result.PageCount}");

        // --- PNG pages for review: page N of the same report ---
        if (Environment.GetEnvironmentVariable("PNG_PAGES") is string pages)
        {
            foreach (var pg in pages.Split(','))
            {
                var di = new System.Collections.Hashtable { ["OutputFormat"] = "PNG", ["StartPage"] = int.Parse(pg), ["EndPage"] = int.Parse(pg), ["DpiX"] = 110, ["DpiY"] = 110 };
                var img = new ReportProcessor().RenderReport("IMAGE", new InstanceReportSource { ReportDocument = report }, di);
                var png = Path.ChangeExtension(args[2], null) + $"_p{pg}.png";
                File.WriteAllBytes(png, img.DocumentBytes);
                Console.WriteLine($"png {png} {img.DocumentBytes.Length} bytes");
            }
        }
    }
}
