using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeavecreditDataAccess
    {
        Task<LeavecreditModel?> _01(LeavecreditModel leavecredit, string schema, string conn);
        Task<LeavecreditModel?> _02(int id, string schema, string conn);
        Task<LeavecreditModel?> _03(int id, LeavecreditModel leavecredit, string schema, string conn);
        Task<LeavecreditModel?> _04(int id, string schema, string conn);
    }
}