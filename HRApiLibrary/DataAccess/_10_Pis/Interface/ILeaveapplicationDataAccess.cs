using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeaveapplicationDataAccess
    {
        Task<LeaveapplicationModel?> _01(LeaveapplicationModel leaveapplication, string schema, string conn);
        Task<LeaveapplicationModel?> _02(int id, string schema, string conn);
        Task<LeaveapplicationModel?> _03(int id, LeaveapplicationModel leaveapplication, string schema, string conn);
        Task<LeaveapplicationModel?> _04(int id, string schema, string conn);
    }
}