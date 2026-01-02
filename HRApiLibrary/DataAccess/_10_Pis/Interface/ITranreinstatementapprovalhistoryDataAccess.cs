using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITranreinstatementapprovalhistoryDataAccess
    {
        Task<TranreinstatementapprovalhistoryModel?> _01(TranreinstatementapprovalhistoryModel Tranreinstatementapprovalhistory, string schema, string conn);
        Task<TranreinstatementapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TranreinstatementapprovalhistoryModel?> _02(string trannumber, string schema, string conn);
        Task<TranreinstatementapprovalhistoryModel?> _03(int id, TranreinstatementapprovalhistoryModel Tranreinstatementapprovalhistory, string schema, string conn);
        Task<TranreinstatementapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}