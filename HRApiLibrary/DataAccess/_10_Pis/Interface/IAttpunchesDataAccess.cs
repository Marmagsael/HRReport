using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IAttpunchesDataAccess
    {
        Task<AttpunchesModel?> _01In(AttpunchesModel attpunches, string schema, string conn);
        Task<AttpunchesModel?> _02(int empmasid, DateTime punchDate, string schema, string conn);
        Task<List<AttpunchesModel?>?> _02s(int empmasid, DateTime punchDate, string schema, string conn);
        Task<AttpunchesModel?> _03(AttpunchesModel attpunches, string schema, string conn);
        Task<AttpunchesModel?> _04(int empmasid, DateTime punchDate, string schema, string conn);
    }
}