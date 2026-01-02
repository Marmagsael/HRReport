using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis;

public class LeavetypeDataAccess : ILeavetypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public LeavetypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<LeavetypeModel?> _01(LeavetypeModel leavetype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Leavetype 
                            (Code, LeaveName, AnivStart, AnivEnd, DefValue) values 
                            (@Code, @LeaveName, @AnivStart, @AnivEnd, @DefValue)";
        await _sql.ExecuteCmd<dynamic>(sql, leavetype, conn);

        sql = $@"SELECT * FROM {schema}.Leavetype WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<LeavetypeModel?>?> _02(string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.Leavetype";
        var data    = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<LeavetypeModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, LeaveName, AnivStart, AnivEnd, DefValue 
                            from {schema}.Leavetype where Id = @Id";
        var data = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<LeavetypeModel?> _03(int id, LeavetypeModel leavetype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Leavetype set 
                            Code        = @Code, 
                            LeaveName   = @LeaveName, 
                            AnivStart   = @AnivStart, 
                            AnivEnd     = @AnivEnd, 
                            DefValue    = @DefValue 
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, leavetype, conn);

        sql = $@" select  * from {schema}.Leavetype x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<LeavetypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Leavetype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Leavetype x where x.Id = @Id ;";
        var data = await _sql.FetchData<LeavetypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
