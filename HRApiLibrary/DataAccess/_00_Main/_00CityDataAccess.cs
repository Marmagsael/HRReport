using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00CityDataAccess : I_00CityDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _00CityDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<CityModel?> _01(CityModel city, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.City (CountryId, CountryCode, RegionId, CityName) values (@CountryId, @CountryCode, @RegionId, @CityName)";
        await _sql.ExecuteCmd<dynamic>(sql, city, conn);

        sql = $@"SELECT * FROM {schema}.City WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<CityModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<CityModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, CountryId, CountryCode, RegionId, CityName from {schema}.City where Id = @Id";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<CityModel?>?> _02ByRegionId(int regionId, string schema, string conn)
    {
        string sql = $@"select  Id, CountryId, CountryCode, RegionId, CityName 
                        from {schema}.City 
                        where RegionId = @RegionId 
                        order by CityName";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { RegionId = regionId }, conn);
        return data;

    }


    public async Task<CityModel?> _03(int id, CityModel city, string schema, string conn)
    {
        string sql = $@"Update {schema}.City set CountryId = @CountryId, CountryCode = @CountryCode, RegionId = @RegionId, CityName = @CityName where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, city, conn);

        sql = $@" select  * from {schema}.City x where x.Id = @Id ;";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<CityModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.City where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.City x where x.Id = @Id ;";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
