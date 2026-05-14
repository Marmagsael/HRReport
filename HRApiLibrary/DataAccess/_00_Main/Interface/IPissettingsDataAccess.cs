using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface IPissettingsDataAccess
    {
        Task<PissettingsModel?> _02(string schema, string conn);
        Task<PissettingsModel?> _03(PissettingsModel pissettings, string schema, string conn);
    }
}