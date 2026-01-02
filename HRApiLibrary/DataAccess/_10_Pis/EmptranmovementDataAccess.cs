using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;


namespace HRApiLibrary.DataAccess._10_Pis;

public class EmptranmovementDataAccess : IEmptranmovementDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmptranmovementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmptranmovementModel?> _01(EmptranmovementModel emptranmovement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Emptranmovement (id, EmpmasId, MovDate, MovNumber, UserId, DateRecorded, TranStart, TranEnd, Remarks, EmpStatusId) values (@id, @EmpmasId, @MovDate, @MovNumber, @UserId, @DateRecorded,@TranStart, @TranEnd, @Remarks, @EmpStatusId)";
        await _sql.ExecuteCmd<dynamic>(sql, emptranmovement, conn);

        sql = $@"SELECT * FROM {schema}.Emptranmovement WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<EmptranmovementModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  id, EmpmasId, MovDate, MovNumber, TranStart, TranEnd, Remarks, EmpStatusId from {schema}.Emptranmovement where Id = @Id";
        var data = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmptranmovementModel?> _02ByMovNo(string movNo, string schema, string conn)
    {
        string sql = $@"select  id, EmpmasId, MovDate, MovNumber, TranStart, TranEnd, Remarks, EmpStatusId from {schema}.Emptranmovement where MovNumber = @MovNumber";
        var data = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { MovNumber = movNo }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmptranmovementModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  e.*, s.name EmpStatus from {schema}.Emptranmovement e
                      left join {schema}.rempstat s on s.Id = e.EmpStatusId where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);

        return data?.LastOrDefault();
    }


    public async Task<EmptranmovementModel?> _03(int id, EmptranmovementModel emptranmovement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Emptranmovement set id = @id, EmpmasId = @EmpmasId, MovDate = @MovDate, MovNumber = @MovNumber, UserId =@UserId, DateRecorded = @DateRecorded, TranStart = @TranStart, TranEnd = @TranEnd, Remarks = @Remarks, EmpStatusId = @EmpStatusId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, emptranmovement, conn);

        sql = $@" select  * from {schema}.Emptranmovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmptranmovementModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Emptranmovement where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Emptranmovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmptranmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
