using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IEmpratesdtlDataAccess
    {
        Task _01(EmpratesdtlModel empratesdtl, string schema, string conn);
        Task _01FromCOA(EmpratesdtlModel empratesdtl, double ratePerHr, double ratePerDay, string schema, string conn);
        Task<List<EmpratesdtlModel?>?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<List<EmpratesdtlModel?>?> _02ByPayrollgrpId(int payrollgrpId, string schema, string conn);
        Task<List<EmpratesdtlModel?>?> _02ByEmpmasIdPayrollgrpId(int empmasId, int payrollgrpId, string schema,
            string conn); 
        Task<EmpratesdtlModel?> _02ByPK(int empmasId, int payrollgrpId, string acctNumber, string schema, string conn);
        Task<EmpratesdtlModel?> _03(EmpratesdtlModel empratesdtl, string schema, string conn);
        Task<EmpratesdtlModel?> _04(EmpratesdtlModel empratesdtl, string schema, string conn);
    }
}