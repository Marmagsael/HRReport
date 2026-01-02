using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeavegrpapproverDataAccess
    {
        Task<LeavegrpapproverModel?>            _01(LeavegrpapproverModel leavegrpapprover, string schema, string conn);
        Task<LeavegrpapproverModel?>            _02(int id, string schema, string conn);
        Task<List<LeavegrpapproverModel?>?>     _02ByLeavegrpIdApproverLevel(int leavegrpid, int approverlevel, string schema, string conn);
        Task<LeavegrpapproverModel?>            _03(int id, LeavegrpapproverModel leavegrpapprover, string schema, string conn);
        Task<LeavegrpapproverModel?>            _04(int id, string schema, string conn);
    }
}