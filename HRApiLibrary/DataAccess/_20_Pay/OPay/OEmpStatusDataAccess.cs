using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class EmpStatusModel
{
    public string   Code        { get; set; } = string.Empty;
    public string   Name        { get; set; } = string.Empty;
    public bool     IsInPayroll { get; set; }
}

public class OEmpStatusDataAccess : IOEmpStatusDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OEmpStatusDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<EmpStatusModel>> _02(string pisdb, string conn)
        => await _sql.FetchLegacy<EmpStatusModel, dynamic>(
               $"select trim(Code) Code, trim(ifnull(Name,'')) Name, (isInPayroll = 1) IsInPayroll from {pisdb}.empstat order by Name", new { }, conn) ?? new();

    public async Task _03(string code, int isInPayroll, string pisdb, string conn)
        => await _sql.ExecuteLegacyCmd($"update {pisdb}.empstat set isInPayroll = @IsInPayroll where trim(Code) = @Code",
        new { Code = code.Trim(), IsInPayroll = isInPayroll }, conn);
}

public interface IOEmpStatusDataAccess
{
    Task<List<EmpStatusModel>> _02(string pisdb, string conn);
    Task                       _03(string code, int isInPayroll, string pisdb, string conn);
}
