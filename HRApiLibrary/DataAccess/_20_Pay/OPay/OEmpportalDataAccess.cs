using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;
using HRApiLibrary.Models._20_PayGeneric.EmpPortal;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OEmpportalDataAccess : IOEmpportalDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OEmpportalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<GTbltrandtlModel>> _02ByTrnAndEmpnumber(string? trn, string? empnumber, string? schema, string? conn)
    {
        string? sql = $@"select * from {schema}.Tbltrandtl where left(Trn,6) = left(@Trn,6) and EmpNumber = @EmpNumber";
        var data = await _sql.FetchData<GTbltrandtlModel, dynamic>(sql, new { Trn = trn, Empnumber = empnumber }, conn);
        return data ?? [];
    }

    public async Task<EP_SSS12102Model> _02SSSRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn)
    {
        EP_SSS12102Model data = new()
        {
            Tbltran = await _02GTbltran_ByEmpNumber_ByAcctNumber(empnumber, acctnumber, paydb, conn),
            Sssprempaydtls = await _02EP_ByEmpNumber_ByAcctNumber<GSssprempaydtlModel>(empnumber, "SSSPremPaydtl", paydb, conn)
        };

        return data;
    }
    
    public async Task<EP_Phic12102Model> _02PHICRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn)
    {
        EP_Phic12102Model data = new()
        {
            Tbltran         = await _02GTbltran_ByEmpNumber_ByAcctNumber(empnumber, acctnumber, paydb, conn),
            GPhicpaydtl     = await _02EP_ByEmpNumber_ByAcctNumber<GPhicpaydtlModel>(empnumber, "PHICPaydtl", paydb, conn)
        };

        return data;
    }
    
    public async Task<EP_Pagibig12102Model> _02PAGIBIGRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn)
    {
        EP_Pagibig12102Model data = new()
        {
            Tbltran             = await _02GTbltran_ByEmpNumber_ByAcctNumber(empnumber, acctnumber, paydb, conn),
            GPagibigpaydtl      = await _02EP_ByEmpNumber_ByAcctNumber<GPagibigpaydtlModel>(empnumber, "PagibigPaydtl", paydb, conn)
        };

        return data;
    }

    // ========================================================================
    // --- Private Functions --------------------------------------------------
    // ========================================================================
    private async Task<List<GTbltranModel>> _02GTbltran_ByEmpNumber_ByAcctNumber(string? empnumber, string? acctnumber, string? paydb, string? conn)
    {
        string? sql = $@" select t.Trn, t.AcctNumber, t.EmpNumber, t.Amount, 
                                IF(t.dTimeStamp IN ('0000-00-00','0000-00-00 00:00:00'), NULL, t.dTimeStamp) AS DTimeStamp,
                                t.Source, t.Postedby, c.AcctName 
                         from {paydb}.Tbltran t 
                         left join {paydb}.ChartOfAcct c on c.AcctNumber = t.AcctNumber
                         where t.AcctNumber = @AcctNumber and t.EmpNumber = @EmpNumber";

        var data = await _sql.FetchData<GTbltranModel, dynamic>(sql, new { AcctNumber = acctnumber, EmpNumber = empnumber }, conn);
        return data ?? [];
    }

    private async Task<List<T>> _02EP_ByEmpNumber_ByAcctNumber<T>(string? empnumber, string? tblName, string? paydb, string? conn)
    {
        string? sql = $@" SELECT t.* FROM {paydb}.{tblName} t WHERE t.EmpNumber = @EmpNumber ORDER BY Trn";
        var data = await _sql.FetchData<T, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return data ?? [];
    }
}


public interface IOEmpportalDataAccess
{
    Task<List<GTbltrandtlModel>>    _02ByTrnAndEmpnumber(string? trn, string? empnumber, string? schema, string? conn);
    Task<EP_SSS12102Model>          _02SSSRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn);
    Task<EP_Phic12102Model>         _02PHICRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn); 
    Task<EP_Pagibig12102Model>      _02PAGIBIGRemittance(string? empnumber, string? acctnumber, string? paydb, string? conn); 
}
