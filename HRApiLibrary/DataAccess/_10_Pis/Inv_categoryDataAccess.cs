using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class Inv_categoryDataAccess : IInv_categoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Inv_categoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Inv_categoryModel?> _01(Inv_categoryModel inv_category, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv_category (Inv_typeId, Name) values (@Inv_typeId, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, inv_category, conn);

        sql = $@"SELECT * FROM {schema}.Inv_category WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<Inv_categoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<Inv_categoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Inv_typeId, Name from {schema}.Inv_category where Id = @Id";
        var data = await _sql.FetchData<Inv_categoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<Inv_categoryModel?> _03(int id, Inv_categoryModel inv_category, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv_category set Inv_typeId = @Inv_typeId, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv_category, conn);

        sql = $@" select  * from {schema}.Inv_category x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_categoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Inv_categoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv_category where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv_category x where x.Id = @Id ;";
        var data = await _sql.FetchData<Inv_categoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}