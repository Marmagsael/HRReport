using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ILeavetypeDataAccess
    {
        Task<LeavetypeModel?> _01(LeavetypeModel leavetype, string schema, string conn);
        Task<LeavetypeModel?> _02(int id, string schema, string conn);
        Task<List<LeavetypeModel?>?> _02(string schema, string conn);
        Task<LeavetypeModel?> _03(int id, LeavetypeModel leavetype, string schema, string conn);
        Task<LeavetypeModel?> _04(int id, string schema, string conn);
    }
}