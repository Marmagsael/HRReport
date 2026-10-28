using HRApiLibrary.Models._20_Pay.OPay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public interface IOPayrollDataAccess
{
    Task<List<OPayClient>> _02Clients(string pisdb, string conn);

    Task<OPayrollSession> _01Load(string yy4, string mm, string prd, string clNumber, DateTime attStart, DateTime attEnd,
                                  string paydb, string pisdb, string conn);

    Task _01LoadEmployeeData(OPayrollSession s);

    Task<string?> _03SaveDraft(OPayrollSession s, string user);

    Task<string?> _04Lock(OPayrollSession s, string user);
}
