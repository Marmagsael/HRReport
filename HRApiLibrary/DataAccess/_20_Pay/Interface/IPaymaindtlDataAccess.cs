using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPaymaindtlDataAccess
    {
        Task<PaymaindtlModel?> _01(PaymaindtlModel paymaindtl, string schema, string conn);
        Task<List<PaymaindtlModel?>?> _01New(string trn, int payrollgrpId, string paydb, string pisdb, string conn); 
        Task<PaymaindtlModel?> _02(int id, string schema, string conn);
        Task<PaymaindtlModel?> _03(int id, PaymaindtlModel paymaindtl, string schema, string conn);
        Task<PaymaindtlModel?> _04(int id, string schema, string conn);
    }
}