using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRdepDataAccess
    {
        Task<RdepModel?>        _01(RdepModel rdep, string schema, string conn);
        Task<RdepModel?>        _02(int id, string schema, string conn);
        Task<List<RdepModel?>?> _02(string schema, string conn);
        Task<RdepModel?>        _03(int id, RdepModel rdep, string schema, string conn);
        Task<RdepModel?>        _04(int id, string schema, string conn);
    }
}