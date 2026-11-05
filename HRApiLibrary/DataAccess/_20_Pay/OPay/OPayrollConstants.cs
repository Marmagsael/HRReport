namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- Fixed values written inside MBPay forms/payroll4.scx. Each one names the procedure it comes from. ----------------
public static class OPayrollConstants
{
    //--- compute_basic, compute_ordinaryday, compute_ot, compute_absent, finalize_manhour ---
    public const double HoursPerDay = 8;

    //--- compute_basic (E001 resets the TH and E001 detail rows), process_13thmo_added (13th month = E001 / 12) ---
    public const string BasicAcct = "E001";
    public const double ThirteenthMonthDivisor = 12;

    //--- compute_d00tax / create_premium_references ---
    public const string TaxAcct = "D001";
    public static readonly string[] GovDeductionAccts = ["D002", "D003", "D004"];

    //--- compute_allowance : allowRate / 30 * daywrk ---
    public const double AllowanceDaysPerMonth = 30;

    //--- compute_provision_for_retirement : daily rate * 22.5 / 12 * 12 / 365 * daywrk ---
    public const double RetirementDays      = 22.5;
    public const double RetirementMonths    = 12;
    public const double RetirementYearDays  = 365;

    //--- compute_freeuniform : 3.28767123287671 * daywrk ---
    public const double FreeUniformPerDay = 3.28767123287671;

    //--- compute_incentive_leave : (basic + ecola + ctpa + sea) / daywrk * 5 / 365 * daywrk ---
    public const double IncentiveLeaveDays     = 5;
    public const double IncentiveLeaveYearDays = 365;

    //--- check_negative_pay : net below 0 blocks the lock, net from 0 to 499 asks for confirmation ---
    public const double LockWarnNetFrom = 0;
    public const double LockWarnNetTo   = 499;

    //--- check_40_percent : deduction / earnings * 100 >= 40 asks for confirmation ---
    public const double MaxDeductionPercent = 40;

    //--- Number of decimals FoxPro keeps in the amount fields (numeric 000000000000.0000) and saves (STR(x,12,4)) ---
    public const int AmountDecimals = 4;
}
