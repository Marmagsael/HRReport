using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

namespace HRApiLibrary.DataAccess._11_AMS;

public class AttscheddefaultDataAccess : IAttscheddefaultDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttscheddefaultDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttscheddefaultModel?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attscheddefault where Id = 1";
        var data = await _sql.FetchData<AttscheddefaultModel?, dynamic>(sql, new { }, conn);

        if (!(data ?? []).Any())
        {
            sql = $@"Insert into {schema}.Attscheddefault 
                        (Id, 
                         D1_In, D1_HrsLength, D1_DutyType, 
                         D2_In, D2_HrsLength, D2_DutyType, 
                         D3_In, D3_HrsLength, D3_DutyType, 
                         D4_In, D4_HrsLength, D4_DutyType,  
                         D5_In, D5_HrsLength, D5_DutyType,  
                         D6_In, D6_HrsLength, D6_DutyType,  
                         D7_In, D7_HrsLength, D7_DutyType) values 
                        (1, 
                         0,     0,           'RD', 
                         800,   900,           'R', 
                         800,   900,           'R', 
                         800,   900,           'R', 
                         800,   900,           'R', 
                         800,   900,           'R', 
                         0,     0,           'RN')";
            await _sql.ExecuteCmd<dynamic>(sql, new { }, conn);

            sql = $@"select  * from {schema}.Attscheddefault where Id = 1";
            data = await _sql.FetchData<AttscheddefaultModel?, dynamic>(sql, new { }, conn);
        }

        return data?.FirstOrDefault();

    }


    public async Task<AttscheddefaultModel?> _03(AttscheddefaultModel attscheddefault, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attscheddefault set 
                            D1_In           = @D1_In, 
                            D1_HrsLength    = @D1_HrsLength,  
                            D1_DutyType     = @D1_DutyType,  
                            D2_In           = @D2_In,  
                            D2_HrsLength    = @D2_HrsLength,  
                            D2_DutyType     = @D2_DutyType,  
                            D3_In           = @D3_In,  
                            D3_HrsLength    = @D3_HrsLength,  
                            D3_DutyType     = @D3_DutyType, 
                            D4_In           = @D4_In, 
                            D4_HrsLength    = @D4_HrsLength, 
                            D4_DutyType     = @D4_DutyType, 
                            D5_In           = @D5_In, 
                            D5_HrsLength    = @D5_HrsLength, 
                            D5_DutyType     = @D5_DutyType, 
                            D6_In           = @D6_In, 
                            D6_HrsLength    = @D6_HrsLength, 
                            D6_DutyType     = @D6_DutyType, 
                            D7_In           = @D7_In, 
                            D7_HrsLength    = @D7_HrsLength, 
                            D7_DutyType     = @D7_DutyType where Id = 1;";
        await _sql.ExecuteCmd<dynamic>(sql, attscheddefault, conn);

        sql = $@" select  * from {schema}.Attscheddefault x where x.Id = 1 ;";
        var data = await _sql.FetchData<AttscheddefaultModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }


}




public interface IAttscheddefaultDataAccess
{
    Task<AttscheddefaultModel?> _02(string schema, string conn);
    Task<AttscheddefaultModel?> _03(AttscheddefaultModel attscheddefault, string schema, string conn);
}