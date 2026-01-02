using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPaymainhistoryDataAccess
    {
        Task<PaymainhistoryModel?> _01(PaymainhistoryModel paymainhistory, string schema, string conn);
        Task<PaymainhistoryModel?> _02(int id, string schema, string conn);
        Task<List<PaymainhistoryModel?>?> _02ByTrn(string trn, string schema, string conn);
        Task<PaymainhistoryModel?> _04(int id, string schema, string conn);
    }
}