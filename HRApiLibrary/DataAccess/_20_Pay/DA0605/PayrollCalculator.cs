using HRApiLibrary.Models._20_Pay.M0605;

namespace HRApiLibrary.DataAccess._20_Pay.DA0605;

//--- Payroll computation rules ported from MBPay (FoxPro) form payroll4.scx ----------------------------------------------
//--- No database access: callers load the data, call these methods and save the results.
//--- See docs/MBPay-Phase1-Computation.md for the FoxPro behaviour and the decisions (D1-D5) applied here.
public static class PayrollCalculator
{
    public static double Round2(double v) => Math.Round(v, 2, MidpointRounding.AwayFromZero);

    //--- compute_basic ------------------------------------------------------------------------------------------------
    //--- Monthly = half the rate, semi-monthly = the rate (paid even with 0 days), daily/hourly = rate x days.
    //--- An employee without a rate uses the client rate x days.
    public static double BasicPay(PayCalcEmployee e, double daysWorked, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        var isPeriodRate = e.RateBasis is PayRateBasis.Monthly or PayRateBasis.SemiMonthly or PayRateBasis.Annually;

        if (daysWorked == 0 && !isPeriodRate) return 0;
        if (e.Rate == 0) return Round2(e.ClientRate * daysWorked);

        return Round2(e.RateBasis switch
        {
            PayRateBasis.Monthly     => e.Rate / 2,
            PayRateBasis.SemiMonthly => e.Rate,
            PayRateBasis.Annually    => e.Rate / 24,
            PayRateBasis.Hourly      => e.Rate * daysWorked * o.HoursPerDay,
            _                        => e.Rate * daysWorked
        });
    }

    //--- compute_ot ---------------------------------------------------------------------------------------------------
    //--- Hourly rate = daily rate / 8. Daily-rated employees use their own rate, others the salary-grade daily rate,
    //--- and employees without a rate the client rate. A fixed amount per hour (ratesamt.rot) overrides the formula.
    public static double Overtime(PayCalcEmployee e, double hours, double otRate, double? fixedAmtPerHour = null, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        if (hours == 0) return 0;
        if (fixedAmtPerHour > 0) return Round2(fixedAmtPerHour.Value * hours);

        var daily = e.Rate == 0                         ? e.ClientRate
                  : e.RateBasis == PayRateBasis.Daily   ? e.Rate
                  : e.RateBasis == PayRateBasis.Hourly  ? e.Rate * o.HoursPerDay
                  :                                       e.SalaryGradeDailyRate;
        if (daily == 0) return 0;

        return Round2(daily / o.HoursPerDay * otRate * hours);
    }

    //--- compute_absent: negative earning = hours x salary-grade daily rate / 8 (client rate when no salary grade) -----
    public static double Absent(PayCalcEmployee e, double hours, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        var daily = e.SalaryGradeDailyRate > 0 ? e.SalaryGradeDailyRate : e.ClientRate;
        if (daily == 0 || hours == 0) return 0;

        return Round2(-(hours * daily / o.HoursPerDay));
    }

    //--- Sum of earnings in the given accounts (current period + saved earlier this month) ------------------------------
    public static double MonthToDate(IEnumerable<PayCalcLine> current, IEnumerable<PayCalcLine> savedThisMonth, ICollection<string> accts)
        => current.Concat(savedThisMonth).Where(l => accts.Contains(l.AcctNumber ?? "")).Sum(l => l.Amount);

    static PremiumBracket? FindBracket(IEnumerable<PremiumBracket> table, double amount)
        => table.FirstOrDefault(b => amount >= b.FStart && amount <= b.FEnd);

    //--- compute_d01sss -----------------------------------------------------------------------------------------------
    //--- Bracket from month-to-date earnings; deduct the bracket EE less SSS already deducted this month.
    //--- IsMaxSss uses the highest bracket. Negative results become 0 (D1).
    public static PremiumResult Sss(PayCalcEmployee e, double monthToDateEarnings, double eeDeducted, double erDeducted,
                                    IList<PremiumBracket> table)
    {
        var r = new PremiumResult { BasisAmount = monthToDateEarnings };
        if (!e.WithSss || table.Count == 0) return r;

        var b = e.IsMaxSss ? table.OrderBy(x => x.FEnd).Last() : FindBracket(table, monthToDateEarnings);
        r.Ee  = Round2(Math.Max(0, (b?.Ee ?? 0) - eeDeducted));
        r.Er  = Round2(Math.Max(0, (b?.Er ?? 0) - erDeducted));
        r.Ecc = b?.Ecc ?? 0;
        return r;
    }

    //--- compute_d04phic: EE = fixed EE + (earnings x percent / 100) / 2, less PhilHealth already deducted this month --
    public static PremiumResult Phic(PayCalcEmployee e, double monthToDateEarnings, double eeDeducted, double erDeducted,
                                     IList<PremiumBracket> table)
    {
        var r = new PremiumResult { BasisAmount = monthToDateEarnings };
        if (!e.WithPhic || monthToDateEarnings == 0) return r;

        var b = FindBracket(table, monthToDateEarnings);
        if (b == null) return r;

        var share = monthToDateEarnings * b.Percent / 100 / 2;
        r.Ee = Round2(Math.Max(0, b.Ee + share - eeDeducted));
        r.Er = Round2(Math.Max(0, b.Er + share - erDeducted));
        return r;
    }

    //--- compute_d03pagibig: bracket EE less Pag-IBIG already deducted this month --------------------------------------
    public static PremiumResult Pagibig(PayCalcEmployee e, double monthToDateEarnings, double eeDeducted, double erDeducted,
                                        IList<PremiumBracket> table)
    {
        var r = new PremiumResult { BasisAmount = monthToDateEarnings };
        if (!e.WithPagibig) return r;

        var b = FindBracket(table, monthToDateEarnings);
        r.Ee = Round2(Math.Max(0, (b?.Ee ?? 0) - eeDeducted));
        r.Er = Round2(Math.Max(0, (b?.Er ?? 0) - erDeducted));
        return r;
    }

    //--- compute_d00tax -----------------------------------------------------------------------------------------------
    //--- Taxable = current-period taxable earnings - SSS - PHIC - Pag-IBIG (current period). Per-period table, no
    //--- annualization (D4). Excess below 1 is ignored. No bracket = no tax.
    public static double Tax(PayCalcEmployee e, double taxableEarnings, double sss, double phic, double pagibig,
                             IList<TaxBracket> table)
    {
        if (!e.WithTax) return 0;

        var taxable = Round2(taxableEarnings - sss - phic - pagibig);
        var b = table.FirstOrDefault(x => taxable >= x.SAmt && taxable <= x.EAmt);
        if (b == null) return 0;

        var excess = taxable - b.SAmt;
        if (excess < 1) excess = 0;

        return Round2(Math.Max(0, b.Fix + excess * b.Percentage));
    }

    //--- calc_loans: amortization capped at the balance; loans already deducted in this period are skipped ------------
    public static List<PayCalcLine> Loans(IEnumerable<PayCalcLoan> activeLoans, ICollection<int> loanIdsDeductedThisPeriod)
        => activeLoans
            .Where(l => l.Balance > 0 && !(l.Id.HasValue && loanIdsDeductedThisPeriod.Contains(l.Id.Value)))
            .Select(l => new PayCalcLine(l.DedNCode ?? "", Round2(Math.Min(l.Amort, l.Balance)), l.Id))
            .Where(l => l.Amount > 0)
            .ToList();

    public static double Net(IEnumerable<PayCalcLine> lines)
        => Round2(lines.Sum(l => (l.AcctNumber ?? "").StartsWith("E") ? l.Amount
                               : (l.AcctNumber ?? "").StartsWith("D") ? -l.Amount : 0));

    //--- minimumnet ---------------------------------------------------------------------------------------------------
    //--- When net is below MinimumNetTrigger, zero unlocked deductions (never D001-D004, D2) starting from the highest
    //--- SortDed, until net reaches MinimumNetTarget (D5). Mutates the amounts in `lines`.
    public static MinimumNetResult ApplyMinimumNet(List<PayCalcLine> lines, IEnumerable<PayCalcAccount> accounts, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        var res = new MinimumNetResult
        {
            Earnings         = Round2(lines.Where(l => (l.AcctNumber ?? "").StartsWith("E")).Sum(l => l.Amount)),
            DeductionsBefore = Round2(lines.Where(l => (l.AcctNumber ?? "").StartsWith("D")).Sum(l => l.Amount))
        };
        var net = Net(lines);
        res.NetAfter = net;
        if (net >= o.MinimumNetTrigger) return res;

        var acct = accounts.Where(a => a.AcctNumber != null).GroupBy(a => a.AcctNumber!).ToDictionary(g => g.Key, g => g.First());
        var candidates = lines
            .Where(l => (l.AcctNumber ?? "").StartsWith("D") && l.Amount > 0 &&
                        !o.GovernmentAccts.Contains(l.AcctNumber) &&
                        !(acct.TryGetValue(l.AcctNumber!, out var a) && a.IsLock))
            .OrderByDescending(l => acct.TryGetValue(l.AcctNumber!, out var a) ? a.SortDed : 0)
            .ToList();

        foreach (var l in candidates)
        {
            if (net >= o.MinimumNetTarget) break;
            res.Undeducted.Add(new PayCalcLine(l.AcctNumber!, l.Amount, l.RefId));
            net = Round2(net + l.Amount);
            l.Amount = 0;
        }
        res.NetAfter = net;
        return res;
    }

    //--- check_negative_pay: negative net blocks posting, below MinimumNetTarget asks for confirmation -----------------
    public static NetPayStatus CheckNet(double net, PayCalcOptions? o = null)
    {
        o ??= new PayCalcOptions();
        if (net < 0) return NetPayStatus.Negative;
        return net < o.MinimumNetTarget ? NetPayStatus.BelowMinimum : NetPayStatus.Ok;
    }

    //--- empmas.rateID used by MBPay (FoxPro): 1 monthly, 2 daily, 3 semi-monthly ---------------------------------------
    public static PayRateBasis RateBasisFromFoxProRateId(int? rateId) => rateId switch
    {
        1 => PayRateBasis.Monthly,
        3 => PayRateBasis.SemiMonthly,
        _ => PayRateBasis.Daily
    };

    //--- Payrate.Id used by HRSys (see _0504_02_EmpRates.razor); anything else is treated as daily ----------------------
    public static PayRateBasis RateBasisFromPayrateId(int? payrateId) => payrateId switch
    {
        1 => PayRateBasis.Hourly,
        4 => PayRateBasis.Monthly,
        6 => PayRateBasis.Annually,
        _ => PayRateBasis.Daily
    };
}
