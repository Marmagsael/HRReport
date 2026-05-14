using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00UsercompanyaddDataAccess : I_00UsercompanyaddDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public _00UsercompanyaddDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<UserCompanyModel?> _01(UserCompanyModel userscompany, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Userscompany 
                            (OwnerId,  CompanySName,  CompanyName,  CountryId,  RegionId,  CityId,  Zipcode,  CurrencyId,  StorageId,  AMSSchema,  ApplicantSchema,  PISSchema,  PaySchema) values 
                            (@OwnerId, @CompanySName, @CompanyName, @CountryId, @RegionId, @CityId, @Zipcode, @CurrencyId, @StorageId, @AMSSchema, @ApplicantSchema, @PISSchema, @PaySchema); 
                        SELECT * FROM {schema}.Userscompany WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, userscompany, conn);
        return res.FirstOrDefault();
    }


    public async Task<UserCompanyModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, OwnerId, CompanySName, CompanyName, CountryId, RegionId, CityId, Zipcode, CurrencyId, StorageId, AMSSchema, ApplicantSchema, PISSchema, PaySchema 
                            from {schema}.Userscompany where Id = @Id";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<UserCompanyModel?> _03(int id, UserCompanyModel userscompany, string schema, string conn)
    {
        string sql = $@"Update {schema}.Userscompany set 
                            OwnerId         = @OwnerId, 
                            CompanySName    = @CompanySName,  
                            CompanyName     = @CompanyName,  
                            CountryId       = @CountryId,  
                            RegionId        = @RegionId,  
                            CityId          = @CityId,  
                            Zipcode         = @Zipcode,  
                            CurrencyId      = @CurrencyId,  
                            StorageId       = @StorageId,  
                            AMSSchema       = @AMSSchema,  
                            ApplicantSchema = @ApplicantSchema,  
                            PISSchema       = @PISSchema,  
                            PaySchema       = @PaySchema where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, userscompany, conn);

        sql = $@" select  * from {schema}.Userscompany x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<UserCompanyModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Userscompany where Id = @Id;
                        Select  * from {schema}.Userscompany x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
