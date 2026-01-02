using HRApiLibrary.DataAccess._00_MainTrans.Interfaces;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;

namespace HRApiLibrary.DataAccess._00_MainTrans;

public class EngagementDataAccess : IEngagementDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EngagementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EngagementModel?> _01(EngagementModel engagement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Engagement 
						(OwnerId,  CompanyId,  Module,  RoleId,  DateInvited,  DateApproved,  Status) values 
						(@OwnerId, @CompanyId, @Module, @RoleId, @DateInvited, @DateApproved, @Status);
						SELECT * FROM {schema}.Engagement WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<EngagementModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }
    public async Task<EngagementModel?> _01Invite(EngagementModel engagement, string schema, string conn)
    {
	    // Console.WriteLine($"schema : {schema} * conn :{conn} * OwnerId : {engagement.OwnerId} " +
	    //                   $" * CompanyId : {engagement.CompanyId} * Module:  {engagement.Module}");
	    //
        string sql = $@"Insert into {schema}.Engagement
						(OwnerId,  CompanyId,  Module,  RoleId,  DateInvited,   Status) values 
						(@OwnerId, @CompanyId, @Module, 0,       now(), 		'FA') 
						on duplicate key update Module = Module;
						SELECT * FROM {schema}.Engagement WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<EngagementModel?, dynamic>(sql, engagement, conn);
        return res.FirstOrDefault();
    }


    public async Task<EngagementModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  e.* from {schema}.Engagement e 
							where e.Id = @Id";
        var data = await _sql.FetchData<EngagementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<EngagementModel?> _03(int id, EngagementModel engagement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Engagement set 
								OwnerId 		= @OwnerId, 
								CompanyId 		= @CompanyId, 
								Module 			= @Module, 
								RoleId 			= @RoleId, 
								DateInvited 	= @DateInvited, 
								DateApproved 	= @DateApproved, 
								Status 			= @Status where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, engagement, conn);

        sql = $@" select  * from {schema}.Engagement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EngagementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EngagementModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Engagement where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Engagement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EngagementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}