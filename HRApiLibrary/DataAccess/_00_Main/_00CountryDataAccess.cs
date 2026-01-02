using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00CountryDataAccess : I_00CountryDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00CountryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<CountryModel?> _01(CountryModel country, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Country (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, country, conn);

        sql = $@"SELECT * FROM {schema}.Country WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<CountryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<CountryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Country where Id = @Id";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<CountryModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Country order by Name";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<CountryModel?> _03(int id, CountryModel country, string schema, string conn)
    {
        string sql = $@"Update {schema}.Country set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, country, conn);

        sql = $@" select  * from {schema}.Country x where x.Id = @Id ;";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<CountryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Country where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Country x where x.Id = @Id ;";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
