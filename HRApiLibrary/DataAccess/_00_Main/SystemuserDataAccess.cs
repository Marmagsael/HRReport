using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class SystemuserDataAccess : ISystemuserDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public SystemuserDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<SystemuserModel?> _01(SystemuserModel user, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.SystemUser 
                            (SystemId,  Status,  DateInvited,  DateAccepted,  IsApproved) values 
                            (@SystemId, @Status, @DateInvited, @DateAccepted, @IsApproved) 
                            on duplicate key update status = status ; 
                        SELECT * FROM {schema}.SystemUser WHERE SystemId = @SystemId; ";
        var res = await _sql.FetchData<SystemuserModel?, dynamic>(sql, user, conn);

        return res.FirstOrDefault();
    }


    public async Task<SystemuserModel?> _02(int systemId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.SystemUser where SystemId = @SystemId";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<SystemuserModel?>?> _02Lst(int systemId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.SystemUser where SystemId = @SystemId";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data;
    }
    
    public async Task<List<SystemuserModel?>?> _02Lst_WName(int systemId, string schemaModule, string schemaPis, string conn)
    {
        string sql = $@"select  u.*, 
                            concat(trim(ifnull(e.emplastnm,'')),', ',trim(ifnull(e.empfirstnm,'')),' ',trim(ifnull(e.empmidnm,''))) UserName
                        from {schemaModule}.SystemUser u 
                        left join {schemaPis}.Empmas e on e.SystemId = u.SystemId 
                        where u.SystemId = @SystemId";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data;
    }
    
    public async Task<List<SystemuserModel?>?> _02ByStatus(string status, string schema, string conn)
    {
        string sql = $@"SELECT u.*, 
                            concat(trim(ifnull(e.emplastnm,'')),', ',trim(ifnull(e.empfirstnm,'')),' ',trim(ifnull(e.empmidnm,''))) UserName
                            FROM {schema}.Systemuser u
                            left join MainPis.Empmas e on e.Id = u.systemId 
                        where Status = @Status;";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { Status = status }, conn);
        return data;
    }
    

    public async Task<List<SystemuserModel?>?> _02List(int systemId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.SystemUser where SystemId = @SystemId";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data;
    }


    public async Task<SystemuserModel?> _03(int systemId, SystemuserModel user, string schema, string conn)
    {
        string sql = $@"Update {schema}.SystemUser set 
                            SystemId        = @SystemId, 
                            Status          = @Status,  
                            DateInvited     = @DateInvited,  
                            DateAccepted    = @DateAccepted,  
                            IsApproved      = @IsApproved where SystemId = @SystemId;";
        await _sql.ExecuteCmd<dynamic>(sql, user, conn);

        sql = $@" select  * from {schema}.SystemUser x where x.SystemId = @SystemId ;";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<SystemuserModel?> _04(int systemId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.SystemUser where SystemId = @SystemId;
                        select  * from {schema}.SystemUser x where x.Id = @Id ;";
        var data = await _sql.FetchData<SystemuserModel?, dynamic>(sql, new { SystemId = systemId }, conn);
        return data?.FirstOrDefault();
    }

}