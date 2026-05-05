using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OCoinfoDataAccess : IOCoinfoDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OCoinfoDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OCompanyInfoModel?> _01(OCompanyInfoModel coinfo, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Coinfo (CoName, CoAdd, TelNo, RegPeriod, CurrBasicRate, min, SDOSource, SDOSaveTo, shortname, cologo, acctno, SSS, PHIC, TIN, PAGIBIG, sssMemType, sssdocno, sssLocCode, schemapis, schemapay, schemaequip, schemaams, schemalumpsum, schematruc, schemaipay, isHeadOffice) values (@CoName, @CoAdd, @TelNo, @RegPeriod, @CurrBasicRate, @min, @SDOSource, @SDOSaveTo, @shortname, @cologo, @acctno, @SSS, @PHIC, @TIN, @PAGIBIG, @sssMemType, @sssdocno, @sssLocCode, @schemapis, @schemapay, @schemaequip, @schemaams, @schemalumpsum, @schematruc, @schemaipay, @isHeadOffice)";
        //await _sql.ExecuteCmd<dynamic>(sql, coinfo, conn);

        sql = $@"SELECT * FROM {schema}.Coinfo WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<OCompanyInfoModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OCompanyInfoModel?> _02(string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Coinfo ";
        var data = await _sql.FetchData<OCompanyInfoModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<OCompanyInfoModel?> _03(int? id, OCompanyInfoModel coinfo, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Coinfo set CoName = @CoName, CoAdd = @CoAdd, TelNo = @TelNo, RegPeriod = @RegPeriod, CurrBasicRate = @CurrBasicRate, min = @min, SDOSource = @SDOSource, SDOSaveTo = @SDOSaveTo, shortname = @shortname, cologo = @cologo, acctno = @acctno, SSS = @SSS, PHIC = @PHIC, TIN = @TIN, PAGIBIG = @PAGIBIG, sssMemType = @sssMemType, sssdocno = @sssdocno, sssLocCode = @sssLocCode, schemapis = @schemapis, schemapay = @schemapay, schemaequip = @schemaequip, schemaams = @schemaams, schemalumpsum = @schemalumpsum, schematruc = @schematruc, schemaipay = @schemaipay, isHeadOffice = @isHeadOffice where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, coinfo, conn);

        sql = $@" select  * from {schema}.Coinfo x where x.Id = @Id ;";
        var data = await _sql.FetchData<OCompanyInfoModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OCompanyInfoModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Coinfo where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Coinfo x where x.Id = @Id ;";
        var data = await _sql.FetchData<OCompanyInfoModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOCoinfoDataAccess
{
    Task<OCompanyInfoModel?> _01(OCompanyInfoModel coinfo, string? schema, string? conn);
    Task<OCompanyInfoModel?> _02(string? schema, string? conn);
    Task<OCompanyInfoModel?> _03(int? id, OCompanyInfoModel coinfo, string? schema, string? conn);
    Task<OCompanyInfoModel?> _04(int? id, string? schema, string? conn);
}
