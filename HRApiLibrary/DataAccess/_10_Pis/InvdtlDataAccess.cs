using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class InvdtlDataAccess : IInvdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public InvdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<InvdtlModel?> _01(InvdtlModel invdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Invdtl (InvId, Descr_, Value_) values (@InvId, @Descr_, @Value_)";
        await _sql.ExecuteCmd<dynamic>(sql, invdtl, conn);

        sql = $@"SELECT * FROM {schema}.Invdtl WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<InvdtlModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<InvdtlModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, InvId, Descr_, Value_ from {schema}.Invdtl where Id = @Id";
        var data = await _sql.FetchData<InvdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<InvdtlModel?> _03(int id, InvdtlModel invdtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Invdtl set InvId = @InvId, Descr_ = @Descr_, Value_ = @Value_ where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, invdtl, conn);

        sql = $@" select  * from {schema}.Invdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<InvdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<InvdtlModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Invdtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Invdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<InvdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
