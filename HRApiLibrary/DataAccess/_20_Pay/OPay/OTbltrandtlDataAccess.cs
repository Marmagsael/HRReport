using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OTbltrandtlDataAccess : IOTbltrandtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OTbltrandtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(GTbltrandtlModel tbltrandtl, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Tbltrandtl 
						(TRN,  EmpNumber,  DtlCd,  nVal,  uom) values 
						(@TRN, @EmpNumber, @DtlCd, @nVal, @uom)";
        await _sql.ExecuteCmd<dynamic>(sql, tbltrandtl, conn);

    }


    public async Task<List<GTbltrandtlModel?>?> _02ByTrnAndEmpnumber(string? trn, string? empnumber, string? schema, string? conn)
    {
        string? sql  = $@"select  * from {schema}.Tbltrandtl where left(Trn,6) = left(@Trn,6) and EmpNumber = @EmpNumber";
        var data    = await _sql.FetchData<GTbltrandtlModel?, dynamic>(sql, new { Trn = trn, Empnumber = empnumber }, conn);
        
        return data??[];
    }


    public async Task<GTbltrandtlModel?> _03(GTbltrandtlModel tbltrandtl, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Tbltrandtl set 
								nVal = @nVal, 
								uom = @uom where TRN = @TRN and EmpNumber = @EmpNumber and DtlCd = @DtlCd; 
						select  * from {schema}.Tbltrandtl where TRN = @TRN and EmpNumber = @EmpNumber and DtlCd = @DtlCd;";
        var data = await _sql.FetchData<GTbltrandtlModel?, dynamic>(sql, tbltrandtl, conn);
        return data?.FirstOrDefault();
    }

    public async Task<GTbltrandtlModel?> _04(GTbltrandtlModel tbltrandtl, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Tbltrandtl 	where TRN = @TRN and EmpNumber = @EmpNumber and DtlCd = @DtlCd;
						 Select  * from {schema}.Tbltrandtl where TRN = @TRN and EmpNumber = @EmpNumber and DtlCd = @DtlCd ;";
        var data = await _sql.FetchData<GTbltrandtlModel?, dynamic>(sql, tbltrandtl, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOTbltrandtlDataAccess
{
    Task _01(GTbltrandtlModel tbltrandtl, string? schema, string? conn);
    Task<List<GTbltrandtlModel?>?> _02ByTrnAndEmpnumber(string? trn, string? empnumber, string? schema, string? conn);
    Task<GTbltrandtlModel?> _03(GTbltrandtlModel tbltrandtl, string? schema, string? conn);
    Task<GTbltrandtlModel?> _04(GTbltrandtlModel tbltrandtl, string? schema, string? conn);
}
