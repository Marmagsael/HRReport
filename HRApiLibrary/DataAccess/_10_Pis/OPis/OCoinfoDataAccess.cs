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
        string? sql = $@"Insert into {schema}.Coinfo (CoName, CoAdd, Add1, Add2, RegNo, TelNo, RegPeriod, shortname, acctno, SSS, PHIC, TIN, PAGIBIG, sssMemType, sssdocno, sssLocCode)
                         values (@CoName, @CoAdd, @Add1, @Add2, @RegNo, @TelNo, @RegPeriod, @Shortname, @AcctNo, @Sss, @Phic, @Tin, @Pagibig, @SssMemType, @Sssdocno, @SssLocCode)";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, coinfo, conn);

        return await _02(schema, conn);
    }


    public async Task<OCompanyInfoModel?> _02(string? schema, string? conn)
    {
        string? sql = $@"select  * from {schema}.Coinfo ";
        var data = await _sql.FetchLegacy<OCompanyInfoModel?, dynamic>(sql, new { }, conn);
        return data?.FirstOrDefault();
    }


    // -- The company row is found by its current name (CoName is the primary key and can be renamed here) --
    public async Task<OCompanyInfoModel?> _03(string? currentCoName, OCompanyInfoModel coinfo, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Coinfo set CoName = @CoName, CoAdd = @CoAdd, Add1 = @Add1, Add2 = @Add2, RegNo = @RegNo, TelNo = @TelNo, RegPeriod = @RegPeriod,
                                shortname = @Shortname, acctno = @AcctNo, SSS = @Sss, PHIC = @Phic, TIN = @Tin, PAGIBIG = @Pagibig,
                                sssMemType = @SssMemType, sssdocno = @Sssdocno, sssLocCode = @SssLocCode
                         where CoName = @CurrentCoName;";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new
        {
            CurrentCoName = currentCoName, coinfo.CoName, coinfo.CoAdd, coinfo.Add1, coinfo.Add2, coinfo.RegNo, coinfo.TelNo, coinfo.RegPeriod,
            coinfo.Shortname, coinfo.AcctNo, coinfo.Sss, coinfo.Phic, coinfo.Tin, coinfo.Pagibig, coinfo.SssMemType, coinfo.Sssdocno, coinfo.SssLocCode
        }, conn);

        return await _02(schema, conn);
    }

    public async Task<OCompanyInfoModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Coinfo where Id = @Id;";
        await _sql.ExecuteLegacyCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Coinfo x where x.Id = @Id ;";
        var data = await _sql.FetchLegacy<OCompanyInfoModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOCoinfoDataAccess
{
    Task<OCompanyInfoModel?> _01(OCompanyInfoModel coinfo, string? schema, string? conn);
    Task<OCompanyInfoModel?> _02(string? schema, string? conn);
    Task<OCompanyInfoModel?> _03(string? currentCoName, OCompanyInfoModel coinfo, string? schema, string? conn);
    Task<OCompanyInfoModel?> _04(int? id, string? schema, string? conn);
}
