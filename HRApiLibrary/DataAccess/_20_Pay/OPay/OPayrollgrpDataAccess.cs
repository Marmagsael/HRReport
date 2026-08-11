using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OPayrollgrpDataAccess : IOPayrollgrpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPayrollgrpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrollgrpModel?> _01(PayrollgrpModel payrollgrp, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Payrollgrp (Code, ClNumber,  Name, MinMoRate, RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr,  Status) values 
                                                        (@Code, @ClNumber, @Name, @MinMoRate, @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr, @Status); 
                        SELECT * FROM {schema}.Payrollgrp WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, payrollgrp, conn);

        var id = res.FirstOrDefault()?.Id;

        sql = $@"Update {schema}.Payrollgrp set  code = lpad(Id,5,'0') where Id = @Id ";
        await _sql.ExecuteCmd(sql, new { Id = id }, conn);

        sql = $@"SELECT * FROM {schema}.Payrollgrp where Id = @Id ";
        res = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);

        return res.FirstOrDefault();
    }


    public async Task<PayrollgrpModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, ClNumber, Name, RatePerHr, RatePerDay, RatePerMonth, RatePerYr, MinMoRate, Status, PayRateId from {schema}.Payrollgrp where Id = @Id";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PayrollgrpModel>?> _02(string schemapay, string schemapis, string conn)
    {
        string sql = $@" SELECT p.*, c.Clname Deployment
                        FROM {schemapay}.Payrollgrp p
                        LEFT JOIN {schemapis}.client c on c.clnumber = p.clnumber
                        ORDER BY Name";
        var data = await _sql.FetchData<PayrollgrpModel, dynamic>( sql,new { }, conn );
        return data ?? new List<PayrollgrpModel>();
    }

    public async Task<List<PayrollgrpModel>?> _02ByName(string name,  string schema, string conn)
    {
        string sql = $@" SELECT Id, Code, ClNumber, Name, RatePerHr, RatePerDay, RatePerMonth, RatePerYr, MinMoRate, Status, PayRateId FROM {schema}.Payrollgrp  WHERE UPPER(TRIM(Name)) = UPPER(TRIM(@Name)) LIMIT 1";
        var data = await _sql.FetchData<PayrollgrpModel, dynamic>(sql, new { Name = name}, conn);
        return data ?? new List<PayrollgrpModel>();
    }

    public async Task<List<TbltranModel?>?> _02CheckToTblTran(string? clNumber, string? schema, string? conn)
    {
        Console.WriteLine($"{clNumber} {schema} {conn}");
        string? sql = $@"select  * from {schema}.tbltran where right(trn,5) = @ClNumber limit 1 ";
        var data = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { ClNumber = clNumber }, conn);
        return data;
    }

    public async Task<List<DeprecModel?>?> _02CheckToDeprec(int? payrollgrpId, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.deprec where payrollgrpId = @PayrollGrpId limit 1";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { PayrollGrpId = payrollgrpId }, conn);
        return data;
    }

    public async Task<PayrollgrpModel?> _03(int? id, PayrollgrpModel payrollgrp, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrollgrp set  ClNumber = @ClNumber, Name = @Name, RatePerHr = @RatePerHr, RatePerDay = @RatePerDay, RatePerMonth = @RatePerMonth, RatePerYr = @RatePerYr, MinMoRate = @MinMoRate, Status = @Status, PayRateId = @PayRateId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, payrollgrp, conn);

        sql = $@" select  * from {schema}.Payrollgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PayrollgrpModel?> _04(int? id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Payrollgrp where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Payrollgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOPayrollgrpDataAccess
{
    Task<PayrollgrpModel?> _01(PayrollgrpModel payrollgrp, string schema, string conn);
    Task<PayrollgrpModel?> _02(int id, string schema, string conn);
    Task<List<PayrollgrpModel>?> _02(string schemapay, string schemapis, string conn);
    Task<List<PayrollgrpModel>?> _02ByName(string name, string schema, string conn);
    Task<List<TbltranModel?>?> _02CheckToTblTran(string? clNumber, string? schema, string? conn);
    Task<List<DeprecModel?>?> _02CheckToDeprec(int? payrollgrpId, string? schema, string? conn);
    Task<PayrollgrpModel?> _03(int? id, PayrollgrpModel payrollgrp, string schema, string conn);
    Task<PayrollgrpModel?> _04(int? id, string schema, string conn);
}