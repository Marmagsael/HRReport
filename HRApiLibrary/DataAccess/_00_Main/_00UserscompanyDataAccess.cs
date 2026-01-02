using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00UserscompanyDataAccess : I_00UserscompanyDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00UserscompanyDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<UserCompanyModel?> _01(UserCompanyModel userscompany, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"Insert into {schema}.Userscompany 
                            (OwnerId,  CompanySName,  CompanyName,  CountryId,  RegionId,  CityId,  Zipcode,  CurrencyId) values 
                            (@OwnerId, @CompanySName, @CompanyName, @CountryId, @RegionId, @CityId, @Zipcode, @CurrencyId); 
                        update {schema}.Userscompany set  
                                AMSSchema       = concat('U',{userscompany.OwnerId},'C',@@IDENTITY,'Ams'), 
                                ApplicantSchema = concat('U',{userscompany.OwnerId},'C',@@IDENTITY,'App'), 
                                PISSchema       = concat('U',{userscompany.OwnerId},'C',@@IDENTITY,'Pis'), 
                                PaySchema       = concat('U',{userscompany.OwnerId},'C',@@IDENTITY,'Pay')  
                            where Id = @@IDENTITY; 
                        update {schema}.Users set DefaultCoId = @@IDENTITY where Id = {userscompany.OwnerId};
                        
                        SELECT * FROM {schema}.Userscompany WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, userscompany, conn);

        //Create Schema ------------------------------------------------------------------
        if(res!=null)
        {
            var uco = res.FirstOrDefault();
            string schemaAms = uco!.AmsSchema!;
            string schemaApp = uco!.ApplicantSchema!;
            string schemaPis = uco!.PisSchema!;
            string schemaPay = uco!.PaySchema!;

            string sql1 = @$"create database if not exists {schemaAms};
                             create database if not exists {schemaApp};
                             create database if not exists {schemaPis};
                             create database if not exists {schemaPay};";

            await _sql.ExecuteCmd<dynamic>(sql1, new { }, conn);
        }


        return res?.FirstOrDefault();
    }
    
    public async Task<UserCompanyModel?> _02(int id, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"select  u.*, c.Name CountryName,  
                            concat(trim(ifnull(e.EmpLastNm,'')), ', ',trim(ifnull(e.EmpFirstNm,'')), ' ', trim(ifnull(e.EmpMidNm,'')) )  OwnerName 
                        from {schema}.Userscompany u 
                            left join Mainpis.Empmas e on e.Id = u.OwnerId 
                            left join {schema}.Country c on c.Id = u.CountryId 
                        where u.Id = @Id";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<UserCompanyModel?>?> _02Lst(int id, string schema="Main", string conn="MySqlConn")
    {
        var sql = $@"select  u.*, c.Name CountryName,  
                            concat(trim(ifnull(e.EmpLastNm,'')), ', ',trim(ifnull(e.EmpFirstNm,'')), ' ', trim(ifnull(e.EmpMidNm,'')) )  OwnerName 
                        from {schema}.Userscompany u 
                            left join Mainpis.Empmas e on e.Id = u.OwnerId 
                            left join {schema}.Country c on c.Id = u.CountryId 
                        where u.Id = @Id";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }

    public async Task<UserCompanyModel?> _02ByUserDefaultCoId(int userId, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@"select  u.*, c.Name CountryName,  
                            concat(trim(ifnull(e.EmpLastNm,'')), ', ',trim(ifnull(e.EmpFirstNm,'')), ' ', trim(ifnull(e.EmpMidNm,'')) )  OwnerName 
                        from {schema}.Userscompany u 
                            left join Mainpis.Empmas e on e.Id = u.OwnerId 
                            left join {schema}.Country c on c.Id = u.CountryId 
                        where u.Id in (select DefaultCoId from {schema}.users where Id = @UserId)";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<UserCompanyModel?>?> _02ByUserId(int userId, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"select  u.*, c.Name CountryName from {schema}.Userscompany u 
                            left join {schema}.Country c on c.Id = u.CountryId
                        where OwnerId = @UserId";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }

    public async Task<List<UserCompanyModel?>?> _02ByCompanySName(string companySName, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"select  * from {schema}.Userscompany where lower(CompanySName) = lower(@CompanySName)";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { CompanySName = companySName }, conn);
        return data;
    }
    

    public async Task<UserCompanyModel?> _03(int id, UserCompanyModel userscompany, string schema, string conn)
    {
        string sql = $@"Update {schema}.Userscompany set OwnerId = @OwnerId, CompanySName = @CompanySName, CompanyName = @CompanyName, CountryId = @CountryId, RegionId = @RegionId, CityId = @CityId, Zipcode = @Zipcode, CurrencyId = @CurrencyId, StorageId = @StorageId, AMSSchema = @AMSSchema, ApplicantSchema = @ApplicantSchema, PISSchema = @PISSchema, PaySchema = @PaySchema where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, userscompany, conn);

        sql = $@" select  * from {schema}.Userscompany x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<UserCompanyModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Userscompany where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Userscompany x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
