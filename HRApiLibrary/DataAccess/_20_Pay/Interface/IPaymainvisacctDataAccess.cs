using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPaymainvisacctDataAccess
    {
        Task<PaymainvisacctModel?> _01(PaymainvisacctModel paymainvisacct, int payrollgrpId, double defRate, string paydb, string pisdb, int userId, string conn);
        Task<PaymainvisacctModel?> _01Tmp(PaymainvisacctModel paymainvisacct, int payrollgrpId, double defRate, string paydb, string pisdb, int userId, string conn);
        Task<List<PaymainvisacctModel?>?> _01s(List<PaymainvisacctModel?>? paymainvisaccts, string trn, string schema, string conn); 
        Task<List<PaymainvisacctModel?>?> _02ByTrns(string trn, string schema, string conn);
        Task<PaymainvisacctModel?> _04(PaymainvisacctModel paymainvisacct, string schema, string conn);
        Task _04(string trn, string acctNumber, string schema, string conn); 
    }
}