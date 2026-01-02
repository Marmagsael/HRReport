using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class EmpmovementDataAccess : IEmpmovementDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpmovementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmpmovementModel?> _01(EmpmovementModel empmovement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmovement 
                            (EmpmasId,  Date,  RefNo,  Mode,  Dtls,  UserId,  Created) values 
                            (@EmpmasId, @Date, @RefNo, @Mode, @Dtls, @UserId, @Created); 
                        SELECT * FROM {schema}.Empmovement WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, empmovement, conn);
        return res.FirstOrDefault();
    }


    public async Task<EmpmovementModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmovement where Id = @Id";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmovementModel?> _02ByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmovement where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmovementModel?> _02ByRefno(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmovement where Refno = @EmpmasId";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmovementModel?>?> _02ListByEmpmasId(int empmasId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmovement where EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }



    public async Task<EmpmovementModel?> _03(int id, EmpmovementModel empmovement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Empmovement set 
                            EmpmasId = @EmpmasId,  
                            Date    = @Date,  
                            RefNo   = @RefNo,  
                            Mode    = @Mode,  
                            Dtls    = @Dtls,  
                            UserId  = @UserId,  
                            Created = @Created where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmovement, conn);

        sql = $@" select  * from {schema}.Empmovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmovementModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmovement where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}