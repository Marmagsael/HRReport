using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PayrateDataAccess : IPayrateDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PayrateDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrateModel?> _01(PayrateModel payrate, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Payrate (RateName) values (@RateName)";
        await _sql.ExecuteCmd<dynamic>(sql, payrate, conn);

        sql = $@"SELECT * FROM {schema}.Payrate WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<PayrateModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<PayrateModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, RateName from {schema}.Payrate where Id = @Id";
        var data = await _sql.FetchData<PayrateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<PayrateModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Payrate";
        var data = await _sql.FetchData<PayrateModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<PayrateModel?> _03(int id, PayrateModel payrate, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrate set RateName = @RateName where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, payrate, conn);

        sql = $@" select  * from {schema}.Payrate x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PayrateModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Payrate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Payrate x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}


