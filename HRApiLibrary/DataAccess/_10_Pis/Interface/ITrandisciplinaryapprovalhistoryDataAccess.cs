using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITrandisciplinaryapprovalhistoryDataAccess
    {
        Task<TrandisciplinaryapprovalhistoryModel?> _01(TrandisciplinaryapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn);
        Task<TrandisciplinaryapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TrandisciplinaryapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn);
        Task<TrandisciplinaryapprovalhistoryModel?> _03(int id, TrandisciplinaryapprovalhistoryModel Trandeviationapprovalhistory, string schema, string conn);
        Task<TrandisciplinaryapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}