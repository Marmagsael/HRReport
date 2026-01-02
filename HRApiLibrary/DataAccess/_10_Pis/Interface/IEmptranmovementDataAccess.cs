using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmptranmovementDataAccess
    {
        Task<EmptranmovementModel?> _01(EmptranmovementModel emptranmovement, string schema, string conn);
        Task<EmptranmovementModel?> _02(int id, string schema, string conn);
        Task<EmptranmovementModel?> _02ByMovNo(string movNo, string schema, string conn);
        Task<EmptranmovementModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<EmptranmovementModel?> _03(int id, EmptranmovementModel emptranmovement, string schema, string conn);
        Task<EmptranmovementModel?> _04(int id, string schema, string conn);
    }
}