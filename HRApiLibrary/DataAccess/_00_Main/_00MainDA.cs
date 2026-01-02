
using System.Numerics;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._90_Utils;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using HRApiLibrary.Models._10_Pis;
using System.Runtime.InteropServices.Marshalling;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00MainDA : I_00MainDA
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IConfiguration _config;

    public _00MainDA(I_90_001_MySqlDataAccess sql, IConfiguration config)
    {
        _sql = sql;
        _config = config;
    }

    /* 
     * 1) UsersCompany 
     * 2) CompanyUsers
     * 3) Country 
     * 4) ProvicenState 
     * 5) City
     * 6) Menus 
     * 7) Users 
     * 8) CompanyAssignedToUser 
     * 9) UseridIdentity - (Claims)
     * 
     */

    // -- 9) UserClaimsContent - (Claims) -------------------------------------------------------------------------
    public UserClaimsModel _02UserClaimsContent(IEnumerable<Claim?> claims)
    {
        var uc = new UserClaimsModel
        {
            SchemaMain      = _config.GetSection("Schema:Main").Value,
            SchemaMainPis   = _config.GetSection("Schema:MainPis").Value,
            Conn            = _config.GetSection("Schema:DefConn").Value,
            ConnNoDb        = _config.GetSection("Schema:DefConnNoDb").Value
        };

        if (claims?.Count() > 0)
        {
            var val = claims.Where(c => c?.Type == "UserId").FirstOrDefault()?.Value;
            if (val != null) uc.UserId = int.Parse(val);
            uc.DefCompanyId = claims.Where(c => c?.Type == "DefCompayId").FirstOrDefault()?.Value;

            var userId = uc.UserId.ToString();
            var uprefix = "U" + userId + "C1";

            if (string.IsNullOrEmpty(uc.DefCompanyId))
            {
                uc.SchemaMainPis    = uprefix + "Pis";
                uc.SchemaUserPay    = uprefix + "Pay";
                uc.SchemaUserAms    = uprefix + "Ams";
                uc.SchemaUserApp    = uprefix + "App";
                uc.SchemaUserAcctg  = uprefix + "Acctg";

            }
            else
            {
                uc.SchemaUserPis    = claims.Where(c => c?.Type == "PisSchema").FirstOrDefault()?.Value;
                uc.SchemaUserPay    = claims.Where(c => c?.Type == "PaySchema").FirstOrDefault()?.Value;
                uc.SchemaUserAcctg  = claims.Where(c => c?.Type == "AcctgSchema").FirstOrDefault()?.Value;
                uc.SchemaUserApp    = claims.Where(c => c?.Type == "ApplicantSchema").FirstOrDefault()?.Value;
                uc.SchemaUserAms    = claims.Where(c => c?.Type == "AmsSchema").FirstOrDefault()?.Value;
                uc.CoName           = claims.Where(c => c?.Type == "CoName").FirstOrDefault()?.Value;
            }
            //var baseConn = _config.GetConnectionString(uc.Conn);
            //uc.ConnPay      = uc.ConnNoDb;   
            //uc.ConnPis      = $"{baseConn}; database={uc.SchemaUserPis}";
            //uc.ConnAcctg    = $"{baseConn}; database={uc.SchemaUserAcctg}";
        }
        
        return uc;
    }

    private async Task<UserClaimsModel> _02UserClaimsContentProcess(IEnumerable<Claim?> claims)
    {
        var uc = new UserClaimsModel
        {
            SchemaMain = _config.GetSection("Schema:Main").Value,
            SchemaMainPis = _config.GetSection("Schema:MainPis").Value,
            Conn = _config.GetSection("Schema:DefConn").Value
        };

        if (claims.Count() <= 0) return uc;

        var val = claims.Where(c => c?.Type == "UserId").FirstOrDefault()?.Value;
        if (val != null) uc.UserId = int.Parse(val);

        val = claims.Where(c => c?.Type == "DefCompayId").FirstOrDefault()?.Value;
        if (val != null) uc.DefCompanyId = val;


        var userId = uc.UserId.ToString();
        var uprefix = "U" + userId + "C1";
        uc.SchemaMainPis = uprefix + "Pis";
        uc.SchemaUserPay = uprefix + "Pay";
        uc.SchemaUserAms = uprefix + "Ams";
        uc.SchemaUserApp = uprefix + "App";
        uc.SchemaUserAcctg = uprefix + "Acctg";

        if (string.IsNullOrEmpty(uc.DefCompanyId))
        {
            var uco = await _02UserCompanyPerUser(uc.UserId);
            uc.CoName = uco.FirstOrDefault()?.CompanyName;
            return uc;
        }

        var defCoId = int.Parse(uc.DefCompanyId);

        if (defCoId <= 0) return uc;
        var res = await _02UserCompany(defCoId, uc.SchemaMain, uc.Conn);

        if (res == null) return uc;
        var ownerId = res.OwnerId.ToString().ToString();
        var coId = res.Id.ToString().ToString();
        var prefix = "U" + ownerId + "C" + coId;

        uc.SchemaMainPis = prefix + "Pis";
        uc.SchemaUserPay = prefix + "Pay";
        uc.SchemaUserAms = prefix + "Ams";
        uc.SchemaUserApp = prefix + "App";
        uc.SchemaUserAcctg = prefix + "Acctg";
        uc.CoName = res.CompanyName;

        return uc;
    }
    // -- 1) Users Company -------------------------------------------------------------------------------------

    public async Task<UserCompanyModel?> _01UserCompany(UserCompanyModel userscompany, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Userscompany (OwnerId,  CompanySName,  CompanyName,  CountryId,  RegionId,  CityId,  Zipcode,  CurrencyId,  StorageId,  AMSSchema,  ApplicantSchema,  PISSchema,  PaySchema) values 
                                                          (@OwnerId, @CompanySName, @CompanyName, @CountryId, @RegionId, @CityId, @Zipcode, @CurrencyId, @StorageId, @AMSSchema, @ApplicantSchema, @PISSchema, @PaySchema)";
        await _sql.ExecuteCmd<dynamic>(sql, userscompany, conn);

        sql = $@"SELECT * FROM {schema}.Userscompany WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    public async Task<UserCompanyModel?> _02UserCompany(int id, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@"select  Id, OwnerId, CompanySName, CompanyName, CountryId, RegionId, CityId, Zipcode, 
                            CurrencyId, StorageId, AMSSchema, ApplicantSchema, PISSchema, PaySchema from {schema}.Userscompany where Id = @Id";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<UserCompanyModel?>> _02UserCompanyPerUser(int id, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@"select  Id, OwnerId, CompanySName, CompanyName, CountryId, RegionId, CityId, Zipcode, 
                            CurrencyId, StorageId, AMSSchema, ApplicantSchema, PISSchema, PaySchema from {schema}.Userscompany where OwnerId = @OwnerId";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { OwnerId = id }, conn);
        return data;
    }
    public async Task<List<UserCompanyModel?>> _02UserCompanyPerOwnerId(int id, string schema = "Main", string conn = "MySqlConn")
    {
        var sql = $@"select  * from {schema}.Userscompany where OwnerId = @OwnerId";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { OwnerId = id }, conn);
        return data;
    }

    public async Task<UserCompanyModel?> _03UserCompany(int id, UserCompanyModel userscompany, string schema, string conn)
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
    public async Task<UserCompanyModel?> _04UserCompany(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Userscompany where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Userscompany x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserCompanyModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // -- 2) CompanyUsers
    public async Task<CompanyUsersModel?> _01CompanyUsers(CompanyUsersModel companyusers, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Companyusers 
                            (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId) values 
                            (@UserId, @CompanyId, @Status, @DateInvited, @DateAccepted, @CompanyUserTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, companyusers, conn);

        sql = $@"SELECT * FROM {schema}.Companyusers WHERE UserId =@UserId and CompanyId=@CompanyId";

        var res = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = companyusers.UserId, CompanyId = companyusers.CompanyId }, conn);

        return res.FirstOrDefault();
    }
    public async Task<CompanyUsersModel?> _02CompanyUsers(int userId, int companyId, string schema, string conn)
    {
        string sql = $@"select  UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId from {schema}.Companyusers where UserId =@UserId and CompanyId=@CompanyId";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<CompanyUsersModel?>> _02CompanyUsersPerCompany(int companyId, string schema, string conn)
    {
        string sql = $@"select  UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId from {schema}.Companyusers where CompanyId =@CompanyId";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { CompanyId = companyId }, conn);
        return data;
    }

    public async Task<CompaniesAssignedToUserModel?> _02CompanyUsers_WTypeWCoDtls(int userId, int companyId, string schema, string conn)
    {
        string sql = $@"select  cu.*, t.Id as UserTypeId, t.Name as UserType, t.IsVisible as IsTypeVisible, uc.*
                            from {schema}.Companyusers cu
                            left join Main.CompanyuserType t on t.id = cu.CompanyuserTypeId
                            left join Main.UsersCompany uc on uc.Id = cu.CompanyId
                       where cu.UserId = @UserId and cu.CompanyId = @CompanyId";

        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);
        return data.FirstOrDefault();
    }
    public async Task<List<CompaniesAssignedToUserModel?>> _02CompanyUsers_WTypeWCoDtls_List(int userId, string schema, string conn)
    {
        string sql = $@"select  cu.*, t.Id as UserTypeId, t.Name as UserType, t.IsVisible as IsTypeVisible, uc.*
                            from {schema}.Companyusers cu
                            left join {schema}.CompanyuserType t on t.id = cu.CompanyuserTypeId
                            left join {schema}.UsersCompany uc on uc.Id = cu.CompanyId
                       where cu.UserId = @UserId";

        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }
    public async Task<List<CompaniesAssignedToUserModel?>> _02CompanyUsers_Owned_WTypeWCoDtls_List(int userId, string schema, string conn)
    {
        string sql = $@"select  cu.*, t.Id as UserTypeId, t.Name as UserType, t.IsVisible as IsTypeVisible, uc.*
                            from  {schema}.Companyusers cu
                            left join  {schema}.CompanyuserType t on t.id = cu.CompanyuserTypeId
                            left join  {schema}.UsersCompany uc on uc.Id = cu.CompanyId
                       where cu.UserId = @UserId and uc.OwnerId = @UserId";

        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }

    public async Task<List<CompaniesAssignedToUserModel?>> _02CompanyUsers_Employment_WTypeWCoDtls_List(int userId, string schema, string conn)
    {
        string sql = $@"select  cu.*, t.Id as UserTypeId, t.Name as UserType, t.IsVisible as IsTypeVisible, uc.*
                            from  {schema}.Companyusers cu
                            left join  {schema}.CompanyuserType t on t.id = cu.CompanyuserTypeId
                            left join  {schema}.UsersCompany uc on uc.Id = cu.CompanyId
                       where cu.UserId = @UserId and uc.OwnerId != @UserId";

        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }

    public async Task<CompanyUsersModel?> _03CompanyUsers(int userId, int companyId, CompanyUsersModel companyusers, string schema, string conn)
    {
        string sql = $@"Update {schema}.Companyusers set 
                            Status              = @Status, 
                            DateInvited         = @DateInvited, 
                            DateAccepted        = @DateAccepted, 
                            CompanyUserTypeId   = @CompanyUserTypeId 
                        WHERE UserId =@UserId and CompanyId=@CompanyId;";
        await _sql.ExecuteCmd<dynamic>(sql, companyusers, conn);

        sql = $@" select  * from {schema}.Companyusers x WHERE UserId =@UserId and CompanyId=@CompanyId;";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<CompanyUsersModel?> _04CompanyUsers(int userId, int companyId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Companyusers WHERE UserId =@UserId and CompanyId=@CompanyId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);

        sql = $@" select  * from {schema}.Companyusers x WHERE UserId =@UserId and CompanyId=@CompanyId;";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);
        return data?.FirstOrDefault();
    }


    // -- 3) Country 
    public async Task<CountryModel?> _01Country(CountryModel country, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Country (Code, Name) values (@Code, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, country, conn);

        sql = $@"SELECT * FROM {schema}.Country WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<CountryModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    public async Task<CountryModel?> _02Country(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Country where Id = @Id";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<CountryModel?>> _02CountryList(string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name from {schema}.Country order by Name";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<CountryModel?> _03Country(int id, CountryModel country, string schema, string conn)
    {
        string sql = $@"Update {schema}.Country set Code = @Code, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, country, conn);

        sql = $@" select  * from {schema}.Country x where x.Id = @Id ;";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<CountryModel?> _04Country(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Country where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Country x where x.Id = @Id ;";
        var data = await _sql.FetchData<CountryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // -- 4) ProvinceState 
    public async Task<ProvinceStateModel?> _01ProvinceState(ProvinceStateModel ProvinceState, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.ProvinceState (Code, Name, CountryId) values (@Code, @Name, @CountryId)";
        await _sql.ExecuteCmd<dynamic>(sql, ProvinceState, conn);

        sql = $@"SELECT * FROM {schema}.ProvinceState WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    public async Task<ProvinceStateModel?> _02ProvinceState(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, CountryId from {schema}.ProvinceState where Id = @Id";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<ProvinceStateModel?>> _02ProvinceStatePerCountry(int countryId, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Name, CountryId from {schema}.ProvinceState where CountryId = @CountryId";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { CountryId = countryId }, conn);
        return data;
    }
    public async Task<ProvinceStateModel?> _03ProvinceState(int id, ProvinceStateModel ProvinceState, string schema, string conn)
    {
        string sql = $@"Update {schema}.ProvinceState set Code = @Code, Name = @Name, CountryId = @CountryId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, ProvinceState, conn);

        sql = $@" select  * from {schema}.ProvinceState x where x.Id = @Id ;";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<ProvinceStateModel?> _04ProvinceState(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.ProvinceState where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.ProvinceState x where x.Id = @Id ;";
        var data = await _sql.FetchData<ProvinceStateModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    // -- 5) City
    public async Task<CityModel?> _01City(CityModel city, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.City (CountryId, CountryCode, RegionId, CityName) values (@CountryId, @CountryCode, @RegionId, @CityName)";
        await _sql.ExecuteCmd<dynamic>(sql, city, conn);

        sql = $@"SELECT * FROM {schema}.City WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<CityModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    public async Task<CityModel?> _02City(int id, string schema, string conn)
    {
        string sql = $@"select  Id, CountryId, CountryCode, RegionId, CityName from {schema}.City where Id = @Id";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<CityModel?>> _02CityPerRegion(int regionId, string schema, string conn)
    {
        string sql = $@"select  Id, CountryId, CountryCode, RegionId, CityName from {schema}.City where RegionId = @RegionId";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { RegionId = regionId }, conn);
        return data;
    }
    public async Task<CityModel?> _03City(int id, CityModel city, string schema, string conn)
    {
        string sql = $@"Update {schema}.City set CountryId = @CountryId, CountryCode = @CountryCode, RegionId = @RegionId, CityName = @CityName where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, city, conn);

        sql = $@" select  * from {schema}.City x where x.Id = @Id ;";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<CityModel?> _04City(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.City where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.City x where x.Id = @Id ;";
        var data = await _sql.FetchData<CityModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // -- 6) Menus 
    public async Task<MenusModel?> _01Menu(string menuName, MenusModel Menus, string schema, string conn)
    {

        string sql = $@"Insert into {schema}.{menuName} 
                            (IdParent,  Indent,  Type,  Code,  Icon1,  Icon2,  DispText,  IsWithChild,  IsSelected,  Controller,  Action,  Odr,  IsPaid) values 
                            (@IdParent, @Indent, @Type, @Code, @Icon1, @Icon2, @DispText, @IsWithChild, @IsSelected, @Controller, @Action, @Odr, @IsPaid)";
        await _sql.ExecuteCmd<dynamic>(sql, Menus, conn);

        sql = $@"SELECT * FROM {schema}.{menuName} WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<MenusModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    public async Task<MenusModel?> _02Menu(string menuName, int id, string schema, string conn)
    {
        string sql = $@"select  Id, IdParent, Indent, Type, Code, Icon1, Icon2, DispText, IsWithChild, IsSelected, Controller, Action, Odr, IsPaid from {schema}.{menuName} where Id = @Id";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<MenusModel?>> _02MenuList(string menuName, int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.{menuName} order by odr";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }

    public async Task<List<MenusModel?>> _02MenuList_ByModule(string menuName, string moduleNo, string schema, string conn)
    {
        string sql = $@"SELECT * FROM {schema}.{menuName} where left(Id,1) = @ModuleNo  order by odr";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { ModuleNo = moduleNo }, conn);
        return data;
    }

    public async Task<MenusModel?> _03Menu(string menuName, int id, MenusModel Menus, string schema, string conn)
    {
        string sql = $@"Update {schema}.{menuName} set 
                            IdParent    = @IdParent, 
                            Indent      = @Indent,  
                            Type        = @Type,  
                            Code        = @Code,  
                            Icon1       = @Icon1,  
                            Icon2       = @Icon2,  
                            DispText    = @DispText,  
                            IsWithChild = @IsWithChild,  
                            IsSelected  = @IsSelected,  
                            Controller  = @Controller,  
                            Action      = @Action,  
                            Odr         = @Odr,  
                            IsPaid      = @IsPaid where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Menus, conn);

        sql = $@" select  * from {schema}.{menuName} x where x.Id = @Id ;";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MenusModel?> _03MenuSpecificIsSelected(string menuName, int id, MenusModel Menus, string schema, string conn)
    {
        _ = $@"Update {schema}.{menuName} set 
                            IsSelected  = 0;";


        string sql = $@"Update {schema}.{menuName} set 
                            IsSelected  = @IsSelected 
                            where Id = @Id;";

        await _sql.ExecuteCmd<dynamic>(sql, Menus, conn);

        sql = $@" select  * from {schema}.{menuName} x where x.Id = @Id ;";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MenusModel?> _03MenuResetIsSelected(string menuName, string schema, string conn)
    {
        string sql = $@"Update {schema}.{menuName} set 
                            IsSelected  = 0;";


        await _sql.ExecuteCmd<dynamic>(sql, conn);

        sql = $@" select  * from {schema}.{menuName} x ;";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<MenusModel?> _04Menu(string menuName, int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.{menuName} where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.{menuName} x where x.Id = @Id ;";
        var data = await _sql.FetchData<MenusModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    // -- 7) Users 
    public async Task<UsersModel?> _01Users(UsersModel user, string schema = "Main", string conn = "MySqlConn")
    {
        var newuser = await _sql.FetchData<UsersModel?, dynamic>($"select * from {schema}.users where LoginName = @LoginName", new { LoginName = user.LoginName }, conn);
        if (newuser == null) { return newuser?.FirstOrDefault(); }
        string sql = $@"Insert into {schema}.users  (LoginName,  Password,            Email,  Domain,  UserType,  Status,  DefaultCoId)  Values 
                                                    (@LoginName, sha2(@Password,512), @Email, @Domain, @UserType, @Status, @DefaultCoId);";
        await _sql.ExecuteCmd<dynamic>(sql, user, conn);

        sql = $@" select  * from {schema}.Users e where e.LoginName = @LoginName and e.Email = @Email";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = user.LoginName, Email = user.Email }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<UsersModel?> _02UsersById(int id, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where Id = @Id";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<UsersModel?>?> _02UsersByIdAndEmailLst(int id, string email, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where Id = @Id";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = id, Email = email }, conn);
        return data;
    }

    public async Task<UsersModel?> _02UsersByEmail(string email, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where Email = @Email";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<UsersModel?>?> _02UsersByEmailLst(string email, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  u.*, 
                            concat(trim(ifnull(e.EmplastNm,'')),', ' ,trim(ifnull(e.EmpFirstNm,'')),' ', trim(ifnull(E.EmpMidNm,''))) AFullName 
                          from {schema}.Users u 
                          left join MainPis.Empmas e on e.Id = u.id
                          where u.Email = @Email 
                          order by e.EmplastNm, e.EmpFirstNm";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email }, conn);
        return data;
    }

    public async Task<UsersModel?> _02UsersByLoginName(string loginname, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where LoginName = @LoginName";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = loginname }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<UsersModel?> _02UsersLoginEmail(string email, string password, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where e.Email = @Email and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email, Password = password }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<UsersModel?> _02UsersLoginLoginName(string loginName, string password, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e 
                         where e.LoginName = @LoginName and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = loginName, Password = password }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<UsersModel?> _03Users(int id, UsersModel user, string schema = "Main", string conn = "MySqlConn")
    {
        string sql = $@"Update {schema}.users set 
                            LoginName   = @LoginName, 
                            Email       = @Email, 
                            Domain      = @Domain, 
                            UserType    = @UserType, 
                            Status      = @Status, 
                            DefaultCoId = @DefaultCoId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, user, conn);

        sql = $@" select  * from {schema}.Users e where e.Id = @Id ;";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }
    public async Task _04Users(int id, string schema = "Main", string conn = "MySqlConn")
    {
        string msql = @" Delete from " + schema + @".Users where Id = @Id; ";
        await _sql.ExecuteCmd<dynamic>(msql, new { Id = id }, conn);
    }

    // --- Company Assigned to user --------------------------------------------------------------------------------------------------------------
    public async Task<List<CompaniesAssignedToUserModel?>> _02CompaniesAssignedToUser(int empmasId, string schema, string conn)
    {
        string sql = $@"SELECT uc.*, cu.*, t.Name UserType, t.IsVisible UTypeVisible  FROM {schema}.companyusers cu
                            left join main.userscompany uc on uc.id = cu.UserId
                            left join main.CompanyUserType t on t.id = cu.CompanyUserTypeId
                         where cu.UserId = @UserId";
        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = empmasId }, conn);
        return data;
    }

    public async Task<List<CompaniesAssignedToUserModel?>> _02UserEmploymentList(int empmasId, string schema, string conn) // --- Company Not Owned by the user ----------------
    {
        string sql = $@"SELECT uc.*, cu.*, t.Name UserType, t.IsVisible UTypeVisible  FROM {schema}.companyusers cu
                            left join main.userscompany uc on uc.id = cu.UserId
                            left join main.CompanyUserType t on t.id = cu.CompanyUserTypeId
                         where cu.UserId = @UserId and cu.UserId != uc.OwnerId";
        var data = await _sql.FetchData<CompaniesAssignedToUserModel?, dynamic>(sql, new { UserId = empmasId }, conn);
        return data;
    }

    // --- User Company Access  --------------------------------------------------------------------------------------------------------------
    public async Task<List<Uc_accessreqModel?>> _01Uc_Access(Uc_accessreqModel uca, string schema, string conn)
    {

        var query = $@"SELECT u.*, 
                            concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail
                        FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas u2 on u2.Id = u.Empmasid 
                            left join main.userscompany uc on uc.Id = u.Ucrequestingid 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                        where u.EmpmasId = @Empmasid and u.UCRequestingId = @Ucrequestingid and u.IsAddressed = 0";

        var res = await _sql.FetchData<Uc_accessreqModel?, dynamic>(query, uca, conn);

        if (res?.Count > 0) return res;

        string sql = $@"Insert into {schema}.Uc_accessreq 
                            (EmpmasId,  RequestedById,  UCRequestingId,  DateRequested, Isaddressed) values 
                            (@EmpmasId, @RequestedById, @UCRequestingId, now(),         0) ; 
                         SELECT u.*, 
                            concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail
                        FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas u2 on u2.Id = u.EmpmasId 
                            left join main.userscompany uc on uc.Id = u.UCRequestingId 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                        WHERE u.Id = (SELECT @@IDENTITY)";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, uca, conn);

        return data;
    }

    public async Task<List<Uc_accessreqModel?>> _02Uc_Access_NotAddressed(int empmasId, string schema, string conn)
    {
        string sql = $@"SELECT u.*, 
                            concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail 
                      FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas u2 on u2.Id = u.EmpmasId 
                            left join main.userscompany uc on uc.Id = u.UCRequestingId 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                      where u.EmpmasId = @EmpmasId and u.Isaddressed = 0 ";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;

    }

    public async Task<List<Uc_accessreqModel?>> _02Uc_Access_ByEmpmasId(int id, string schema, string conn)
    {
        string sql = $@"SELECT u.*, 
                            concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail 
                      FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas u2 on u2.Id = u.EmpmasId 
                            left join main.userscompany uc on uc.Id = u.UCRequestingId 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                      where u.Id = @Id";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, new { Id = id }, conn);
        return data;

    }

    public async Task<List<Uc_accessreqModel?>> _02Uc_Accessreq_ById(int id, string schema, string conn)
    {
        string sql = $@"SELECT u.*, 
                           concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail   
                      FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas    u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas    u2 on u2.Id = u.EmpmasId 
                            left join main.userscompany uc on uc.Id = u.UCRequestingId 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                      where u.Id = @Id  ";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }

    public async Task<List<Uc_accessreqModel?>> _02Uc_Access_ProfileAccessRequest(int empmasId, string schema, string conn)
    {
        string sql = $@"SELECT u.*, 
                            concat(trim(ifnull(u1.EmpLastNm,'')), ', ',trim(ifnull(u1.EmpFirstNm,'')), ' ', trim(ifnull(u1.EmpMidNm,'')) )  RequestorName,
                            trim(ifnull(u1.EmpFirstNm,''))  RequestorFirstName,
                            concat(trim(ifnull(u2.EmpLastNm,'')), ', ',trim(ifnull(u2.EmpFirstNm,'')), ' ', trim(ifnull(u2.EmpMidNm,'')) )  EmpmasName,
                            trim(ifnull(u2.EmpFirstNm,''))  EmpmasFirstName,
                            uc.CompanyName, u3.Email  EmpmasEmail, u4.Email RequestorEmail   
                      FROM {schema}.uc_accessreq u
                            left join mainpis.Empmas u1 on u1.Id = u.RequestedbyId 
                            left join mainpis.Empmas u2 on u2.Id = u.EmpmasId 
                            left join main.userscompany uc on uc.Id = u.UCRequestingId 
                            left join main.users u3 on u3.Id = u.Empmasid
                            left join main.users u4 on u4.Id = u.RequestedbyId
                      where u.EmpmasId = @EmpmasId and u.Isaddressed = 0 ";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;

    }

    public async Task<Uc_accessreqModel?> _03Uc_Access_Approve(Uc_accessreqModel uc_accessreq, string schema, string conn)
    {
        var sql = $@"Update {schema}.Uc_accessreq set 
                            DateApproved    = now(), 
                            Allowed         = @Allowed, 
                            AInfo           = @Ainfo, 
                            APersonalData   = @ApersonalData, 
                            AAddress        = @Aaddress, 
                            AEducaion       = @Aeducaion, 
                            AFamily         = @Afamily, 
                            AReferences     = @Areferences, 
                            AEmployment     = @Aemployment, 
                            ATrainings      = @Atrainings, 
                            Isaddressed     = 1 
                     where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, uc_accessreq, conn);

        var id = uc_accessreq.Id;
        var data = await _02Uc_Access_ByEmpmasId(id, schema, conn);
       
        return data?.FirstOrDefault();
    }

    public async Task<List<Uc_accessreqModel?>?> _03Uc_Access_Deny(Uc_accessreqModel uc_accessreq, string schema, string conn)
    {
        var sql = $@"Update {schema}.Uc_accessreq set 
                            Isaddressed     = 1 
                     where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, uc_accessreq, conn);

        var data = await _02Uc_Access_ProfileAccessRequest(uc_accessreq.Empmasid, schema, conn) ?? null;
        return data;
    }

    public async Task<Uc_accessreqModel?> _04Uc_Access_Request(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Uc_accessreq where Id = @id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Uc_accessreq x where x.EmpmasId = @empmasId ;";
        var data = await _sql.FetchData<Uc_accessreqModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    //--- System User ******************************************************
    public async Task<List<SystemuserModel?>?> _02SystemUsers(int systemId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.User where SystemId = @SystemId";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data;
    }

    // --- Company Assigned to user --------------------------------------------------------------------------------------------------------------
    public async Task<List<AtttypeModel?>> _02AttTypes(string schema, string conn)
    {
        var sql = $@"SELECT * FROM {schema}.Atttype order by Id";
        var data = await _sql.FetchData<AtttypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<AttdutytypeModel?>> _02AttDutyTypes(string schema, string conn)
    {
        var sql = $@"SELECT * FROM {schema}.AttDutytype";
        var data = await _sql.FetchData<AttdutytypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }


  

}
