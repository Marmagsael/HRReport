using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00CompanyusersDataAccess : I_00CompanyusersDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00CompanyusersDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<CompanyUsersModel?> _01(CompanyUsersModel companyusers, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Companyusers 
                        (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById) values 
                        (@UserId, @CompanyId, @Status, @DateInvited, @DateAccepted, @CompanyUserTypeId, @InvitedById) 
                        on duplicate key update Status = 'A', DateAccepted = @DateAccepted, CompanyUserTypeId = 3,  
                                                InvitedById = @InvitedById;  
                        SELECT * FROM {schema}.Companyusers WHERE UserId = @UserId and CompanyId = @CompanyId; ";
        var res = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, companyusers, conn);

        return res.FirstOrDefault();
    }


    public async Task<CompanyUsersModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Companyusers where Id = @Id";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<CompanyUsersModel?>?> _02ByUserId(int userId, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"select  c.*
                          , uc.CompanySName, uc.CompanyName, c1.Name CountryName 
                  from {schema}.Companyusers c 
                  left join {schema}.UsersCompany uc on uc.Id = c.CompanyId 
                  left join {schema}.Country      c1 on c1.Id = uc.CountryId
                  where c.UserId = @UserId and c.DateAccepted > '2020-01-01' and c.Status = 'A'";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = userId }, conn);
        return data;
    }
    public async Task<List<CompanyUsersModel?>?> _02ByUseridCompanyid(int userId, int companyId, string schema="Main", string conn="MySqlConn")
    {
        string sql = $@"select  c.*
                                , uc.CompanySName, uc.CompanyName, c1.Name CountryName 
                        from {schema}.Companyusers c 
                        left join {schema}.UsersCompany uc on uc.Id = c.CompanyId 
                        left join {schema}.Country      c1 on c1.Id = uc.CountryId
                        where c.UserId = @UserId and CompanyId = @CompanyId; ";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, conn);
        return data;
    }
    


    public async Task<CompanyUsersModel?> _03(int id, CompanyUsersModel companyusers, string schema, string conn)
    {
        string sql = $@"Update {schema}.Companyusers set 
                            UserId          = @UserId,  
                            CompanyId       = @CompanyId,  
                            Status          = @Status,  
                            DateInvited     = @DateInvited,  
                            DateAccepted    = @DateAccepted,  
                            CompanyUserTypeId = @CompanyUserTypeId, 
                            InvitedById         = @InvitedById where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, companyusers, conn);

        sql = $@" select  * from {schema}.Companyusers x where x.Id = @Id ;";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<CompanyUsersModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Companyusers where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Companyusers x where x.Id = @Id ;";
        var data = await _sql.FetchData<CompanyUsersModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
