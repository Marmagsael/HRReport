using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00CountryDataAccess
    {
        Task<CountryModel?> _01(CountryModel country, string schema, string conn);
        Task<CountryModel?> _02(int id, string schema, string conn);
        Task<List<CountryModel?>?> _02(string schema, string conn);
        Task<CountryModel?> _03(int id, CountryModel country, string schema, string conn);
        Task<CountryModel?> _04(int id, string schema, string conn);
    }
}