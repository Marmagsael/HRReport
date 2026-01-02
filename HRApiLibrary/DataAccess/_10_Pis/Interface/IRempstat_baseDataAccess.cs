using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRempstat_baseDataAccess
    {
        Task<List<Rempstat_baseModel?>?> _01(string key, List<int> ids, string schema, string conn);
        Task<Rempstat_baseModel?> _01(string key, Rempstat_baseModel Rempstat_base, string schema, string conn);
        Task<Rempstat_baseModel?> _02(string key, int id, string schema, string conn);
        Task<List<Rempstat_baseModel?>?> _02(string key, string schema, string conn);
        Task<Rempstat_baseModel?> _03(string key, int id, Rempstat_baseModel Rempstat_base, string schema, string conn);
        Task<Rempstat_baseModel?> _04(string key, int id, string schema, string conn);
        Task<List<Rempstat_baseModel?>?> _04(string key, List<int> ids, string schema, string conn);
        Task<List<Rempstat_baseModel?>?> _04(string key, string schema, string conn);
    }
}