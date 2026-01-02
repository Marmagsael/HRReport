using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Inv_typeDataAccess : IInv_typeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Inv_typeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Inv_typeModel?> _01(Inv_typeModel inv_type, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv_type (Name) values (@Name)";
        await _sql.ExecuteCmd<dynamic>(sql, inv_type, conn);

        sql = $@"SELECT * FROM {schema}.Inv_type WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<Inv_typeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<Inv_typeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Inv_type where Id = @Id";
        var data = await _sql.FetchData<Inv_typeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<Inv_typeModel?>?> _02(string schema, string conn)
    {
        var sql = $@"select  Id, Name from {schema}.Inv_type order by Name";
        var data = await _sql.FetchData<Inv_typeModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    

    public async Task<Inv_typeModel?> _03(int id, Inv_typeModel inv_type, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv_type set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv_type, conn);

        sql = $@" select  * from {schema}.Inv_type x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_typeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Inv_typeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv_type where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv_type x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_typeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}