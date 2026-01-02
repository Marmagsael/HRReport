using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRdeploymentDataAccess
    {
        Task<RdeploymentModel?> _01(RdeploymentModel Rdeployment, string schema, string conn);
        Task<RdeploymentModel?> _02(int id, string schema, string conn);
        Task<List<RdeploymentModel?>?> _02(string schema, string conn);
        Task<RdeploymentModel?> _03(int id, RdeploymentModel Rdeployment, string schema, string conn);
        Task<RdeploymentModel?> _04(int id, string schema, string conn);
    }
}