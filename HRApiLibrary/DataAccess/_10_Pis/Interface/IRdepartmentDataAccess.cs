using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRdepartmentDataAccess
    {
        Task<RdepartmentModel?> _01(RdepartmentModel rdepartment, string schema, string conn);
        Task<RdepartmentModel?> _02(int id, string schema, string conn);
        Task<List<RdepartmentModel?>?> _02(string schema, string conn);
        Task<RdepartmentModel?> _03(int id, RdepartmentModel rdepartment, string schema, string conn);
        Task<RdepartmentModel?> _04(int id, string schema, string conn);
    }
}