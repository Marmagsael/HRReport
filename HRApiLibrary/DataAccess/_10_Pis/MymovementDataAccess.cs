using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class MymovementDataAccess : IMymovementDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MymovementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MymovementModel?> _01(MymovementModel mymovement, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Mymovement 
                            (Date,  CompanyId,  RefNo,  Mode,  Dtls,  Created) values 
                            (@Date, @CompanyId, @RefNo, @Mode, @Dtls, @Created); 
                        SELECT * FROM {schema}.Mymovement WHERE ID = (SELECT @@IDENTITY);";
        var res = await _sql.FetchData<MymovementModel?, dynamic>(sql, mymovement, conn);
        return res.FirstOrDefault();
    }


    public async Task<MymovementModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Mymovement where Id = @Id";
        var data = await _sql.FetchData<MymovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<MymovementModel?> _03(int id, MymovementModel mymovement, string schema, string conn)
    {
        string sql = $@"Update {schema}.Mymovement set 
                            Date        = @Date, 
                            CompanyId   = @CompanyId,  
                            RefNo       = @RefNo,  
                            Mode        = @Mode,  
                            Dtls        = @Dtls,  
                            Created     = @Created 
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, mymovement, conn);
        sql = $@" select  * from {schema}.Mymovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<MymovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MymovementModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Mymovement where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Mymovement x where x.Id = @Id ;";
        var data = await _sql.FetchData<MymovementModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
