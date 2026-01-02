using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IDeprecDataAccess
    {
        Task<DeprecModel?>              _01(DeprecModel deprec, string schema, string conn);
        Task<DeprecModel?>              _02(int id, string schema, string conn);
        Task<DeprecModel?>              _02(int id, string schema, string payschema, string conn);
        Task<DeprecModel?>              _02ByEmpnumber(string empnumber, string schema, string conn);
        Task<List<DeprecModel?>?>       _02ByEmpmasIds(List<int> empmasId, string schema, string conn);
        Task<List<DeprecModel?>?>       _02ByStatusIds(List<int> statusIds, string schema,  string conn);
        Task<List<DeprecModel?>?>       _02_ByPayrollgrpIds(int payrollgrpId, string schema, string conn);
        Task<DeprecModel?>              _02DeviationDtlsByEmpmasId(int empmasId, string schema, string conn);
        Task<List<DeprecModel?>?>       _02ByField(string fieldName, string schema, string conn);
        Task<DeprecModel?>              _03(DeprecModel deprec, string schema, string conn);
        Task<DeprecModel?>              _04(int empmasid, string schema, string conn);
    }
}