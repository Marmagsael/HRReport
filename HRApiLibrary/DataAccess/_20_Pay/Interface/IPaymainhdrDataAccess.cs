using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPaymainhdrDataAccess
    {
        Task<PaymainhdrModel?> _01(PaymainhdrModel paymainhdr, string schema, string conn);
        Task<PaymainhdrModel?> _02(int id, string schema, string conn);
        Task<List<PaymainhdrModel?>?> _02ByPeriodTrns(string periodTrn, string schema, string conn);
        Task<PaymainhdrModel?> _02ByTrn(string trn, string schema, string conn);
        Task<List<PaymainhdrModel?>?> _02ByTrns(string trn, string schema, string conn); 
        Task<PaymainhdrModel?> _03(int id, PaymainhdrModel paymainhdr, string schema, string conn);
        Task<PaymainhdrModel?> _04(int id, string schema, string conn);
    }
}