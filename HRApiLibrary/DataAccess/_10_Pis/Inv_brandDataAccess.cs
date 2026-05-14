using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Inv_brandDataAccess : IInv_brandDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Inv_brandDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Inv_brandModel?> _01(Inv_brandModel inv_brand, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv_brand (Inv_CategoryId, Name) values (@Inv_CategoryId, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, inv_brand, conn);

        sql = $@"SELECT * FROM {schema}.Inv_brand WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<Inv_brandModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<Inv_brandModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Inv_CategoryId, Name from {schema}.Inv_brand where Id = @Id";
        var data = await _sql.FetchData<Inv_brandModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<Inv_brandModel?> _03(int id, Inv_brandModel inv_brand, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv_brand set Inv_CategoryId = @Inv_CategoryId, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv_brand, conn);

        sql = $@" select  * from {schema}.Inv_brand x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_brandModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Inv_brandModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv_brand where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv_brand x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_brandModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}