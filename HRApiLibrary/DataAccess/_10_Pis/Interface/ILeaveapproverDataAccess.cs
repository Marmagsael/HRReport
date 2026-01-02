using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeaveapproverDataAccess
    {
        Task<LeaveapproverModel?> _01(LeaveapproverModel leaveapprover, string schema, string conn);
        Task<LeaveapproverModel?> _02(int id, string schema, string conn);
        Task<List<LeaveapproverModel?>?> _02ByLvl(int lvl, string schema, string conn);
        Task<LeaveapproverModel?> _03(int id, LeaveapproverModel leaveapprover, string schema, string conn);
        Task<LeaveapproverModel?> _04(int id, string schema, string conn);
    }
}