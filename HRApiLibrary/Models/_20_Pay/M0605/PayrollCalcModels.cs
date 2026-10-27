namespace HRApiLibrary.Models._20_Pay.M0605;

//--- Rate basis used by PayrollCalculator. Maps from Payrate.Id (1 Hourly, 2 Daily, 4 Monthly, 6 Annually) -----------
public enum PayRateBasis
{
    Hourly      = 1,
    Daily       = 2,
    SemiMonthly = 3,
    Monthly     = 4,
    Annually    = 6
}

public class PayCalcOptions
{
    public double HoursPerDay               { get; set; } = 8;
    public double MinimumNetTrigger         { get; set; } = 1000;       //--- FoxPro minimumnet: start zeroing deductions below this
    public double MinimumNetTarget          { get; set; } = 500;        //--- stop zeroing once net reaches this; also the lock warning level
    public string TaxAcct                   { get; set; } = "D001";
    public string SssAcct                   { get; set; } = "D002";
    public string PhicAcct                  { get; set; } = "D003";
    public string PagibigAcct               { get; set; } = "D004";
    public string[] GovernmentAccts => [TaxAcct, SssAcct, PhicAcct, PagibigAcct];
}

public class PayCalcEmployee
{
    public int?          EmpmasId             { get; set; }
    public string?       EmpNumber            { get; set; }
    public string?       EmpName              { get; set; }
    public PayRateBasis  RateBasis            { get; set; } = PayRateBasis.Daily;
    public double        Rate                 { get; set; }
    public double        ClientRate           { get; set; }             //--- payroll group / client daily rate, used when Rate = 0
    public double        SalaryGradeDailyRate { get; set; }             //--- used for OT (non-daily) and absences
    public bool          WithTax              { get; set; }
    public bool          WithSss              { get; set; }
    public bool          WithPhic             { get; set; }
    public bool          WithPagibig          { get; set; }
    public bool          IsMaxSss             { get; set; }
}

//--- One amount per account (current period, or saved earlier in the month) -------------------------------------------
public class PayCalcLine
{
    public string?  AcctNumber  { get; set; }
    public double   Amount      { get; set; }
    public int?     RefId       { get; set; }

    public PayCalcLine() { }
    public PayCalcLine(string acctNumber, double amount, int? refId = null)
    {
        AcctNumber = acctNumber;
        Amount     = amount;
        RefId      = refId;
    }
}

public class PayCalcAccount
{
    public string?  AcctNumber  { get; set; }
    public bool     IsLock      { get; set; }
    public int      SortDed     { get; set; }
    public bool IsEarning   => (AcctNumber ?? "").StartsWith("E");
    public bool IsDeduction => (AcctNumber ?? "").StartsWith("D");
}

public class PremiumBracket
{
    public double FStart    { get; set; }
    public double FEnd      { get; set; }
    public double Ee        { get; set; }
    public double Er        { get; set; }
    public double Ecc       { get; set; }
    public double Percent   { get; set; }                               //--- PhilHealth only: total premium rate (EE gets half)
}

public class TaxBracket
{
    public double SAmt          { get; set; }
    public double EAmt          { get; set; }
    public double Fix           { get; set; }
    public double Percentage    { get; set; }                           //--- fraction, e.g. 0.15
}

public class PremiumResult
{
    public double BasisAmount   { get; set; }                           //--- month-to-date earnings used for the bracket
    public double Ee            { get; set; }                           //--- amount to deduct this period
    public double Er            { get; set; }
    public double Ecc           { get; set; }
}

public class PayCalcLoan
{
    public int?     Id          { get; set; }
    public string?  DedNCode    { get; set; }
    public double   Amort       { get; set; }
    public double   Balance     { get; set; }
}

public class MinimumNetResult
{
    public double               Earnings            { get; set; }
    public double               DeductionsBefore    { get; set; }
    public double               NetAfter            { get; set; }
    public List<PayCalcLine>    Undeducted          { get; set; } = [];  //--- zeroed deductions, for undeddeduction
}

public enum NetPayStatus { Ok, BelowMinimum, Negative }
