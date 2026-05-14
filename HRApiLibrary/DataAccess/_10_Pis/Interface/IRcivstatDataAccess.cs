using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRcivstatDataAccess
    {
        Task<RCivStatModel?> _01(RCivStatModel rcivstat, string schema, string conn);
        Task<RCivStatModel?> _02(int id, string schema, string conn);
        Task<List<RCivStatModel?>?> _02(string schema, string conn);
        Task<RCivStatModel?> _03(int id, RCivStatModel rcivstat, string schema, string conn);
        Task<RCivStatModel?> _04(int id, string schema, string conn);
    }
}