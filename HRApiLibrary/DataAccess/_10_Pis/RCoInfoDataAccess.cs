using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RCoInfoDataAccess : IRCoInfoDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public RCoInfoDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // -- The company has one row only (Id 1); it is made here when missing --
    public async Task<RCoInfoModel?> _01Save(RCoInfoModel co, string? schema, string? conn)
    {
        string sql = $@"insert into {schema}.RCoInfo (Id, CoName, ShortName, Add1, Add2, TelNo, RegNo, RegPeriod, AcctNo, SssNo, PhicNo, TinNo, PagibigNo, SssMemType, SssDocNo, SssBrCode)
                        values (1, @CoName, @ShortName, @Add1, @Add2, @TelNo, @RegNo, @RegPeriod, @AcctNo, @SssNo, @PhicNo, @TinNo, @PagibigNo, @SssMemType, @SssDocNo, @SssBrCode)
                        on duplicate key update CoName = values(CoName), ShortName = values(ShortName), Add1 = values(Add1), Add2 = values(Add2), TelNo = values(TelNo),
                            RegNo = values(RegNo), RegPeriod = values(RegPeriod), AcctNo = values(AcctNo), SssNo = values(SssNo), PhicNo = values(PhicNo),
                            TinNo = values(TinNo), PagibigNo = values(PagibigNo), SssMemType = values(SssMemType), SssDocNo = values(SssDocNo), SssBrCode = values(SssBrCode);";
        await _sql.ExecuteCmd(sql, co, conn);
        return await _02(schema, conn);
    }

    public async Task<RCoInfoModel?> _02(string? schema, string? conn)
    {
        var data = await _sql.FetchData<RCoInfoModel, dynamic>($"select * from {schema}.RCoInfo order by Id limit 1", new { }, conn);
        return data.FirstOrDefault();
    }
}

public interface IRCoInfoDataAccess
{
    Task<RCoInfoModel?> _01Save(RCoInfoModel co, string? schema, string? conn);
    Task<RCoInfoModel?> _02(string? schema, string? conn);
}
