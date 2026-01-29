using System;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OtbltranDataAccess : IOtbltranDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OtbltranDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<GTbltranModel?> _01(GTbltranModel tbltran, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Tbltran 
                        (TRN,  acctNumber,  empNumber,  amount,  dTimeStamp,  source,  postedby) values 
                        (@TRN, @acctNumber, @empNumber, @amount, @dTimeStamp, @source, @postedby)";
        await _sql.ExecuteCmd<dynamic>(sql, tbltran, conn);
        sql = $@"SELECT * FROM {schema}.Tbltran WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<GTbltranModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<GTbltranModel?>?> _02ByTrnAndEmpnumber(string trn, string empnumber, string schema, string conn)
    {
        string sql = $@"select  t.TRN, t.AcctNumber, t.EmpNumber, t.Amount, t.DTimeStamp, t.Source, t.PostedBy, 
                                dt.nVal DayHr, dt.DayHrs,
                                if(right(left(dt.DtlCd,4),1) = '2', 'Day', 'Hr/s') as Uom 
                        from {schema}.Tbltran t 
                        left join {schema}.Tbltrandtl dt on t.trn = dt.trn and t.acctnumber = left(dt.dtlCd,4) and t.empnumber = dt.empnumber
                        where Trn = @Trn and Empnumber = @Empnumber ;";
        var data = await _sql.FetchData<GTbltranModel?, dynamic>(sql, new { Trn = trn, Empnumber = empnumber }, conn);
        return data;
    }

    public async Task<List<GTbltranModel?>?> _02Trns_ByEmpnumber(string empnumber, string schema, string conn)
    {
        string sql = $@"select distinct t.TRN from {schema}.Tbltran t where Empnumber = @Empnumber ;";
        var data = await _sql.FetchData<GTbltranModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data;
    }


    public async Task<GTbltranModel?> _03(GTbltranModel tbltran, string schema, string conn)
    {
        string sql = $@"Update {schema}.Tbltran set 
                            Amount      = @Amount, 
                            DTimeStamp  = @DTimeStamp, 
                            Source      = @Source, 
                            Postedby    = @Postedby 
                        where Trn = @Trn and AcctNumber and @AcctNumber and EmpNumber and @EmpNumber ;";
        await _sql.ExecuteCmd<dynamic>(sql, tbltran, conn);

        sql = $@" select  * from {schema}.Tbltran where Trn = @Trn and AcctNumber and @AcctNumber and EmpNumber and @EmpNumber ;";
        var data = await _sql.FetchData<GTbltranModel?, dynamic>(sql, tbltran, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(GTbltranModel tbltran, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Tbltran where  Trn = @Trn and AcctNumber and @AcctNumber and EmpNumber and @EmpNumber;";
        await _sql.ExecuteCmd<dynamic>(sql, tbltran, conn);

    }
}


public interface IOtbltranDataAccess
{
    Task<GTbltranModel?> _01(GTbltranModel tbltran, string schema, string conn);
    Task<List<GTbltranModel?>?> _02ByTrnAndEmpnumber(string trn, string empnumber, string schema, string conn);
    Task<List<GTbltranModel?>?> _02Trns_ByEmpnumber(string empnumber, string schema, string conn);
    Task<GTbltranModel?> _03(GTbltranModel tbltran, string schema, string conn);
    Task _04(GTbltranModel tbltran, string schema, string conn);
}
