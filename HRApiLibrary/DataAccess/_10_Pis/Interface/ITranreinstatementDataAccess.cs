using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITranreinstatementDataAccess
    {
        Task<TranreinstatementModel?> _01(TranreinstatementModel Tranreinstatement, string schema, string conn);
        Task<TranreinstatementModel?> _02(int id, string schema, string conn);
        Task<TranreinstatementModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
        Task<TranreinstatementModel?> _02ByTrnNumber(string trnNumber, string schema, string conn);
        Task<TranreinstatementModel?> _03(int id, TranreinstatementModel Tranreinstatement, string schema, string conn);
        Task<TranreinstatementModel?> _04(int id, string schema, string conn);
    }
}