using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeaveapplicationDataAccess
    {
        Task<LeaveapplicationModel?>            _01(LeaveapplicationModel leaveapplication, string schema, string conn);
        Task<LeaveapplicationModel?>            _02(int id, string schema, string conn);
        Task<List<LeaveapplicationModel?>?>     _02Chk_Entry_LvType(int leaveTypeId, string schema, string conn);
        Task<double>                            _02LvBalance(int lvTypeId, int empmasId, int yr, string schema, string conn);
        Task<List<LeaveapplicationModel?>?>     _02ByRequest(int empmasId, string schema, string conn);
        Task<List<LeaveapplicationModel?>?>     _02ForApproval_PerApprover(int approverId, string pisdb, string conn); 
        Task<LeaveapplicationModel?>            _03(int id, LeaveapplicationModel leaveapplication, string schema, string conn);
        Task                                    _03Return(LeaveapplicationModel lva, int approverId, string schema, string conn);
        Task<LeaveapplicationModel?>            _03SendForApproval(LeaveapplicationModel lva, string schema, string conn);
        Task                                    _03Approve(LeaveapplicationModel lva, int approverId, string schema, string conn);
        Task<LeaveapplicationModel?>            _04(int id, string schema, string conn);
        
    }
}