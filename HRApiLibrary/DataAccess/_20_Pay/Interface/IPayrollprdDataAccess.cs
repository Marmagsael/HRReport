using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPayrollprdDataAccess
    {
        Task<PayrollprdModel?>          _01(PayrollprdModel payrollprd, string schema, string conn);
        Task<PayrollprdModel?>          _02(int id, string schema, string conn);
        Task<PayrollprdModel?>          _02(string schema, string conn);
        Task<List<PayrollprdModel?>?>   _02Open(string schema, string conn);
        Task<List<PayrollprdModel?>?>   _02OpenPerMonth(string schema, string conn); 
        Task<List<PayrollprdModel?>?>   _02PerYr(int yr, string schema, string conn);
        Task<PayrollprdModel?>          _03(int id, PayrollprdModel payrollprd, string schema, string conn);
        Task                            _03SetActive(int id, string schema, string conn);
        Task                            _03LockPeriod(int payrollprdId, int userId, string schema, string conn); 
        Task<PayrollprdModel?>          _04(int id, string schema, string conn);
    }
}