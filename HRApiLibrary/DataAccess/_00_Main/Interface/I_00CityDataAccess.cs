using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00CityDataAccess
    {
        Task<CityModel?>            _01(CityModel city, string schema, string conn);
        Task<CityModel?>            _02(int id, string schema, string conn);
        Task<List<CityModel?>?>     _02ByRegionId(int regionId, string schema, string conn);
        Task<CityModel?>            _03(int id, CityModel city, string schema, string conn);
        Task<CityModel?>            _04(int id, string schema, string conn);
    }
}