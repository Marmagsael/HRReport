using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPayrollgrpratesDataAccess
    {
        Task<PayrollgrpratesModel?> _01(PayrollgrpratesModel payrollgrprates, string schema, string conn);
        Task<PayrollgrpratesModel?> _02(int payrollgrpId, string coaAcctnumber, string schema, string conn);
        Task<List<PayrollgrpratesModel?>?> _02ByPayrollgrpId(int payrollgrpId, string schema, string conn);
        Task<PayrollgrpratesModel?> _03(PayrollgrpratesModel payrollgrprates, string schema, string conn);
        Task<PayrollgrpratesModel?> _04(int payrollgrpId, string coaAcctnumber, string schema, string conn);
    }
}