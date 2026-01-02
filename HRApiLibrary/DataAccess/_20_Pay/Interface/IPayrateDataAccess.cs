using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPayrateDataAccess
    {
        Task<PayrateModel?> _01(PayrateModel payrate, string schema, string conn);
        Task<PayrateModel?> _02(int id, string schema, string conn);
        Task<List<PayrateModel?>?> _02(string schema, string conn);
        Task<PayrateModel?> _03(int id, PayrateModel payrate, string schema, string conn);
        Task<PayrateModel?> _04(int id, string schema, string conn);
    }
}