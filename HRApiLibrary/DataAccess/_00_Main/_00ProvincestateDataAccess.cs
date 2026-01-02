using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00ProvincestateDataAccess : I_00ProvincestateDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _00ProvincestateDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ProvinceStateModel?> _01(ProvinceStateModel provincestate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Provincestate (Code, Name, CountryId) values (@Code, @Name, @CountryId)";
        await _sql.ExecuteCmd<dynamic>(sql, provincestate, conn);
        sql = $@"SELECT * FROM {schema}.Provincestate WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<ProvinceStateModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, CountryId from {schema}.Provincestate where Id = @Id";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<ProvinceStateModel?>?> _02ByCountryId(int countryId, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, CountryId from {schema}.Provincestate 
                            where CountryId = @CountryId
                            order by Name " ;
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { CountryId = countryId }, conn);
        return data;
    }


    public async Task<ProvinceStateModel?> _03(int id, ProvinceStateModel provincestate, string schema, string conn)
    {
        string sql = $@"Update {schema}.Provincestate set Code = @Code, Name = @Name, CountryId = @CountryId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, provincestate, conn);

        sql = $@" select  * from {schema}.Provincestate x where x.Id = @Id ;";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ProvinceStateModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Provincestate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Provincestate x where x.Id = @Id ;";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
