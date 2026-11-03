using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OAcctMappingDataAccess : IOAcctMappingDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    private static readonly (string Group, string Field, string Label)[] Fields =
    {
        ("Regular pay",          "daywrk",       "Days worked"),
        ("Regular pay",          "absent",       "Absent"),
        ("Regular pay",          "late",         "Late"),
        ("Regular pay",          "utime",        "Undertime"),
        ("Regular pay",          "retropay",     "Retro pay"),
        ("Overtime and holiday", "rot",          "Regular overtime"),
        ("Overtime and holiday", "rd",           "Rest day"),
        ("Overtime and holiday", "rdot",         "Rest day overtime"),
        ("Overtime and holiday", "lh",           "Legal holiday"),
        ("Overtime and holiday", "lhot",         "Legal holiday overtime"),
        ("Overtime and holiday", "rdlh",         "Rest day on legal holiday"),
        ("Overtime and holiday", "rdlhot",       "Rest day on legal holiday overtime"),
        ("Overtime and holiday", "sh",           "Special holiday"),
        ("Overtime and holiday", "shot",         "Special holiday overtime"),
        ("Overtime and holiday", "rdsh",         "Rest day on special holiday"),
        ("Overtime and holiday", "rdshot",       "Rest day on special holiday overtime"),
        ("Overtime and holiday", "DH",           "Double holiday"),
        ("Overtime and holiday", "DHOT",         "Double holiday overtime"),
        ("Overtime and holiday", "RDDH",         "Rest day on double holiday"),
        ("Overtime and holiday", "RDDHOT",       "Rest day on double holiday overtime"),
        ("Night differential",   "rn",           "Regular night"),
        ("Night differential",   "rnot",         "Regular night overtime"),
        ("Night differential",   "nd",           "Night differential"),
        ("Night differential",   "ndot",         "Night differential overtime"),
        ("Night differential",   "ndrd",         "Night differential rest day"),
        ("Night differential",   "ndrdot",       "Night differential rest day overtime"),
        ("Night differential",   "ndlh",         "Night differential legal holiday"),
        ("Night differential",   "ndlhot",       "Night differential legal holiday overtime"),
        ("Night differential",   "ndrdlh",       "Night differential rest day legal holiday"),
        ("Night differential",   "ndrdlhot",     "Night differential rest day legal holiday overtime"),
        ("Night differential",   "ndsh",         "Night differential special holiday"),
        ("Night differential",   "ndshot",       "Night differential special holiday overtime"),
        ("Night differential",   "ndrdsh",       "Night differential rest day special holiday"),
        ("Night differential",   "ndrdshot",     "Night differential rest day special holiday overtime"),
        ("Night differential",   "NDDH",         "Night differential double holiday"),
        ("Night differential",   "NDDHOT",       "Night differential double holiday overtime"),
        ("Night differential",   "NDRDDH",       "Night differential rest day double holiday"),
        ("Night differential",   "NDRDDHOT",     "Night differential rest day double holiday overtime"),
        ("Allowances",           "sea",          "SEA"),
        ("Allowances",           "sealh",        "SEA legal holiday"),
        ("Allowances",           "seardlh",      "SEA rest day legal holiday"),
        ("Allowances",           "ctpa",         "CTPA"),
        ("Allowances",           "ctpalh",       "CTPA legal holiday"),
        ("Allowances",           "ctpardlh",     "CTPA rest day legal holiday"),
        ("Allowances",           "Allowance",    "Allowance"),
        ("Allowances",           "mealAllowance","Meal allowance"),
        ("Allowances",           "IncentLeave",  "Incentive leave"),
        ("Allowances",           "FreeUniform",  "Free uniform"),
        ("Allowances",           "_13thMo",      "13th month"),
        ("Contributions",        "sss",          "SSS (employee)"),
        ("Contributions",        "SSSer",        "SSS (employer)"),
        ("Contributions",        "pagibig",      "Pag-IBIG (employee)"),
        ("Contributions",        "pagibiger",    "Pag-IBIG (employer)"),
        ("Contributions",        "phic",         "PhilHealth"),
        ("Others",               "CashBond",     "Cash bond"),
        ("Others",               "ProvForRetirement", "Provision for retirement"),
        ("Others",               "custom1",      "Custom 1"),
        ("Others",               "custom2",      "Custom 2"),
        ("Others",               "custom3",      "Custom 3"),
    };

    public OAcctMappingDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<AcctMapRowModel>> _02(string schema, string conn)
    {
        var rows    = await _sql.FetchLegacy<dynamic, dynamic>($"select * from {schema}.paymaindtlsetup limit 1", new { }, conn);
        var setup   = rows.FirstOrDefault() is IDictionary<string, object> d
                        ? new Dictionary<string, object>(d, StringComparer.OrdinalIgnoreCase)
                        : new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        var accts   = await _02Accounts(schema, conn);
        var rates   = accts.Where(a => !string.IsNullOrWhiteSpace(a.AcctNumber))
                           .GroupBy(a => a.AcctNumber!.Trim(), StringComparer.OrdinalIgnoreCase)
                           .ToDictionary(g => g.Key, g => g.First().OtRate, StringComparer.OrdinalIgnoreCase);

        var labels  = Fields.ToDictionary(f => f.Field, f => (f.Group, f.Label), StringComparer.OrdinalIgnoreCase);
        var order   = rows.FirstOrDefault() is IDictionary<string, object> raw ? raw.Keys.ToList() : Fields.Select(f => f.Field).ToList();

        var list = new List<AcctMapRowModel>();
        foreach (var field in order)
        {
            if (!labels.TryGetValue(field, out var info)) continue;
            var code = (setup[field]?.ToString() ?? string.Empty).Trim();
            list.Add(new AcctMapRowModel
            {
                Field = field, Label = info.Label, Group = info.Group, AcctCode = code,
                OtRate = rates.TryGetValue(code, out var r) ? r : 0
            });
        }
        return list;
    }

    public async Task<List<AcctMapAccountModel>> _02Accounts(string schema, string conn)
    {
        var data = await _sql.FetchLegacy<AcctMapAccountModel, dynamic>(
            $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName, ifnull(OTrate,0) OtRate from {schema}.chartofacct order by AcctName", new { }, conn) ?? new();
        data.Insert(0, new AcctMapAccountModel { AcctNumber = string.Empty, AcctName = string.Empty });
        return data;
    }

    public async Task<string?> _03(List<AcctMapRowModel> changed, string schema, string conn)
    {
        var known = Fields.Select(f => f.Field).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var accts = (await _02Accounts(schema, conn)).Select(a => a.AcctNumber ?? string.Empty).ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var row in changed)
        {
            if (!known.Contains(row.Field))                 return $"{row.Field} is not a payroll field.";
            if (!accts.Contains(row.AcctCode ?? string.Empty)) return $"Account {row.AcctCode} was not found for {row.Label}.";
            if (row.OtRate < 0)                             return $"OT rate for {row.Label} is not valid.";
        }

        foreach (var row in changed)
        {
            var field = Fields.First(f => string.Equals(f.Field, row.Field, StringComparison.OrdinalIgnoreCase)).Field;
            var code  = (row.AcctCode ?? string.Empty).Trim();

            await _sql.ExecuteLegacyCmd($"update {schema}.paymaindtlsetup set `{field}` = @Code where idpaymaindtlsetup != 0", new { Code = code }, conn);
            if (code.Length > 0)
                await _sql.ExecuteLegacyCmd($"update {schema}.chartofacct set OTrate = @Rate where trim(AcctNumber) = @Code", new { Rate = row.OtRate, Code = code }, conn);
        }
        return null;
    }
}

public interface IOAcctMappingDataAccess
{
    Task<List<AcctMapRowModel>>     _02(string schema, string conn);
    Task<List<AcctMapAccountModel>> _02Accounts(string schema, string conn);
    Task<string?>                   _03(List<AcctMapRowModel> changed, string schema, string conn);
}
