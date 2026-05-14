using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmpmovementDataAccess
    {
        Task<EmpmovementModel?> _01(EmpmovementModel empmovement, string schema, string conn);
        Task<EmpmovementModel?> _02(int id, string schema, string conn);
        Task<EmpmovementModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<EmpmovementModel?> _02ByRefno(int empmasId, string schema, string conn);
        Task<List<EmpmovementModel?>?> _02ListByEmpmasId(int empmasId, string schema, string conn);
        Task<EmpmovementModel?> _03(int id, EmpmovementModel empmovement, string schema, string conn);
        Task<EmpmovementModel?> _04(int id, string schema, string conn);
    }
}