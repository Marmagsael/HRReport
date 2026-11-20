using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class ChartAcctModel
{
    public string?  AcctNumber       { get; set; }
    public string?  AcctName         { get; set; }
    public string?  AcctType         { get; set; } = "E";
    public string?  ShortDesc        { get; set; }
    public int      IsTaxable        { get; set; }
    public int      IsYtdAcct        { get; set; }
    public int      IsTaxExcl        { get; set; }
    public int      HasRateOverBasic { get; set; }
    public double   OtRate           { get; set; } = 1;
    public int      IsOt             { get; set; }
    public int      IsLegalHoliday   { get; set; }
    public int      WithSss          { get; set; }
    public int      WithPhic         { get; set; }
    public int      WithPagibig      { get; set; }
    public int      IsLock           { get; set; }
}

public class AcctRef
{
    public string TableName  { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
}

public class OChartAccountDataAccess : IOChartAccountDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OChartAccountDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<ChartAcctModel>> _02(string schema, string conn)
    {
        var sql = $@"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName, trim(AcctType) AcctType, trim(ifnull(shortDesc,'')) ShortDesc,
                            ifnull(isTaxable,'0') + 0 IsTaxable, ifnull(isYTDAcct,'0') + 0 IsYtdAcct, ifnull(isTaxExcl,'0') + 0 IsTaxExcl,
                            ifnull(hasRateOverBasic,'0') + 0 HasRateOverBasic, ifnull(OTrate,0) OtRate, ifnull(isOT,0) IsOt,
                            ifnull(isLegalHoliday,0) IsLegalHoliday, ifnull(withSSS,0) WithSss, ifnull(withPHIC,0) WithPhic,
                            ifnull(withPagibig,0) WithPagibig, ifnull(islock,'0') + 0 IsLock
                     from {schema}.chartofacct order by trim(AcctNumber)";
        return await _sql.FetchLegacy<ChartAcctModel, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<string?> _01(ChartAcctModel m, string schema, string conn)
    {
        var msg = Validate(m);
        if (msg != null) return msg;

        var code = m.AcctNumber!.Trim().ToUpperInvariant();
        var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.chartofacct where trim(AcctNumber) = @Code", new { Code = code }, conn);
        if (dup.FirstOrDefault() > 0) return $"Account number {code} already exists.";

        var dupName = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.chartofacct where lower(trim(AcctName)) = lower(@Name) and AcctType = @Type", new { Name = m.AcctName!.Trim(), Type = m.AcctType }, conn);
        if (dupName.FirstOrDefault() > 0) return $"An account named {m.AcctName!.Trim()} already exists.";

        await _sql.ExecuteLegacyCmd($@"insert into {schema}.chartofacct
                (AcctNumber, AcctName, AcctType, shortDesc, isTaxable, isYTDAcct, isTaxExcl, hasRateOverBasic, OTrate, isOT, isLegalHoliday,
                 withSSS, withPHIC, withPagibig, islock, isOthers, timedMode)
                values (@Code, @Name, @Type, @Short, @IsTaxable, @IsYtd, @IsTaxExcl, @HasRate, @OtRate, @IsOt, @IsLh, @Sss, @Phic, @Pagibig, '0', 0, @Timed)",
            Params(m, code), conn);
        return null;
    }

    public async Task<string?> _03(ChartAcctModel m, string schema, string conn)
    {
        var msg = Validate(m);
        if (msg != null) return msg;

        var code = m.AcctNumber!.Trim();
        var cur = await _sql.FetchLegacy<int, dynamic>($"select ifnull(islock,'0') + 0 from {schema}.chartofacct where trim(AcctNumber) = @Code", new { Code = code }, conn);
        if (cur.Count == 0) return "Account was not found.";
        if (cur[0] == 1) return "This account is locked. It cannot be edited.";

        var dupName = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.chartofacct where lower(trim(AcctName)) = lower(@Name) and AcctType = @Type and trim(AcctNumber) <> @Code", new { Name = m.AcctName!.Trim(), Type = m.AcctType, Code = code }, conn);
        if (dupName.FirstOrDefault() > 0) return $"An account named {m.AcctName!.Trim()} already exists.";

        await _sql.ExecuteLegacyCmd($@"update {schema}.chartofacct set AcctName = @Name, shortDesc = @Short, isTaxable = @IsTaxable, isYTDAcct = @IsYtd,
                isTaxExcl = @IsTaxExcl, hasRateOverBasic = @HasRate, OTrate = @OtRate, isOT = @IsOt, isLegalHoliday = @IsLh,
                withSSS = @Sss, withPHIC = @Phic, withPagibig = @Pagibig
                where trim(AcctNumber) = @Code and ifnull(islock,'0') <> '1'", Params(m, code), conn);
        return null;
    }

    public async Task<string?> _04(string acctNumber, string schema, string conn)
    {
        var code = acctNumber.Trim();
        var cur = await _sql.FetchLegacy<int, dynamic>($"select ifnull(islock,'0') + 0 from {schema}.chartofacct where trim(AcctNumber) = @Code", new { Code = code }, conn);
        if (cur.Count == 0) return "Account was not found.";
        if (cur[0] == 1) return "This account is locked. It cannot be deleted.";

        var cols = await _sql.FetchLegacy<string, dynamic>("select column_name from information_schema.columns where table_schema = @Schema and table_name = 'paymaindtlsetup' and column_name <> 'idpaymaindtlsetup'", new { Schema = schema }, conn);
        if (cols.Count > 0)
        {
            var where = string.Join(" or ", cols.Select(c => $"trim(`{c}`) = @Code"));
            var mapped = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.paymaindtlsetup where {where}", new { Code = code }, conn);
            if (mapped.FirstOrDefault() > 0) return "This account is mapped in Payroll Account Mapping. It cannot be deleted.";
        }

        var refs = await _sql.FetchLegacy<AcctRef, dynamic>(
            @"select table_name TableName, column_name ColumnName from information_schema.columns
              where table_schema = @Schema and lower(column_name) in ('acctnumber', 'acctcode', 'dtlcd')
                and lower(table_name) not like 'chartofacct%' and lower(table_name) not like 'tmp%'", new { Schema = schema }, conn) ?? [];

        foreach (var r in refs)
        {
            try
            {
                var isDtl = r.ColumnName.Equals("DtlCd", StringComparison.OrdinalIgnoreCase);
                var where = isDtl ? $"substring_index(trim(`{r.ColumnName}`), '_', 1) = @Code" : $"trim(`{r.ColumnName}`) = @Code";
                var n = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.`{r.TableName}` where {where}", new { Code = code }, conn);
                if (n.FirstOrDefault() > 0) return $"This account is used in {r.TableName}. It cannot be deleted.";
            }
            catch { }
        }

        foreach (var t in new[] { "tmptbltran", "tmptbltrandtl", "tmptblotherearnings", "tmptblotherdeductions" })
        {
            try
            {
                var col = t.EndsWith("dtl") ? "DtlCd" : "acctNumber";
                var where = col == "DtlCd" ? "substring_index(trim(DtlCd), '_', 1) = @Code" : "trim(acctNumber) = @Code";
                var n = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.`{t}` where {where}", new { Code = code }, conn);
                if (n.FirstOrDefault() > 0) return $"This account is used in {t}. It cannot be deleted.";
            }
            catch { }
        }

        await _sql.ExecuteLegacyCmd($"delete from {schema}.chartofacct where trim(AcctNumber) = @Code and ifnull(islock,'0') <> '1'", new { Code = code }, conn);
        return null;
    }

    private static object Params(ChartAcctModel m, string code) => new
    {
        Code = code, Name = m.AcctName!.Trim(), Type = m.AcctType, Short = (m.ShortDesc ?? string.Empty).Trim(),
        IsTaxable = m.IsTaxable.ToString(), IsYtd = m.IsYtdAcct.ToString(), IsTaxExcl = m.IsTaxExcl.ToString(), HasRate = m.HasRateOverBasic.ToString(),
        m.OtRate, m.IsOt, IsLh = m.IsLegalHoliday, Sss = m.WithSss, Phic = m.WithPhic, Pagibig = m.WithPagibig,
        Timed = m.HasRateOverBasic == 1 ? 1 : 0
    };

    private static string? Validate(ChartAcctModel m)
    {
        if (string.IsNullOrWhiteSpace(m.AcctNumber))  return "Enter the account number.";
        if (m.AcctNumber.Trim().Length > 5)           return "The account number can have 5 characters at most.";
        if (string.IsNullOrWhiteSpace(m.AcctName))    return "Enter the account name.";
        if (m.AcctName.Trim().Length > 60)            return "The account name can have 60 characters at most.";
        if (m.AcctType is not ("E" or "D"))           return "Choose Earnings or Deductions.";
        if ((m.ShortDesc ?? "").Trim().Length > 10)   return "The short name can have 10 characters at most.";
        if (m.OtRate < 0)                             return "Rate is not valid.";
        return null;
    }
}

public interface IOChartAccountDataAccess
{
    Task<List<ChartAcctModel>> _02(string schema, string conn);
    Task<string?>              _01(ChartAcctModel m, string schema, string conn);
    Task<string?>              _03(ChartAcctModel m, string schema, string conn);
    Task<string?>              _04(string acctNumber, string schema, string conn);
}
