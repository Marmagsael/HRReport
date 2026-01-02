using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavegrpDataAccess : ILeavegrpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavegrpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavegrpModel?> _01(LeavegrpModel leavegrp, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leavegrp (Name) values (@Name); 
                        SELECT * FROM {schema}.Leavegrp WHERE ID = (SELECT @@IDENTITY);";
        var res = await _sql.FetchData<LeavegrpModel?, dynamic>(sql, leavegrp, conn);

        return res.FirstOrDefault();
    }


    public async Task<LeavegrpModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name from {schema}.Leavegrp where Id = @Id";
        var data = await _sql.FetchData<LeavegrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<LeavegrpModel?>?> _02(string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.Leavegrp order by Name";
        var data    = await _sql.FetchData<LeavegrpModel?, dynamic>(sql, new { }, conn);
        return data;
    }



    public async Task<LeavegrpModel?> _03(int id, LeavegrpModel leavegrp, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavegrp set Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavegrp, conn);

        sql = $@" select  * from {schema}.Leavegrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavegrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavegrpModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavegrp where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavegrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavegrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
