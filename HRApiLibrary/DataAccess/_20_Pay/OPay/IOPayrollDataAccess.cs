using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

//--- What bLock.Click asks the user before it locks (check_negative_pay, check_40_percent) ---
public class OPayLockCheck
{
    public bool          AlreadyLocked  { get; set; }
    public List<string>  Negative       { get; set; } = [];     //--- net below 0 : blocks the lock
    public List<string>  BelowMinimum   { get; set; } = [];     //--- net from 0 to 499 : asks "Do you want to lock transaction?"
    public List<string>  Over40Percent  { get; set; } = [];     //--- deductions are 40% or more of earnings : asks the same
}

public interface IOPayrollDataAccess
{
    Task<List<OPayClient>> _02Clients(string paydb, string pisdb, string conn);
    Task<List<OPayPeriod>> _02Periods(string paydb, string conn);

    //--- payrollnew4 bLoad.Click : paymain_save and all the *_fetch procedures ---
    Task<OPayrollSession> _01Load(string yy4, string mm, string prd, string grpCode, DateTime attStart, DateTime attEnd,
                                  string paydb, string pisdb, string conn, IProgress<string>? progress = null);

    Task _01LoadEmployeeData(OPayrollSession s, IProgress<string>? progress = null);
    Task<List<OPayEmp>> _02Employees(OPayrollSession s, IEnumerable<string> empNumbers);

    //--- PayrollAddEmp4 : employees in payroll that are not in this payroll yet, found by name or number ---
    Task<List<OPayEmp>> _02AddCandidates(OPayrollSession s, string? find);

    //--- payroll4 bAddEmployee / bRemoveSelectedEmployee ---
    Task _05AddEmployees(OPayrollSession s, IEnumerable<string> empNumbers);
    Task<string?> _05RemoveEmployees(OPayrollSession s, IEnumerable<string> empNumbers);

    //--- payroll4 bSave.Click (after bProcess) ---
    Task<string?> _03SaveDraft(OPayrollSession s, string user);

    //--- payroll4 bLock.Click ---
    OPayLockCheck _04LockCheck(OPayrollSession s);
    Task<string?> _04Lock(OPayrollSession s, string user);
}
