using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmpblockpostDataAccess
    {
        Task<EmpblockpostModel?> _01(EmpblockpostModel empblockpost, string schema, string conn);
        Task<EmpblockpostModel?> _02(int userId, int deploymentId, string schema, string conn);
        Task<List<EmpblockpostModel?>?> _02(int EmpmasId, string schema, string conn);
        Task<List<EmpblockpostModel?>?> _02(string schema, string conn);
        Task<EmpblockpostModel?> _04(EmpblockpostModel rec, string schema, string conn);
    }
}