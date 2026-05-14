using HRApiLibrary.Models._00_MainPis;

namespace HRApiLibrary.DataAccess._00_MainTrans.Interfaces
{
    public interface IEngagementDataAccess
    {
        Task<EngagementModel?> _01(EngagementModel engagement, string schema, string conn);
        Task<EngagementModel?> _01Invite(EngagementModel engagement, string schema, string conn); 
        Task<EngagementModel?> _02(int id, string schema, string conn);
        Task<EngagementModel?> _03(int id, EngagementModel engagement, string schema, string conn);
        Task<EngagementModel?> _04(int id, string schema, string conn);
    }
}