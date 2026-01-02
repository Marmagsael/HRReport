using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IPenaltyDataAccess
    {
        Task<RpenaltyModel?> _01(RpenaltyModel Penalty, string schema, string conn);
        Task<RpenaltyModel?> _02(int id, string schema, string conn);
        Task<List<RpenaltyModel?>?> _02(string schema, string conn);
        Task<RpenaltyModel?> _03(int id, RpenaltyModel Penalty, string schema, string conn);
        Task<RpenaltyModel?> _04(int id, string schema, string conn);
    }
}