using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITranexonerateapprovalhistoryDataAccess
    {
        Task<TranexonerateapprovalhistoryModel?> _01(TranexonerateapprovalhistoryModel Tranexonerateapprovalhistory, string schema, string conn);
        Task<TranexonerateapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TranexonerateapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn);
        Task<TranexonerateapprovalhistoryModel?> _03(int id, TranexonerateapprovalhistoryModel Tranexonerateapprovalhistory, string schema, string conn);
        Task<TranexonerateapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}