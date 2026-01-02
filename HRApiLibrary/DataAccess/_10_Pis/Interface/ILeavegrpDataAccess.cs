using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeavegrpDataAccess
    {
        Task<LeavegrpModel?>            _01(LeavegrpModel leavegrp, string schema, string conn);
        Task<LeavegrpModel?>            _02(int id, string schema, string conn);
        Task<List<LeavegrpModel?>?>     _02(string schema, string conn);
        Task<LeavegrpModel?>            _03(int id, LeavegrpModel leavegrp, string schema, string conn);
        Task<LeavegrpModel?>            _04(int id, string schema, string conn);
    }
}