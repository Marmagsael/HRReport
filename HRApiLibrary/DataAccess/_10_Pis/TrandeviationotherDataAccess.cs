using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis;

public class TrandeviationotherDataAccess : ITrandeviationotherDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TrandeviationotherDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TrandeviationotherModel?> _01(TrandeviationotherModel Trandeviationother, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Trandeviationother (TranNumber, Remarks, Link) values (@TranNumber, @Remarks, @Link)";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationother, conn);

        sql = $@"SELECT * FROM {schema}.Trandeviationother WHERE TranNumber = @TranNumber";

        var res = await _sql.FetchData<TrandeviationotherModel?, dynamic>(sql, new { Trandeviationother.TranNumber }, conn);

        return res.FirstOrDefault();
    }



    public async Task<TrandeviationotherModel?> _02ByTrn(string trnNumber, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Trandeviationother where TranNumber = @TrnNumber";
        var data = await _sql.FetchData<TrandeviationotherModel?, dynamic>(sql, new { TrnNumber = trnNumber }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<TrandeviationotherModel?> _03(string trnNumber, TrandeviationotherModel Trandeviationother, string schema, string conn)
    {
        string sql = $@"Update {schema}.Trandeviationother set  Remarks = @Remarks , Link = @Link where TranNumber = @TranNumber;";
        await _sql.ExecuteCmd<dynamic>(sql, Trandeviationother, conn);

        sql = $@" select  * from {schema}.Trandeviationother x where TranNumber = @TranNumber ;";
        var data = await _sql.FetchData<TrandeviationotherModel?, dynamic>(sql, new { TranNumber = trnNumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TrandeviationotherModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Trandeviationotherwhere TranNumber = @TrnNumber;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Trandeviationother x where TranNumber = @TrnNumber ;";
        var data = await _sql.FetchData<TrandeviationotherModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
