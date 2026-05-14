using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavedefaultapproverDataAccess : ILeavedefaultapproverDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavedefaultapproverDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavedefaultapproverModel?> _01(LeavedefaultapproverModel leavedefaultapprover, string schema, string conn)
    {
        string? cmd = $@"select * from {schema}.Leavedefaultapprover where Lvl = @Lvl and EmpmasId = @EmpmasId";
        var ouput   = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>
                                    (cmd, new { Lvl = leavedefaultapprover.Lvl, EmpmasId = leavedefaultapprover.EmpmasId }, conn);
        if (ouput?.Count < 1)
        {
            string sql = $@"Insert into {schema}.Leavedefaultapprover 
                            (Lvl,  EmpmasId,  Designation) values 
                            (@Lvl, @EmpmasId, @Designation)";
            await _sql.ExecuteCmd<dynamic>(sql, leavedefaultapprover, conn);

            sql = $@"SELECT * FROM {schema}.Leavedefaultapprover WHERE ID = (SELECT @@IDENTITY)";
            var res = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>(sql, new { }, conn);
            return res.FirstOrDefault();
        } else { return null; }

    }


    public async Task<LeavedefaultapproverModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Lvl, EmpmasId, Designation from {schema}.Leavedefaultapprover where Id = @Id";
        var data = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<LeavedefaultapproverModel?>?> _02ByLvl(int lvl, string schema, string conn)
    {
        string sql = $@"select l.Id, l.Lvl, l.EmpmasId, l.Designation,  
                            concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) Fullname
                        from {schema}.Leavedefaultapprover l 
                        left join {schema}.empmas e on e.Id = l.EmpmasId 
                        where Lvl = @Lvl 
                        order by e.EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>(sql, new { Lvl = lvl }, conn);
        return data;
    }
    

    public async Task<LeavedefaultapproverModel?> _03(int id, LeavedefaultapproverModel leavedefaultapprover, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavedefaultapprover set 
                            Lvl         = @Lvl,  
                            EmpmasId    = @EmpmasId,  
                            Designation = @Designation where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavedefaultapprover, conn);

        sql = $@" select  * from {schema}.Leavedefaultapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavedefaultapproverModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavedefaultapprover where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavedefaultapprover x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavedefaultapproverModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}