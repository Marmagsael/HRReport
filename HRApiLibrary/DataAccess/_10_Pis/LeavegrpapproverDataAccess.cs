using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavegrpapproverDataAccess : ILeavegrpapproverDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavegrpapproverDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavegrpapproverModel?> _01(LeavegrpapproverModel leavegrpapprover, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leavegrpapprover 
                            (LeaveGrpId, ApproverId, ApproverLevel) values 
                            (@LeaveGrpId, @ApproverId, @ApproverLevel); 
                        SELECT * FROM {schema}.Leavegrpapprover WHERE ID = (SELECT @@IDENTITY);";
        var res = await _sql.FetchData<LeavegrpapproverModel?, dynamic>(sql, leavegrpapprover, conn);

        return res.FirstOrDefault();
    }


    public async Task<LeavegrpapproverModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, LeaveGrpId, ApproverId, ApproverLevel from {schema}.Leavegrpapprover where Id = @Id";
        var data = await _sql.FetchData<LeavegrpapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<LeavegrpapproverModel?>?> _02ByLeavegrpIdApproverLevel(int leavegrpid, int approverlevel, string schema, string conn)
    {
        string sql = $@"select  l.Id, LeaveGrpId, ApproverId, ApproverLevel,  
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) ApproverName 
                        from {schema}.Leavegrpapprover l 
                            left join {schema}.Empmas   e on e.Id = l.ApproverId 
                            left join {schema}.Leavegrp g on g.Id = l.LeaveGrpId 
                        where l.LeaveGrpId = @Leavegrpid and l.ApproverLevel = @Approverlevel 
                        order by e.emplastnm, e.empfirstnm, e.empmidnm ";
        var data = await _sql.FetchData<LeavegrpapproverModel?, dynamic>(sql, new { LeaveGrpId = leavegrpid, ApproverLevel = approverlevel }, conn);
        return data;
    }


    public async Task<LeavegrpapproverModel?> _03(int id, LeavegrpapproverModel leavegrpapprover, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavegrpapprover set 
                                LeaveGrpId      = @LeaveGrpId, 
                                ApproverId      = @ApproverId,  
                                ApproverLevel   = @ApproverLevel where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavegrpapprover, conn);

        sql = $@" select  * from {schema}.Leavegrpapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavegrpapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavegrpapproverModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavegrpapprover where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavegrpapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavegrpapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
