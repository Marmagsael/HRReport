using HRApiLibrary.DataAccess._20_Pay.DA0605;
using HRApiLibrary.Models._20_Pay.M0605;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HRApiLibrary.Tests._20_Pay;

//--- Expected values follow MBPay (FoxPro) payroll4.scx rules; see docs/MBPay-Phase1-Computation.md --------------------
[TestClass]
public class PayrollCalculatorTests
{
    static PayCalcEmployee Emp(PayRateBasis basis = PayRateBasis.Daily, double rate = 600, double clientRate = 0, double sgDaily = 0) => new()
    {
        EmpNumber = "00001", RateBasis = basis, Rate = rate, ClientRate = clientRate, SalaryGradeDailyRate = sgDaily,
        WithTax = true, WithSss = true, WithPhic = true, WithPagibig = true
    };

    static readonly List<PremiumBracket> SssTable =
    [
        new() { FStart = 0,     FEnd = 5249.99,  Ee = 250, Er = 510,  Ecc = 10 },
        new() { FStart = 5250,  FEnd = 5749.99,  Ee = 275, Er = 560,  Ecc = 10 },
        new() { FStart = 5750,  FEnd = 9999.99,  Ee = 450, Er = 910,  Ecc = 10 },
        new() { FStart = 10000, FEnd = 999999,   Ee = 1000, Er = 2000, Ecc = 30 }
    ];

    static readonly List<PremiumBracket> PhicTable =
    [
        new() { FStart = 0.01,  FEnd = 10000,    Ee = 250, Er = 250, Percent = 0 },
        new() { FStart = 10000.01, FEnd = 99999.99, Ee = 0, Er = 0, Percent = 5 },
        new() { FStart = 100000, FEnd = 9999999, Ee = 2500, Er = 2500, Percent = 0 }
    ];

    static readonly List<PremiumBracket> PagibigTable =
    [
        new() { FStart = 0,    FEnd = 1500,    Ee = 30,  Er = 30 },
        new() { FStart = 1500.01, FEnd = 9999999, Ee = 200, Er = 200 }
    ];

    static readonly List<TaxBracket> TaxTable =
    [
        new() { SAmt = 0,        EAmt = 10417,     Fix = 0,       Percentage = 0 },
        new() { SAmt = 10417.01, EAmt = 16666,     Fix = 0,       Percentage = 0.15 },
        new() { SAmt = 16666.01, EAmt = 33332,     Fix = 937.50,  Percentage = 0.20 },
        new() { SAmt = 33332.01, EAmt = 9999999,   Fix = 4270.70, Percentage = 0.25 }
    ];

    //--- Basic pay -------------------------------------------------------------------------------------------------------
    [TestMethod] public void Basic_Daily_RateTimesDays()        => Assert.AreEqual(6000, PayrollCalculator.BasicPay(Emp(), 10));
    [TestMethod] public void Basic_Daily_ZeroDays_IsZero()      => Assert.AreEqual(0, PayrollCalculator.BasicPay(Emp(), 0));
    [TestMethod] public void Basic_Monthly_IsHalf()             => Assert.AreEqual(10000, PayrollCalculator.BasicPay(Emp(PayRateBasis.Monthly, 20000), 0));
    [TestMethod] public void Basic_SemiMonthly_IsRate()         => Assert.AreEqual(9000, PayrollCalculator.BasicPay(Emp(PayRateBasis.SemiMonthly, 9000), 3));
    [TestMethod] public void Basic_NoRate_UsesClientRate()      => Assert.AreEqual(5100, PayrollCalculator.BasicPay(Emp(rate: 0, clientRate: 510), 10));
    [TestMethod] public void Basic_Hourly_RateTimesHours()      => Assert.AreEqual(1000, PayrollCalculator.BasicPay(Emp(PayRateBasis.Hourly, 50), 2.5));

    //--- Overtime / absent -----------------------------------------------------------------------------------------------
    [TestMethod] public void Ot_Daily_UsesOwnRate()             => Assert.AreEqual(187.5, PayrollCalculator.Overtime(Emp(rate: 600), 2, 1.25));
    [TestMethod] public void Ot_Monthly_UsesSalaryGradeDaily()  => Assert.AreEqual(160, PayrollCalculator.Overtime(Emp(PayRateBasis.Monthly, 20000, sgDaily: 800), 1.6, 1));
    [TestMethod] public void Ot_FixedAmountOverrides()          => Assert.AreEqual(300, PayrollCalculator.Overtime(Emp(), 3, 1.25, fixedAmtPerHour: 100));
    [TestMethod] public void Ot_NoRates_IsZero()                => Assert.AreEqual(0, PayrollCalculator.Overtime(Emp(PayRateBasis.Monthly, 20000), 2, 1.25));
    [TestMethod] public void Absent_IsNegative()                => Assert.AreEqual(-300, PayrollCalculator.Absent(Emp(sgDaily: 600), 4));
    [TestMethod] public void Absent_FallsBackToClientRate()     => Assert.AreEqual(-255, PayrollCalculator.Absent(Emp(clientRate: 510), 4));

    //--- SSS -------------------------------------------------------------------------------------------------------------
    [TestMethod]
    public void Sss_SecondCutoff_UsesMonthToDateEarnings()
    {
        //--- 1st cutoff earned 5,000 and paid 250; 2nd cutoff earns 5,000 -> month 10,000 -> bracket EE 1,000 -> 750 now
        var r = PayrollCalculator.Sss(Emp(), 10000, 250, 510, SssTable);
        Assert.AreEqual(750, r.Ee);
        Assert.AreEqual(1490, r.Er);
    }

    [TestMethod] public void Sss_NotMember_IsZero()             => Assert.AreEqual(0, PayrollCalculator.Sss(new PayCalcEmployee(), 10000, 0, 0, SssTable).Ee);
    [TestMethod] public void Sss_Overdeducted_ClampsToZero()    => Assert.AreEqual(0, PayrollCalculator.Sss(Emp(), 3000, 450, 0, SssTable).Ee);

    [TestMethod]
    public void Sss_Max_UsesHighestBracket()
    {
        var e = Emp(); e.IsMaxSss = true;
        Assert.AreEqual(1000, PayrollCalculator.Sss(e, 100, 0, 0, SssTable).Ee);
    }

    //--- PhilHealth ------------------------------------------------------------------------------------------------------
    [TestMethod] public void Phic_Percent_EmployeeGetsHalf()    => Assert.AreEqual(500, PayrollCalculator.Phic(Emp(), 20000, 0, 0, PhicTable).Ee);
    [TestMethod] public void Phic_LessDeducted()                => Assert.AreEqual(300, PayrollCalculator.Phic(Emp(), 20000, 200, 0, PhicTable).Ee);
    [TestMethod] public void Phic_FixedBracket()                => Assert.AreEqual(250, PayrollCalculator.Phic(Emp(), 8000, 0, 0, PhicTable).Ee);
    [TestMethod] public void Phic_NoEarnings_IsZero()           => Assert.AreEqual(0, PayrollCalculator.Phic(Emp(), 0, 0, 0, PhicTable).Ee);

    [TestMethod]
    public void Phic_NotMember_NeverNegative()
    {
        var e = Emp(); e.WithPhic = false;
        Assert.AreEqual(0, PayrollCalculator.Phic(e, 20000, 300, 0, PhicTable).Ee);
    }

    //--- Pag-IBIG --------------------------------------------------------------------------------------------------------
    [TestMethod] public void Pagibig_Bracket()                  => Assert.AreEqual(200, PayrollCalculator.Pagibig(Emp(), 8000, 0, 0, PagibigTable).Ee);
    [TestMethod] public void Pagibig_AlreadyPaid_IsZero()       => Assert.AreEqual(0, PayrollCalculator.Pagibig(Emp(), 16000, 200, 200, PagibigTable).Ee);

    [TestMethod]
    public void Pagibig_NotMember_IsZero()
    {
        var e = Emp(); e.WithPagibig = false;
        Assert.AreEqual(0, PayrollCalculator.Pagibig(e, 8000, 0, 0, PagibigTable).Ee);
    }

    //--- Tax -------------------------------------------------------------------------------------------------------------
    [TestMethod]
    public void Tax_DeductsPremiumsBeforeLookup()
    {
        //--- 20,000 - 500 - 250 - 200 = 19,050 -> 937.50 + (19,050 - 16,666.01) x 20% = 1,414.30
        Assert.AreEqual(1414.30, PayrollCalculator.Tax(Emp(), 20000, 500, 250, 200, TaxTable));
    }

    [TestMethod] public void Tax_ExcessBelowOne_Ignored()       => Assert.AreEqual(937.50, PayrollCalculator.Tax(Emp(), 16666.5, 0, 0, 0, TaxTable));
    [TestMethod] public void Tax_BelowThreshold_IsZero()        => Assert.AreEqual(0, PayrollCalculator.Tax(Emp(), 10000, 0, 0, 0, TaxTable));

    [TestMethod]
    public void Tax_NotTaxable_IsZero()
    {
        var e = Emp(); e.WithTax = false;
        Assert.AreEqual(0, PayrollCalculator.Tax(e, 50000, 0, 0, 0, TaxTable));
    }

    //--- Loans -----------------------------------------------------------------------------------------------------------
    [TestMethod]
    public void Loans_CappedAtBalance_AndSkipsAlreadyDeducted()
    {
        var loans = new[]
        {
            new PayCalcLoan { Id = 1, DedNCode = "D010", Amort = 1000, Balance = 400 },
            new PayCalcLoan { Id = 2, DedNCode = "D011", Amort = 500,  Balance = 5000 },
            new PayCalcLoan { Id = 3, DedNCode = "D012", Amort = 500,  Balance = 0 }
        };
        var res = PayrollCalculator.Loans(loans, new HashSet<int> { 2 });

        Assert.AreEqual(1, res.Count);
        Assert.AreEqual("D010", res[0].AcctNumber);
        Assert.AreEqual(400, res[0].Amount);
        Assert.AreEqual(1, res[0].RefId);
    }

    //--- Minimum net / net check -----------------------------------------------------------------------------------------
    [TestMethod]
    public void MinimumNet_ZerosUnlockedDeductions_KeepsGovernment_StopsAtTarget()
    {
        var lines = new List<PayCalcLine>
        {
            new("E001", 3000), new("D002", 250), new("D003", 150), new("D004", 100),
            new("D010", 1000), new("D020", 800), new("D030", 500)
        };
        var accts = new List<PayCalcAccount>
        {
            new() { AcctNumber = "D010", SortDed = 1 },
            new() { AcctNumber = "D020", SortDed = 2 },
            new() { AcctNumber = "D030", SortDed = 3, IsLock = true }
        };
        //--- net 200 -> zero D020 (highest unlocked sort) -> 1,000 >= 500, stop; D030 is locked
        var res = PayrollCalculator.ApplyMinimumNet(lines, accts);

        Assert.AreEqual(1000, res.NetAfter);
        Assert.AreEqual(1, res.Undeducted.Count);
        Assert.AreEqual("D020", res.Undeducted[0].AcctNumber);
        Assert.AreEqual(1000, lines.Single(l => l.AcctNumber == "D010").Amount);
        Assert.AreEqual(250, lines.Single(l => l.AcctNumber == "D002").Amount);
    }

    [TestMethod]
    public void MinimumNet_AboveTrigger_NoChange()
    {
        var lines = new List<PayCalcLine> { new("E001", 6000), new("D010", 1000) };
        var res = PayrollCalculator.ApplyMinimumNet(lines, []);
        Assert.AreEqual(0, res.Undeducted.Count);
        Assert.AreEqual(5000, res.NetAfter);
    }

    [TestMethod]
    public void CheckNet_Levels()
    {
        Assert.AreEqual(NetPayStatus.Negative,     PayrollCalculator.CheckNet(-1));
        Assert.AreEqual(NetPayStatus.BelowMinimum, PayrollCalculator.CheckNet(499));
        Assert.AreEqual(NetPayStatus.Ok,           PayrollCalculator.CheckNet(500));
    }

    [TestMethod]
    public void RateBasis_MapsHrSysPayrateIds()
    {
        Assert.AreEqual(PayRateBasis.Hourly,   PayrollCalculator.RateBasisFromPayrateId(1));
        Assert.AreEqual(PayRateBasis.Daily,    PayrollCalculator.RateBasisFromPayrateId(2));
        Assert.AreEqual(PayRateBasis.Monthly,  PayrollCalculator.RateBasisFromPayrateId(4));
        Assert.AreEqual(PayRateBasis.Annually, PayrollCalculator.RateBasisFromPayrateId(6));
        Assert.AreEqual(PayRateBasis.Daily,    PayrollCalculator.RateBasisFromPayrateId(null));
    }
}
