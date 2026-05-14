using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IDeploymodeDataAccess
    {
        Task<DeploymodeModel?> _01(DeploymodeModel deploymode, string schema, string conn);
        Task<DeploymodeModel?> _02(int id, string schema, string conn);
        Task<List<DeploymodeModel?>?> _02(string schema, string conn);
        Task<DeploymodeModel?> _03(int id, DeploymodeModel deploymode, string schema, string conn);
        Task<DeploymodeModel?> _04(int id, string schema, string conn);
    }
}