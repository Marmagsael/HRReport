using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._00_Main;

public class RpenaltyDataAccess : IPenaltyDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RpenaltyDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RpenaltyModel?> _01(RpenaltyModel Penalty, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rpenalty (DEV_NO, FREQ, PENALTY_NO, DESC_, resetregref, isterminated, days) values (@DEV_NO, @FREQ, @PENALTY_NO, @DESC_, @resetregref, @isterminated, @days)";
        await _sql.ExecuteCmd<dynamic>(sql, Penalty, conn);

        sql = $@"SELECT * FROM {schema}.Rpenalty WHERE Id = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<RpenaltyModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<RpenaltyModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select * from {schema}.Rpenalty where Id = @Id";
        var data = await _sql.FetchData<RpenaltyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RpenaltyModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select * from {schema}.Rpenalty";
        var data = await _sql.FetchData<RpenaltyModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<RpenaltyModel?> _03(int id, RpenaltyModel Penalty, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rpenalty set DEV_NO = @DEV_NO, FREQ = @FREQ, PENALTY_NO = @PENALTY_NO, DESC_ = @DESC_, resetregref = @resetregref, isterminated = @isterminated, days = @days where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Penalty, conn);

        sql = $@" select  * from {schema}.Rpenalty x where x.Id = @Id ;";
        var data = await _sql.FetchData<RpenaltyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RpenaltyModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rpenalty where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rpenalty x where x.Id = @Id ;";
        var data = await _sql.FetchData<RpenaltyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
