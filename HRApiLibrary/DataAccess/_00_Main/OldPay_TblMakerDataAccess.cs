using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._00_Main;

public class OldPay_TblMakerDataAccess : IOldPay_TblMakerDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OldPay_TblMakerDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // -- The legacy tables use '0000-00-00' date defaults, which strict mode rejects --
    // -- Zero dates are allowed for this statement only, then the session goes back to the global sql_mode --
    private Task ExecLegacy(string? sql, string? conn)
    {
        string mode = "TRIM(BOTH ',' FROM REPLACE(REPLACE(REPLACE(REPLACE(@@SESSION.sql_mode, 'NO_ZERO_IN_DATE', ''), 'NO_ZERO_DATE', ''), ',,', ','), ',,', ','))";
        return _sql.ExecuteCmd($"SET SESSION sql_mode = {mode}; {sql?.TrimEnd().TrimEnd(';')}; SET SESSION sql_mode = @@GLOBAL.sql_mode;", new { }, conn);
    }

    public async Task _01OldPayTable(string? schema, string? connName = "MySqlConn")
    {
        await _sql.ExecuteCmd($"CREATE SCHEMA IF NOT EXISTS {schema} DEFAULT CHARACTER SET latin1;", new { }, connName);

        await _0113thmogrp(schema, connName);
        await _0113thmosched(schema, connName);
        await _0113thmosettings(schema, connName);
        await _01Config(schema, connName);
        await _01EmpmasTaxcnt(schema, connName);
        await _01Migrationtbllist(schema, connName);
        await _01Payperiod(schema, connName);
        await _01Servers(schema, connName);
        await _01Acctaccess(schema, connName);
        await _01Acctconfig(schema, connName);
        await _01Acctremoveracctaccess(schema, connName);
        await _01Acctremoveradminaccess(schema, connName);
        await _01Acctremovertbltran(schema, connName);
        await _01Acctremovertemplate(schema, connName);
        await _01ActuarialAcct(schema, connName);
        await _01Alphalistconfig(schema, connName);
        await _01Alphalistconfigdesc(schema, connName);
        await _01AnTax(schema, connName);
        await _01AnnualizeSettings(schema, connName);
        await _01AuditPayrollLock(schema, connName);
        await _01AuditTrail(schema, connName);
        await _01Chartofacct(schema, connName);
        await _01Chartofacct161215(schema, connName);
        await _01Chartofacct170118(schema, connName);
        await _01ChartofacctArchived(schema, connName);
        await _01Coaconfig(schema, connName);
        await _01Coamatching(schema, connName);
        await _01Dedmandatory(schema, connName);
        await _01DedmandatoryTrans(schema, connName);
        await _01Dedmandatoryexemption(schema, connName);
        await _01Dedn(schema, connName);
        await _01Deductiondressing(schema, connName);
        await _01Domainaccess(schema, connName);
        await _01Earn(schema, connName);
        await _01Empdedexemption(schema, connName);
        await _01Empmasprevsal(schema, connName);
        await _01Empstatpisval(schema, connName);
        await _01FixedEarnings(schema, connName);
        await _01FixedEarningsGrp(schema, connName);
        await _01Holiday(schema, connName);
        await _01Hospitalizationacct(schema, connName);
        await _01Hospitalizationdtl(schema, connName);
        await _01Hospitalizationdtlrchieved(schema, connName);
        await _01Hospitalizationgrp(schema, connName);
        await _01Hospitalizationhdr(schema, connName);
        await _01Hospitalizationsavings(schema, connName);
        await _01Hospitalizationstatus(schema, connName);
        await _01Hospitalizationtrans(schema, connName);
        await _01Hostpitalizationdedsched(schema, connName);
        await _01Idissuance(schema, connName);
        await _01Incentiveleave(schema, connName);
        await _01Loandtl(schema, connName);
        await _01Loanhdr(schema, connName);
        await _01Loans(schema, connName);
        await _01Loans18(schema, connName);
        await _01Loansacctaccess(schema, connName);
        await _01Loansref(schema, connName);
        await _01Loansref1801(schema, connName);
        await _01Loansref1802(schema, connName);
        await _01LoansrefRepAccess(schema, connName);
        await _01Mealallowopt(schema, connName);
        await _01Medtbl(schema, connName);
        await _01Menubars(schema, connName);
        await _01Notminimum(schema, connName);
        await _01Pagibigpaydtl(schema, connName);
        await _01Pagibigpayhdr(schema, connName);
        await _01Pagibigtbl(schema, connName);
        await _01PaygrpMovhistory(schema, connName);
        await _01Paymaindtl(schema, connName);
        await _01Paymaindtlsetup(schema, connName);
        await _01Paymainhdr(schema, connName);
        await _01Payrollgrp(schema, connName);
        await _01Payrollprd(schema, connName);
        await _01Pepsiemplyee(schema, connName);
        await _01Period(schema, connName);
        await _01Phicpaydtl(schema, connName);
        await _01Phicpayhdr(schema, connName);
        await _01PisvalEmpdeploymenthist(schema, connName);
        await _01Quitclaimcoa(schema, connName);
        await _01Ratesamt(schema, connName);
        await _01Refpagibigtbl(schema, connName);
        await _01Refpgibigtbl(schema, connName);
        await _01Refphictbl(schema, connName);
        await _01Refssstbl(schema, connName);
        await _01Retirementopt(schema, connName);
        await _01Retropay(schema, connName);
        await _01Salgrade(schema, connName);
        await _01Sbpl(schema, connName);
        await _01Semimonthlytax(schema, connName);
        await _01Semimonthlytaxold(schema, connName);
        await _01Sssloandtl(schema, connName);
        await _01Sssloanhdr(schema, connName);
        await _01Sssprempaydtl(schema, connName);
        await _01Sssprempayhdr(schema, connName);
        await _01Ssstbl(schema, connName);
        await _01Ssstbl2025bak(schema, connName);
        await _01Sysmenu(schema, connName);
        await _01Taxmatrix(schema, connName);
        await _01Taxrefund2012(schema, connName);
        await _01Tblatexemption(schema, connName);
        await _01Tblbatch(schema, connName);
        await _01Tblbilldtl(schema, connName);
        await _01Tblbillhdr(schema, connName);
        await _01Tblclientposrate(schema, connName);
        await _01Tbldedperclient(schema, connName);
        await _01Tbldefaultcontents(schema, connName);
        await _01Tbldeferd(schema, connName);
        await _01Tblotherdedref(schema, connName);
        await _01Tblotherdeductions(schema, connName);
        await _01TblotherdeductionsResched(schema, connName);
        await _01Tblotherearnings(schema, connName);
        await _01Tblperrate(schema, connName);
        await _01Tblroblink(schema, connName);
        await _01Tbltran(schema, connName);
        await _01Tbltran110801(schema, connName);
        await _01Tbltran130129(schema, connName);
        await _01Tbltran180602(schema, connName);
        await _01Tbltran20180502(schema, connName);
        await _01Tbltran20191001(schema, connName);
        await _01Tbltran20191001dtl(schema, connName);
        await _01Tbltran20230920(schema, connName);
        await _01TbltranArchived(schema, connName);
        await _01Tbltranajinomotomakati(schema, connName);
        await _01Tbltranarchieved(schema, connName);
        await _01Tbltrandeployment(schema, connName);
        await _01Tbltrandtl(schema, connName);
        await _01Tbltrandtl1(schema, connName);
        await _01Tbltrandtlajinomotomakati(schema, connName);
        await _01Tbltrandtlx(schema, connName);
        await _01Tbltrandtlx1(schema, connName);
        await _01Tbltranerro(schema, connName);
        await _01Tbltransource(schema, connName);
        await _01Tbltrantemper(schema, connName);
        await _01Tbltranx(schema, connName);
        await _01Temptotalhrs(schema, connName);
        await _01TmpdedmandatoryTrans(schema, connName);
        await _01TmpfixedEarnings(schema, connName);
        await _01Tmploans(schema, connName);
        await _01Tmppaymaindtl(schema, connName);
        await _01Tmppaymaindtl01(schema, connName);
        await _01Tmppaymainhdr(schema, connName);
        await _01Tmpretropay(schema, connName);
        await _01Tmptblotherdeductions(schema, connName);
        await _01Tmptblotherearnings(schema, connName);
        await _01Tmptbltran(schema, connName);
        await _01Tmptbltrandtl(schema, connName);
        await _01Tmptbltransource(schema, connName);
        await _01Tmpundeddeduction(schema, connName);
        await _01TotalTr(schema, connName);
        await _01Trandtl110801(schema, connName);
        await _01Trca(schema, connName);
        await _01Undeddeduction(schema, connName);
        await _01Uniformopt(schema, connName);
        await _01Uom(schema, connName);
        await _01Useraccess(schema, connName);
        await _01Usr(schema, connName);
        await _01Usrmenu(schema, connName);
        await _01Version(schema, connName);

        // -- Added from the reference (192.168.0.103 pay) --
        await _01Domainusr(schema, connName);
    }

    private async Task _0113thmogrp(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`13thmogrp` (
                        `trn` char(12) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        PRIMARY KEY (`trn`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _0113thmosched(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`13thmosched` (
                        `yr` int unsigned NOT NULL DEFAULT '0',
                        `prd` int unsigned NOT NULL DEFAULT '0',
                        `batch` char(2) NOT NULL DEFAULT '0',
                        `trn` char(12) NOT NULL DEFAULT 'YYMMOO-XXXXX',
                        `TRN_Release` char(6) NOT NULL DEFAULT 'YYMMPP',
                        PRIMARY KEY (`yr`,`prd`,`batch`,`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _0113thmosettings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`13thmosettings` (
                        `id13thMoSettings` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` char(5) DEFAULT '',
                        `withMonthlyPay` int unsigned DEFAULT '1',
                        PRIMARY KEY (`id13thMoSettings`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Config(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`_config` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `COAFile` varchar(45) NOT NULL,
                        `ClientRate` varchar(45) NOT NULL,
                        `ClientRateEmp` varchar(45) NOT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);

        // -- Columns added after the table was created (reference : 192.168.0.103 pay) --
        var columns = await _sql.FetchData<dynamic, dynamic>($"DESC {schema}.`_config`", new { }, conn);
        foreach (var column in new[] { "ClientRate", "ClientRateEmp" })
        {
            if (columns.Any(c => string.Equals((string)c.Field, column, StringComparison.OrdinalIgnoreCase))) continue;
            await ExecLegacy($"ALTER TABLE {schema}.`_config` ADD COLUMN `{column}` varchar(45) NOT NULL", conn);
        }
    }

    private async Task _01EmpmasTaxcnt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`_empmas_taxcnt` (
                        `empnumber` varchar(5) NOT NULL,
                        `taxCnt` bigint NOT NULL DEFAULT '0',
                        `wtax` double(21,4) DEFAULT NULL,
                        `BPayCnt` bigint DEFAULT '0',
                        `empname` varchar(63) DEFAULT NULL,
                        `datehired` date DEFAULT NULL,
                        `separate` date DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Migrationtbllist(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`_migrationtbllist` (
                        `tblname` varchar(80) NOT NULL,
                        PRIMARY KEY (`tblname`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Payperiod(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`_payperiod` (
                        `prd` char(6) NOT NULL,
                        `status` varchar(6) NOT NULL,
                        PRIMARY KEY (`prd`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Servers(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`_servers` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(45) NOT NULL,
                        `address` varchar(45) NOT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctaccess` (
                        `username` varchar(25) NOT NULL,
                        `Grp` varchar(10) NOT NULL,
                        `AcctNumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`username`,`Grp`,`AcctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctconfig(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctconfig` (
                        `MealAllow` varchar(10) NOT NULL,
                        `Uniform` varchar(10) NOT NULL,
                        `ProvRetirement` varchar(10) NOT NULL,
                        `BasicPay` varchar(10) NOT NULL,
                        `FMealAllow` varchar(60) NOT NULL,
                        `FUniform` varchar(60) NOT NULL,
                        `FProvRetirement` varchar(60) NOT NULL,
                        `MealOTAmt` double(16,2) NOT NULL DEFAULT '0.00',
                        `ctpa` char(5) DEFAULT '0',
                        `ctpa_LH` char(5) DEFAULT '0',
                        `sea` char(5) DEFAULT '0',
                        `sea_LH` char(5) DEFAULT '0',
                        `OtherDedYrDisplay` int unsigned DEFAULT '2015',
                        `incentiveLeave` char(5) DEFAULT 'e018',
                        `ctpa_rd` char(15) DEFAULT '',
                        `ctpa_rdlh` char(15) DEFAULT '',
                        `ecola` char(15) DEFAULT '',
                        `ecola_rd` varchar(45) DEFAULT ' ',
                        `ecola_lh` varchar(45) DEFAULT ' ',
                        `ecola_rdlh` varchar(45) DEFAULT ' ',
                        `sea_rd` char(15) DEFAULT '',
                        `sea_rdlh` char(15) DEFAULT '',
                        `_3thMo` char(15) DEFAULT 'E019',
                        `projBDefAcct` char(5) DEFAULT '',
                        PRIMARY KEY (`MealAllow`)
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctremoveracctaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctremoveracctaccess` (
                        `idacctRemoverAcctAccess` int unsigned NOT NULL AUTO_INCREMENT,
                        `username` varchar(25) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`idacctRemoverAcctAccess`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctremoveradminaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctremoveradminaccess` (
                        `idAcctRemoverAdminAccess` int unsigned NOT NULL AUTO_INCREMENT,
                        `username` varchar(25) NOT NULL,
                        PRIMARY KEY (`idAcctRemoverAdminAccess`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctremovertbltran(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctremovertbltran` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Acctremovertemplate(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`acctremovertemplate` (
                        `idacctRemoverTemplate` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` varchar(5) NOT NULL,
                        `acctnumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`idacctRemoverTemplate`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01ActuarialAcct(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`actuarial_acct` (
                        `acctnumber` char(4) NOT NULL,
                        `acctname` varchar(80) NOT NULL,
                        PRIMARY KEY (`acctnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Alphalistconfig(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`alphalistconfig` (
                        `ctrlKey` int unsigned NOT NULL AUTO_INCREMENT,
                        `form` varchar(45) NOT NULL,
                        `acctKey` int unsigned NOT NULL,
                        `acctNumber` char(4) NOT NULL,
                        `amt` double(12,2) NOT NULL DEFAULT '0.00',
                        PRIMARY KEY (`ctrlKey`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Alphalistconfigdesc(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`alphalistconfigdesc` (
                        `ctrlkey` int unsigned NOT NULL AUTO_INCREMENT,
                        `form` varchar(45) NOT NULL,
                        `acctKey` int unsigned NOT NULL,
                        `descr` varchar(254) NOT NULL,
                        PRIMARY KEY (`ctrlkey`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01AnTax(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`an_tax` (
                        `code` varchar(5) NOT NULL DEFAULT '',
                        `amt` double(10,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01AnnualizeSettings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`annualize_settings` (
                        `category` char(10) NOT NULL,
                        `code` char(10) NOT NULL DEFAULT '',
                        `val_int` int unsigned NOT NULL DEFAULT '0',
                        `val_char` char(15) NOT NULL DEFAULT '',
                        PRIMARY KEY (`category`,`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01AuditPayrollLock(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`audit_payroll_lock` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `trn` char(12) NOT NULL,
                        `tdate` datetime NOT NULL,
                        `action` varchar(50) NOT NULL,
                        `user` varchar(25) NOT NULL,
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01AuditTrail(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`audit_trail` (
                        `idaudit_trail` int unsigned NOT NULL AUTO_INCREMENT,
                        `tdate` datetime NOT NULL,
                        `source` char(5) NOT NULL,
                        `action` char(45) NOT NULL,
                        `userName` char(25) NOT NULL,
                        PRIMARY KEY (`idaudit_trail`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Chartofacct(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`chartofacct` (
                        `AcctNumber` varchar(5) NOT NULL,
                        `AcctName` varchar(60) NOT NULL DEFAULT ' ',
                        `AcctType` varchar(1) NOT NULL DEFAULT ' ',
                        `isTaxable` varchar(1) NOT NULL DEFAULT '0',
                        `isYTDAcct` varchar(1) NOT NULL DEFAULT '0',
                        `isTaxExcl` varchar(1) NOT NULL DEFAULT '0',
                        `islock` varchar(1) NOT NULL DEFAULT '0',
                        `ischargeable` varchar(1) NOT NULL DEFAULT '0',
                        `hasRateOverBasic` varchar(1) NOT NULL DEFAULT '0',
                        `isOthers` int unsigned NOT NULL DEFAULT '1',
                        `isFixed` varchar(1) NOT NULL DEFAULT '0',
                        `timedMode` int unsigned NOT NULL DEFAULT '0' COMMENT '0->NOT TIMED,1->/HR, 2->/DAY',
                        `shortDesc` varchar(10) NOT NULL DEFAULT ' ',
                        `show01` int unsigned NOT NULL DEFAULT '0',
                        `sort` int unsigned NOT NULL DEFAULT '999',
                        `special_` int unsigned NOT NULL DEFAULT '0' COMMENT '1->TAX,2->ECOLA',
                        `show02` int unsigned NOT NULL DEFAULT '0',
                        `DedSort` int unsigned NOT NULL DEFAULT '999',
                        `isTH` int unsigned NOT NULL DEFAULT '0',
                        `Deferd` int unsigned NOT NULL DEFAULT '0',
                        `isOT` int unsigned NOT NULL DEFAULT '0',
                        `isMealAcct` int unsigned NOT NULL DEFAULT '0',
                        `formula` varchar(45) NOT NULL DEFAULT '''',
                        `OTrate` double(7,4) NOT NULL DEFAULT '1.0000',
                        `withSSS` int unsigned NOT NULL DEFAULT '0',
                        `withPHIC` int unsigned NOT NULL DEFAULT '0',
                        `withPagibig` int unsigned NOT NULL DEFAULT '0',
                        `ceiling` double(8,2) NOT NULL DEFAULT '0.00',
                        `isGovAcct` int unsigned NOT NULL DEFAULT '0',
                        `isLegalHoliday` int unsigned NOT NULL DEFAULT '0',
                        `is13thMoAcct` int unsigned NOT NULL DEFAULT '0',
                        `isextloan` int unsigned NOT NULL DEFAULT '0',
                        `ExtLoanPercentage` decimal(5,4) DEFAULT '0.0000',
                        `status_` tinyint unsigned NOT NULL DEFAULT '1',
                        `customRate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `Taxable_Type` int unsigned DEFAULT '0',
                        `MWE_Type` int unsigned DEFAULT '0',
                        `TaxExptAmt` double(12,4) DEFAULT '0.0000',
                        PRIMARY KEY (`AcctNumber`)
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Chartofacct161215(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`chartofacct161215` (
                        `AcctNumber` varchar(5) NOT NULL,
                        `AcctName` varchar(60) NOT NULL DEFAULT ' ',
                        `AcctType` varchar(1) NOT NULL DEFAULT ' ',
                        `isTaxable` varchar(1) NOT NULL DEFAULT '0',
                        `isYTDAcct` varchar(1) NOT NULL DEFAULT '0',
                        `isTaxExcl` varchar(1) NOT NULL DEFAULT '0',
                        `islock` varchar(1) NOT NULL DEFAULT '0',
                        `ischargeable` varchar(1) NOT NULL DEFAULT '0',
                        `hasRateOverBasic` varchar(1) NOT NULL DEFAULT '0',
                        `isOthers` varchar(1) NOT NULL DEFAULT '0',
                        `isFixed` varchar(1) NOT NULL DEFAULT '0',
                        `timedMode` int unsigned NOT NULL DEFAULT '0' COMMENT '0->NOT TIMED,1->/HR, 2->/DAY',
                        `shortDesc` varchar(10) NOT NULL DEFAULT ' ',
                        `show01` int unsigned NOT NULL DEFAULT '0',
                        `sort` int unsigned NOT NULL DEFAULT '999',
                        `special_` int unsigned NOT NULL DEFAULT '0' COMMENT '1->TAX,2->ECOLA',
                        `show02` int unsigned NOT NULL DEFAULT '0',
                        `DedSort` int unsigned NOT NULL DEFAULT '999',
                        `isTH` int unsigned NOT NULL DEFAULT '0',
                        `Deferd` int unsigned NOT NULL DEFAULT '0',
                        `isOT` int unsigned NOT NULL DEFAULT '0',
                        `isMealAcct` int unsigned NOT NULL DEFAULT '0',
                        `formula` varchar(45) NOT NULL DEFAULT '''',
                        `OTrate` double(7,4) NOT NULL DEFAULT '1.0000',
                        `withSSS` int unsigned NOT NULL DEFAULT '0',
                        `withPHIC` int unsigned NOT NULL DEFAULT '0',
                        `withPagibig` int unsigned NOT NULL DEFAULT '0',
                        `ceiling` double(8,2) NOT NULL DEFAULT '0.00',
                        `isGovAcct` int unsigned NOT NULL DEFAULT '0',
                        `isLegalHoliday` int unsigned NOT NULL DEFAULT '0',
                        `is13thMoAcct` int unsigned NOT NULL DEFAULT '0',
                        `isextloan` int unsigned NOT NULL DEFAULT '0',
                        `ExtLoanPercentage` decimal(5,4) DEFAULT '0.0000',
                        `status_` tinyint unsigned NOT NULL DEFAULT '1',
                        `customRate` double(12,4) NOT NULL DEFAULT '0.0000'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Chartofacct170118(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`chartofacct170118` (
                        `AcctNumber` varchar(5) NOT NULL,
                        `AcctName` varchar(60) NOT NULL DEFAULT ' ',
                        `AcctType` varchar(1) NOT NULL DEFAULT ' ',
                        `isTaxable` varchar(1) NOT NULL DEFAULT '0',
                        `isYTDAcct` varchar(1) NOT NULL DEFAULT '0',
                        `isTaxExcl` varchar(1) NOT NULL DEFAULT '0',
                        `islock` varchar(1) NOT NULL DEFAULT '0',
                        `ischargeable` varchar(1) NOT NULL DEFAULT '0',
                        `hasRateOverBasic` varchar(1) NOT NULL DEFAULT '0',
                        `isOthers` varchar(1) NOT NULL DEFAULT '0',
                        `isFixed` varchar(1) NOT NULL DEFAULT '0',
                        `timedMode` int unsigned NOT NULL DEFAULT '0' COMMENT '0->NOT TIMED,1->/HR, 2->/DAY',
                        `shortDesc` varchar(10) NOT NULL DEFAULT ' ',
                        `show01` int unsigned NOT NULL DEFAULT '0',
                        `sort` int unsigned NOT NULL DEFAULT '999',
                        `special_` int unsigned NOT NULL DEFAULT '0' COMMENT '1->TAX,2->ECOLA',
                        `show02` int unsigned NOT NULL DEFAULT '0',
                        `DedSort` int unsigned NOT NULL DEFAULT '999',
                        `isTH` int unsigned NOT NULL DEFAULT '0',
                        `Deferd` int unsigned NOT NULL DEFAULT '0',
                        `isOT` int unsigned NOT NULL DEFAULT '0',
                        `isMealAcct` int unsigned NOT NULL DEFAULT '0',
                        `formula` varchar(45) NOT NULL DEFAULT '''',
                        `OTrate` double(7,4) NOT NULL DEFAULT '1.0000',
                        `withSSS` int unsigned NOT NULL DEFAULT '0',
                        `withPHIC` int unsigned NOT NULL DEFAULT '0',
                        `withPagibig` int unsigned NOT NULL DEFAULT '0',
                        `ceiling` double(8,2) NOT NULL DEFAULT '0.00',
                        `isGovAcct` int unsigned NOT NULL DEFAULT '0',
                        `isLegalHoliday` int unsigned NOT NULL DEFAULT '0',
                        `is13thMoAcct` int unsigned NOT NULL DEFAULT '0',
                        `isextloan` int unsigned NOT NULL DEFAULT '0',
                        `ExtLoanPercentage` decimal(5,4) DEFAULT '0.0000',
                        `status_` tinyint unsigned NOT NULL DEFAULT '1',
                        `customRate` double(12,4) NOT NULL DEFAULT '0.0000'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01ChartofacctArchived(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`chartofacct_archived` (
                        `AcctNumber` varchar(5) NOT NULL,
                        `AcctName` varchar(60) NOT NULL DEFAULT ' ',
                        `AcctType` varchar(1) NOT NULL DEFAULT ' ',
                        `isTaxable` varchar(1) NOT NULL DEFAULT '0',
                        `isYTDAcct` varchar(1) NOT NULL DEFAULT '0',
                        `isTaxExcl` varchar(1) NOT NULL DEFAULT '0',
                        `islock` varchar(1) NOT NULL DEFAULT '0',
                        `ischargeable` varchar(1) NOT NULL DEFAULT '0',
                        `hasRateOverBasic` varchar(1) NOT NULL DEFAULT '0',
                        `isOthers` int unsigned NOT NULL DEFAULT '1',
                        `isFixed` varchar(1) NOT NULL DEFAULT '0',
                        `timedMode` int unsigned NOT NULL DEFAULT '0' COMMENT '0->NOT TIMED,1->/HR, 2->/DAY',
                        `shortDesc` varchar(10) NOT NULL DEFAULT ' ',
                        `show01` int unsigned NOT NULL DEFAULT '0',
                        `sort` int unsigned NOT NULL DEFAULT '999',
                        `special_` int unsigned NOT NULL DEFAULT '0' COMMENT '1->TAX,2->ECOLA',
                        `show02` int unsigned NOT NULL DEFAULT '0',
                        `DedSort` int unsigned NOT NULL DEFAULT '999',
                        `isTH` int unsigned NOT NULL DEFAULT '0',
                        `Deferd` int unsigned NOT NULL DEFAULT '0',
                        `isOT` int unsigned NOT NULL DEFAULT '0',
                        `isMealAcct` int unsigned NOT NULL DEFAULT '0',
                        `formula` varchar(45) NOT NULL DEFAULT '''',
                        `OTrate` double(7,4) NOT NULL DEFAULT '1.0000',
                        `withSSS` int unsigned NOT NULL DEFAULT '0',
                        `withPHIC` int unsigned NOT NULL DEFAULT '0',
                        `withPagibig` int unsigned NOT NULL DEFAULT '0',
                        `ceiling` double(8,2) NOT NULL DEFAULT '0.00',
                        `isGovAcct` int unsigned NOT NULL DEFAULT '0',
                        `isLegalHoliday` int unsigned NOT NULL DEFAULT '0',
                        `is13thMoAcct` int unsigned NOT NULL DEFAULT '0',
                        `isextloan` int unsigned NOT NULL DEFAULT '0',
                        `ExtLoanPercentage` decimal(5,4) DEFAULT '0.0000',
                        `status_` tinyint unsigned NOT NULL DEFAULT '1',
                        `customRate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `Taxable_Type` int unsigned DEFAULT '0',
                        `MWE_Type` int unsigned DEFAULT '0',
                        `TaxExptAmt` double(12,4) DEFAULT '0.0000',
                        PRIMARY KEY (`AcctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Coaconfig(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`coaconfig` (
                        `keyword` varchar(25) NOT NULL,
                        `AcctNumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`keyword`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Coamatching(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`coamatching` (
                        `idcoaMatching` int unsigned NOT NULL AUTO_INCREMENT,
                        `acctg_acct` char(5) NOT NULL,
                        `pay_acct` char(5) NOT NULL,
                        PRIMARY KEY (`idcoaMatching`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Dedmandatory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dedmandatory` (
                        `idDedMandatory` int unsigned NOT NULL AUTO_INCREMENT,
                        `acctNumber` varchar(45) DEFAULT NULL,
                        `ContAmt` double(12,4) DEFAULT NULL,
                        `MaxAmt` double(12,4) DEFAULT NULL,
                        `sched` int unsigned DEFAULT NULL COMMENT '1 - 1 to 15; 2 - 16 to end of month; 3 - both ',
                        `remarks` varchar(200) DEFAULT ' ',
                        `status` char(1) DEFAULT 'A' COMMENT 'A - Active, I - InActive; S - Stop',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        PRIMARY KEY (`idDedMandatory`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01DedmandatoryTrans(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dedmandatory_trans` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `amount` double(12,4) DEFAULT '0.0000',
                        `islock` int unsigned DEFAULT '0',
                        PRIMARY KEY (`trn`,`empnumber`,`acctnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Dedmandatoryexemption(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dedmandatoryexemption` (
                        `IdDedMandatory` int unsigned NOT NULL AUTO_INCREMENT,
                        `ClNumber` char(5) NOT NULL,
                        PRIMARY KEY (`IdDedMandatory`,`ClNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Dedn(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dedn` (
                        `DEDNCODE` varchar(2) DEFAULT NULL,
                        `DEDNDESC` varchar(20) DEFAULT NULL,
                        `SHOWN` varchar(1) DEFAULT NULL,
                        `FREQ` double(1,0) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Deductiondressing(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`deductiondressing` (
                        `iddeductionDressing` int unsigned NOT NULL AUTO_INCREMENT,
                        `AcctNumber` char(5) DEFAULT NULL,
                        PRIMARY KEY (`iddeductionDressing`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Domainaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`domainaccess` (
                        `iduserAccess` int unsigned NOT NULL AUTO_INCREMENT,
                        `username` varchar(25) DEFAULT NULL,
                        `idsysmenu` int unsigned DEFAULT NULL,
                        `module` char(10) DEFAULT NULL,
                        PRIMARY KEY (`iduserAccess`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Earn(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`earn` (
                        `EARNCODE` varchar(2) DEFAULT NULL,
                        `EARNDESC` varchar(15) DEFAULT NULL,
                        `SHOWN` varchar(1) DEFAULT NULL,
                        `TAXABLE` varchar(1) DEFAULT NULL,
                        `GOVNDEDN` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empdedexemption(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empdedexemption` (
                        `idempDedExemption` int unsigned NOT NULL AUTO_INCREMENT,
                        `clnumber` char(10) NOT NULL,
                        `acctnumber` char(5) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        PRIMARY KEY (`idempDedExemption`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empmasprevsal(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmasprevsal` (
                        `empNumber` varchar(5) NOT NULL DEFAULT '',
                        `regAssignedTo` varchar(5) DEFAULT NULL,
                        `prevGrossCompInc` double(9,2) NOT NULL DEFAULT '0.00',
                        `basicSMW` double(9,2) NOT NULL DEFAULT '0.00',
                        `holidayPay` double(9,2) NOT NULL DEFAULT '0.00',
                        `OTPay` double(9,2) NOT NULL DEFAULT '0.00',
                        `nightDiff` double(9,2) NOT NULL DEFAULT '0.00',
                        `hazardPay` double(9,2) NOT NULL DEFAULT '0.00',
                        `e13thMoNTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `deMiniMisNTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `sssPhicEtcCOntNTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `salOtherCompNTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `e13thMoTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `salOtherCompTaxable` double(9,2) NOT NULL DEFAULT '0.00',
                        `efrom` date NOT NULL DEFAULT '0000-00-00',
                        `eto` date NOT NULL DEFAULT '0000-00-00',
                        `taxwithheld_prev` double(9,2) NOT NULL DEFAULT '0.00',
                        PRIMARY KEY (`empNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empstatpisval(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstatpisval` (
                        `CODE` varchar(2) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01FixedEarnings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`fixed_earnings` (
                        `idfixed_earnings` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` char(5) NOT NULL,
                        `dStart` date DEFAULT '0000-00-00',
                        `dEnd` date DEFAULT '0000-00-00',
                        `acctNumber` char(5) DEFAULT '',
                        `amount` double(18,4) DEFAULT '0.0000',
                        `idsched` int unsigned DEFAULT '0' COMMENT '1 - per pay, 2 - every first period, 3 - every 2nd period',
                        `createdby` char(25) DEFAULT '',
                        `terminatedby` char(25) DEFAULT '',
                        `dayspara` double(18,2) DEFAULT '15.00',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        `trnPosted` char(12) DEFAULT '',
                        PRIMARY KEY (`idfixed_earnings`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01FixedEarningsGrp(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`fixed_earnings_grp` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `clnumber` char(5) NOT NULL,
                        `dStart` date DEFAULT NULL,
                        `dEnd` date DEFAULT NULL,
                        `acctNumber` char(5) DEFAULT '',
                        `amount` double(18,4) DEFAULT '0.0000',
                        `createdby` char(25) DEFAULT '',
                        `terminatedby` char(25) DEFAULT '',
                        `dayspara` double(18,2) DEFAULT '15.00',
                        `p1` int DEFAULT '0',
                        `p2` int DEFAULT '0',
                        `p3` int DEFAULT '0',
                        `p4` int DEFAULT '0',
                        `p5` int DEFAULT '0',
                        `perdayearnings` int DEFAULT '0',
                        `trnPosted` char(12) DEFAULT '',
                        `desig_` varchar(5) DEFAULT NULL,
                        `PosCode` varchar(5) DEFAULT NULL,
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Holiday(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`holiday` (
                        `H_DATE` date DEFAULT NULL,
                        `H_DESC` varchar(20) DEFAULT NULL,
                        `H_CODE` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationacct(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationacct` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `savingsacct` char(5) DEFAULT '',
                        `fund` double(12,2) DEFAULT '0.00',
                        `amort` double(12,2) DEFAULT '0.00',
                        `isdefault` tinyint unsigned DEFAULT '0',
                        `remarks` varchar(250) DEFAULT ' ',
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationdtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationdtl` (
                        `year` smallint unsigned NOT NULL DEFAULT '0',
                        `mo` char(2) NOT NULL DEFAULT '',
                        `prd` char(2) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `ded` double(12,2) DEFAULT '0.00',
                        `trnposted` char(12) DEFAULT '',
                        PRIMARY KEY (`year`,`mo`,`prd`,`acctnumber`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationdtlrchieved(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationdtlrchieved` (
                        `year` smallint unsigned NOT NULL DEFAULT '0',
                        `mo` char(2) NOT NULL DEFAULT '',
                        `prd` char(2) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `ded` double(12,2) DEFAULT '0.00',
                        `trnposted` char(11) DEFAULT '',
                        PRIMARY KEY (`year`,`mo`,`prd`,`acctnumber`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationgrp(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationgrp` (
                        `empnumber` char(5) NOT NULL,
                        `acctnumber` char(5) NOT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationhdr` (
                        `year` int unsigned NOT NULL,
                        `mo` char(2) NOT NULL DEFAULT '',
                        `prd` char(2) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `ded` double(12,4) DEFAULT '0.0000',
                        `totded` double(16,4) DEFAULT '0.0000',
                        `isdefault` smallint unsigned DEFAULT '0',
                        PRIMARY KEY (`year`,`mo`,`prd`,`acctnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationsavings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationsavings` (
                        `empnumber` char(5) NOT NULL,
                        `saving` double(12,4) DEFAULT '0.0000',
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationstatus` (
                        `code` varchar(5) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hospitalizationtrans(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hospitalizationtrans` (
                        `year` smallint unsigned NOT NULL DEFAULT '0',
                        `mo` char(2) NOT NULL DEFAULT '',
                        `prd` char(2) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `ded` double(12,2) DEFAULT '0.00',
                        `trnposted` char(11) DEFAULT '',
                        PRIMARY KEY (`year`,`mo`,`prd`,`acctnumber`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Hostpitalizationdedsched(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`hostpitalizationdedsched` (
                        `year` int unsigned NOT NULL DEFAULT '0',
                        `month` char(2) NOT NULL DEFAULT '',
                        `pp` char(2) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `amort` double(12,2) DEFAULT '0.00',
                        PRIMARY KEY (`year`,`month`,`pp`,`acctnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Idissuance(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`idissuance` (
                        `idIDIssuance` int unsigned NOT NULL AUTO_INCREMENT,
                        `empno` char(10) NOT NULL,
                        `requested` datetime DEFAULT NULL,
                        `issued` datetime DEFAULT NULL,
                        PRIMARY KEY (`idIDIssuance`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Incentiveleave(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`incentiveleave` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `acctNumber` char(5) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `amount` double(18,4) NOT NULL DEFAULT '0.0000',
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        PRIMARY KEY (`trn`,`acctNumber`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loandtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loandtl` (
                        `idloanDtl` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `trn` char(12) DEFAULT NULL,
                        `amount` double(18,4) NOT NULL,
                        `acctnumber` varchar(5) NOT NULL,
                        `loandate` date NOT NULL,
                        `amort` double(18,4) NOT NULL,
                        `balance` double(18,4) NOT NULL,
                        PRIMARY KEY (`idloanDtl`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loanhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loanhdr` (
                        `idloanhdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `paydate` date NOT NULL,
                        `yr` char(4) NOT NULL,
                        `mo` char(2) NOT NULL,
                        `amount` double(18,4) NOT NULL,
                        `remarks` varchar(100) DEFAULT NULL,
                        `acctcode` varchar(5) DEFAULT NULL,
                        PRIMARY KEY (`idloanhdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loans(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loans` (
                        `NUMBER` int unsigned NOT NULL AUTO_INCREMENT,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `DEDNDESC` varchar(20) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `Principal` double(12,4) DEFAULT '0.0000',
                        `MoToPay` int unsigned DEFAULT '0',
                        `InterestRate` double(12,4) DEFAULT '0.0000',
                        `InterestAmt` double(12,4) DEFAULT '0.0000',
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `STATUS` varchar(1) DEFAULT NULL,
                        `ENCODEBY` varchar(10) DEFAULT NULL,
                        `ENCODEDT` varchar(20) DEFAULT NULL,
                        `CHANGEBY` varchar(10) DEFAULT NULL,
                        `CHANGEDT` varchar(20) DEFAULT NULL,
                        `POSTED` varchar(1) DEFAULT NULL,
                        `EMPLASTNM` varchar(15) DEFAULT NULL,
                        `POSTFLAG` varchar(24) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `payMode` varchar(2) NOT NULL DEFAULT '',
                        `payStart` date NOT NULL DEFAULT '0000-00-00',
                        `payRes` date NOT NULL DEFAULT '0000-00-00',
                        `cvno` char(15) NOT NULL DEFAULT '',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        `TRNLastPosted` char(12) DEFAULT '',
                        PRIMARY KEY (`NUMBER`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loans18(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loans18` (
                        `NUMBER` int unsigned NOT NULL DEFAULT '0',
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `DEDNDESC` varchar(20) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `STATUS` varchar(1) DEFAULT NULL,
                        `ENCODEBY` varchar(10) DEFAULT NULL,
                        `ENCODEDT` varchar(10) DEFAULT NULL,
                        `CHANGEBY` varchar(10) DEFAULT NULL,
                        `CHANGEDT` varchar(10) DEFAULT NULL,
                        `POSTED` varchar(1) DEFAULT NULL,
                        `EMPLASTNM` varchar(15) DEFAULT NULL,
                        `POSTFLAG` varchar(24) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `payMode` varchar(2) NOT NULL DEFAULT '',
                        `payStart` date NOT NULL DEFAULT '0000-00-00',
                        `payRes` date NOT NULL DEFAULT '0000-00-00',
                        `cvno` char(15) NOT NULL DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loansacctaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loansacctaccess` (
                        `username` varchar(25) NOT NULL,
                        `AcctNumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`username`,`AcctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loansref(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loansref` (
                        `NUMBER` int unsigned DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `TRN` varchar(12) DEFAULT NULL,
                        `CONTROL_NO` int unsigned NOT NULL AUTO_INCREMENT,
                        `LoanAmt` double(12,2) DEFAULT '0.00',
                        `cvno` int unsigned DEFAULT '0',
                        PRIMARY KEY (`CONTROL_NO`)
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loansref1801(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loansref1801` (
                        `NUMBER` int unsigned DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `TRN` varchar(12) DEFAULT NULL,
                        `CONTROL_NO` int unsigned NOT NULL DEFAULT '0',
                        `LoanAmt` double(12,2) DEFAULT '0.00',
                        `cvno` int unsigned DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loansref1802(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loansref1802` (
                        `NUMBER` int unsigned DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `TRN` varchar(12) DEFAULT NULL,
                        `CONTROL_NO` int unsigned NOT NULL DEFAULT '0',
                        `LoanAmt` double(12,2) DEFAULT '0.00',
                        `cvno` int unsigned DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01LoansrefRepAccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loansref_rep_access` (
                        `empnumber` varchar(25) NOT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mealallowopt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mealallowopt` (
                        `empnumber` varchar(10) NOT NULL,
                        `MealAllow` double(18,2) NOT NULL,
                        `isMgr` int unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Medtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`medtbl` (
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) DEFAULT NULL,
                        `FMEDEE` double(6,2) DEFAULT NULL,
                        `FMEDER` double(6,2) DEFAULT NULL,
                        `percent` double(5,2) DEFAULT '2.75',
                        PRIMARY KEY (`FSTART`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Menubars(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`menubars` (
                        `code` varchar(5) NOT NULL,
                        `command_` varchar(254) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Notminimum(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`notminimum` (
                        `EMPNUMBER` varchar(5) NOT NULL DEFAULT '',
                        PRIMARY KEY (`EMPNUMBER`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Pagibigpaydtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`pagibigpaydtl` (
                        `idPagIbigPayDtl` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `trn` char(12) DEFAULT NULL,
                        `EES` double(18,4) NOT NULL,
                        `ERS` double(18,4) NOT NULL,
                        PRIMARY KEY (`idPagIbigPayDtl`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Pagibigpayhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`pagibigpayhdr` (
                        `idpagibigPayHdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `paydate` date NOT NULL,
                        `yr` char(4) NOT NULL,
                        `mo` char(2) NOT NULL,
                        `amount` double(18,4) NOT NULL,
                        `remarks` varchar(100) DEFAULT NULL,
                        PRIMARY KEY (`idpagibigPayHdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Pagibigtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`pagibigtbl` (
                        `FSTART` double(8,2) NOT NULL DEFAULT '0.00',
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `EmpCont` double(6,2) DEFAULT NULL,
                        `Emplyr` double(6,2) DEFAULT NULL,
                        PRIMARY KEY (`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01PaygrpMovhistory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`paygrp_movhistory` (
                        `idpayGrp_movHistory` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` char(5) DEFAULT '',
                        `clnumber` char(5) DEFAULT '',
                        `datetime` datetime DEFAULT '0000-00-00 00:00:00',
                        `username` char(25) DEFAULT '',
                        `Action` char(1) DEFAULT '',
                        PRIMARY KEY (`idpayGrp_movHistory`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Paymaindtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`paymaindtl` (
                        `trn` char(15) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `SSS` double(16,4) NOT NULL DEFAULT '0.0000',
                        `SSSer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibig` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibiger` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phic` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phicer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `paystatus` char(10) NOT NULL DEFAULT '0',
                        `coid` char(10) NOT NULL DEFAULT '0',
                        `brid` char(10) NOT NULL DEFAULT '0',
                        `depid` char(10) NOT NULL DEFAULT '0',
                        `secid` char(10) NOT NULL DEFAULT '0',
                        `daywrk` double(12,4) NOT NULL DEFAULT '0.0000',
                        `absent` double(12,4) NOT NULL DEFAULT '0.0000',
                        `late` double(12,4) NOT NULL DEFAULT '0.0000',
                        `utime` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rn` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rnot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `shot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sea` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sealh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `seardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpa` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpalh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom1` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom2` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom3` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndrd` double(12,4) DEFAULT '0.0000',
                        `ndrdot` double(12,4) DEFAULT '0.0000',
                        `ndrdlh` double(12,4) DEFAULT '0.0000',
                        `ndrdlhot` double(12,4) DEFAULT '0.0000',
                        `ndrdsh` double(12,4) DEFAULT '0.0000',
                        `ndrdshot` double(12,4) DEFAULT '0.0000',
                        `nd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`trn`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Paymaindtlsetup(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`paymaindtlsetup` (
                        `idpaymaindtlsetup` int unsigned NOT NULL AUTO_INCREMENT,
                        `daywrk` char(5) NOT NULL DEFAULT '',
                        `absent` char(5) NOT NULL DEFAULT '',
                        `late` char(5) NOT NULL DEFAULT '',
                        `utime` char(5) NOT NULL DEFAULT '',
                        `rn` char(5) NOT NULL DEFAULT '',
                        `rnot` char(5) NOT NULL DEFAULT '',
                        `rot` char(5) NOT NULL DEFAULT '',
                        `rd` char(5) NOT NULL DEFAULT '',
                        `rdot` char(5) NOT NULL DEFAULT '',
                        `lh` char(5) NOT NULL DEFAULT '',
                        `lhot` char(5) NOT NULL DEFAULT '',
                        `rdlh` char(5) NOT NULL DEFAULT '',
                        `rdlhot` char(5) NOT NULL DEFAULT '',
                        `sh` char(5) NOT NULL DEFAULT '',
                        `shot` char(5) NOT NULL DEFAULT '',
                        `rdsh` char(5) NOT NULL DEFAULT '',
                        `rdshot` char(5) NOT NULL DEFAULT '',
                        `sea` char(5) NOT NULL DEFAULT '',
                        `sealh` char(5) NOT NULL DEFAULT '',
                        `seardlh` char(5) NOT NULL DEFAULT '',
                        `ctpa` char(5) NOT NULL DEFAULT '',
                        `ctpalh` char(5) NOT NULL DEFAULT '',
                        `ctpardlh` char(5) NOT NULL DEFAULT '',
                        `sss` char(5) NOT NULL,
                        `SSSer` char(5) NOT NULL DEFAULT '',
                        `pagibig` char(5) NOT NULL DEFAULT '',
                        `pagibiger` char(5) NOT NULL DEFAULT '',
                        `phic` char(5) NOT NULL DEFAULT '',
                        `custom1` char(5) NOT NULL DEFAULT '',
                        `custom2` char(5) NOT NULL DEFAULT '',
                        `custom3` char(5) NOT NULL DEFAULT '',
                        `Allowance` char(5) NOT NULL DEFAULT '',
                        `CashBond` char(5) NOT NULL DEFAULT '',
                        `IncentLeave` char(5) NOT NULL DEFAULT '',
                        `mealAllowance` char(5) NOT NULL DEFAULT '',
                        `ProvForRetirement` char(5) NOT NULL DEFAULT '',
                        `FreeUniform` char(5) NOT NULL DEFAULT '',
                        `_13thMo` char(5) NOT NULL,
                        `nd` char(5) NOT NULL,
                        `ndot` char(5) NOT NULL,
                        `ndrd` char(5) NOT NULL,
                        `ndrdot` char(5) NOT NULL,
                        `ndlh` char(5) NOT NULL,
                        `ndlhot` char(5) NOT NULL,
                        `ndrdlh` char(5) NOT NULL,
                        `ndrdlhot` char(5) NOT NULL,
                        `ndsh` char(5) NOT NULL,
                        `ndshot` char(5) NOT NULL,
                        `DH` char(5) DEFAULT '',
                        `DHOT` char(5) DEFAULT '',
                        `RDDH` char(5) DEFAULT '',
                        `RDDHOT` char(5) DEFAULT '',
                        `NDDH` char(5) DEFAULT '',
                        `NDDHOT` char(5) DEFAULT '',
                        `NDRDDH` char(5) DEFAULT '',
                        `NDRDDHOT` char(5) DEFAULT '',
                        `ndrdsh` char(5) NOT NULL,
                        `ndrdshot` char(5) NOT NULL,
                        `retropay` char(5) DEFAULT '',
                        PRIMARY KEY (`idpaymaindtlsetup`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Paymainhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`paymainhdr` (
                        `trn` char(12) NOT NULL,
                        `clrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `minrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `withSEA` int unsigned NOT NULL DEFAULT '0',
                        `SEARate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `withCTPA` int unsigned NOT NULL DEFAULT '0',
                        `CTPARate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ecolaRevised` int unsigned NOT NULL DEFAULT '0',
                        `billrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `user` char(25) NOT NULL DEFAULT '',
                        `status` char(10) NOT NULL DEFAULT 'New',
                        `datecreated` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `dateposted` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `attStart` date NOT NULL DEFAULT '0000-00-00',
                        `attEnd` date NOT NULL DEFAULT '0000-00-00',
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Payrollgrp(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`payrollgrp` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Code` char(5) DEFAULT NULL,
                        `ClNumber` char(5) DEFAULT '',
                        `Name` varchar(80) DEFAULT NULL,
                        `RatePerHr` double(12,4) DEFAULT '0.0000',
                        `RatePerDay` double(12,4) DEFAULT '0.0000',
                        `RatePerMonth` double(12,4) DEFAULT '0.0000',
                        `RatePerYr` double(12,4) DEFAULT '0.0000',
                        `MinDailyRate` double(12,4) DEFAULT '0.0000',
                        `Status` char(1) DEFAULT 'A',
                        `PayRateId` int DEFAULT '0',
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Payrollprd(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`payrollprd` (
                        `pyear` char(4) NOT NULL,
                        `pmonth` char(2) NOT NULL,
                        `prd` char(2) NOT NULL,
                        `attStart` date DEFAULT NULL,
                        `attEnd` date DEFAULT NULL,
                        PRIMARY KEY (`pyear`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Pepsiemplyee(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`pepsiemplyee` (
                        `empnumber` varchar(5) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Period(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`period` (
                        `PdNumber` varchar(2) NOT NULL,
                        `PdName` varchar(45) NOT NULL,
                        PRIMARY KEY (`PdNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Phicpaydtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`phicpaydtl` (
                        `idPHICPayDtl` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `trn` char(12) DEFAULT NULL,
                        `EES` double(18,4) NOT NULL,
                        `ERS` double(18,4) NOT NULL,
                        PRIMARY KEY (`idPHICPayDtl`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Phicpayhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`phicpayhdr` (
                        `idPHICPayHdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `paydate` date NOT NULL,
                        `yr` char(4) NOT NULL,
                        `mo` char(2) NOT NULL,
                        `amount` double(18,4) NOT NULL,
                        `remarks` varchar(100) DEFAULT NULL,
                        PRIMARY KEY (`idPHICPayHdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01PisvalEmpdeploymenthist(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`pisval_empdeploymenthist` (
                        `TRN` char(12) NOT NULL,
                        `EmpNumber` char(5) NOT NULL,
                        `Client_` char(5) DEFAULT '',
                        `EmpStat_` char(1) DEFAULT NULL,
                        `lockBy` varchar(25) DEFAULT ' ',
                        `TimeStamp` datetime DEFAULT NULL,
                        PRIMARY KEY (`TRN`,`EmpNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Quitclaimcoa(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`quitclaimcoa` (
                        `idQuitClaimCOA` int unsigned NOT NULL AUTO_INCREMENT,
                        `grp` char(15) DEFAULT NULL,
                        `acctNumber` char(5) DEFAULT NULL,
                        PRIMARY KEY (`idQuitClaimCOA`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ratesamt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ratesamt` (
                        `clnumber` char(5) NOT NULL DEFAULT '',
                        `basic` double(12,2) DEFAULT '0.00',
                        `rot` double(12,2) DEFAULT '0.00',
                        `lh` double(12,2) DEFAULT '0.00',
                        `lhot` double(12,2) DEFAULT '0.00',
                        `sh` double(12,2) DEFAULT '0.00',
                        `shot` double(12,2) DEFAULT '0.00',
                        `dh` double(12,2) DEFAULT '0.00',
                        `dhot` double(12,2) DEFAULT '0.00',
                        `rd` double(12,2) DEFAULT '0.00',
                        `rdot` double(12,2) DEFAULT '0.00',
                        `rdlh` double(12,2) DEFAULT '0.00',
                        `rdlhot` double(12,2) DEFAULT '0.00',
                        `rdsh` double(12,2) DEFAULT '0.00',
                        `rdshot` double(12,2) DEFAULT '0.00',
                        `rddh` double(12,2) DEFAULT '0.00',
                        `rddhot` double(12,2) DEFAULT '0.00',
                        PRIMARY KEY (`clnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Refpagibigtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`refpagibigtbl` (
                        `yrstart` int NOT NULL DEFAULT '0',
                        `yrend` int NOT NULL DEFAULT '0',
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `FEE` double(6,2) DEFAULT NULL,
                        `FER` double(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL,
                        PRIMARY KEY (`yrstart`,`yrend`,`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Refpgibigtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`refpgibigtbl` (
                        `yrstart` int NOT NULL DEFAULT '0',
                        `yrend` int NOT NULL DEFAULT '0',
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `FSSSEE` double(6,2) DEFAULT NULL,
                        `FSSSER` double(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL,
                        PRIMARY KEY (`yrstart`,`yrend`,`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Refphictbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`refphictbl` (
                        `yrstart` int NOT NULL DEFAULT '0',
                        `yrend` int NOT NULL DEFAULT '0',
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `EE` double(6,2) DEFAULT NULL,
                        `ER` double(6,2) DEFAULT NULL,
                        `percent` double(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL,
                        PRIMARY KEY (`yrstart`,`yrend`,`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Refssstbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`refssstbl` (
                        `yrstart` int NOT NULL DEFAULT '0',
                        `yrend` int NOT NULL DEFAULT '0',
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `EE` double(6,2) DEFAULT NULL,
                        `ER` double(16,2) DEFAULT NULL,
                        `ECC` decimal(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL,
                        PRIMARY KEY (`yrstart`,`yrend`,`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Retirementopt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`retirementopt` (
                        `empnumber` varchar(10) NOT NULL,
                        `withRetirement` int unsigned NOT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Retropay(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`retropay` (
                        `idretropay` int unsigned NOT NULL AUTO_INCREMENT,
                        `yr` int unsigned DEFAULT '0',
                        `mo` int unsigned DEFAULT '0',
                        `prd` char(2) DEFAULT '0',
                        `empnumber` char(5) DEFAULT '',
                        `acctNumber` char(5) DEFAULT '',
                        `amt` decimal(12,2) DEFAULT '0.00',
                        `remarks` char(15) DEFAULT NULL,
                        `isposted` int unsigned DEFAULT '0',
                        `clnumber` char(5) DEFAULT '',
                        PRIMARY KEY (`idretropay`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Salgrade(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`salgrade` (
                        `sgcode` char(10) NOT NULL,
                        `sgrade` varchar(45) NOT NULL,
                        `monthlyRate` double(12,4) NOT NULL,
                        `dailyRate` double(12,4) NOT NULL,
                        `wtax` int unsigned NOT NULL DEFAULT '0',
                        `wSSS` int unsigned NOT NULL DEFAULT '0',
                        `wGSIS` int unsigned NOT NULL DEFAULT '1',
                        `wPHIC` int unsigned NOT NULL DEFAULT '1',
                        `wPagibig` int unsigned NOT NULL DEFAULT '1',
                        `isLock` int unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`sgcode`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sbpl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sbpl` (
                        `empno` char(5) NOT NULL DEFAULT '',
                        PRIMARY KEY (`empno`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Semimonthlytax(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`semimonthlytax` (
                        `TaxCode` varchar(5) NOT NULL,
                        `SAmt` double(10,4) NOT NULL,
                        `EAmt` double(10,4) NOT NULL,
                        `Fix` double(10,4) NOT NULL,
                        `Percentage` double(10,4) NOT NULL,
                        `fff` int unsigned NOT NULL AUTO_INCREMENT,
                        PRIMARY KEY (`fff`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Semimonthlytaxold(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`semimonthlytaxold` (
                        `TaxCode` varchar(5) NOT NULL,
                        `SAmt` double(10,4) NOT NULL,
                        `EAmt` double(10,4) NOT NULL,
                        `Fix` double(10,4) NOT NULL,
                        `Percentage` double(10,4) NOT NULL,
                        `fff` int unsigned NOT NULL AUTO_INCREMENT,
                        PRIMARY KEY (`fff`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sssloandtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sssloandtl` (
                        `idsssLoanDtl` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `trn` char(12) DEFAULT NULL,
                        `amount` double(18,2) NOT NULL,
                        `amort` double(18,2) NOT NULL,
                        `balance` double(18,2) NOT NULL,
                        `paystart` date NOT NULL,
                        `penalty` double(18,2) NOT NULL,
                        PRIMARY KEY (`idsssLoanDtl`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sssloanhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sssloanhdr` (
                        `sssLoanHdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `paydate` date NOT NULL,
                        `yr` char(4) NOT NULL,
                        `mo` char(2) NOT NULL,
                        `amount` double(18,4) NOT NULL,
                        `remarks` varchar(100) DEFAULT NULL,
                        PRIMARY KEY (`sssLoanHdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sssprempaydtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sssprempaydtl` (
                        `idsssPremPayDtl` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `trn` char(12) DEFAULT NULL,
                        `ee` double(18,4) NOT NULL,
                        `er` double(18,4) NOT NULL,
                        `cc` double(18,4) NOT NULL,
                        `sal` double(12,4) DEFAULT '0.0000',
                        `stat` char(1) DEFAULT 'N',
                        PRIMARY KEY (`idsssPremPayDtl`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sssprempayhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sssprempayhdr` (
                        `idsssPremPayHdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `orno` char(15) NOT NULL,
                        `paydate` date NOT NULL,
                        `yr` char(4) NOT NULL,
                        `mo` char(2) NOT NULL,
                        `amount` double(18,4) NOT NULL,
                        `remarks` varchar(100) DEFAULT NULL,
                        PRIMARY KEY (`idsssPremPayHdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ssstbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ssstbl` (
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `FSSSEE` double(6,2) DEFAULT NULL,
                        `FSSSER` double(6,2) DEFAULT NULL,
                        `FECC` double(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL,
                        PRIMARY KEY (`FSTART`,`FEND`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ssstbl2025bak(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ssstbl_2025bak` (
                        `FSTART` double(8,2) NOT NULL,
                        `FEND` double(8,2) NOT NULL DEFAULT '0.00',
                        `FSSSEE` double(6,2) DEFAULT NULL,
                        `FSSSER` double(6,2) DEFAULT NULL,
                        `FECC` double(6,2) DEFAULT NULL,
                        `compensation` double(10,2) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sysmenu(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sysmenu` (
                        `idsysmenu` int unsigned NOT NULL AUTO_INCREMENT,
                        `sourcetype` char(15) DEFAULT NULL,
                        `source` varchar(45) DEFAULT NULL,
                        `name` varchar(80) DEFAULT NULL,
                        `dispval` varchar(80) DEFAULT NULL,
                        `barorder` int unsigned DEFAULT '99',
                        `priorityno` int unsigned DEFAULT NULL,
                        `Action` varchar(80) DEFAULT ' ',
                        `withchild` int unsigned DEFAULT '0',
                        PRIMARY KEY (`idsysmenu`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Taxmatrix(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`taxmatrix` (
                        `s_amt` double(10,4) NOT NULL DEFAULT '0.0000' COMMENT 'starting amount',
                        `e_amt` double(11,2) NOT NULL DEFAULT '0.00' COMMENT 'ending amount',
                        `fix` double(10,4) NOT NULL DEFAULT '0.0000' COMMENT 'fixed deduction',
                        `p_amt` double(10,4) NOT NULL DEFAULT '0.0000' COMMENT 'percentage',
                        `controlno` int unsigned NOT NULL AUTO_INCREMENT,
                        PRIMARY KEY (`controlno`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Taxrefund2012(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`taxrefund2012` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblatexemption(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblatexemption` (
                        `TAXCODE` varchar(5) NOT NULL,
                        `Exemption` double(10,2) NOT NULL,
                        PRIMARY KEY (`TAXCODE`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblbatch(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblbatch` (
                        `TRN` varchar(12) NOT NULL,
                        `Batch` varchar(45) NOT NULL,
                        PRIMARY KEY (`TRN`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblbilldtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblbilldtl` (
                        `hdrTRN` int unsigned NOT NULL,
                        `dtlTRN` int unsigned NOT NULL AUTO_INCREMENT,
                        `Description` varchar(254) NOT NULL,
                        `Qty` double(10,4) NOT NULL,
                        `Rate` double(10,4) NOT NULL,
                        `hrs` double(12,4) NOT NULL DEFAULT '0.0000',
                        `Amt` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`dtlTRN`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblbillhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblbillhdr` (
                        `TRN` int unsigned NOT NULL AUTO_INCREMENT,
                        `TransDate` date NOT NULL,
                        `ClNumber` varchar(5) NOT NULL,
                        `Total1` double(12,4) NOT NULL DEFAULT '0.0000',
                        `AOPercent` double(9,4) NOT NULL DEFAULT '0.0000',
                        `AOAmt` double(12,4) NOT NULL DEFAULT '0.0000',
                        `VatPercent` double(9,4) NOT NULL DEFAULT '0.0000',
                        `VatAmt` double(12,4) NOT NULL DEFAULT '0.0000',
                        `Total2` double(12,4) NOT NULL DEFAULT '0.0000',
                        `TaxPercent` double(9,4) NOT NULL DEFAULT '0.0000',
                        `TaxAmt` double(12,4) NOT NULL DEFAULT '0.0000',
                        `GTotal` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`TRN`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblclientposrate(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblclientposrate` (
                        `clNumber` varchar(5) NOT NULL,
                        `PositionID` varchar(5) NOT NULL DEFAULT '',
                        `PosRate` double(10,4) NOT NULL DEFAULT '0.0000',
                        `RateType` double(1,0) NOT NULL DEFAULT '1',
                        `BillRate` double(10,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`clNumber`,`PositionID`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbldedperclient(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbldedperclient` (
                        `dedperclient` int unsigned NOT NULL AUTO_INCREMENT,
                        `CLNUMBER` varchar(5) NOT NULL,
                        `YYMMPP` varchar(6) NOT NULL DEFAULT '',
                        `AcctNumber` varchar(5) NOT NULL,
                        `Amt` double(10,2) NOT NULL,
                        PRIMARY KEY (`dedperclient`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbldefaultcontents(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbldefaultcontents` (
                        `year` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `period` varchar(2) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        PRIMARY KEY (`year`,`month`,`period`,`acctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbldeferd(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbldeferd` (
                        `Date` date NOT NULL DEFAULT '0000-00-00',
                        `AcctNumber` varchar(5) NOT NULL DEFAULT '',
                        `EmpNumber` varchar(5) NOT NULL DEFAULT '',
                        `Amt` double(10,4) NOT NULL DEFAULT '0.0000',
                        `DefID` int unsigned NOT NULL AUTO_INCREMENT,
                        PRIMARY KEY (`DefID`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblotherdedref(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblotherdedref` (
                        `trn` char(12) NOT NULL,
                        `year` int NOT NULL DEFAULT '0',
                        `month` int NOT NULL DEFAULT '0',
                        `period` char(2) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `amount` double(12,2) NOT NULL DEFAULT '0.00',
                        PRIMARY KEY (`year`,`month`,`period`,`empnumber`,`acctnumber`,`trn`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblotherdeductions(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblotherdeductions` (
                        `year` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `periodID` varchar(2) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `Processed` int unsigned NOT NULL DEFAULT '0',
                        `Remarks` varchar(45) NOT NULL DEFAULT '',
                        `contproc` int unsigned DEFAULT '0',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        `TRNPosted` char(12) DEFAULT '',
                        PRIMARY KEY (`year`,`acctNumber`,`empNumber`,`periodID`,`month`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TblotherdeductionsResched(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblotherdeductions_resched` (
                        `year` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `periodID` varchar(2) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `Processed` int unsigned NOT NULL DEFAULT '0',
                        `Remarks` varchar(45) NOT NULL DEFAULT '',
                        `contproc` int unsigned DEFAULT '0',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        `nyear` int unsigned NOT NULL,
                        `nmonth` int unsigned NOT NULL,
                        `np1` int unsigned DEFAULT '0',
                        `np2` int unsigned DEFAULT '0',
                        `np3` int unsigned DEFAULT '0',
                        `np4` int unsigned DEFAULT '0',
                        `np5` int unsigned DEFAULT '0',
                        `action` char(8) DEFAULT '',
                        `user` char(25) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblotherearnings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblotherearnings` (
                        `year` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `periodID` varchar(2) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `processed` int unsigned NOT NULL DEFAULT '0',
                        `Remarks` varchar(45) DEFAULT NULL,
                        `CONTROL_NO` int unsigned NOT NULL AUTO_INCREMENT,
                        `effective` date DEFAULT '0000-00-00',
                        `contproc` int unsigned DEFAULT '0',
                        `p1` int unsigned DEFAULT '0',
                        `p2` int unsigned DEFAULT '0',
                        `p3` int unsigned DEFAULT '0',
                        `p4` int unsigned DEFAULT '0',
                        `p5` int unsigned DEFAULT '0',
                        `trnposted` char(12) DEFAULT '',
                        PRIMARY KEY (`CONTROL_NO`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblperrate(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblperrate` (
                        `rateID` int unsigned NOT NULL,
                        `perRate` varchar(30) NOT NULL,
                        PRIMARY KEY (`rateID`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblroblink(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblroblink` (
                        `acctNumber` varchar(5) NOT NULL DEFAULT '',
                        `ROB` double(6,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`acctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' ',
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`,`source`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran110801(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran110801` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00'
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran130129(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran130129` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran180602(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran180602` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' '
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran20180502(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran20180502` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran20191001(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran20191001` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' '
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran20191001dtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran20191001dtl` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL,
                        `uom` char(10) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltran20230920(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran20230920` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' '
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TbltranArchived(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltran_archived` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltranajinomotomakati(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltranajinomotomakati` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltranarchieved(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltranarchieved` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' ',
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandeployment(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandeployment` (
                        `trn` char(10) NOT NULL DEFAULT '',
                        `clnumber` char(5) NOT NULL,
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandtl` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL,
                        `uom` char(10) DEFAULT '',
                        PRIMARY KEY (`TRN`,`EmpNumber`,`DtlCd`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandtl1(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandtl1` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandtlajinomotomakati(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandtlajinomotomakati` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL,
                        `uom` char(10) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandtlx(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandtlx` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrandtlx1(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrandtlx1` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL,
                        PRIMARY KEY (`TRN`,`EmpNumber`,`DtlCd`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltranerro(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltranerro` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `source` char(5) NOT NULL DEFAULT '',
                        `postedby` varchar(25) DEFAULT ' '
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltransource(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltransource` (
                        `trn` char(12) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `acctNumber` char(5) NOT NULL,
                        `amount` double(12,4) NOT NULL,
                        `source` char(20) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltrantemper(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltrantemper` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tbltranx(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbltranx` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00'
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Temptotalhrs(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`temptotalhrs` (
                        `trn` varchar(12) NOT NULL,
                        `EmpNumber` varchar(45) NOT NULL,
                        PRIMARY KEY (`trn`,`EmpNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TmpdedmandatoryTrans(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmpdedmandatory_trans` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `acctnumber` char(5) NOT NULL DEFAULT '',
                        `amount` double(12,4) DEFAULT '0.0000',
                        `islock` int unsigned DEFAULT '0',
                        PRIMARY KEY (`trn`,`empnumber`,`acctnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TmpfixedEarnings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmpfixed_earnings` (
                        `idfixed_earnings` int unsigned NOT NULL,
                        `trn` char(12) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmploans(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmploans` (
                        `NUMBER` int unsigned NOT NULL DEFAULT '0',
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `DEDNCODE` varchar(4) DEFAULT NULL,
                        `DEDNDESC` varchar(20) DEFAULT NULL,
                        `AMOUNT` double(8,2) DEFAULT NULL,
                        `AMORT` double(7,2) DEFAULT NULL,
                        `BALANCE` double(8,2) DEFAULT NULL,
                        `STATUS` varchar(1) DEFAULT NULL,
                        `ENCODEBY` varchar(10) DEFAULT NULL,
                        `ENCODEDT` varchar(20) DEFAULT NULL,
                        `CHANGEBY` varchar(10) DEFAULT NULL,
                        `CHANGEDT` varchar(20) DEFAULT NULL,
                        `POSTED` varchar(1) DEFAULT NULL,
                        `EMPLASTNM` varchar(15) DEFAULT NULL,
                        `POSTFLAG` varchar(24) DEFAULT NULL,
                        `REMARKS` varchar(75) DEFAULT NULL,
                        `payMode` varchar(2) NOT NULL DEFAULT '',
                        `payStart` date NOT NULL DEFAULT '0000-00-00',
                        `payRes` date NOT NULL DEFAULT '0000-00-00',
                        `cvno` char(15) NOT NULL DEFAULT '',
                        `trn` char(12) DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmppaymaindtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmppaymaindtl` (
                        `trn` char(15) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `SSS` double(16,4) NOT NULL DEFAULT '0.0000',
                        `SSSer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibig` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibiger` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phic` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phicer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `paystatus` char(10) NOT NULL DEFAULT '0',
                        `coid` char(10) NOT NULL DEFAULT '0',
                        `brid` char(10) NOT NULL DEFAULT '0',
                        `depid` char(10) NOT NULL DEFAULT '0',
                        `secid` char(10) NOT NULL DEFAULT '0',
                        `daywrk` double(12,4) NOT NULL DEFAULT '0.0000',
                        `absent` double(12,4) NOT NULL DEFAULT '0.0000',
                        `late` double(12,4) NOT NULL DEFAULT '0.0000',
                        `utime` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rn` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rnot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `shot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sea` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sealh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `seardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpa` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpalh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom1` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom2` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom3` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndrd` double(12,4) DEFAULT '0.0000',
                        `ndrdot` double(12,4) DEFAULT '0.0000',
                        `ndrdlh` double(12,4) DEFAULT '0.0000',
                        `ndrdlhot` double(12,4) DEFAULT '0.0000',
                        `ndrdsh` double(12,4) DEFAULT '0.0000',
                        `ndrdshot` double(12,4) DEFAULT '0.0000',
                        `nd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`trn`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmppaymaindtl01(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmppaymaindtl01` (
                        `trn` char(15) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `SSS` double(16,4) NOT NULL DEFAULT '0.0000',
                        `SSSer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibig` double(16,4) NOT NULL DEFAULT '0.0000',
                        `pagibiger` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phic` double(16,4) NOT NULL DEFAULT '0.0000',
                        `phicer` double(16,4) NOT NULL DEFAULT '0.0000',
                        `paystatus` char(10) NOT NULL DEFAULT '0',
                        `coid` char(10) NOT NULL DEFAULT '0',
                        `brid` char(10) NOT NULL DEFAULT '0',
                        `depid` char(10) NOT NULL DEFAULT '0',
                        `secid` char(10) NOT NULL DEFAULT '0',
                        `daywrk` double(12,4) NOT NULL DEFAULT '0.0000',
                        `absent` double(12,4) NOT NULL DEFAULT '0.0000',
                        `late` double(12,4) NOT NULL DEFAULT '0.0000',
                        `utime` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rn` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rnot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `lhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `shot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rdshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sea` double(12,4) NOT NULL DEFAULT '0.0000',
                        `sealh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `seardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpa` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpalh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ctpardlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom1` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom2` double(12,4) NOT NULL DEFAULT '0.0000',
                        `custom3` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndrd` double(12,4) DEFAULT '0.0000',
                        `ndrdot` double(12,4) DEFAULT '0.0000',
                        `ndrdlh` double(12,4) DEFAULT '0.0000',
                        `ndrdlhot` double(12,4) DEFAULT '0.0000',
                        `ndrdsh` double(12,4) DEFAULT '0.0000',
                        `ndrdshot` double(12,4) DEFAULT '0.0000',
                        `nd` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndlhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndsh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ndshot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `dhot` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddh` double(12,4) NOT NULL DEFAULT '0.0000',
                        `rddhot` double(12,4) NOT NULL DEFAULT '0.0000'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmppaymainhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmppaymainhdr` (
                        `trn` char(12) NOT NULL,
                        `clrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `minrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `withSEA` int unsigned NOT NULL DEFAULT '0',
                        `SEARate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `withCTPA` int unsigned NOT NULL DEFAULT '0',
                        `CTPARate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `ecolaRevised` int unsigned NOT NULL DEFAULT '0',
                        `billrate` double(12,4) NOT NULL DEFAULT '0.0000',
                        `user` char(25) NOT NULL DEFAULT '',
                        `status` char(10) NOT NULL DEFAULT 'New',
                        `datecreated` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `dateposted` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `attStart` date NOT NULL DEFAULT '0000-00-00',
                        `attEnd` date NOT NULL DEFAULT '0000-00-00',
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmpretropay(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmpretropay` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `empNumber` varchar(5) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL DEFAULT '',
                        `amount` double(10,4) DEFAULT NULL,
                        PRIMARY KEY (`trn`,`empNumber`,`acctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmptblotherdeductions(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmptblotherdeductions` (
                        `year` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `periodID` varchar(2) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `Processed` int unsigned NOT NULL DEFAULT '0',
                        `Remarks` varchar(45) NOT NULL DEFAULT '',
                        `trn` char(12) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmptblotherearnings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmptblotherearnings` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `control_no` int unsigned DEFAULT NULL,
                        `empNumber` varchar(5) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL DEFAULT '',
                        `amount` double(10,4) DEFAULT NULL,
                        PRIMARY KEY (`trn`,`empNumber`,`acctNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmptbltran(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmptbltran` (
                        `TRN` varchar(12) NOT NULL,
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
                        `postedBy` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`TRN`,`acctNumber`,`empNumber`) USING BTREE
                        ) ENGINE=MyISAM DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmptbltrandtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmptbltrandtl` (
                        `TRN` varchar(12) NOT NULL,
                        `EmpNumber` varchar(5) NOT NULL,
                        `DtlCd` varchar(10) NOT NULL,
                        `nVal` double(10,4) NOT NULL,
                        `uom` char(10) DEFAULT '',
                        PRIMARY KEY (`TRN`,`EmpNumber`,`DtlCd`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmptbltransource(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmptbltransource` (
                        `trn` char(12) NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `acctNumber` char(5) NOT NULL,
                        `amount` double(12,4) NOT NULL,
                        `source` char(20) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tmpundeddeduction(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tmpundeddeduction` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `acctnumber` char(10) NOT NULL DEFAULT '',
                        `amount` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TotalTr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`total_tr` (
                        `EmpNumber` varchar(5) DEFAULT NULL,
                        `yyyy` int unsigned DEFAULT NULL,
                        `amt` double(12,4) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trandtl110801(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trandtl110801` (
                        `TRN` varchar(12) NOT NULL DEFAULT '',
                        `acctNumber` varchar(5) NOT NULL,
                        `empNumber` varchar(5) NOT NULL,
                        `amount` double(10,4) NOT NULL,
                        `dTimeStamp` datetime NOT NULL DEFAULT '0000-00-00 00:00:00'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trca(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trca` (
                        `EmpNumber` varchar(5) DEFAULT NULL,
                        `Amount` double(12,2) DEFAULT NULL,
                        `EarnRefNo` int unsigned NOT NULL,
                        PRIMARY KEY (`EarnRefNo`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Undeddeduction(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`undeddeduction` (
                        `trn` char(12) NOT NULL DEFAULT '',
                        `empnumber` char(5) NOT NULL DEFAULT '',
                        `acctnumber` char(10) NOT NULL DEFAULT '',
                        `amount` double(12,4) NOT NULL DEFAULT '0.0000',
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Uniformopt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`uniformopt` (
                        `empnumber` varchar(10) NOT NULL,
                        `withUniform` int unsigned NOT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Uom(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`uom` (
                        `single` char(10) NOT NULL,
                        `multi` char(10) NOT NULL,
                        PRIMARY KEY (`single`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Useraccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`useraccess` (
                        `iduserAccess` int unsigned NOT NULL AUTO_INCREMENT,
                        `username` varchar(25) DEFAULT NULL,
                        `idsysmenu` int unsigned DEFAULT NULL,
                        PRIMARY KEY (`iduserAccess`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Usr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`usr` (
                        `UserName` varchar(25) NOT NULL DEFAULT '',
                        `FullName` varchar(45) NOT NULL DEFAULT '',
                        `Pwrd` varchar(60) NOT NULL DEFAULT '',
                        `Stat_` int unsigned NOT NULL DEFAULT '0',
                        `DednAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `ArchievedAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `PartDedSetupAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `DedSumAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `DedSumEmpAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `MoDedRepAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `ConDedSumAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `ESumAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `EHisAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `MoERepAccess` tinyint unsigned NOT NULL DEFAULT '0',
                        `Email` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`UserName`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Usrmenu(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`usrmenu` (
                        `User` varchar(25) NOT NULL,
                        `MenuCode_` varchar(5) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Version(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`version` (
                        `version` int unsigned NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    // -- Tables added from the reference (192.168.0.103 pay) --------------------
    private async Task _01Domainusr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`domainusr` (
                        `Empnumber` varchar(5) NOT NULL,
                        `Status` varchar(25) DEFAULT NULL,
                        `CreatedBy` int unsigned DEFAULT NULL,
                        `DateCreated` timestamp NULL DEFAULT CURRENT_TIMESTAMP,     -- MySQL 5.5 : only TIMESTAMP defaults to now
                        PRIMARY KEY (`Empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }
}
