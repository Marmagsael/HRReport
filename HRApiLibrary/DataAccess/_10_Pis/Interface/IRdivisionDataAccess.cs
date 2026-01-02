using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRdivisionDataAccess
    {
        Task<RdivisionModel?> _01(RdivisionModel rdivision, string schema, string conn);
        Task<RdivisionModel?> _02(int id, string schema, string conn);
        Task<List<RdivisionModel?>?> _02(string schema, string conn);
        Task<RdivisionModel?> _03(int id, RdivisionModel rdivision, string schema, string conn);
        Task<RdivisionModel?> _04(int id, string schema, string conn);
    }
}