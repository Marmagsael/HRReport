using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess; 

public class _001CompanyusertypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _001CompanyusertypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<CompanyUserTypeModel?> _01(CompanyUserTypeModel companyusertype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Companyusertype (Name, IsVisible) values (@Name, @IsVisible)";
        await _sql.ExecuteCmd<dynamic>(sql, companyusertype, conn);
        sql = $@"SELECT * FROM {schema}.Companyusertype WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<CompanyUserTypeModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }


    public async Task<CompanyUserTypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name, IsVisible from {schema}.Companyusertype where Id = @Id";
        var data = await _sql.FetchData<CompanyUserTypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<CompanyUserTypeModel?> _03(int id, CompanyUserTypeModel companyusertype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Companyusertype set Name = @Name, IsVisible = @IsVisible where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, companyusertype, conn);

        sql = $@" select  * from {schema}.Companyusertype x where x.Id = @Id ;";
        var data = await _sql.FetchData<CompanyUserTypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<CompanyUserTypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Companyusertype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Companyusertype x where x.Id = @Id ;";
        var data = await _sql.FetchData<CompanyUserTypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
