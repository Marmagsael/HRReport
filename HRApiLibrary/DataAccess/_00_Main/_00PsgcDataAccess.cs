using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00PsgcDataAccess : I_00PsgcDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _00PsgcDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    private const string Flds = "PsgcCode, Name, Level, RegCode, ProvCode, CityMunCode, Version";

    public async Task<List<PsgcModel?>?> _02Regions(string? schema, string? conn)
    {
        string? sql = $@"select {Flds} from {schema}.Psgc where Level = 'Reg' order by PsgcCode";
        return await _sql.FetchData<PsgcModel?, dynamic>(sql, new { }, conn);
    }

    public async Task<List<PsgcModel?>?> _02Provinces(string? regCode, string? schema, string? conn)
    {
        string? sql = $@"select {Flds} from {schema}.Psgc where Level = 'Prov' and RegCode = @RegCode order by Name";
        return await _sql.FetchData<PsgcModel?, dynamic>(sql, new { RegCode = regCode }, conn);
    }

    // -- With a province : its cities / municipalities plus the region's independent cities (HUC);
    //    without one (e.g. NCR) : the region's cities that have no province --
    public async Task<List<PsgcModel?>?> _02CityMuns(string? regCode, string? provCode, string? schema, string? conn)
    {
        string? sql = $@"select {Flds} from {schema}.Psgc
                        where Level in ('City', 'Mun') and RegCode = @RegCode
                          and (ProvCode = '' or ProvCode = @ProvCode)
                        order by Name";
        return await _sql.FetchData<PsgcModel?, dynamic>(sql, new { RegCode = regCode, ProvCode = provCode ?? "" }, conn);
    }

    public async Task<List<PsgcModel?>?> _02Barangays(string? cityMunCode, string? schema, string? conn)
    {
        string? sql = $@"select {Flds} from {schema}.Psgc where Level = 'Bgy' and CityMunCode = @CityMunCode order by Name";
        return await _sql.FetchData<PsgcModel?, dynamic>(sql, new { CityMunCode = cityMunCode }, conn);
    }
}
