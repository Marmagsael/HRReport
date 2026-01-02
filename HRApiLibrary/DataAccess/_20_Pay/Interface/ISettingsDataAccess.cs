using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface ISettingsDataAccess
    {
        Task<SettingsModel?> _02(int id, string schema, string conn);
        Task<SettingsModel?> _03(int id, SettingsModel settings, string schema, string conn);
    }
}