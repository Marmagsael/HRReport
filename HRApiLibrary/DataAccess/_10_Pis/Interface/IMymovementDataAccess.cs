using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IMymovementDataAccess
    {
        Task<MymovementModel?> _01(MymovementModel mymovement, string schema, string conn);
        Task<MymovementModel?> _02(int id, string schema, string conn);
        Task<MymovementModel?> _03(int id, MymovementModel mymovement, string schema, string conn);
        Task<MymovementModel?> _04(int id, string schema, string conn);
    }
}