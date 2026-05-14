using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;


namespace HRApiLibrary.DataAccess._20_Pay;

public class Fixedearnings_grp_empDataAccess : IFixedearnings_grp_empDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Fixedearnings_grp_empDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Fixedearnings_grp_empModel?> _01(Fixedearnings_grp_empModel fixedearnings_grp_emp, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Fixedearnings_grp_emp 
                            (FixedEarnings_grpId, EmpmasId) values 
                            (@FixedEarnings_grpId, @EmpmasId) 
                        on duplicate key update EmpmasId = @EmpmasId";
        await _sql.ExecuteCmd<dynamic>(sql, fixedearnings_grp_emp, conn);

        sql = $@"SELECT * FROM {schema}.Fixedearnings_grp_emp 
                 WHERE FixedEarnings_grpId=@FixedEarnings_grpId and EmpmasId=@EmpmasId";
        var res = await _sql.FetchData<Fixedearnings_grp_empModel?, dynamic>(sql, fixedearnings_grp_emp, conn);
        return res.FirstOrDefault();
        
    }


    public async Task<List<Fixedearnings_grp_empModel?>?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  FixedEarnings_grpId, EmpmasId from {schema}.Fixedearnings_grp_emp where FixedEarnings_grpId = @Id";
        var data = await _sql.FetchData<Fixedearnings_grp_empModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }



    public async Task<Fixedearnings_grp_empModel?> _04_ByFixedEarnings_grpId(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Fixedearnings_grp_emp where FixedEarnings_grpId = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Fixedearnings_grp_emp x where x.Id = @Id ;";
        var data = await _sql.FetchData<Fixedearnings_grp_empModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<Fixedearnings_grp_empModel?> _04_PerEmployee(int fixedEarnings_grpId, int empmasId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Fixedearnings_grp_emp where  FixedEarnings_grpId=@FixedEarnings_grpId and EmpmasId = @EmpmasId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { FixedEarnings_grpId= fixedEarnings_grpId, EmpmasId=empmasId }, conn);

        sql = $@" select  * from {schema}.Fixedearnings_grp_emp where FixedEarnings_grpId=@FixedEarnings_grpId and EmpmasId = @EmpmasId ;";
        var data = await _sql.FetchData<Fixedearnings_grp_empModel?, 
                    dynamic>(sql, new { FixedEarnings_grpId= fixedEarnings_grpId, EmpmasId=empmasId }, conn);
        return data?.FirstOrDefault();
    }
    
}