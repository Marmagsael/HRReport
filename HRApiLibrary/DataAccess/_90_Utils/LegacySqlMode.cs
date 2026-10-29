namespace HRApiLibrary.DataAccess._90_Utils;

// -- Legacy schemas ({Code}_OPis / {Code}_OPay : MBPay, made for MySQL 5.5) run in the sql_mode they were made for :
//    no strict mode and zero dates allowed. Inserts that leave out a NOT NULL date get '0000-00-00' as MBPay expects,
//    and '0000-00-00' can be compared in a where. The session gets the server mode back before the pooled connection is reused --
public static class LegacySqlMode
{
    public const string Set =
        "SET SESSION sql_mode = TRIM(BOTH ',' FROM REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@@SESSION.sql_mode, " +
        "'STRICT_TRANS_TABLES', ''), 'STRICT_ALL_TABLES', ''), 'NO_ZERO_IN_DATE', ''), 'NO_ZERO_DATE', ''), ',,', ','), ',,', ','))";

    public const string Restore = "SET SESSION sql_mode = @@GLOBAL.sql_mode";
}
