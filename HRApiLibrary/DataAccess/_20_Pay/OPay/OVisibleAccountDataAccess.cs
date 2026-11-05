using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class VisibleAccountModel
{
    public string   AcctNumber  { get; set; } = string.Empty;
    public string   AcctName    { get; set; } = string.Empty;
    public string   ShortDesc   { get; set; } = string.Empty;
    public int      Shown       { get; set; }
    public bool     IsShown     { get; set; }
    public bool     IsDefault   { get; set; }
}

public class OVisibleAccountDataAccess : IOVisibleAccountDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    private static readonly string[] DefaultNames =
    {
        "basic pay", "reg. ot", "night diff. pay", "rest day pay", "rest day ot pay",
        "special holiday pay", "special holiday ot pay"
    };

    public OVisibleAccountDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    private static string Column(string? period) => period == "02" ? "show02" : "show01";

    public async Task<List<VisibleAccountModel>> _02(string? period, string schema, string conn)
    {
        bool saved = period is "01" or "02";
        var col  = Column(period);
        var rows = await _sql.FetchLegacy<VisibleAccountModel, dynamic>(
            $@"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName, trim(ifnull(shortDesc,'')) ShortDesc, ifnull({col},0) Shown
               from {schema}.chartofacct
               where AcctType = 'E' and hasRateOverBasic = '1'
               order by sort, AcctName", new { }, conn) ?? new();

        bool noneSaved = !saved || rows.All(r => r.Shown == 0);
        foreach (var r in rows)
        {
            r.IsDefault = DefaultNames.Contains(r.AcctName.Trim().ToLower());
            r.IsShown   = noneSaved ? r.IsDefault : r.Shown == 1;
        }
        return rows;
    }

    public async Task _03(string? period, List<string> shown, string schema, string conn)
    {
        var col = Column(period);
        await _sql.ExecuteLegacyCmd(
            $@"update {schema}.chartofacct
               set {col} = if(trim(AcctNumber) in @Shown, 1, 0)
               where AcctType = 'E' and hasRateOverBasic = '1'",
            new { Shown = shown.Count == 0 ? new List<string> { "" } : shown }, conn);
    }
}

public interface IOVisibleAccountDataAccess
{
    Task<List<VisibleAccountModel>> _02(string? period, string schema, string conn);
    Task                            _03(string? period, List<string> shown, string schema, string conn);
}
