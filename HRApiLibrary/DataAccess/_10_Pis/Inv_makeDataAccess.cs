using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Inv_makeDataAccess : IInv_makeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Inv_makeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Inv_makeModel?> _01(Inv_makeModel inv_make, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv_make (Inv_CategoryId, Name) values (@Inv_CategoryId, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, inv_make, conn);

        sql = $@"SELECT * FROM {schema}.Inv_make WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<Inv_makeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<Inv_makeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Inv_CategoryId, Name from {schema}.Inv_make where Id = @Id";
        var data = await _sql.FetchData<Inv_makeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<Inv_makeModel?> _03(int id, Inv_makeModel inv_make, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv_make set Inv_CategoryId = @Inv_CategoryId, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv_make, conn);

        sql = $@" select  * from {schema}.Inv_make x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_makeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Inv_makeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv_make where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv_make x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_makeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}