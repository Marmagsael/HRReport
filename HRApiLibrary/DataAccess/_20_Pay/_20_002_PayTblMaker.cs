using System.Data;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._90_Utils;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace HRApiLibrary.DataAccess._20_Pay;

public class _20_002_PayTblMaker : I_20_002_PayTblMaker
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly IConfiguration _config;

    public _20_002_PayTblMaker(I_90_001_MySqlDataAccess sql, IConfiguration config)
    {
        _sql = sql;
        _config = config;
    }



    public async Task _01(string schema, string country = "PH", string connName = "MySqlConn")
    {

        await _01_001_MainPay(connName);
        await _01_001_CreateSP(connName);

        
        await _01_001_Schema(schema, connName);
        await _00_001_Settings(schema, connName);
        await _00_001_SettingsRecreate(schema, connName);
        await _00_001_PayrollPrd(schema, connName);
        await _00_002_SystemUser(schema, connName);
        await _00_003_SystemUserModuleAccess(schema, connName);
        await _00_004_SystemUserOtherAccess(schema, connName);
        await _00_004_PremContSource(schema, connName);

        await _01_001_Paytran(schema, connName);
        await _01_002_Menus("Menus", schema, connName);
        await _01_004_PayrollGrp(schema, connName);
        await _01_004_PayrollGrpRecreate(schema, connName);
        await _01_004_01_PayRate(schema, connName);
        await _01_004_02_PayrollGrpRates(schema, connName);
        await _01_004_03_UserPayrollInProcess(schema, connName);

        await _01_005_SalaryGrade(schema, connName);
        await _01_006_EmpRates(schema, connName);
        await _01_006_DepRecSettings(schema, connName);
        await _01_007_EmpRatesDtl(schema, connName);

        await _01_101_Coa(schema, connName, country);
        await _01_101_CoaTax(schema, connName, country);
        await _01_101_CoaSSS(schema, connName, country);
        await _01_101_CoaPHIC(schema, connName, country);
        await _01_101_CoaPagibig(schema, connName, country);
        await _01_101_DutyRendered(schema, connName, country);
        await _01_201_FixedEarnings(schema, connName, country);
        await _01_201_FixedEarningsRecreate(schema, connName, country);
        await _01_202_FixedEarnings_Grp(schema, connName, country);
        await _01_203_FixedEarnings_Grp_Emp(schema, connName, country);

        await _01_251_Loans(schema, connName, country);
        await _01_251_LoansRecreate(schema, connName, country);
        await _01_252_DedMandatory(schema, connName, country);
        await _01_252_DedMandatoryTran(schema, connName);

        await _01_301_PayMainVisibleAcct(schema, connName, country);
        await _01_301_PayMainHdr(schema, connName, country);
        await _01_302_PayMainDtl(schema, connName, country);
        await _01_304_PayMainHistory(schema, connName, country);
        await _01_305_PayRateType(schema, connName);

        await _01_401_TmpManHr(schema, connName, country);
        await _01_401_ManHr(schema, connName, country);
        await _01_402_TmpTbltran(schema, connName, country);
        await _01_402_Tbltran(schema, connName, country);


        await _01PH("MainPay", country, connName);
        await _01PH(schema, country, connName);
      


        await EmpNumberLengthCorrection(schema, connName, country);
        await Fld_Count_Correction(schema, connName, country);
        await Entry_Correction(schema, connName, country);
    }


    public async Task _01PH(string schema, string country = "PH", string connName = "MySqlConn")
    {
       if(schema != "MainPay") await _01_101_Coa_Insert_PHDatas(schema, connName);

        await Menus("MainPis", connName, country);
        await Menus("MainPay", connName, country);
        await _01_102_MatrixWTax(schema, connName, country);
        await _01_103_MatrixSss(schema, connName, country);
        await _01_104_MatrixPhic(schema, connName, country);
        await _01_104_MatrixPagibig(schema, connName, country);

        if (schema != "MainPay") await _01_303_PaymaindtlsetupPH(schema, connName, country);

    }

    
    //********************************************************************
    //*** Private Functions **********************************************
    //********************************************************************


    private async Task Entry_Correction(string schema, string connName, string country = "PH")
    {
        //--- TmpTbltranEmplist.PayrollgrpId ---------------------------------------------------------------------
        string sql = $@"Select * from {schema}.Coa where AcctNumber ='E000' ";
        var res = await _sql.FetchData<CoaModel, dynamic>(sql, new { }, connName);

        if (res.Count == 0)
        {
            sql = $@"  Insert into {schema}.Coa (AcctNumber, AcctName,    AcctType, ShortDesc,   HasRateOverBasic, RateOverBasic, SortEarn, SortDed, IsLock, IsSelected) values 
                                                ('E000',     'Tardiness', 'E',      'TARDINESS', 1,                -1,            1,        9999,     1,     0);";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task Fld_Count_Correction(string schema, string connName, string country = "PH")
    {
        //--- TmpTbltranEmplist.PayrollgrpId ---------------------------------------------------------------------
        string sql = $@"Desc {schema}.TmpTbltranEmplist ";
        var res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 4)
        {
            sql = $@"  ALTER TABLE {schema}.`TmpTbltranEmplist` ADD COLUMN `PayrollgrpId` int;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- Settings.TaxPeriodCode ---------------------------------------------------------------------
        sql = $@"Desc {schema}.Settings ";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 10)
        {
            sql = $@"  ALTER TABLE {schema}.Settings ADD COLUMN `TaxPeriodCode` char(3) Default 'SM';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- Settings.RevCompliance ---------------------------------------------------------------------
        sql = $@"Desc {schema}.Settings ";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 21)
        {
            sql = $@"  ALTER TABLE {schema}.Settings  
                            ADD COLUMN `RevTax`     CHAR(7) NOT NULL DEFAULT '2023-01',  
                            ADD COLUMN `RevSSS`     CHAR(7) NOT NULL DEFAULT '2025-01',
                            ADD COLUMN `RevPHIC`    CHAR(7) NOT NULL DEFAULT '2025-01',
                            ADD COLUMN `RevPagibig` CHAR(7) NOT NULL DEFAULT '2019-01';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        

        //--- Matrix.Revision ---------------------------------------------------------------------
        sql = $@"Desc {schema}.matrixwtax ";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 10)
        {
            sql = $@"  ALTER TABLE {schema}.matrixwtax  
                            ADD COLUMN `Revision`   CHAR(7) NOT NULL DEFAULT '2023-01'; 
                       ALTER TABLE {schema}.matrixSss  
                            ADD COLUMN `Revision`  CHAR(7) NOT NULL DEFAULT '2025-01';
                        ALTER TABLE {schema}.matrixPHIC   
                            ADD COLUMN `Revision`  CHAR(7) NOT NULL DEFAULT '2025-01';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }



        //--- TmpTbltran.Source ---------------------------------------------------------------------
        sql = $@"Desc {schema}.TmpTbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 10)
        {
            sql = $@"  ALTER TABLE {schema}.`TmpTbltran` ADD COLUMN `Source` char(5) Default '-';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- TmpTbltran.Source ---------------------------------------------------------------------
        sql = $@"Desc {schema}.Tbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 10)
        {
            sql = $@"  ALTER TABLE {schema}.`Tbltran` ADD COLUMN `Source` char(5) Default '-';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- TmpTbltran.Status ---------------------------------------------------------------------
        sql = $@"Desc {schema}.TmpTbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 11)
        {
            sql = $@"  ALTER TABLE {schema}.`TmpTbltran` ADD COLUMN `Status` char(1) Default '-';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- Tbltran.Status ---------------------------------------------------------------------
        sql = $@"Desc {schema}.Tbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 11)
        {
            sql = $@"  ALTER TABLE {schema}.`Tbltran` ADD COLUMN `Status` char(1) Default '-';";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }


        //--- TmpTbltran.RefId ---------------------------------------------------------------------
        sql = $@"Desc {schema}.TmpTbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 12)
        {
            sql = $@"  ALTER TABLE {schema}.`TmpTbltran` ADD COLUMN `RefId` int Default 0;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        
        //--- Tbltran.RefId ---------------------------------------------------------------------
        sql = $@"Desc {schema}.Tbltran";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 12)
        {
            sql = $@"  ALTER TABLE {schema}.`Tbltran` ADD COLUMN `RefId` int Default 0;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //--- Settings.PremContSource 2525.11.26 ----------------------------------------------------
        sql = $@"Desc {schema}.Settings ";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 25)
        { sql = $@"  ALTER TABLE {schema}.Settings ADD COLUMN `PremContSourceId` int  DEFAULT 1;";
          await _sql.ExecuteCmd(sql, new { }, connName); }


    }


    private async Task EmpNumberLengthCorrection(string schema, string connName, string country = "PH")
    {
        await _01_101_01_FieldLengthQuery(schema, "Emprates", "EmpNumber", 5, 10, connName, country);
        await _01_101_01_FieldLengthQuery(schema, "EmpRateHist", "EmpNumber", 5, 10, connName, country);
        await _01_101_01_FieldLengthQuery(schema, "ManHr", "EmpNumber", 5, 10, connName, country);
        await _01_101_01_FieldLengthQuery(schema, "TmpManHr", "EmpNumber", 5, 10, connName, country);
        await _01_101_01_FieldLengthQuery(schema, "TmpTbltran", "EmpNumber", 5, 10, connName, country);
        await _01_101_01_FieldLengthQuery(schema, "Tbltran", "EmpNumber", 5, 10, connName, country);
    }
    private async Task _01_101_01_FieldLengthQuery(string schema, string tblName, string fldName,
        int fldLenFrom, int fldLenTo, string connName, string country = "PH")
    {

        var sql = $@"SELECT COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS
                     WHERE TABLE_SCHEMA  = '{schema}' AND 
                           TABLE_NAME    = @TblName and 
                           Column_Name   = @FldName ;";

        var res = await _sql.FetchData<TableModel, dynamic>(sql, new { TblName = tblName, FldName = fldName }, connName);

        if (res.Count <= 0) return;
        if (res.FirstOrDefault()?.CHARACTER_MAXIMUM_LENGTH != fldLenFrom) return;

        sql = $@"ALTER TABLE `{schema}`.`{tblName}` MODIFY COLUMN `{fldName}` CHAR({fldLenTo}) ;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }



    //--- Schema Main ------------------------------------------------------
    private async Task _01_001_MainPay(string connName, string schema="MainPay")
    {
        string sql = $"CREATE DATABASE IF NOT EXISTS MainPay";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"CREATE TABLE if not exists  {schema}.VerUpdate (
                  `Id`      int unsigned    NOT     NULL                    AUTO_INCREMENT,
                  `VerCtrl` double(10,2)    NOT     NULL DEFAULT '0.0000',
                  PRIMARY KEY (`Id`)) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
        
        sql = "select * from MainPay.VerUpdate";
        var res = await _sql.FetchData<VerupdateModel,dynamic>(sql, new { }, connName);
        if (res.Count <= 0)
        {
            sql = "insert into MainPay.VerUpdate (id) values (1) ; "; 
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
        
    }
    
    private async Task _01_001_CreateSP(string connName, string schema = "MainPay")
    {
        var sql = $"SELECT * FROM {schema}.VerUpdate LIMIT 1";
        var res = await _sql.FetchData<VerupdateModel, dynamic>(sql, new { }, connName);

        if (res.FirstOrDefault()?.VerCtrl <= 0)
        {
            //Console.WriteLine("--- Writing SP ------------------");
            //await Sp_02TmpTbltran();
        }

        async Task Sp_02TmpTbltran()
        {
            string connName1 = _config.GetConnectionString("MySqlConn"); // resolves actual string
            await using var conn = new MySqlConnection(connName1);
            Console.WriteLine(conn);
            
            await conn.OpenAsync();

            // First: Drop the procedure
            await using (var dropCmd = new MySqlCommand($"DROP PROCEDURE IF EXISTS {schema}._02Tmptbltran_by_Trn;", conn))
            {
                await dropCmd.ExecuteNonQueryAsync();
            }

            // Second: Create the procedure
            var createProcedure = $@"
            CREATE PROCEDURE {schema}._02Tmptbltran_by_Trn(
                IN pTrn VARCHAR(12),
                IN pDb VARCHAR(50)
            )
            BEGIN
                DECLARE v_sql TEXT;
                SET v_sql = CONCAT('SELECT * FROM ', pDb, '.TmpTbltran WHERE trn = ''', pTrn, '''');
                SET @sql = v_sql;
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            END;";

            await using (var createCmd = new MySqlCommand(createProcedure, conn))
            {
                await createCmd.ExecuteNonQueryAsync();
            }
        }
    }
    
    
    
    //--- Schema Main ------------------------------------------------------
    private async Task _01_001_Schema(string schema, string connName)
    {
        string sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _00_001_Settings(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.Settings (
                          Id                INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          Yeartodays        INTEGER UNSIGNED            DEFAULT 293,
                          Semiannualtodays  INTEGER UNSIGNED            DEFAULT 148,
                          Monthtodays       INTEGER UNSIGNED            DEFAULT 22,
                          SemiMonthtodays   INTEGER UNSIGNED            DEFAULT 10,
                          DaysPerWeek       INTEGER UNSIGNED            DEFAULT 5,
                          Daytohours        INTEGER UNSIGNED            DEFAULT 8,
                          NDStart           INTEGER UNSIGNED            DEFAULT 2200,
                          NDEnd             INTEGER UNSIGNED            DEFAULT 600,
                          PayrollType       Char(10)                    Default 'Bi-Monthly',
                          TaxPeriodCode     Char(3)                     Default  'SM',         /* Mo - Monthly, SM-Bi-Monthly, WK - Weekly */
                          RevTax            CHAR(7) NOT NULL            DEFAULT '2023-01',
                          RevSSS            CHAR(7) NOT NULL            DEFAULT '2025-01',
                          RevPHIC           CHAR(7) NOT NULL            DEFAULT '2025-01',
                          RevPagibig        CHAR(7) NOT NULL            DEFAULT '2024-02',
                          PremContSourceId  INTEGER UNSIGNED            DEFAULT 1,
                          AllowedMoPrd      INTEGER UNSIGNED            DEFAULT 3,
                          CoShortName       Char(15)                    Default '',
                          CoFullName        Char(80)                    Default '',
                          CoAddress         Char(160)                   Default '',
                          CoContactNos      Char(80)                    Default '',
                          RegNo             Char(25)                    Default '',
                          TIN               Char(25)                    Default '',
                          SSSNo             Char(25)                    Default '',
                          PhicNo            Char(25)                    Default '',
                          PagibigNo         Char(25)                    Default '',
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;";


        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"select * from {schema}.Settings limit 1 ";
        var res = await _sql.FetchData<SettingsModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@" insert into {schema}.Settings (Daytohours) values (8)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }
    private async Task _00_001_PayrollPrd(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.`PayrollPrd` (
                          `Id`              INTEGER     UNSIGNED NOT NULL AUTO_INCREMENT,
                          `Yr`              INTEGER     UNSIGNED                                COMMENT 'YYYY',
                          `Mo`              CHAR(2)                                             COMMENT '01-12',
                          `Prd`             CHAR(2)                                             COMMENT '01,02,03,04,05',
                          `Openby`          INTEGER     UNSIGNED                                COMMENT 'UserId',
                          `DateOpened`      DATETIME,
                          `Closedby`        INTEGER     UNSIGNED                                COMMENT 'UserId',
                          `DateClosed`      DATETIME,
                          `Status`          CHAR(1)                 DEFAULT 'O'                 COMMENT 'A-Active, O-Open, C-Close',
                          PRIMARY KEY (`Id`)) ENGINE = InnoDB;";


        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"select * from {schema}.PayrollPrd limit 1 ";
        var res = await _sql.FetchData<PayrollprdModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@" insert into {schema}.PayrollPrd 
                            (Yr,             Mo,                            Prd,      Openby,   DateOpened,       Status) 
                      select Year(now()) Yr, LPAD(MONTH(NOW()), 2, '0') mo, '01' Prd, 0 Openby, now() DateOpened, 'A' Status  ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }


    private async Task _00_001_SettingsRecreate(string schema, string connName)
    {

        string sql = $@"Desc {schema}.settings ";
        var res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 18)
        {
            sql = $@"  ALTER TABLE {schema}.`settings` ADD COLUMN `CoShortName` CHAR(15) AFTER `PayrollType`;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        sql = $@"Desc {schema}.settings ";
        res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count == 19)
        {
            sql = $@"  ALTER TABLE {schema}.`settings` ADD COLUMN `AllowedMoPrd` INTEGER UNSIGNED DEFAULT 3 AFTER `PayrollType`;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    public async Task _00_002_SystemUser(string schema, string connName)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.SystemUser (
                          SystemId          INTEGER UNSIGNED NOT NULL,
                          Status            Varchar(2)                  Default 'FA',
                          DateInvited       DATETIME         NOT NULL,
                          DateAccepted      DATETIME         NOT NULL,
                          IsApproved        int                         Default 0,
                          PRIMARY KEY(`SystemId`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    public async Task _00_003_SystemUserModuleAccess(string schema, string connName)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.`SystemUserModuleAccess` (
                          SystemId          INTEGER UNSIGNED NOT NULL,
                          Menus10userId     INTEGER UNSIGNED NOT NULL,
                          PRIMARY KEY(`SystemId`, `Menus10userId`))  ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    public async Task _00_004_SystemUserOtherAccess(string schema, string connName)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.SystemUserOtherAccess (
                          SystemId          INTEGER     UNSIGNED NOT NULL,
                          OtherAccessId     INTEGER     UNSIGNED NOT NULL,
                          PRIMARY KEY(`SystemId`, `OtherAccessId`))
                        ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_001_Paytran(string schema, string connName)
    {

        string sql = @$"CREATE TABLE if not exists {schema}.Paytran (
                            Trn             CHAR(12),
                            EmpmasId        INTEGER ,
                            PayrollgrpId    INTEGER ,
                            AttStart        DateTime, 
                            AttEnd          DateTime, ";

        for (int i = 0; i < 120; i++)
        {
            string sql1 = $" E{i:000}";
            sql += sql1 + "U   DOUBLE(12,4)        DEFAULT 0, ";
            sql += sql1 + "R   DOUBLE(12,4)        DEFAULT 0, ";
            sql += sql1 + "M   DOUBLE(12,4)        DEFAULT 0, ";
            sql += sql1 + "A   DOUBLE(12,4)        DEFAULT 0, ";

        }
        sql += "  PRIMARY KEY(`Trn`,`EmpmasId`) ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }
    
    private async Task _00_004_PremContSource(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.PremContSource (
                        Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        Source          CHAR(25)             Default ' ',
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"select * from {schema}.PremContSource limit 1 ";
        var res = await _sql.FetchData<PremcontsourceModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"Insert into {schema}.PremContSource (Id, Source   ) values
                        (1,  'Actual Earnings' ), (2,  'Deployment Monthly Rate' )  ;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }


    private async Task _01_002_Menus(string menuName, string schema, string connName)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.{menuName}
        (
            Id INTEGER UNSIGNED NOT NULL, 
                            IdParent     INT NULL, 
                            Indent       INT NULL, 
                            Type         NCHAR(10) NULL, 
                            Code         NCHAR(10) NULL, 
                            Icon1        VARCHAR(80) NULL, 
                            Icon2        VARCHAR(80) NULL, 
                            DispText     VARCHAR(80) NULL, 
                            IsWithChild  SMALLINT NULL, 
                            Controller   VARCHAR(50) NULL, 
                            Action       VARCHAR(50) NULL, 
                            Odr          SMALLINT NULL DEFAULT 0,
                            PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_004_PayrollGrp(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.PayrollGrp (
                        Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        ClNumber        CHAR(5)             Default ' ',
                        Name            VARCHAR(80),
                        RatePerHr       DOUBLE(12,4)        DEFAULT 0,
                        RatePerDay      DOUBLE(12,4)        DEFAULT 0,
                        RatePerMonth    DOUBLE(12,4)        DEFAULT 0,
                        RatePerYr       DOUBLE(12,4)        DEFAULT 0,
                        MinMoRate       DOUBLE(12,4)        DEFAULT 0,
                        Status          Char(1)             Default 'A',
                        PayRateId       int                 DEFAULT 0,
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"select * from {schema}.PayrollGrp limit 1 ";
        var res = await _sql.FetchData<PayrollgrpModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"Insert into {schema}.PayrollGrp 
                        (ClNumber, Name   ) values 
                        ('00001',  'Main' ) ;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task _01_004_PayrollGrpRecreate(string schema, string connName, string country = "PH")
    {
        string sql = $@"Desc {schema}.PayrollGrp ";
        var res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count < 10)
        {
            var sql1 =
                $@"ALTER TABLE {schema}.`payrollgrp` ADD COLUMN `MinMoRate` DOUBLE(12,4) DEFAULT 0 AFTER `RatePerYr`;";
            await _sql.ExecuteCmd(sql1, new { }, connName);
        }
    }

    private async Task _01_004_01_PayRate(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.PayRate (
                        Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        RateName        VARCHAR(80)         Default '',
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"select * from {schema}.PayRate limit 1 ";
        var res = await _sql.FetchData<PayRateModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"Insert into {schema}.PayRate (RateName) values 
                            ('Hourly'), ('Daily'), ('Semi-Monthly'),('Monthly'), ('Semi-Annually'), ('Annually') ;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }
    private async Task _01_004_02_PayrollGrpRates(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.payrollgrpRates (
                          PayrollgrpId      INTEGER UNSIGNED    NOT NULL,
                          coaAcctnumber     CHAR(5)             ,
                          RateHr            DOUBLE(12,4)        ,
                          RateDay           DOUBLE(12,4)        ,
                          RateMonth         DOUBLE(12,4)        ,
                          RateYr            DOUBLE(12,4)        ,
                          PRIMARY           KEY (`PayrollgrpId`, `coaAcctnumber`))
                        ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_004_03_UserPayrollInProcess(string schema, string connName)
    {

        string sql = @$"CREATE TABLE if not exists {schema}.UserPayInProcess (
                            UserId          INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            Trn             CHAR(12),
                            PayrollgrpId    INTEGER UNSIGNED,
                            AttStart        DateTime, 
                            AttEnd          DateTime, 
                            Yr              int, 
                            Month           char(2),
                            Period          char(2),
                            PRIMARY KEY(`UserId`) ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01_005_SalaryGrade(string schema, string connName)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.`SalGrade` (
                          `SgCode`          char(10)            NOT NULL,
                          `SGrade`          varchar(45)         NOT NULL,
                          `MonthlyRate`     double(12,4)        NOT NULL,
                          `DailyRate`       double(12,4)        NOT NULL,
                          `HourlyRate`      double(12,4)        NOT NULL,
                          `WTax`            int(10) unsigned    NOT NULL DEFAULT 0,
                          `WSss`            int(10) unsigned    NOT NULL DEFAULT 0,
                          `WGsis`           int(10) unsigned    NOT NULL DEFAULT 1,
                          `WPhic`           int(10) unsigned    NOT NULL DEFAULT 1,
                          `WPagibig`        int(10) unsigned    NOT NULL DEFAULT 1,
                          `IsLock`          int(10) unsigned    NOT NULL DEFAULT 0,
                          PRIMARY KEY (`sgcode`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_006_EmpRates(string schema, string connName)
    {

        string sql = @$"CREATE TABLE  if not exists {schema}.EmpRates (
                          EmpmasId          INTEGER         UNSIGNED    NOT NULL DEFAULT 0,
                          EmpNumber         CHAR(10)                     DEFAULT ' ',
                          PayrollgrpId      INTEGER         UNSIGNED    DEFAULT 0,
                          EmpRate           DOUBLE(12,4)                DEFAULT 0,
                          PayrateId         INTEGER         UNSIGNED    DEFAULT 1,
                          UsePaygrpRates    smallint(5)     UNSIGNED    DEFAULT 1,
                          RatePerHr         DOUBLE(12,4)                DEFAULT 0,
                          RatePerDay        DOUBLE(12,4)                DEFAULT 0,
                          RatePerMonth      DOUBLE(12,4)                DEFAULT 0,
                          RatePerYr         DOUBLE(12,4)                DEFAULT 0,
                          PRIMARY KEY (`EmpmasId`,`PayrollgrpId`) )    ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"CREATE TABLE  if not exists {schema}.EmpRatesHist (
                          Id                INTEGER         UNSIGNED    NOT NULL AUTO_INCREMENT,
                          EmpmasId          INTEGER         UNSIGNED    DEFAULT 0,
                          EmpNumber         CHAR(10)                     DEFAULT ' ',
                          PayrollgrpId      INTEGER         UNSIGNED    DEFAULT 0,
                          EmpRate           DOUBLE(12,4)                DEFAULT 0,
                          PayRateId         Integer                     Default 1, 
                          UsePaygrpRates    smallint(5)     UNSIGNED    DEFAULT 1,
                          RatePerHr         DOUBLE(12,4)                DEFAULT 0,
                          RatePerDay        DOUBLE(12,4)                DEFAULT 0,
                          RatePerMonth      DOUBLE(12,4)                DEFAULT 0,
                          RatePerYr         DOUBLE(12,4)                DEFAULT 0,
                          Created           Datetime                    ,
                          UserId            INTEGER         UNSIGNED    DEFAULT 0,
                          Action            char(10)                    DEFAULT ' ',
                          PRIMARY KEY (`Id`) )    ENGINE = InnoDB;";

        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01_006_DepRecSettings(string schema, string connName)
    {

        var sql = @$"CREATE TABLE if not exists  {schema}.`deprecSettings` (
                          `EmpmasId`        int(10) unsigned NOT NULL DEFAULT 0,
                          `Wtax`            int(10) unsigned DEFAULT 1,
                          `Wsss`            int(10) unsigned DEFAULT 1,
                          `Wgsis`           int(10) unsigned DEFAULT 0,
                          `Wpagibig`        int(10) unsigned DEFAULT 1,
                          `Wphic`           int(10) unsigned DEFAULT 1,
                          PRIMARY KEY (`EmpmasId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"insert into {schema}.deprecSettings (EmpmasId) 
                    select distinct EmpmasId from {schema}.emprates where EmpmasId not in 
                           (select EmpmasId from {schema}.deprecSettings ) ";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01_007_EmpRatesDtl(string schema, string connName)
    {

        string sql = @$"CREATE TABLE if not exists {schema}.EmpratesDtl (
                          EmpmasId      INTEGER         UNSIGNED    NOT NULL AUTO_INCREMENT,
                          PayrollGrpId  INTEGER         UNSIGNED    NOT NULL,
                          AcctNumber    CHAR(5)                     NOT NULL,
                          Rate          DOUBLE(12,4)                DEFAULT 0,
                          PayrateId     INTEGER         UNSIGNED    DEFAULT 0,
                          PRIMARY KEY(`EmpmasId`, `AcctNumber`, `PayrollGrpId`))
                        ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_101_DutyRendered(string schema, string connName, string country)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        var sql = @$"CREATE TABLE if not exists {schema}.DutyRendered (
                            AcctNumber          char(5)                 NOT NULL,
                            IsLock              int        unsigned     NOT NULL DEFAULT 0,
                            PRIMARY KEY (`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $"select * from {schema}.DutyRendered limit 1";
        var dr = await _sql.FetchData<DutyrenderedModel, dynamic>(sql, new { }, connName);
        if (dr == null || dr.Count == 0)
        {
            sql = @$"insert into {schema}.DutyRendered (AcctNumber, IsLock) values 
                        ('E001',1),  
                        ('E002',1), 
                        ('E003',1), 
                        ('E007',1), 
                        ('E009',1), 
                        ('E013',1), 
                        ('E017',1), 
                        ('E021',1), 
                        ('E023',0), 
                        ('E025',1), 
                        ('E027',0), 
                        ('E029',1), 
                        ('E031',0), 
                        ('E033',1)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    // --- Reference Tables ------------------------------------------------
    private async Task _01_101_Coa(string schema, string connName, string country)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string sql = @$"CREATE TABLE if not exists {schema}.Coa (
                            AcctNumber          char(5)                 NOT NULL,
                            AcctName            varchar(60)             NOT NULL DEFAULT ' ',
                            AcctType            Char(1)                 NOT NULL DEFAULT ' ',
                            ShortDesc           varchar(20)             NOT NULL DEFAULT ' ',
                            HasRateOverBasic    int        unsigned     NOT NULL DEFAULT 999,
                            RateOverBasic       double(5,2), 
                            SortEarn            int        unsigned     NOT NULL DEFAULT 999,
                            SortDed             int        unsigned     NOT NULL DEFAULT 999,
                            IsLock              int        unsigned     NOT NULL DEFAULT 0,
                            IsSelected          smallint   DEFAULT 0,
                            PRIMARY KEY (`AcctNumber`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        if (country == "PH")
        {
            sql = $"select * from {schema}.Coa limit 1";
            var coa = await _sql.FetchData<CoaModel, dynamic>(sql, new { }, connName);
            if (coa == null || coa.Count == 0)
            {
                sql = @$"insert into {schema}.Coa 
                        (AcctNumber, AcctName,                      AcctType,   ShortDesc,      HasRateOverBasic,   RateOverBasic,  SortEarn,   SortDed,    IsLock,  IsSelected ) values 
                        ('E000','Tardiness',                        'E',        'TARDINESS',    1,                  -1,             1,          999,        1,       0),
                        ('E001','Basic Pay',                        'E',        'BASIC',        1,                  1,              1,          999,        1,       1),
                        ('E002','Regular Pay',                      'E',        'Regular',      1,                  1,              2,          999,        1,       0),
                        ('E003','Reg. OT',                          'E',        'REG OT',       1,                  1.25,           3,          999,        1,       1),
                        ('E005','ND Pay',                           'E',        'ND',           1,                  0.1,            4,          999,        1,       0),
                        ('E007','Rest Day Pay',                     'E',        'RD',           1,                  0.3,            5,          999,        1,       0),
                        ('E009','Rest Day OT Pay',                  'E',        'RDOT',         1,                  1.69,           6,          999,        1,       0),
                        ('E011','Legal Holiday Pay',                'E',        'LH',           1,                  1,              7,          999,        1,       0),
                        ('E013','Legal Holiday OT Pay',             'E',        'LHOT',         1,                  2.6,            8,          999,        1,       0),
                        ('E015','Special Holiday Pay',              'E',        'SH',           1,                  0.3,            9,          999,        1,       0),
                        ('E017','Special Holiday OT Pay',           'E',        'SHOT',         1,                  1.69,           10,         999,        1,       0),
                        ('E019','Double Hol',                       'E',        'DH',           1,                  2,              11,         999,        1,       0),
                        ('E021','Double Hol OT',                    'E',        'DH OT ',       1,                  3.9,            12,         999,        1,       0),
                        ('E023','RD - Legal Holiday Pay',           'E',        'RDLH',         1,                  1.6,            13,         999,        1,       0),
                        ('E025','RD - Legal Holiday OT Pay',        'E',        'RDLHOT',       1,                  3.38,           14,         999,        1,       0),
                        ('E027','RD - Special Holiday Pay',         'E',        'RDSH',         1,                  0.5,            15,         999,        1,       0),
                        ('E029','RD - Special Holiday OT Pay',      'E',        'RDSHOT',       1,                  1.95,           16,         999,        1,       0),
                        ('E031','RD - Double Hol',                  'E',        'RD DH ',       1,                  2.9,            17,         999,        1,       0),
                        ('E033','RD - Double Hol OT',               'E',        'RD DHOT',      1,                  5.07,           18,         999,        1,       0),
                        ('E035','ND - Restday Pay',                 'E',        'NDRD',         1,                  0.03,           19,         999,        1,       0),
                        ('E037','ND - Restday OT Pay',              'E',        'NDRDOT',       1,                  0.169,          20,         999,        1,       0),
                        ('E039','ND - Legal Holiday Pay',           'E',        'NDLH',         1,                  0.1,            21,         999,        1,       0),
                        ('E041','ND - Legal Holiday OT',            'E',        'NDLHOT',       1,                  0.26,           22,         999,        1,       0),
                        ('E043','ND - Special Holiday Pay',         'E',        'NDSH',         1,                  0.03,           23,         999,        1,       0),
                        ('E045','ND - Special Holiday OT',          'E',        'NDSHOT',       1,                  0.169,          24,         999,        1,       0),
                        ('E047','ND - Restday Legal Hol Pay',       'E',        'NDRDLH',       1,                  0.16,           25,         999,        1,       0),
                        ('E049','ND - Restday Legal Hol OT Pay',    'E',        'NDRDLHOT',     1,                  0.338,          26,         999,        1,       0),
                        ('E051','ND - Restday Special Hol Pay',     'E',        'NDRDSH',       1,                  0.05,           27,         999,        1,       0),
                        ('E053','ND - Restday Special Hol OT Pay',  'E',        'NDRDSHOT',     1,                  0.195,          28,         999,        1,       0),
                        ('E055','Hazard Pay',                       'E',        'HAZARD',       1,                  1,              29,         999,        1,       0),
                        ('E057','Allowance',                        'E',        'ALLOW',        0,                  0,              30,         999,        1,       0),
                        ('E059','Incentive Leave',                  'E',        'INCENT LV',    0,                  0,              31,         999,        1,       0),
                        ('E061','Functional Incentive',             'E',        'F. Incent',    0,                  0,              32,         999,        1,       0),
                        ('E063','13th Month',                       'E',        '13thMo',       0,                  0,              33,         999,        1,       0),
                        ('E065','Salary Adjust.',                   'E',        'SAL ADJ.'  ,   0,                  0,              34,         999,        1,       0),
                        ('E067','Refund of Dedn',                   'E',        'REFND DED',    0,                  0,              35,         999,        1,       0),
                        ('E069','Retro Pay',                        'E',        'RETROPAY',     0,                  0,              36,         999,        1,       0),
                        ('E071','Tax Refund',                       'E',        'TAX REFND',    0,                  0,              37,         999,        1,       0),
                        ('E073','Addtl Salary',                     'E',        'ADD SAL',      0,                  0,              38,         999,        1,       0),
                        ('E075','Bonus',                            'E',        'BONUS',        0,                  0,              39,         999,        1,       0),
                        ('E077','Annual Incentive',                 'E',        'A INCENT',     0,                  0,              40,         999,        1,       0),
                        ('E079','Paternity Leave PAY',              'E',        'PATERNITY',    1,                  1,              41,         999,        1,       0),
                        ('E081','Maternity Leave Pay',              'E',        'MATERNITY',    1,                  1,              42,         999,        1,       0),
                        ('E083','Rice Subsidy',                     'E',        'RICE',         0,                  0,              43,         999,        1,       0),
                        ('E085','Meal Allowance',                   'E',        'MEAL',         0,                  0,              44,         999,        1,       0),
                        ('E087','Transpo Allowance',                'E',        'TRANSPO',      0,                  0,              45,         999,        0,       0),
                        ('E089','Allowance - Others',               'E',        'ALLOW OTHR ',  0,                  0,              46,         999,        0,       0),
                        ('D001','Witholding Tax',                   'D',        'W Tax',        0,                  0,              1,          1,          1,       0),
                        ('D002','SSS Premium',                      'D',        'SSS Prem',     0,                  0,              2,          2,          1,       0),
                        ('D003','Phil. Health Premium',             'D',        'PHIC Prem',    0,                  0,              3,          3,          1,       0),
                        ('D004','Pagibig Premium',                  'D',        'Pagibig Prem', 0,                  0,              4,          4,          1,       0),
                        ('D005','SSS Loan',                         'D',        'SSS Ln',       0,                  0,              5,          5,          1,       0),
                        ('D006','HDMF Loan',                        'D',        'HDMF LN',      0,                  0,              6,          6,          1,       0),
                        ('D007','Pagibig Loan',                     'D',        'PAGIBIB LN',   0,                  0,              7,          7,          1,       0),
                        ('D008','Housing Loan',                     'D',        'HSE LN',       0,                  0,              8,          8,          1,       0),
                        ('D009','Cash Advances',                    'D',        'CA',           0,                  0,              9,          9,          1,       0),
                        ('D010','HDMF Calamity Loan',               'D',        'HDMF CAL',     0,                  0,              10,         10,         1,       0),
                        ('D011','Personal Loan',                    'D',        'P LOAN',       0,                  0,              11,         11,         1,       0),
                        ('D012','Canteen',                          'D',        'CANTEEN',      0,                  0,              12,         12,         0,       0),
                        ('D013','Salary Adjustment',                'D',        'SAL ADJ',      0,                  0,              13,         13,         0,       0),
                        ('D014','SSS Condonation',                  'D',        'SSS COND',     0,                  0,              14,         14,         1,       0),
                        ('D015','Medical',                          'D',        'MEDICAL',      0,                  0,              15,         15,         0,       0),
                        ('D016','Meals',                            'D',        'MEALS',        0,                  0,              16,         16,         0,       0),
                        ('D017','License Renewal',                  'D',        'LICENSE',      0,                  0,              17,         17,         0,       0),
                        ('D018','Medical',                          'D',        'MEDICAL',      0,                  0,              18,         18,         0,       0),
                        ('D019','Loans',                            'D',        'LOAN',         0,                  0,              19,         19,         0,       0),
                        ('D020','Donations',                        'D',        'DONATION',     0,                  0,              20,         20,         0,       0),
                        ('D021','Uniform',                          'D',        'UNFRM',        0,                  0,              21,         21,         0,       0),
                        ('D022','Hospitalization Assistance',       'D',        'HOSP ASS',     0,                  0,              22,         22,         0,       0),
                        ('D023','SSS Calamity Loan',                'D',        'SSS CAL',      0,                  0,              23,         23,         0,       0)";
                await _sql.ExecuteCmd(sql, new { }, connName);
            }

        }
    }

    private async Task _01_101_CoaTax(string schema, string connName, string country)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.CoaTax (AcctNumber char(5) NOT NULL,
                     PRIMARY KEY (`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_101_CoaSSS(string schema, string connName, string country)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.CoaSSS (AcctNumber char(5) NOT NULL,
                     PRIMARY KEY (`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_101_CoaPHIC(string schema, string connName, string country)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.CoaPHIC (AcctNumber char(5) NOT NULL,
                     PRIMARY KEY (`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_101_CoaPagibig(string schema, string connName, string country)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.CoaPagibig (AcctNumber char(5) NOT NULL,
                     PRIMARY KEY (`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $"select * from {schema}.CoaPagibig limit 1";
        var coa = await _sql.FetchData<CoapagibigModel, dynamic>(sql, new { }, connName);
        if (coa == null || coa.Count == 0)
        {
            sql = @$"insert into {schema}.CoaPagibig (AcctNumber) values ('E000'), ('E001') on duplicate key update AcctNumber = 'E001'";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task _01_101_Coa_Insert_PHDatas(string schema, string connName)
    {
        string? sql = $"select * from {schema}.Coa limit 1";
        var coa = await _sql.FetchData<CoaModel, dynamic>(sql, new { }, connName);
        if (coa == null || coa.Count == 0)
        {
            sql = @$"insert into {schema}.Coa 
                    (AcctNumber, AcctName,                      AcctType,   ShortDesc,      HasRateOverBasic,   RateOverBasic,  SortEarn,   SortDed,    IsLock) values 
                    ('E001','Basic Pay',                        'E',        'BASIC',        1,                  1,              1,          999,        1),
                    ('E002','Regular Pay',                      'E',        'Regular',      1,                  1,              2,          999,        1),
                    ('E003','Reg. OT',                          'E',        'REG OT',       1,                  1.25,           3,          999,        1),
                    ('E005','ND Pay',                           'E',        'ND',           1,                  0.1,            4,          999,        1),
                    ('E007','Rest Day Pay',                     'E',        'RD',           1,                  0.3,            5,          999,        1),
                    ('E009','Rest Day OT Pay',                  'E',        'RDOT',         1,                  1.69,           6,          999,        1),
                    ('E011','Legal Holiday Pay',                'E',        'LH',           1,                  1,              7,          999,        1),
                    ('E013','Legal Holiday OT Pay',             'E',        'LHOT',         1,                  2.6,            8,          999,        1),
                    ('E015','Special Holiday Pay',              'E',        'SH',           1,                  0.3,            9,          999,        1),
                    ('E017','Special Holiday OT Pay',           'E',        'SHOT',         1,                  1.69,           10,         999,        1),
                    ('E019','Double Hol',                       'E',        'DH',           1,                  2,              11,         999,        1),
                    ('E021','Double Hol OT',                    'E',        'DH OT ',       1,                  3.9,            12,         999,        1),
                    ('E023','RD - Legal Holiday Pay',           'E',        'RDLH',         1,                  1.6,            13,         999,        1),
                    ('E025','RD - Legal Holiday OT Pay',        'E',        'RDLHOT',       1,                  3.38,           14,         999,        1),
                    ('E027','RD - Special Holiday Pay',         'E',        'RDSH',         1,                  0.5,            15,         999,        1),
                    ('E029','RD - Special Holiday OT Pay',      'E',        'RDSHOT',       1,                  1.95,           16,         999,        1),
                    ('E031','RD - Double Hol',                  'E',        'RD DH ',       1,                  2.9,            17,         999,        1),
                    ('E033','RD - Double Hol OT',               'E',        'RD DHOT',      1,                  5.07,           18,         999,        1),
                    ('E035','ND - Restday Pay',                 'E',        'NDRD',         0,                  0.03,           19,         999,        1),
                    ('E037','ND - Restday OT Pay',              'E',        'NDRDOT',       0,                  0.169,          20,         999,        1),
                    ('E039','ND - Legal Holiday Pay',           'E',        'NDLH',         0,                  0.1,            21,         999,        1),
                    ('E041','ND - Legal Holiday OT',            'E',        'NDLHOT',       0,                  0.26,           22,         999,        1),
                    ('E043','ND - Special Holiday Pay',         'E',        'NDSH',         0,                  0.03,           23,         999,        1),
                    ('E045','ND - Special Holiday OT',          'E',        'NDSHOT',       0,                  0.169,          24,         999,        1),
                    ('E047','ND - Restday Legal Hol Pay',       'E',        'NDRDLH',       0,                  0.16,           25,         999,        1),
                    ('E049','ND - Restday Legal Hol OT Pay',    'E',        'NDRDLHOT',     0,                  0.338,          26,         999,        1),
                    ('E051','ND - Restday Special Hol Pay',     'E',        'NDRDSH',       0,                  0.05,           27,         999,        1),
                    ('E053','ND - Restday Special Hol OT Pay',  'E',        'NDRDSHOT',     0,                  0.195,          28,         999,        1),
                    ('E055','Hazard Pay',                       'E',        'HAZARD',       0,                  1,              29,         999,        1),
                    ('E057','Allowance',                        'E',        'ALLOW',        0,                  1,              30,         999,        1),
                    ('E059','Incentive Leave',                  'E',        'INCENT LV',    0,                  1,              31,         999,        1),
                    ('E061','Functional Incentive',             'E',        'F. Incent',    0,                  1,              32,         999,        1),
                    ('E063','13th Month',                       'E',        '13thMo',       0,                  1,              33,         999,        1),
                    ('E065','Salary Adjust.',                   'E',        'SAL ADJ.'  ,   0,                  1,              34,         999,        1),
                    ('E067','Refund of Dedn',                   'E',        'REFND DED',    0,                  1,              35,         999,        1),
                    ('E069','Retro Pay',                        'E',        'RETROPAY',     0,                  1,              36,         999,        1),
                    ('E071','Tax Refund',                       'E',        'TAX REFND',    0,                  1,              37,         999,        1),
                    ('E073','Addtl Salary',                     'E',        'ADD SAL',      0,                  1,              38,         999,        1),
                    ('E075','Bonus',                            'E',        'BONUS',        0,                  1,              39,         999,        1),
                    ('E077','Annual Incentive',                 'E',        'A INCENT',     0,                  1,              40,         999,        1),
                    ('E079','Paternity Leave PAY',              'E',        'PATERNITY',    0,                  1,              41,         999,        1),
                    ('E081','Maternity Leave Pay',              'E',        'MATERNITY',    0,                  1,              42,         999,        1),
                    ('E083','Rice Subsidy',                     'E',        'RICE',         0,                  1,              43,         999,        1),
                    ('E085','Meal Allowance',                   'E',        'MEAL',         0,                  1,              44,         999,        1),
                    ('E087','Transpo Allowance',                'E',        'TRANSPO',      0,                  1,              45,         999,        0),
                    ('E089','Allowance - Others',               'E',        'ALLOW OTHR ',  0,                  1,              46,         999,        0),
                    ('D001','Witholding Tax',                   'D',        'W Tax',        0,                  1,              1,          1,          1),
                    ('D002','SSS Premium',                      'D',        'SSS Prem',     0,                  1,              2,          2,          1),
                    ('D003','Phil. Health Premium',             'D',        'PHIC Prem',    0,                  1,              3,          3,          1),
                    ('D004','Pagibig Premium',                  'D',        'Pagibig Prem', 0,                  1,              4,          4,          1),
                    ('D005','SSS Loan',                         'D',        'SSS Ln',       0,                  1,              5,          5,          1),
                    ('D006','HDMF Loan',                        'D',        'HDMF LN',      0,                  1,              6,          6,          1),
                    ('D007','Pagibig Loan',                     'D',        'PAGIBIB LN',   0,                  1,              7,          7,          1),
                    ('D008','Housing Loan',                     'D',        'HSE LN',       0,                  1,              8,          8,          1),
                    ('D009','Cash Advances',                    'D',        'CA',           0,                  1,              9,          9,          1),
                    ('D010','HDMF Calamity Loan',               'D',        'HDMF CAL',     0,                  1,              10,         10,         1),
                    ('D011','Personal Loan',                    'D',        'P LOAN',       0,                  1,              11,         11,         1),
                    ('D012','Canteen',                          'D',        'CANTEEN',      0,                  1,              12,         12,         0),
                    ('D013','Salary Adjustment',                'D',        'SAL ADJ',      0,                  1,              13,         13,         0),
                    ('D014','SSS Condonation',                  'D',        'SSS COND',     0,                  1,              14,         14,         1),
                    ('D015','Medical',                          'D',        'MEDICAL',      0,                  1,              15,         15,         0),
                    ('D016','Meals',                            'D',        'MEALS',        0,                  1,              16,         16,         0),
                    ('D017','License Renewal',                  'D',        'LICENSE',      0,                  1,              17,         17,         0),
                    ('D018','Medical',                          'D',        'MEDICAL',      0,                  1,              18,         18,         0),
                    ('D019','Loans',                            'D',        'LOAN',         0,                  1,              19,         19,         0),
                    ('D020','Donations',                        'D',        'DONATION',     0,                  1,              20,         20,         0),
                    ('D021','Uniform',                          'D',        'UNFRM',        0,                  1,              21,         21,         0),
                    ('D022','Hospitalization Assistance',       'D',        'HOSP ASS',     0,                  1,              22,         22,         0),
                    ('D023','SSS Calamity Loan',                'D',        'SSS CAL',      0,                  1,              23,         23,         0)";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }


    }


    private async Task Menus(string schema, string connName, string country = "PH")
    {
        var sql = @$"CREATE TABLE if not exists {schema}.Menu (
                          Id            INTEGER         UNSIGNED NOT NULL AUTO_INCREMENT,
                          Type          VARCHAR (60),
                          IdParent      INTEGER         UNSIGNED Default 0,
                          Indent        INTEGER         UNSIGNED,
                          Icon          VARCHAR (60),
                          DispText      VARCHAR (80),
                          Action        VARCHAR (45),
                          Odr           INTEGER,
                          PRIMARY KEY (`Id`))   ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }


    private async Task _01_102_MatrixWTax(string schema, string connName, string country = "PH")
    {


        if (country == "PH")
        {
            string sql = @$"CREATE TABLE if not exists {schema}.MatrixWTax (
                            Id              int unsigned NOT NULL AUTO_INCREMENT,
                            From_           date, 
                            To_             date,
                            CountryCode     char(5)                 default 'PH', 
                            PeriodCode      char(2)                 default 'SM', 
                            Revision        CHAR(7)      NOT NULL   DEFAULT '2023-01',  
                            TaxCode         varchar(5)   NOT NULL   default ' ',
                            SAmt            double(10,4) NOT NULL   default 0,
                            EAmt            double(10,4) NOT NULL   default 0,
                            Fix             double(10,4) NOT NULL   default 0,
                            Percentage      double(6,2)             default 0,
                            PRIMARY KEY (`Id`)
                      ) ENGINE=InnoDB AUTO_INCREMENT=0 DEFAULT CHARSET=latin1;";
            await _sql.ExecuteCmd(sql, new { }, connName);

            sql = $@"select * from {schema}.MatrixWTax limit 1 ";
            var res = await _sql.FetchData<WTaxModel, dynamic>(sql, new { }, connName);


            if (res == null || res.Count == 0)
            {
                if (schema == "MainPay")
                {

                    sql = $@"Insert into {schema}.MatrixWtax 
                                  (From_,        To_,          CountryCode,  PeriodCode, TaxCode, SAmt,           EAmt,           Fix,            Percentage) values 
          
                                  -- WEEKLY
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL',      0.0000,     4807.9900,         0.0000,     0.0000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL',   4808.0000,     7691.9900,      4808.0000,     0.1500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL',   7692.0000,    15384.9900,      7692.0000,     0.2000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL',  15385.0000,    38461.9900,     15385.4000,     0.2500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL',  38462.0000,   153845.9900,     38462.4000,     0.3000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'Wk', 'ALL', 153846.0000,   999999.9900,    153846.4000,     0.3500),

                                  -- SEMI-MONTHLY
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL',       0.0000,    10416.9900,         0.0000,     0.0000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL',  10417.0000,     16666.9900,     10417.0000,     0.1500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL',  16667.0000,     33332.9900,     16667.0000,     0.2000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL',  33333.0000,     83332.9900,     33333.0000,     0.2500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL',  83333.0000,    333332.9900,     83333.0000,     0.3000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'SM', 'ALL', 333333.0000,    999999.9900,    333333.0000,     0.3500),

                                  -- MONTHLY
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',       0.0000,    20832.9900,          0.0000,     0.0000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',   20833.0000,    33332.9900,      20833.0000,     0.1500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',   33333.0000,    66666.9900,      33333.0000,     0.2000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',   66667.0000,   166666.9900,      66667.0000,     0.2500),
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',  166667.0000,   666666.9900,     166667.0000,     0.3000),
                                  ('2023-01-01', '2025-12-31', 'PH', 'MO', 'ALL',  666667.0000,   999999.9900,     666667.0000,     0.3500);";

                }
                else
                {
                    sql = $@"Insert into {schema}.MatrixWtax (Id, From_,        To_,          CountryCode,  PeriodCode, Revision, TaxCode, SAmt,           EAmt,           Fix,            Percentage)
                                SELECT * FROM mainpay.MatrixWtax";
                }

                await _sql.ExecuteCmd(sql, new { }, connName);
            }


        }
    }

    private async Task _01_103_MatrixSss(string schema, string connName, string country = "PH")
    {
        string sql = @$"CREATE TABLE if not exists {schema}.MatrixSss (
                            Id              int unsigned NOT NULL AUTO_INCREMENT,
                            Revision        CHAR(7) NOT NULL DEFAULT '202501', 
                            DateStart       date,
                            DateEnd         date, 
                            FStart          double(8,2)             Default 0.00,
                            FEnd            double(8,2) NOT NULL    DEFAULT 0.00,
                            Ee              double(6,2)             DEFAULT 0.00,
                            Er              double(6,2)             DEFAULT 0.00,
                            Ecc             double(6,2)             DEFAULT 0.00,
                            Compensation   double(10,2)             Default 0.00,
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);


        if (country == "PH")
        {
            sql = $@"select * from {schema}.MatrixSss limit 1 ";
            var res = await _sql.FetchData<SssMatrixModel, dynamic>(sql, new { }, connName);
            if (res == null || res.Count == 0)
            {
                sql = $@"Insert into {schema}.MatrixSss
                            (DateStart,    DateEnd,      FStart,    FEnd,       Ee,         Er,         Ecc,    Compensation) values 
                            ('2023-01-01', '2025-12-31', 0.00, 5249.99, 250.00, 500.00, 10.00, 5000.00),
                            ('2023-01-01', '2025-12-31', 5250.00, 5749.99, 275.00, 550.00, 10.00, 5500.00),
                            ('2023-01-01', '2025-12-31', 5750.00, 6249.99, 300.00, 600.00, 10.00, 6000.00),
                            ('2023-01-01', '2025-12-31', 6250.00, 6749.99, 325.00, 650.00, 10.00, 6500.00),
                            ('2023-01-01', '2025-12-31', 6750.00, 7249.99, 350.00, 700.00, 10.00, 7000.00),
                            ('2023-01-01', '2025-12-31', 7250.00, 7749.99, 375.00, 750.00, 10.00, 7500.00),
                            ('2023-01-01', '2025-12-31', 7750.00, 8249.99, 400.00, 800.00, 10.00, 8000.00),
                            ('2023-01-01', '2025-12-31', 8250.00, 8749.99, 425.00, 850.00, 10.00, 8500.00),
                            ('2023-01-01', '2025-12-31', 8750.00, 9249.99, 450.00, 900.00, 10.00, 9000.00),
                            ('2023-01-01', '2025-12-31', 9250.00, 9749.99, 475.00, 950.00, 10.00, 9500.00),
                            ('2023-01-01', '2025-12-31', 9750.00, 10249.99, 500.00, 1000.00, 10.00, 10000.00),
                            ('2023-01-01', '2025-12-31', 10250.00, 10749.99, 525.00, 1050.00, 10.00, 10500.00),
                            ('2023-01-01', '2025-12-31', 10750.00, 11249.99, 550.00, 1100.00, 10.00, 11000.00),
                            ('2023-01-01', '2025-12-31', 11250.00, 11749.99, 575.00, 1150.00, 10.00, 11500.00),
                            ('2023-01-01', '2025-12-31', 11750.00, 12249.99, 600.00, 1200.00, 10.00, 12000.00),
                            ('2023-01-01', '2025-12-31', 12250.00, 12749.99, 625.00, 1250.00, 10.00, 12500.00),
                            ('2023-01-01', '2025-12-31', 12750.00, 13249.99, 650.00, 1300.00, 10.00, 13000.00),
                            ('2023-01-01', '2025-12-31', 13250.00, 13749.99, 675.00, 1350.00, 10.00, 13500.00),
                            ('2023-01-01', '2025-12-31', 13750.00, 14249.99, 700.00, 1400.00, 10.00, 14000.00),
                            ('2023-01-01', '2025-12-31', 14250.00, 14749.99, 725.00, 1450.00, 10.00, 14500.00),
                            ('2023-01-01', '2025-12-31', 14750.00, 15249.99, 750.00, 1500.00, 30.00, 15000.00),
                            ('2023-01-01', '2025-12-31', 15250.00, 15749.99, 775.00, 1550.00, 30.00, 15500.00),
                            ('2023-01-01', '2025-12-31', 15750.00, 16249.99, 800.00, 1600.00, 30.00, 16000.00),
                            ('2023-01-01', '2025-12-31', 16250.00, 16749.99, 825.00, 1650.00, 30.00, 16500.00),
                            ('2023-01-01', '2025-12-31', 16750.00, 17249.99, 850.00, 1700.00, 30.00, 17000.00),
                            ('2023-01-01', '2025-12-31', 17250.00, 17749.99, 875.00, 1750.00, 30.00, 17500.00),
                            ('2023-01-01', '2025-12-31', 17750.00, 18249.99, 900.00, 1800.00, 30.00, 18000.00),
                            ('2023-01-01', '2025-12-31', 18250.00, 18749.99, 925.00, 1850.00, 30.00, 18500.00),
                            ('2023-01-01', '2025-12-31', 18750.00, 19249.99, 950.00, 1900.00, 30.00, 19000.00),
                            ('2023-01-01', '2025-12-31', 19250.00, 19749.99, 975.00, 1950.00, 30.00, 19500.00),
                            ('2023-01-01', '2025-12-31', 19750.00, 20249.99, 1000.00, 2000.00, 30.00, 20000.00),
                            ('2023-01-01', '2025-12-31', 20250.00, 20749.99, 1025.00, 2050.00, 30.00, 20500.00),
                            ('2023-01-01', '2025-12-31', 20750.00, 21249.99, 1050.00, 2100.00, 30.00, 21000.00),
                            ('2023-01-01', '2025-12-31', 21250.00, 21749.99, 1075.00, 2150.00, 30.00, 21500.00),
                            ('2023-01-01', '2025-12-31', 21750.00, 22249.99, 1100.00, 2200.00, 30.00, 22000.00),
                            ('2023-01-01', '2025-12-31', 22250.00, 22749.99, 1125.00, 2250.00, 30.00, 22500.00),
                            ('2023-01-01', '2025-12-31', 22750.00, 23249.99, 1150.00, 2300.00, 30.00, 23000.00),
                            ('2023-01-01', '2025-12-31', 23250.00, 23749.99, 1175.00, 2350.00, 30.00, 23500.00),
                            ('2023-01-01', '2025-12-31', 23750.00, 24249.99, 1200.00, 2400.00, 30.00, 24000.00),
                            ('2023-01-01', '2025-12-31', 24250.00, 24749.99, 1225.00, 2450.00, 30.00, 24500.00),
                            ('2023-01-01', '2025-12-31', 24750.00, 25249.99, 1250.00, 2500.00, 30.00, 25000.00),
                            ('2023-01-01', '2025-12-31', 25250.00, 25749.99, 1275.00, 2550.00, 30.00, 25500.00),
                            ('2023-01-01', '2025-12-31', 25750.00, 26249.99, 1300.00, 2600.00, 30.00, 26000.00),
                            ('2023-01-01', '2025-12-31', 26250.00, 26749.99, 1325.00, 2650.00, 30.00, 26500.00),
                            ('2023-01-01', '2025-12-31', 26750.00, 27249.99, 1350.00, 2700.00, 30.00, 27000.00),
                            ('2023-01-01', '2025-12-31', 27250.00, 27749.99, 1375.00, 2750.00, 30.00, 27500.00),
                            ('2023-01-01', '2025-12-31', 27750.00, 28249.99, 1400.00, 2800.00, 30.00, 28000.00),
                            ('2023-01-01', '2025-12-31', 28250.00, 28749.99, 1425.00, 2850.00, 30.00, 28500.00),
                            ('2023-01-01', '2025-12-31', 28750.00, 29249.99, 1450.00, 2900.00, 30.00, 29000.00),
                            ('2023-01-01', '2025-12-31', 29250.00, 29749.99, 1475.00, 2950.00, 30.00, 29500.00),
                            ('2023-01-01', '2025-12-31', 29750.00, 30249.99, 1500.00, 3000.00, 30.00, 30000.00),
                            ('2023-01-01', '2025-12-31', 30250.00, 30749.99, 1525.00, 3050.00, 30.00, 30500.00),
                            ('2023-01-01', '2025-12-31', 30750.00, 31249.99, 1550.00, 3100.00, 30.00, 31000.00),
                            ('2023-01-01', '2025-12-31', 31250.00, 31749.99, 1575.00, 3150.00, 30.00, 31500.00),
                            ('2023-01-01', '2025-12-31', 31750.00, 32249.99, 1600.00, 3200.00, 30.00, 32000.00),
                            ('2023-01-01', '2025-12-31', 32250.00, 32749.99, 1625.00, 3250.00, 30.00, 32500.00),
                            ('2023-01-01', '2025-12-31', 32750.00, 33249.99, 1650.00, 3300.00, 30.00, 33000.00),
                            ('2023-01-01', '2025-12-31', 33250.00, 33749.99, 1675.00, 3350.00, 30.00, 33500.00),
                            ('2023-01-01', '2025-12-31', 33750.00, 34249.99, 1700.00, 3400.00, 30.00, 34000.00),
                            ('2023-01-01', '2025-12-31', 34250.00, 34749.99, 1725.00, 3450.00, 30.00, 34500.00),
                            ('2023-01-01', '2025-12-31', 34750.00, 99999.99, 1750.00, 3500.00, 30.00, 35000.00);";
                await _sql.ExecuteCmd(sql, new { }, connName);
            }

        }
    }
    private async Task _01_104_MatrixPhic(string schema, string connName, string country = "PH")
    {
        string sql = @$"CREATE TABLE if not exists {schema}.MatrixPhic (
                            Id              int unsigned NOT NULL AUTO_INCREMENT,
                            Revision        CHAR(7) NOT NULL DEFAULT '202501',
                            DateStart       date,
                            DateEnd         date, 
                            FStart          double(8,2)             Default 0.00,
                            FEnd            double(8,2) NOT NULL    DEFAULT 0.00,
                            Ee              double(6,2)             DEFAULT 0.00,
                            Er              double(6,2)             DEFAULT 0.00,
                            Percent         double(6,2)             DEFAULT 0.00,
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);


        if (country == "PH")
        {
            sql = $@"select * from {schema}.MatrixPhic limit 1 ";
            var res = await _sql.FetchData<PhicMatrixModel, dynamic>(sql, new { }, connName);
            if (res == null || res.Count == 0)
            {
                sql = $@" insert into {schema}.MatrixPhic 
                                (DateStart,     DateEnd,        FStart,     FEnd,       Ee,     Er,     Percent) values 
                                ('2022-01-01',  '2025-12-31',   0.00,       10000.00,   200.00, 200.00, 0.00),
                                ('2022-01-01',  '2025-12-31',   10000.01,   79999.99,   0.00,   0.00,   4.00),
                                ('2022-01-01',  '2025-12-31',   80000.00,   999999.99,  900.00, 900.00, 0.00) 
                            ";
                await _sql.ExecuteCmd(sql, new { }, connName);
            }

        }
    }
    private async Task _01_104_MatrixPagibig(string schema, string connName, string country = "PH")
    {
        string sql = @$"CREATE TABLE if not exists {schema}.MatrixPagibig (
                            Id              int unsigned NOT NULL AUTO_INCREMENT,
                            Revision        CHAR(7) NOT NULL DEFAULT '201901',
                            DateStart       date,
                            DateEnd         date, 
                            FStart          double(8,2)             Default 0.00,
                            FEnd            double(12,2) NOT NULL    DEFAULT 0.00,
                            Ee              double(6,2)             DEFAULT 0.00,
                            Er              double(6,2)             DEFAULT 0.00,
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, connName);


        if (country == "PH")
        {
            sql = $@"select * from {schema}.MatrixPagibig limit 1 ";
            var res = await _sql.FetchData<PagibigMatrixModel, dynamic>(sql, new { }, connName);
            if (res == null || res.Count == 0)
            {
                sql = $@" insert into {schema}.MatrixPagibig 
                                (DateStart,     DateEnd,        FStart,     FEnd,            Ee,     Er   ) values 
                                ('2022-01-01',  null,           0.00,       10000000.00,   200.00, 200.00) 
                            ";
                await _sql.ExecuteCmd(sql, new { }, connName);
            }

        }
    }

    // --- Earnings and Deductions ------------------------------------------
    private async Task _01_201_FixedEarnings(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE if not exists  {schema}.FixedEarnings (
                        Id                      int(10)                 unsigned    NOT NULL AUTO_INCREMENT,
                        PayrollgrpId            int                     unsigned,
                        Empnumber               char(5)                             NOT NULL,
                        DStart                  date ,
                        DEnd                    date ,
                        AcctNumber              char(5)                             DEFAULT '',
                        Amount                  double(18,4)                        DEFAULT 0.0000,
                        IdSched                 int(10)                 unsigned    DEFAULT 0 COMMENT '1 - per pay, 2 - every first period, 3 - every 2nd period',
                        CreatedBy               char(25)                            DEFAULT '',
                        TerminatedBy            char(25)                            DEFAULT '',
                        DaysPara                double(18,2)                        DEFAULT 15.00,
                        P1                      int(10)                 unsigned    DEFAULT 0,
                        P2                      int(10)                 unsigned    DEFAULT 0,
                        P3                      int(10)                 unsigned    DEFAULT 0,
                        P4                      int(10)                 unsigned    DEFAULT 0,
                        P5                      int(10)                 unsigned    DEFAULT 0,
                        Status                  char(1)                             DEFAULT 'A',
                        TrnPosted               char(12)                            DEFAULT '',
                        PRIMARY KEY (`Id`)) Engine = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01_201_FixedEarningsRecreate(string schema, string connName, string country = "PH")
    {
        string sql = $@"Desc {schema}.FixedEarnings ";
        var res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count < 19)
        {
            sql = $@"DROP TABLE if exists  {schema}.FixedEarnings; 
                     CREATE TABLE if not exists  {schema}.FixedEarnings (
                        Id                      int(10)                 unsigned    NOT NULL AUTO_INCREMENT,
                        PayrollgrpId            int                     unsigned,
                        Empnumber               char(5)                             NOT NULL,
                        DStart                  date ,
                        DEnd                    date ,
                        AcctNumber              char(5)                             DEFAULT '',
                        Amount                  double(18,4)                        DEFAULT 0.0000,
                        IdSched                 int(10)                 unsigned    DEFAULT 0 COMMENT '1 - per pay, 2 - every first period, 3 - every 2nd period',
                        CreatedBy               char(25)                            DEFAULT '',
                        TerminatedBy            char(25)                            DEFAULT '',
                        DaysPara                double(18,2)                        DEFAULT 12.00,
                        PerdayEarnings          int(10)                 unsigned    DEFAULT 0,
                        P1                      int(10)                 unsigned    DEFAULT 0,
                        P2                      int(10)                 unsigned    DEFAULT 0,
                        P3                      int(10)                 unsigned    DEFAULT 0,
                        P4                      int(10)                 unsigned    DEFAULT 0,
                        P5                      int(10)                 unsigned    DEFAULT 0,
                        Status                  char(1)                             DEFAULT 'A',
                        TrnPosted               char(12)                            DEFAULT '',
                        PRIMARY KEY (`Id`)) Engine = InnoDB;";
            await _sql.ExecuteCmd(sql, new { }, connName);

        }
    }

    private async Task _01_202_FixedEarnings_Grp(string schema, string connName, string country = "PH")
    {
        var sql = $@"CREATE TABLE if not exists {schema}.`FixedEarnings_Grp` (
                      Id                int(10) unsigned NOT NULL AUTO_INCREMENT,
                      PayrollgrpId      int, 
                      DStart            date            DEFAULT NULL,
                      DEnd              date            DEFAULT NULL,
                      AcctNumber        char(5)         DEFAULT '',
                      Amount            double(18,4)    DEFAULT 0.0000,
                      CreatedbyId       int,
                      TerminatedbyId    int ,
                      DaysPara          double(18,2)    DEFAULT 15.00,
                      P1                int(11)         DEFAULT 0,
                      P2                int(11)         DEFAULT 0,
                      P3                int(11)         DEFAULT 0,
                      P4                int(11)         DEFAULT 0,
                      P5                int(11)         DEFAULT 0,
                      Status            char(1)         DEFAULT 'A',
                      PerdayEarnings    int(11)         DEFAULT 0,
                      TRNPosted         char(12)        DEFAULT '',
                      PRIMARY KEY (`id`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_203_FixedEarnings_Grp_Emp(string schema, string connName, string country = "PH")
    {
        var sql = $@"CREATE TABLE if not exists {schema}.FixedEarnings_Grp_Emp (
                          FixedEarnings_grpId   INTEGER UNSIGNED NOT NULL,
                          EmpmasId              INTEGER UNSIGNED NOT NULL,
                      PRIMARY KEY (`FixedEarnings_grpId`, `EmpmasId`))
                    ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_251_Loans(string schema, string connName, string country = "PH")
    {
        string sql = @$"CREATE TABLE  if not exists {schema}.loans (
                              Id                int(10)         unsigned NOT NULL AUTO_INCREMENT,
                              EMPNUMBER         char(5)                     DEFAULT NULL,
                              DATE              date                        DEFAULT NULL,
                              DEDNCODE          varchar(4)                  DEFAULT NULL,
                              DEDNDESC          varchar(20)                 DEFAULT NULL,
                              AMOUNT            double(8,2)                 DEFAULT NULL,
                              AMORT             double(7,2)                 DEFAULT NULL,
                              BALANCE           double(8,2)                 DEFAULT NULL,
                              STATUS            varchar(1)                  DEFAULT NULL,
                              ENCODEBY          varchar(10)                 DEFAULT NULL,
                              ENCODEDT          varchar(20)                 DEFAULT NULL,
                              CHANGEBY          varchar(10)                 DEFAULT NULL,
                              CHANGEDT          varchar(20)                 DEFAULT NULL,
                              POSTED            varchar(1)                  DEFAULT NULL,
                              EMPLASTNM         varchar(15)                 DEFAULT NULL,
                              POSTFLAG          varchar(24)                 DEFAULT NULL,
                              REMARKS           varchar(75)                 DEFAULT NULL,
                              payMode           varchar(2)      NOT NULL    DEFAULT '',
                              payStart          date,
                              payRes            date,
                              cvno              char(15)                    DEFAULT '',
                              p1 int(10)        unsigned                    DEFAULT '0',
                              p2 int(10)        unsigned                    DEFAULT '0',
                              p3 int(10)        unsigned                    DEFAULT '0',
                              p4 int(10)        unsigned                    DEFAULT '0',
                              p5 int(10)        unsigned                    DEFAULT '0',
                              TRNLastPosted     char(12)                    DEFAULT '',
                              PRIMARY KEY (Id)
                            ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;"; 
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"CREATE TABLE if not exists {schema}.loanhdr (
                    id          int(10) unsigned NOT NULL AUTO_INCREMENT,
                    orno        char(15)        NOT NULL,
                    paydate     date            NOT NULL,
                    yr          char(4)         NOT NULL,
                    mo          char(2)         NOT NULL,
                    amount      double(18,4)    NOT NULL,
                    remarks     varchar(100)                DEFAULT NULL,
                    acctcode    varchar(5)                  DEFAULT NULL,
                    PRIMARY KEY (id)
                ) ENGINE=InnoDB AUTO_INCREMENT=244 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"CREATE TABLE if not exists {schema}.loandtl (
                        Id          int(10)     unsigned        NOT NULL AUTO_INCREMENT,
                        orno        char(15)                    NOT NULL,
                        empnumber   char(5)                     NOT NULL,
                        trn         char(12)    DEFAULT NULL,
                        amount      double(18,4)                NOT NULL,
                        acctnumber  varchar(5)                  NOT NULL,
                        loandate    date                        NOT NULL,
                        amort       double(18,4)                NOT NULL,
                        balance     double(18,4)                NOT NULL,
                        PRIMARY KEY (Id)) 
                ENGINE=InnoDB AUTO_INCREMENT=10336 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01_251_LoansRecreate(string schema, string connName, string country = "PH")
    {
        string sql = $@"Desc {schema}.Loans ";
        var res = await _sql.FetchData<TableStructureModel, dynamic>(sql, new { }, connName);

        if (res.Count < 28)
        {
            sql = $@"ALTER TABLE {schema}.`loans` ADD COLUMN `Principal` DOUBLE(12,2) DEFAULT 0 AFTER `DedNDesc`;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task _01_252_DedMandatory(string schema, string connName, string country = "PH")
    {
        string sql = @$"CREATE TABLE if not exists {schema}.Dedmandatory (
                          Id                    int(10)             unsigned            NOT NULL AUTO_INCREMENT,
                          AcctNumber            varchar(45)                     DEFAULT NULL,
                          ContAmt               double(12, 4)                   DEFAULT NULL,
                          MaxAmt                double(12, 4)                   DEFAULT NULL,
                          Remarks               varchar(200)                    DEFAULT ' ',
                          Status                char(1)                         DEFAULT 'A' COMMENT 'A - Active, I - InActive; S - Stop',
                          P1                    int(10)             unsigned    DEFAULT '0',
                          P2                    int(10)             unsigned    DEFAULT '0',
                          P3                    int(10)             unsigned    DEFAULT '0',
                          P4                    int(10)             unsigned    DEFAULT '0',
                          P5                    int(10)             unsigned    DEFAULT '0',
                        PRIMARY KEY (`Id`) ) ENGINE=InnoDB AUTO_INCREMENT=1     DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_252_DedMandatoryTran(string schema, string connName) {
        string sql = @$"CREATE TABLE if not exists {schema}.DedmandatoryTran (
                          EmpmasId              int(10)             unsigned    NOT NULL DEFAULT '0',
                          AcctNumber            varchar(45)                     NOT NULL DEFAULT '-',
                          Amount                double(12, 4)                   DEFAULT NULL,
                          RefId                 int(10)             unsigned    NOT NULL DEFAULT '0',
                        PRIMARY KEY (EmpmasId, AcctNumber, RefId) ) ENGINE=InnoDB AUTO_INCREMENT=1     DEFAULT CHARSET=latin1;"; 
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    
    // --- Transaction Tables ------------------------------------------------
    private async Task _01_301_PayMainVisibleAcct(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.`PaymainVisAcct` (
                          Trn               char(12)          NOT NULL,
                          AcctNumber        char(10)          NOT NULL,
                          PRIMARY KEY (`trn`,`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;
                          CREATE TABLE  if not exists {schema}.`TmpPaymainVisAcct` (
                          Trn               char(12)          NOT NULL,
                          AcctNumber        char(10)          NOT NULL,
                          PRIMARY KEY (`trn`,`AcctNumber`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;
                          ";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_301_PayMainHdr(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.`paymainhdr` (
                          Trn               char(12)          NOT NULL,
                          ClRate            double(12,4)      NOT NULL    DEFAULT 0.0000,
                          MinRate           double(12,4)      NOT NULL    DEFAULT 0.0000,
                          UserId            int               NOT NULL    DEFAULT 0,
                          Status            char(10)          NOT NULL    DEFAULT 'New',
                          DateCreated       datetime,
                          DatePosted        datetime,
                          AttStart          date,
                          AttEnd            date,
                          PRIMARY KEY (`trn`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01_302_PayMainDtl(string schema, string connName, string country = "PH")
    {
        if (country == "PH")
        {
            string sql = $@"CREATE TABLE  if not exists {schema}.`Paymaindtl` (
                              Trn               char(15)        NOT NULL    DEFAULT '',
                              Empnumber         char(5)         NOT NULL    DEFAULT '',
                              EmpmasId          int,
                              Sss               double(16,4)    DEFAULT 0.0000,
                              SssEr             double(16,4)    DEFAULT 0.0000,
                              SssEc             double(16,4)    DEFAULT 0.0000,
                              Pagibig           double(16,4)    DEFAULT 0.0000,
                              PagibigEr         double(16,4)    DEFAULT 0.0000,
                              Phic              double(16,4)    DEFAULT 0.0000,
                              PhicEr            double(16,4)    DEFAULT 0.0000,
                              PayStatus         char(10)        DEFAULT '0',
                              PayrollGrpId      char(10)        DEFAULT '0',
                              CompanyId         int , 
                              BranchId          int, 
                              DayWrk            double(12,4)    NOT NULL    DEFAULT 0.0000,
                              Absent            double(12,4)    DEFAULT 0.0000,
                              Late              double(12,4)    DEFAULT 0.0000,
                              UTime             double(12,4)    DEFAULT 0.0000,
                              Rn                double(12,4)    DEFAULT 0.0000,
                              RnOt              double(12,4)    DEFAULT 0.0000,
                              Rot               double(12,4)    DEFAULT 0.0000,
                              Rd                double(12,4)    DEFAULT 0.0000,
                              RdOt              double(12,4)    DEFAULT 0.0000,
                              Lh                double(12,4)    DEFAULT 0.0000,
                              LhOt              double(12,4)    DEFAULT 0.0000,
                              RdLh              double(12,4)    DEFAULT 0.0000,
                              RdLhOt            double(12,4)    DEFAULT 0.0000,
                              Sh                double(12,4)    DEFAULT 0.0000,
                              ShOt              double(12,4)    DEFAULT 0.0000,
                              RdSh              double(12,4)    DEFAULT 0.0000,
                              RdShOt            double(12,4)    DEFAULT 0.0000,
                              Custom1           double(12,4)    DEFAULT 0.0000,
                              Custom2           double(12,4)    DEFAULT 0.0000,
                              Custom3           double(12,4)    DEFAULT 0.0000,
                              NdRd              double(12,4)    DEFAULT 0.0000,
                              NdRdOt            double(12,4)    DEFAULT 0.0000,
                              NdRdLh            double(12,4)    DEFAULT 0.0000,
                              NdRdLhOt          double(12,4)    DEFAULT 0.0000,
                              NdRdSh            double(12,4)    DEFAULT 0.0000,
                              NdRdShOt          double(12,4)    DEFAULT 0.0000,
                              Nd                double(12,4)    DEFAULT 0.0000,
                              NdOt              double(12,4)    DEFAULT 0.0000,
                              NdLh              double(12,4)    DEFAULT 0.0000,
                              NdLhOt            double(12,4)    DEFAULT 0.0000,
                              NdSh              double(12,4)    DEFAULT 0.0000,
                              NdShOt            double(12,4)    DEFAULT 0.0000,
                              Dh                double(12,4)    DEFAULT 0.0000,
                              DhOt              double(12,4)    DEFAULT 0.0000,
                              RdDh              double(12,4)    DEFAULT 0.0000,
                              RdDhOt            double(12,4)    DEFAULT 0.0000,
                              PRIMARY KEY (trn, empnumber, empmasId)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task _01_303_PaymaindtlsetupPH(string schema, string connName, string country = "PH")
    {
        if (country == "PH")
        {
            string sql = $@"CREATE TABLE  if not exists {schema}.`paymaindtlsetup` (
                              Id                  int                 default 0,
                              daywrk              char(5)             DEFAULT '',
                              absent              char(5)             DEFAULT '',
                              late                char(5)             DEFAULT '',
                              utime               char(5)             DEFAULT '',
                              rn                  char(5)             DEFAULT '',
                              rnot                char(5)             DEFAULT '',
                              rot                 char(5)             DEFAULT '',
                              rd                  char(5)             DEFAULT '',
                              rdot                char(5)             DEFAULT '',
                              lh                  char(5)             DEFAULT '',
                              lhot                char(5)             DEFAULT '',
                              rdlh                char(5)             DEFAULT '',
                              rdlhot              char(5)             DEFAULT '',
                              sh                  char(5)             DEFAULT '',
                              shot                char(5)             DEFAULT '',
                              rdsh                char(5)             DEFAULT '',
                              rdshot              char(5)             DEFAULT '',
                              nd                  char(5)             DEFAULT '',
                              ndot                char(5)             DEFAULT '',
                              ndrd                char(5)             DEFAULT '',
                              ndrdot              char(5)             DEFAULT '',
                              ndrdlh              char(5)             DEFAULT '',
                              ndrdlhot            char(5)             DEFAULT '',
                              ndrdsh              char(5)             DEFAULT '',
                              ndrdshot            varchar(45)         DEFAULT '',
                              ndlh                char(5)             DEFAULT '',
                              ndlhot              char(5)             DEFAULT '',
                              ndsh                char(5)             DEFAULT '',
                              ndshot              char(5)             DEFAULT '',
                              DH                  char(5)             DEFAULT '',
                              DHOT                char(5)             DEFAULT '',
                              RDDH                char(5)             DEFAULT '',
                              RDDHOT              char(5)             DEFAULT '',
                              NDDH                char(5)             DEFAULT '',
                              NDDHOT              char(5)             DEFAULT '',
                              NDRDDH              char(5)             DEFAULT '',
                              NDRDDHOT            char(5)             DEFAULT '',
                              sss                 char(5)             DEFAULT '',
                              pagibig             char(5)             DEFAULT '',
                              phic                char(5)             DEFAULT '',
                              custom1             char(5)             DEFAULT '',
                              custom2             char(5)             DEFAULT '',
                              custom3             char(5)             DEFAULT '',
                              Allowance           char(5)             DEFAULT '',
                              CashBond            char(5)             DEFAULT '',
                              IncentLeave         char(5)             DEFAULT '',
                              mealAllowance       char(5)             DEFAULT '',
                              ProvForRetirement   char(5)             DEFAULT '',
                              FreeUniform         char(5)             DEFAULT '',
                              _13thMo             char(5)             DEFAULT '',
                              retropay            char(5)             DEFAULT '',
                              PRIMARY KEY (Id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
            await _sql.ExecuteCmd(sql, new { }, connName);

            sql = $"select * from {schema}.paymaindtlsetup limit 1";
            var paymaindtlsetup = await _sql.FetchData<CoaModel, dynamic>(sql, new { }, connName);
            if (paymaindtlsetup == null || paymaindtlsetup.Count == 0)
            {
                sql = @$"INSERT INTO {schema}.paymaindtlsetup (daywrk, absent, rot,    rd,     rdot,   lh,       lhot,   rdlh,    rdlhot, sh,            shot,          rdsh,              rdshot,      nd,          
                                                               RDDH,   RDDHOT, NDDH,   NDDHOT, NDRDDH, NDRDDHOT, sss,    pagibig, phic,   IncentLeave,   mealAllowance, ProvForRetirement, FreeUniform, retropay) VALUES 
                                                              ('E001', 'E002', 'E003', 'E005', 'E006', 'E007',   'E008', 'E011',  'E012', 'E009',        'E010',        'E013',            'E014',      'E004', 
                                                               'E081', 'E082', 'E083', 'E084', ' ',    ' ',      'D002', 'D004',  'D003', 'E018',        'E029',        'E028',            'E025',      'E023');";
                await _sql.ExecuteCmd(sql, new { }, connName);
            }
        }
    }

    private async Task _01_304_PayMainHistory(string schema, string connName, string country)
    {
        string sql = $@"CREATE TABLE  if not exists  {schema}.PaymainHistory (
                          Id        INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          Trn       CHAR(12) NOT NULL,
                          UserId    INTEGER UNSIGNED NOT NULL,
                          Posted    DATETIME,
                          Action    VARCHAR(60),
                          PRIMARY KEY(`Id`))
                        ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_305_PayRateType(string schema, string connName)
    {

        string sql = @$"CREATE TABLE if not exists {schema}.PayrateType (
                            Id          INTEGER         UNSIGNED    NOT NULL AUTO_INCREMENT,
                            Code        CHAR(5)                     NOT NULL,
                            Name        VARCHAR(45)                 NOT NULL,
                            PRIMARY KEY (`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $"select * from {schema}.PayrateType limit 1";
        var res = await _sql.FetchData<PayRateTypeModel, dynamic>(sql, new { }, connName);
        if (res.Count < 1)
        {
            sql = @$"INSERT INTO {schema}.PayrateType 
                        (Id, Code,      Name) VALUES 
                        (1, 'WK',       'Weekly'),  
                        (2, 'BWK',      'Bi-weekly'),  
                        (3, 'SMo',      'Semi-Monthly'),  
                        (4, 'Mo',       'Monthly'),  
                        (5, 'ANN',      'Annually') ;";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

        //------------------------------------------------------------------------------------------
        sql = @$"CREATE TABLE if not exists {schema}.PayrateTypeDtls (
                            Id              INTEGER         UNSIGNED    NOT NULL AUTO_INCREMENT,
                            PayrateTypeId   Integer, 
                            Name            VARCHAR(45)                 NOT NULL,
                            PeriodCode      Char(2), 
                            PRIMARY KEY (`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $"select * from {schema}.PayrateTypeDtls limit 1";
        var res1 = await _sql.FetchData<PayRateTypeDtlModel, dynamic>(sql, new { }, connName);

        if (res1.Count < 1)
        {
            sql = @$"Insert into {schema}.PayrateTypeDtls values 
                        (1, 1, 'Week 1', '01'),
                        (2, 1, 'Week 2', '02'),
                        (3, 1, 'Week 3', '03'),
                        (4, 1, 'Week 4', '04'),
                        (5, 1, 'Week 5', '05'),
                        (6, 2, 'First Half', '01'),
                        (7, 2, 'Second Half', '02'),
                        (8, 3, 'First Half', '01'),
                        (9, 3, 'Second Half', '02')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

    }

    private async Task _01_401_TmpManHr(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.ManHr (
                           Trn          varchar(12)     NOT NULL DEFAULT ' ',
                           EmpmasId     int             NOT NULL default 0,
                           EmpNumber    char(10)        NOT NULL default ' ',
                           AcctNumber   char(5)         NOT NULL default ' ',
                           Date         Date,
                           DayTypeId    Int             Not null Default 0, 
                           Qty          double(10,4)    NOT NULL Default 0,
                           Rate         double(10,4)    NOT NULL Default 0,
                           RateTypeId   int             NOT NULL DEFAULT 0,      
                        PRIMARY KEY (Trn, EmpmasId, EmpNumber, AcctNumber) USING BTREE )  
                        ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_401_ManHr(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.TmpManHr (
                           Trn          varchar(12)     NOT NULL DEFAULT ' ',
                           EmpmasId     int             NOT NULL default 0,
                           EmpNumber    char(10)         NOT NULL default ' ',
                           AcctNumber   char(5)         NOT NULL default ' ',
                           Date         Date,
                           DayTypeId    Int             Not null Default 0, 
                           Qty          double(10,4)    NOT NULL Default 0,
                           Rate         double(10,4)    NOT NULL Default 0,
                           RateTypeId   int             NOT NULL DEFAULT 0,      
                        PRIMARY KEY (Trn, EmpmasId, EmpNumber, AcctNumber) USING BTREE ) 
                        ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01_402_TmpTbltran(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.TmpTbltran (
                            TRN             varchar(12)     NOT NULL    DEFAULT ' ',  
                            EmpmasId        int             NOT NULL    default 0,
                            empNumber       varchar(10)                  DEFAULT ' ',
                            acctNumber      char(5)         NOT NULL,
                            Qty             double(10,4)    NOT NULL    Default 1,
                            Rate            double(10,4)    NOT NULL    Default 0,
                            RateTypeId      int             NOT NULL    DEFAULT 1,      
                            amount          double(10,4)    NOT NULL    Default 0,
                            dTimeStamp      datetime,
                            postedby        int                         DEFAULT 0,
                            Source          char(5)                     Default '-',
                            Status          char(1)                     Default '-',
                            RefId           int             NOT NULL    default 0,
                            PRIMARY         KEY (TRN, acctNumber, EmpmasId, Source, RefId) USING BTREE) 
                        ENGINE=InnoDB DEFAULT CHARSET=latin1;"; 
                              
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"CREATE TABLE  if not exists {schema}.TmpTbltranEmpList (
                      trn           CHAR(12) NOT NULL,
                      empmasId      INTEGER UNSIGNED NOT NULL,
                      Emprate       DOUBLE(12,4) NOT NULL,
                      PayrateId     INTEGER UNSIGNED NOT NULL,
                      PayrollgrpId  INTEGER UNSIGNED NOT NULL,
                      PRIMARY KEY(`trn`,`empmasId`)) ENGINE = InnoDB;";

        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@"CREATE TABLE  if not exists {schema}.TmpTbltranCoaList (
                      trn           CHAR(12) NOT NULL,
                      AcctNumber    CHAR(5) NOT NULL,
                      PRIMARY KEY(`trn`,`AcctNumber`)) ENGINE = InnoDB;";

        await _sql.ExecuteCmd(sql, new { }, connName);



    }

    private async Task _01_402_Tbltran(string schema, string connName, string country = "PH")
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.tbltran (
                            TRN             varchar(12)     NOT NULL    DEFAULT ' ',  
                            EmpmasId        int             NOT NULL    default 0,
                            empNumber       varchar(10)                  DEFAULT ' ',
                            acctNumber      char(5)         NOT NULL,
                            Qty             double(10,4)    NOT NULL    Default 1,
                            Rate            double(10,4)    NOT NULL    Default 0,
                            RateTypeId      int             NOT NULL    DEFAULT 1,      
                            amount          double(10,4)    NOT NULL    Default 0,
                            dTimeStamp      datetime,
                            postedby        int                         DEFAULT 0,
                            Source          char(5)                     Default '-',
                            Status          char(1)                     Default '-',
                            RefId           int             NOT NULL    default 0,
                            PRIMARY         KEY (TRN, acctNumber, EmpmasId, Source, RefId) USING BTREE) 
                        ENGINE=InnoDB DEFAULT CHARSET=latin1;"; 

        await _sql.ExecuteCmd(sql, new { }, connName);

    }



}
