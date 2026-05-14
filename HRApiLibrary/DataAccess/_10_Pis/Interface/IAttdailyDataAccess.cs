using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IAttdailyDataAccess
    {
        Task<AttdailyModel?> _01(AttdailyModel attdaily, string schema, string conn);
        Task<AttdailyModel?> _01PunchIn(AttdailyModel attdaily, string schema, string conn);
        Task<AttdailyModel?> _01PunchOut(AttdailyModel attdaily, string schema, string conn);
        Task<AttdailyModel?> _02(int id, DateTime punchdate, string schema, string conn);
        Task<List<AttdailyModel?>?> _02ByMonth(int empmasId, int yr, int month, string schema, string conn);
        Task<AttdailyModel?> _02CurrPunch(int empmasId, string schema, string conn);
        Task<AttdailyModel?> _02PrevPunch(int empmasId, string schema, string conn);
        Task<AttdailyModel?> _03(int id, AttdailyModel attdaily, string schema, string conn);
        Task<AttdailyModel?> _04(int empmasId, DateTime punchDate, string schema, string conn);
    }
}