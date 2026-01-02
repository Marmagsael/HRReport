using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITranreinstatementapprovalDataAccess
    {
        Task<TranreinstatementapprovalModel?> _01(TranreinstatementapprovalModel tranmovement, string schema, string conn);
        Task<TranreinstatementapprovalModel?> _02(int id, string schema, string conn);
        Task<TranreinstatementapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<TranreinstatementapprovalModel?> _03(int id, TranreinstatementapprovalModel tranmovement, string schema, string conn);
        Task<TranreinstatementapprovalModel?> _04(int id, string schema, string conn);
    }
}