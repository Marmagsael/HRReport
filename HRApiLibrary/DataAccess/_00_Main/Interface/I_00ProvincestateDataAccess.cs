using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00ProvincestateDataAccess
    {
        Task<ProvinceStateModel?> _01(ProvinceStateModel provincestate, string schema, string conn);
        Task<ProvinceStateModel?> _02(int id, string schema, string conn);
        Task<List<ProvinceStateModel?>?> _02ByCountryId(int countryId, string schema, string conn);
        Task<ProvinceStateModel?> _03(int id, ProvinceStateModel provincestate, string schema, string conn);
        Task<ProvinceStateModel?> _04(int id, string schema, string conn);
    }
}