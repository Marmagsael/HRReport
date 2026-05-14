using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITrandeploymentapprovalDataAccess
    {
        Task<TrandeploymentapprovalModel?> _01(TrandeploymentapprovalModel tranmovement, string schema, string conn);
        Task<TrandeploymentapprovalModel?> _02(int id, string schema, string conn);
        Task<TrandeploymentapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<TrandeploymentapprovalModel?> _03(int id, TrandeploymentapprovalModel tranmovement, string schema, string conn);
        Task<TrandeploymentapprovalModel?> _04(int id, string schema, string conn);
    }
}