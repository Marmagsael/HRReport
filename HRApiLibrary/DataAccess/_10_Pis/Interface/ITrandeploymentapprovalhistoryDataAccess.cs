using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITrandeploymentapprovalhistoryDataAccess
    {
        Task<TrandeploymentapprovalhistoryModel?> _01(TrandeploymentapprovalhistoryModel tranmovapprovalhistory, string schema, string conn);
        Task<TrandeploymentapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TrandeploymentapprovalhistoryModel?> _02(string trannumber, string schema, string conn);
        Task<TrandeploymentapprovalhistoryModel?> _03(int id, TrandeploymentapprovalhistoryModel tranmovapprovalhistory, string schema, string conn);
        Task<TrandeploymentapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}