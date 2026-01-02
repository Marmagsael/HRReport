using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RcivstatDataAccess : IRcivstatDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RcivstatDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RCivStatModel?> _01(RCivStatModel rcivstat, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rcivstat (Code, Name) values (@Code, @Name); 
                        SELECT * FROM {schema}.Rcivstat WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<RCivStatModel?, dynamic>(sql, rcivstat, conn);

        return res.FirstOrDefault();
    }


    public async Task<RCivStatModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Rcivstat where Id = @Id";
        var data = await _sql.FetchData<RCivStatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RCivStatModel?>?> _02(string schema, string conn)
    {
        string sql  = $@"select  Id, Code, Name from {schema}.Rcivstat ";
        var data    = await _sql.FetchData<RCivStatModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<RCivStatModel?> _03(int id, RCivStatModel rcivstat, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rcivstat set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rcivstat, conn);

        sql = $@" select  * from {schema}.Rcivstat x where x.Id = @Id ;";
        var data = await _sql.FetchData<RCivStatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RCivStatModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rcivstat where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rcivstat x where x.Id = @Id ;";
        var data = await _sql.FetchData<RCivStatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}