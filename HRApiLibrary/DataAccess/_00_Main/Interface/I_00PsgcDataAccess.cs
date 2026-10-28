using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00PsgcDataAccess
    {
        Task<List<PsgcModel?>?>     _02Regions(string? schema, string? conn);
        Task<List<PsgcModel?>?>     _02Provinces(string? regCode, string? schema, string? conn);
        Task<List<PsgcModel?>?>     _02CityMuns(string? regCode, string? provCode, string? schema, string? conn);
        Task<List<PsgcModel?>?>     _02Barangays(string? cityMunCode, string? schema, string? conn);
    }
}
