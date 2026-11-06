using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OEmpstatMgtDataAccess : IOEmpstatMgtDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OEmpstatMgtDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<OEmpstatModel?>?> _02StatMgtStatuses(string? schema, string? conn)
    {
        string sql = $@"select * from {schema}.empstat where code in (select code from {schema}.empstat_statmgt)";
        return await _sql.FetchLegacy<OEmpstatModel?, dynamic>(sql, new { }, conn);
    }

    public async Task<string?> _02Remarks(string? empnumber, string? schema, string? conn)
    {
        string sql = $@"select Remarks from {schema}.statusremarks where empnumber = @EmpNumber";
        var res = await _sql.FetchLegacy<string?, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return res?.FirstOrDefault();
    }

    public async Task _01UpsertRemarks(string? empnumber, string? remarks, string? schema, string? conn)
    {
        string sql = $@"select count(*) from {schema}.statusremarks where empnumber = @EmpNumber";
        var res = await _sql.FetchLegacy<int, dynamic>(sql, new { EmpNumber = empnumber }, conn);

        sql = res != null && res.FirstOrDefault() > 0
            ? $@"update {schema}.statusremarks set Remarks = @Remarks where empnumber = @EmpNumber"
            : $@"insert into {schema}.statusremarks (EmpNumber, Remarks) values (@EmpNumber, @Remarks)";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new { EmpNumber = empnumber, Remarks = remarks }, conn);
    }

    public async Task _03ChangeStatus(string? empnumber, string? newStatus, DateTime? separate, bool setSeparate, bool setRemarks, string? remarks, string? schema, string? conn)
    {
        string sepSet = (setSeparate ? ", separate = @Separate" : string.Empty) + (setRemarks ? ", remarks = @Remarks" : string.Empty);

        string sql = $@"update {schema}.empmas set empstat_ = @NewStatus,
                               client_ = '', movmode = '', movnumber = '', movdate = NULL{sepSet}
                        where empnumber = @EmpNumber;";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new { EmpNumber = empnumber, NewStatus = newStatus, Separate = separate, Remarks = remarks }, conn);
    }

    public async Task _01StatusMovement(string? empnumber, string? user, string? statFrom, string? statTo, string? module, string? schema, string? conn)
    {
        string sql = $@"insert into {schema}.empstatmovement (empnumber, user, stat_from, stat_to, datemove, module)
                        values (@EmpNumber, @User, @StatFrom, @StatTo, now(), @Module)";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new { EmpNumber = empnumber, User = user, StatFrom = statFrom, StatTo = statTo, Module = module }, conn);
    }
}

public interface IOEmpstatMgtDataAccess
{
    Task<List<OEmpstatModel?>?> _02StatMgtStatuses(string? schema, string? conn);
    Task<string?> _02Remarks(string? empnumber, string? schema, string? conn);
    Task _01UpsertRemarks(string? empnumber, string? remarks, string? schema, string? conn);
    Task _03ChangeStatus(string? empnumber, string? newStatus, DateTime? separate, bool setSeparate, bool setRemarks, string? remarks, string? schema, string? conn);
    Task _01StatusMovement(string? empnumber, string? user, string? statFrom, string? statTo, string? module, string? schema, string? conn);
}
