using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITrandeviationapprovalhistoryDataAccess
    {
        Task<TrandeviationapprovalhistoryModel?> _01(TrandeviationapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn);
        Task<TrandeviationapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TrandeviationapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn);
        Task<TrandeviationapprovalhistoryModel?> _03(int id, TrandeviationapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn);
        Task<TrandeviationapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}