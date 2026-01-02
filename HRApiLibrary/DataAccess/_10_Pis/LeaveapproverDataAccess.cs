using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeaveapproverDataAccess : ILeaveapproverDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeaveapproverDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeaveapproverModel?> _01(LeaveapproverModel leaveapprover, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leaveapprover 
                            (EmpmasId,  ApproverId,  ApproverLevel) values 
                            (@EmpmasId, @ApproverId, @ApproverLevel); 
                        SELECT * FROM {schema}.Leaveapprover WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<LeaveapproverModel?, dynamic>(sql, leaveapprover, conn);

        return res.FirstOrDefault();
    }


    public async Task<LeaveapproverModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, EmpmasId, ApproverId, ApproverLevel from {schema}.Leaveapprover where Id = @Id";
        var data = await _sql.FetchData<LeaveapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<LeaveapproverModel?>?> _02ByLvl(int lvl, string schema, string conn)
    {
        string sql = $@"select l.*,  
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) Fullname
                        from {schema}.Leaveapprover l 
                        left join {schema}.empmas e on e.Id = l.EmpmasId 
                        where l.ApproverLevel = @Lvl 
                        order by e.EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<LeaveapproverModel?, dynamic>(sql, new { Lvl = lvl }, conn);
        return data;
    }


    public async Task<LeaveapproverModel?> _03(int id, LeaveapproverModel leaveapprover, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leaveapprover set 
                            EmpmasId        = @EmpmasId, 
                            ApproverId      = @ApproverId, 
                            ApproverLevel   = @ApproverLevel where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leaveapprover, conn);

        sql = $@" select  * from {schema}.Leaveapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeaveapproverModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leaveapprover where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leaveapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeaveapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}