using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class LoanhdrDataAccess : ILoanhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LoanhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LoanhdrModel?> _01(LoanhdrModel loanhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Loanhdr (id, orno, paydate, yr, mo, amount, remarks, acctcode) values (@id, @orno, @paydate, @yr, @mo, @amount, @remarks, @acctcode)";
        await _sql.ExecuteCmd<dynamic>(sql, loanhdr, conn);

        sql = $@"SELECT * FROM {schema}.Loanhdr WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<LoanhdrModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<LoanhdrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  id, orno, paydate, yr, mo, amount, remarks, acctcode from {schema}.Loanhdr where Id = @Id";
        var data = await _sql.FetchData<LoanhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<LoanhdrModel?> _03(int id, LoanhdrModel loanhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Loanhdr set id = @id, orno = @orno, paydate = @paydate, yr = @yr, mo = @mo, amount = @amount, remarks = @remarks, acctcode = @acctcode where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, loanhdr, conn);

        sql = $@" select  * from {schema}.Loanhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<LoanhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LoanhdrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Loanhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Loanhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<LoanhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}