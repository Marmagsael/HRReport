using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._11_AMS;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRMvc.StartupConfig;

// -- DB update : appsettings "Migrations": "true" runs the table makers, "false" runs nothing ----------------
// -- 1). startup : main + MainPis masterfiles
// -- 2). after login / switch company : the current company's schemas only (PIS, AMS, Pay, Acctg, old PIS, old Pay)
// -- The makers only add what is missing (CREATE IF NOT EXISTS, ALTER if missing, seed if empty) -------------
// -- A new company always gets its tables + initial data in L01CompanySetup.CreateSchemas (not tied to this switch)
public static class MigrationExt
{
    private const string LockName = "hrseries_table_makers";

    private class CompanySchemas
    {
        public string?  PISSchema   { get; set; }
        public string?  AMSSchema   { get; set; }
        public string?  PaySchema   { get; set; }
        public string?  AcctgSchema { get; set; }
        public string?  OldPis      { get; set; }
        public string?  OldPay      { get; set; }
        public int?     CountryId   { get; set; }
    }

    public static bool On(IConfiguration config) => config.GetValue<string>("Migrations") == "true";

    // -- Startup : main + MainPis; a failure stops the app, so it never runs on a half updated database --
    public static async Task RunMigrations(this WebApplication app)
    {
        var config  = app.Configuration;
        var log     = app.Logger;

        if (!On(config))
        {
            log.LogInformation("Migrations : off (appsettings Migrations is not true)");
            return;
        }

        string main     = config.GetSection("Schema:Main").Value    ?? "Main";
        string mainPis  = config.GetSection("Schema:MainPis").Value ?? "MainPis";
        string conn     = config.GetSection("Schema:DefConn").Value ?? "MySqlConn";

        using var scope = app.Services.CreateScope();
        var sp          = scope.ServiceProvider;
        var sql         = sp.GetRequiredService<I_90_001_MySqlDataAccess>();

        // -- One app instance at a time : the lock lives on this connection --
        using var lockCn = sql.GetConnString(conn);
        lockCn.Open();

        try
        {
            if (Convert.ToInt32(Scalar(lockCn, $"SELECT GET_LOCK('{LockName}', 900)")) != 1)
                throw new InvalidOperationException("Migrations : could not get the lock (another instance is running them).");

            try
            {
                var started = DateTime.Now;

                log.LogInformation("Migrations : main");
                await sp.GetRequiredService<I_00MainTblMakerAccess>()._01MainTable(main, conn);

                log.LogInformation("Migrations : {Schema} masterfiles", mainPis);
                await sp.GetRequiredService<I_00MainPisMasterfileMaker>()._01MainPisMasterfile(mainPis, conn);

                log.LogInformation("Migrations : done in {Seconds:0} s", (DateTime.Now - started).TotalSeconds);
            }
            finally
            {
                Scalar(lockCn, $"SELECT RELEASE_LOCK('{LockName}')");
            }
        }
        catch (Exception ex)
        {
            log.LogCritical(ex, "Migrations : failed, the app is stopped. Fix the database or set Migrations to false, then start the app again.");
            throw;
        }
    }

    // -- After login / switch company : the current company only; a failure is logged and the login goes on --
    public static async Task RunCompany(IServiceProvider sp, int? companyId)
    {
        var config = sp.GetRequiredService<IConfiguration>();
        if (!On(config) || companyId is null or <= 0) return;

        var log         = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Migrations");
        string main     = config.GetSection("Schema:Main").Value    ?? "Main";
        string mainPis  = config.GetSection("Schema:MainPis").Value ?? "MainPis";
        string conn     = config.GetSection("Schema:DefConn").Value ?? "MySqlConn";
        string lockName = $"{LockName}_{companyId}";
        var sql         = sp.GetRequiredService<I_90_001_MySqlDataAccess>();

        try
        {
            // -- One run per company at a time (two users of the same company logging in together) --
            using var lockCn = sql.GetConnString(conn);
            lockCn.Open();

            if (Convert.ToInt32(Scalar(lockCn, $"SELECT GET_LOCK('{lockName}', 900)")) != 1)
                throw new InvalidOperationException($"Migrations : could not get the lock for company {companyId}.");

            try
            {
                var started = DateTime.Now;
                await RunCompanySchemas(sp, sql, log, main, mainPis, conn, companyId.Value);
                log.LogInformation("Migrations : company {CompanyId} done in {Seconds:0} s", companyId, (DateTime.Now - started).TotalSeconds);
            }
            finally
            {
                Scalar(lockCn, $"SELECT RELEASE_LOCK('{lockName}')");
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Migrations : company {CompanyId} failed, the login goes on.", companyId);
        }
    }

    // -- The company's schemas that exist on this server, each schema run once ------------------------------
    private static async Task RunCompanySchemas(IServiceProvider sp, I_90_001_MySqlDataAccess sql, ILogger log, string main, string mainPis, string conn, int companyId)
    {
        var pis     = sp.GetRequiredService<I_00MainPisTblMakerAccess>();
        var ams     = sp.GetRequiredService<IAMSTableMaker>();
        var pay     = sp.GetRequiredService<I_20_002_PayTblMaker>();
        var acctg   = sp.GetRequiredService<I_AcctgTableMaker>();
        var oldPis  = sp.GetRequiredService<IOldPis_TblMakerDataAccess>();
        var oldPay  = sp.GetRequiredService<IOldPay_TblMakerDataAccess>();

        var existing = (await sql.FetchData<string, dynamic>("select lower(schema_name) from information_schema.schemata", new { }, conn))
                       .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { main, mainPis, "MainPay", "mysql", "sys", "information_schema", "performance_schema" };
        var done     = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        bool Exists(string? schema) => !string.IsNullOrWhiteSpace(schema)
                                    && System.Text.RegularExpressions.Regex.IsMatch(schema.Trim(), @"^\w+$")
                                    && existing.Contains(schema.Trim())
                                    && !reserved.Contains(schema.Trim());
        bool Pick(string? schema)   => Exists(schema) && done.Add(schema!.Trim());

        var c = (await sql.FetchData<CompanySchemas, dynamic>(
            $"select PISSchema, AMSSchema, PaySchema, AcctgSchema, OldPis, OldPay, CountryId from {main}.UsersCompany where Id = @Id",
            new { Id = companyId }, conn)).FirstOrDefault();
        if (c == null) return;

        if (Pick(c.PISSchema))
        {
            log.LogInformation("Migrations : PIS {Schema}", c.PISSchema);
            await pis._01MainPisTableInternal(c.PISSchema!.Trim(), conn);
            await pis._01MainPisTable(c.PISSchema!.Trim(), conn);
        }
        // -- AMS : its own schema when it exists, else the PIS schema (AMS is kept there for now) --
        string? amsSchema = Exists(c.AMSSchema) ? c.AMSSchema!.Trim() : Exists(c.PISSchema) ? c.PISSchema!.Trim() : null;
        if (amsSchema != null)
        {
            log.LogInformation("Migrations : AMS {Schema}", amsSchema);
            await ams._01AMSTable(amsSchema, conn);
        }
        if (Pick(c.PaySchema))
        {
            log.LogInformation("Migrations : Pay {Schema}", c.PaySchema);
            await pay._01(c.PaySchema!.Trim(), "PH", conn);
            if (c.CountryId == 2) await pay._01PH(c.PaySchema!.Trim(), "PH", conn);
        }
        if (Pick(c.AcctgSchema))
        {
            log.LogInformation("Migrations : Acctg {Schema}", c.AcctgSchema);
            await acctg.AccountingTableMaker(c.AcctgSchema!.Trim(), conn);
        }
        if (Pick(c.OldPis))
        {
            log.LogInformation("Migrations : old PIS {Schema}", c.OldPis);
            await oldPis._01OldPisTable(c.OldPis!.Trim(), conn);
            await oldPis._01OldPisDefaults(c.OldPis!.Trim(), mainPis, conn);
        }
        if (Pick(c.OldPay))
        {
            log.LogInformation("Migrations : old Pay {Schema}", c.OldPay);
            await oldPay._01OldPayTable(c.OldPay!.Trim(), conn);
        }
    }

    private static object? Scalar(System.Data.IDbConnection cn, string sqlText)
    {
        using var cmd = cn.CreateCommand();
        cmd.CommandText = sqlText;
        return cmd.ExecuteScalar();
    }
}
