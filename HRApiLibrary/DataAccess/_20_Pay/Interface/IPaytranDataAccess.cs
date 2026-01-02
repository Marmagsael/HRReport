using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPaytranDataAccess
    {
        Task _01(PaytranModel paytran, string schema, string conn);
        Task _01New(PaytranModel paytran, string schema, string conn);
        Task<List<PaytranModel?>?> _01New(string trn, List<PaytranModel?> paytrans, string paydb, string pisdb,
            string conn); 
        Task<List<PaytranModel?>?> _02ByTrn(string trn, string paydb, string pisdb, string conn);
        Task _03(PaytranModel paytran, string schema, string conn);
        Task _03AttDuration(string trn, DateTime attStart, DateTime attEnd, string schema, string conn);
        Task _03PayrollgrpId(string trn, int payrollgrpId, string schema, string conn);
        Task _04ByTrn(string trn, string schema, string conn);
    }
}