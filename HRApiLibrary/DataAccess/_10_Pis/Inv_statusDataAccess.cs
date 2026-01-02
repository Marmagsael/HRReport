using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Inv_statusDataAccess : IInv_statusDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Inv_statusDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Inv_statusModel?> _01(Inv_statusModel inv_status, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv_status (Name) values (@Name)";
        await _sql.ExecuteCmd<dynamic>(sql, inv_status, conn);

        sql = $@"SELECT * FROM {schema}.Inv_status WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<Inv_statusModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<Inv_statusModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Inv_status where Id = @Id";
        var data = await _sql.FetchData<Inv_statusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<Inv_statusModel?> _03(int id, Inv_statusModel inv_status, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv_status set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv_status, conn);

        sql = $@" select  * from {schema}.Inv_status x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_statusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Inv_statusModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv_status where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv_status x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_statusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}