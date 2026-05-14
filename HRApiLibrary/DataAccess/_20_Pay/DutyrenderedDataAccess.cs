using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class DutyrenderedDataAccess : IDutyrenderedDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DutyrenderedDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DutyrenderedModel?> _01(DutyrenderedModel dutyrendered, string schema, string conn)
    {
        //Console.WriteLine($"AcctNumber: {dutyrendered.AcctNumber} * AcctName :  {dutyrendered.AcctName} * Conn : {conn} * Schema: {schema} ");
        var sql = $@"Insert into {schema}.Dutyrendered (AcctNumber, IsLock) values (@AcctNumber, @IsLock);
                     SELECT * FROM {schema}.Dutyrendered WHERE AcctNumber = @AcctNumber;";

        var res = await _sql.FetchData<DutyrenderedModel?, dynamic>(sql, dutyrendered, conn);
        return res.FirstOrDefault();
    }


    public async Task<DutyrenderedModel?> _02(string acctNumber, string schema, string conn)
    {
        var sql = $@"select  AcctNumber, IsLock from {schema}.Dutyrendered where AcctNumber = @AcctNumber";
        var data = await _sql.FetchData<DutyrenderedModel?, dynamic>(sql, new { AcctNumber = acctNumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<DutyrenderedModel?>?> _02s(string schema, string conn)
    {
        var sql = $@"select  d.AcctNumber, d.IsLock, c.AcctName from {schema}.Dutyrendered d 
                     left join {schema}.Coa c on c.AcctNumber = d.AcctNumber 
                     Order by AcctNumber; ";
        var data = await _sql.FetchData<DutyrenderedModel?, dynamic>(sql, new { }, conn);
        return data;
    }



    public async Task<DutyrenderedModel?> _03(DutyrenderedModel dutyrendered, string schema, string conn)
    {
        var sql = $@"Update {schema}.Dutyrendered set AcctNumber = @AcctNumber, IsLock = @IsLock where AcctNumber = @AcctNumber;
                     Select  * from {schema}.Dutyrendered x where AcctNumber = @AcctNumber ;";
        var data = await _sql.FetchData<DutyrenderedModel?, dynamic>(sql, dutyrendered, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DutyrenderedModel?> _04(string acctNumber, string schema, string conn)
    {
        var sql = $@"Delete from {schema}.Dutyrendered where AcctNumber = @AcctNumber;
                     Select  * from {schema}.Dutyrendered where AcctNumber = @AcctNumber  ;";
        var data = await _sql.FetchData<DutyrenderedModel?, dynamic>(sql, new { AcctNumber = acctNumber }, conn);
        return data?.FirstOrDefault();
    }
}