using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._90_Utils;
namespace HRApiLibrary.DataAccess._20_Pay.OPay;

using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.Models._20_Pay.OPay;
using Microsoft.AspNetCore.Http.Extensions;
using System.Linq;
public class OPayrollgrpDataAccess : IOPayrollgrpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPayrollgrpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrollgrpModel?> _01(PayrollgrpModel payrollgrp, string schema, string conn)
    {
        var sql = $@"Insert into {schema}.Payrollgrp (Code, ClNumber,  Name, MinDailyRate, RatePerHr,  RatePerDay,  RatePerMonth,  RatePerYr,  Status) values 
                                                        (@Code, @ClNumber, @Name, @MinDailyRate, @RatePerHr, @RatePerDay, @RatePerMonth, @RatePerYr, @Status); 
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
        string sql = $@"select  Id, Code, ClNumber, Name, RatePerHr, RatePerDay, RatePerMonth, RatePerYr, MinDailyRate, Status, PayRateId from {schema}.Payrollgrp where Id = @Id";
        var data = await _sql.FetchData<PayrollgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PayrollgrpModel>?> _02(string schemapay, string conn)
    {
        string sql = $@" SELECT p.* FROM {schemapay}.Payrollgrp p  ORDER BY Name";
        var data = await _sql.FetchData<PayrollgrpModel, dynamic>( sql,new { }, conn );
        return data ?? new List<PayrollgrpModel>();
    }


    public async Task<GridResultModel<PayrollgrpModel>> _02Grid( GridRequestModel request, string schemapay,  string schemapis, string conn)
    {
        var columns = new Dictionary<string, string>
        {
            ["Code"] = "p.Code",
            ["Name"] = "p.Name",
            ["ClNumber"] = "p.ClNumber",
            ["RatePerHr"] = "p.RatePerHr",
            ["RatePerDay"] = "p.RatePerDay",
            ["RatePerMonth"] = "p.RatePerMonth",
            ["RatePerYr"] = "p.RatePerYr",
            ["MinDailyRate"] = "p.MinDailyRate",
            ["Status"] = "p.Status",
            ["Deployment"] = "c.Clname"
        };

        // SORTING
        var sortColumn = columns.GetValueOrDefault(
            request.SortField,
            "p.Name");

        var sortOrder =
            request.SortDirection == "DESC"
                ? "DESC"
                : "ASC";

        // PARAMETERS
        var parameters = new Dictionary<string, object>
        {
            ["PageSize"] = request.PageSize,
            ["Offset"] = request.Offset
        };

        // FILTERING
        var where = GridHelperDataAccess.BuildWhere( request.Filters,  columns,    parameters);

        // RECORDS COUNT
        string countSql = $@" SELECT COUNT(*)  FROM {schemapay}.Payrollgrp p LEFT JOIN {schemapis}.client c  ON c.clnumber = p.clnumber {where}";

        var totalResult = await _sql.FetchData<int, dynamic>(  countSql, parameters, conn);
        var total       = totalResult?.FirstOrDefault() ?? 0;

        // DATA
        string sql = $@"  SELECT  p.*, c.Clname AS Deployment FROM {schemapay}.Payrollgrp p   LEFT JOIN {schemapis}.client c   ON c.clnumber = p.clnumber {where}  ORDER BY {sortColumn} {sortOrder}  LIMIT @Offset, @PageSize";

        var data =   await _sql.FetchData<PayrollgrpModel, dynamic>(    sql,   parameters,  conn);

        return new GridResultModel<PayrollgrpModel>
        {
            Data = data ?? new List<PayrollgrpModel>(),
            Total = total
        };
    }

    public async Task<List<PayrollgrpModel>?> _02ByName(string name,  string schema, string conn)
    {
        string sql = $@" SELECT Id, Code, ClNumber, Name, RatePerHr, RatePerDay, RatePerMonth, RatePerYr, MinDailyRate, Status, PayRateId FROM {schema}.Payrollgrp  WHERE UPPER(TRIM(Name)) = UPPER(TRIM(@Name)) LIMIT 1";
        var data = await _sql.FetchData<PayrollgrpModel, dynamic>(sql, new { Name = name}, conn);
        return data ?? new List<PayrollgrpModel>();
    }

    public async Task<bool> _02CheckToTblTran( string? code,string? schema, string? conn)
    {
        string sql = $@" SELECT 1 FROM {schema}.tbltran WHERE RIGHT(trn, 5) = @Code LIMIT 1";

        var data = await _sql.FetchData<int, dynamic>( sql, new { Code = code }, conn);
        return data?.Any() == true;
    }


    public async Task<List<ODeprecModel?>?> _02CheckToDeprec(int? payrollgrpId, string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.deprec where payrollgrpId = @PayrollGrpId limit 1";
        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { PayrollGrpId = payrollgrpId }, conn);
        return data;
    }

    public async Task<PayrollgrpModel?> _03(int? id, PayrollgrpModel payrollgrp, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrollgrp set  ClNumber = @ClNumber, Name = @Name, RatePerHr = @RatePerHr, RatePerDay = @RatePerDay, RatePerMonth = @RatePerMonth, RatePerYr = @RatePerYr, MinDailyRate = @MinDailyRate, Status = @Status, PayRateId = @PayRateId where Id = @Id;";
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
    Task<PayrollgrpModel?>                  _01(PayrollgrpModel payrollgrp, string schema, string conn);
    Task<PayrollgrpModel?>                  _02(int id, string schema, string conn);
    Task<List<PayrollgrpModel>?>            _02(string schemapay, string conn);
    Task<List<PayrollgrpModel>?>            _02ByName(string name, string schema, string conn);
    Task<bool>                              _02CheckToTblTran(string? code, string? schema, string? conn);
    Task<List<ODeprecModel?>?>              _02CheckToDeprec(int? payrollgrpId, string? schema, string? conn);
    Task<GridResultModel<PayrollgrpModel>> _02Grid(GridRequestModel request, string schemapay, string schemapis, string conn);
    Task<PayrollgrpModel?>                  _03(int? id, PayrollgrpModel payrollgrp, string schema, string conn);
    Task<PayrollgrpModel?>                  _04(int? id, string schema, string conn);
}