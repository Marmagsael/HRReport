using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeavedefaultapproverDataAccess
    {
        Task<LeavedefaultapproverModel?> _01(LeavedefaultapproverModel leavedefaultapprover, string schema, string conn);
        Task<LeavedefaultapproverModel?> _02(int id, string schema, string conn);
        Task<List<LeavedefaultapproverModel?>?> _02ByLvl(int lvl, string schema, string conn);
        Task<LeavedefaultapproverModel?> _03(int id, LeavedefaultapproverModel leavedefaultapprover, string schema, string conn);
        Task<LeavedefaultapproverModel?> _04(int id, string schema, string conn);
    }
}