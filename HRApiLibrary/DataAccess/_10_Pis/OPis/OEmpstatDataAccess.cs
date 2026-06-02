using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OEmpstatDataAccess : IOEmpstatDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OEmpstatDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OEmpstatModel?> _01(OEmpstatModel empstat, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Empstat 
							(CODE, NAME, ISRESIGNED, isonleaved, isfloating, issuspended, isInPayroll, inLicVer, inOE, isDeviation) values 
							(@Code, @Name, @IsResigned, @IsOnLeaved, @IsFloating, @IsSuspended, @IsInPayroll, @InLicVer, @InOe, @IsDeviation); 
						SELECT * FROM {schema}.Empstat WHERE Code = @Code; ";
        var res = await _sql.FetchData<OEmpstatModel?, dynamic>(sql, empstat, conn);
        return res.FirstOrDefault();
    }


    public async Task<List<OEmpstatModel?>?> _02ByCodes(string? code, string? schema, string? conn)
    {
        string? sql = $@"select  CODE, NAME, ISRESIGNED, isonleaved, isfloating, issuspended, isInPayroll, inLicVer, inOE, isDeviation 
						from {schema}.Empstat where Code = @Code";
        var data = await _sql.FetchData<OEmpstatModel?, dynamic>(sql, new { Code = code }, conn);
        return data;
    }
    
    public async Task<List<OEmpstatModel?>?> _02s(string? schema, string? conn)
    {
        string? sql = $@"select  *  from {schema}.Empstat order by Name ";
        var data = await _sql.FetchData<OEmpstatModel?, dynamic>(sql, new {  }, conn);
        return data;
    }


    public async Task<OEmpstatModel?> _03(OEmpstatModel empstat, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Empstat set 
							NAME = @NAME, 
							ISRESIGNED = @ISRESIGNED, 
							isonleaved = @isonleaved, 
							isfloating = @isfloating, 
							issuspended = @issuspended, 
							isInPayroll = @isInPayroll, 
							inLicVer = @inLicVer, 
							inOE = @inOE, 
							isDeviation = @isDeviation where CODE = @Code;
						select  * from {schema}.Empstat x where Code = @Code ;";
        var data = await _sql.FetchData<OEmpstatModel?, dynamic>(sql, empstat, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(string? code, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Empstat where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Code = code }, conn);

    }
}

public interface IOEmpstatDataAccess
{
    Task<OEmpstatModel?> _01(OEmpstatModel empstat, string? schema, string? conn);
    Task<List<OEmpstatModel?>?> _02ByCodes(string? code, string? schema, string? conn);
    Task<List<OEmpstatModel?>?> _02s(string? schema, string? conn); 
    Task<OEmpstatModel?> _03(OEmpstatModel empstat, string? schema, string? conn);
    Task _04(string? code, string? schema, string? conn);
    
}
