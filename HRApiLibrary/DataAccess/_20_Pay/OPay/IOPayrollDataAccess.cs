using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public interface IOPayrollDataAccess
{
    Task<List<OPayClient>> _02Clients(string paydb, string pisdb, string conn);
    Task<List<OPayPeriod>> _02Periods(string paydb, string conn);

    Task<OPayrollSession> _01Load(string yy4, string mm, string prd, string grpCode, DateTime attStart, DateTime attEnd,
                                  string paydb, string pisdb, string conn, IProgress<string>? progress = null);

    Task _01LoadEmployeeData(OPayrollSession s, IProgress<string>? progress = null);
    Task<List<OPayEmp>> _02Employees(OPayrollSession s, IEnumerable<string> empNumbers);

    Task<string?> _03SaveDraft(OPayrollSession s, string user);

    Task<string?> _04Lock(OPayrollSession s, string user);
}
