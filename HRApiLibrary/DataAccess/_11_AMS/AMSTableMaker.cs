using HRApiLibrary.DataAccess._90_Utils.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._11_AMS;

public class AMSTableMaker : IAMSTableMaker
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public AMSTableMaker(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01AMSTable(string schema, string connName = "MySqlConn")
    {
        try
        {
            await _01AttSchedDefault(schema, connName);
            await _01AttSchedWeeklyHdr(schema, connName);
            await _01AttSchedWeeklyDtl(schema, connName);
            await _01AttSchedDaily(schema, connName);
            await _01AttAdvanceSchedule(schema, connName);
            await _01Atttemplate_Schedule(schema, connName);
            await Ams_OTSettings(schema, connName);
            await _01BioDailyPunches(schema, connName);
            await _01BioManHourHdr(schema, connName);
            await _01BioManHour(schema, connName);
            await _01BioManHourMapping(schema, connName);
            await _01Sites(schema, connName);
            



            await _03Atttemplate_Schedule(schema, connName);
        }

        catch (Exception ex)    {   Console.WriteLine($"Error in _01AttSchedDefault: {ex}"); }

    }

    private async Task _01AttSchedDefault(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.AttSchedDefault (
                          Id                    INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          D1_In                 int             DEFAULT 0,
                          D1_HrsLength          int             DEFAULT 0,
                          D1_DutyType           char(2)         DEFAULT 'RD',
                          D2_In                 int             DEFAULT 800,
                          D2_HrsLength          int             DEFAULT 900,
                          D2_DutyType           char(2)         DEFAULT 'R',
                          D3_In                 int             DEFAULT 800,
                          D3_HrsLength          int             DEFAULT 900,
                          D3_DutyType           char(2)         DEFAULT 'R',
                          D4_In                 int             DEFAULT 800,
                          D4_HrsLength          int             DEFAULT 900,
                          D4_DutyType           char(2)         DEFAULT 'R',
                          D5_In                 int             DEFAULT 800,
                          D5_HrsLength          int             DEFAULT 900,
                          D5_DutyType           char(2)         DEFAULT 'R',
                          D6_In                 int             DEFAULT 800,
                          D6_HrsLength          int             DEFAULT 9,
                          D6_DutyType           char(2)         DEFAULT 'R',
                          D7_In                 int             DEFAULT 0,
                          D7_HrsLength          int             DEFAULT 0,
                          D7_DutyType           char(2)         DEFAULT 'RN',
                          PRIMARY KEY(`Id`)
                        ) ENGINE = InnoDB DEFAULT CHARSET = latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    
    private async Task _01Biolog(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.Biolog (
                        Id              BIGINT          NOT NULL AUTO_INCREMENT,
                        DeviceNo        VARCHAR(20)     NOT NULL,
                        BiometricEmpNo  VARCHAR(10)     NOT NULL,
                        LogDatetime     DATETIME        NOT NULL,
                        PunchDate       DATE            NOT NULL,
                        VerifyMode      TINYINT         NOT NULL DEFAULT 1,
                        AttState        TINYINT         NOT NULL DEFAULT 0,
                        WorkCode        VARCHAR(20)     NULL,
                        ImportBatch     VARCHAR(50)     NOT NULL,
                        ImportedOn      DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        IsProcessed     TINYINT(1)      NOT NULL DEFAULT 0,
                        PRIMARY KEY (Id),
                        UNIQUE INDEX uidx_biolog_emp_datetime  (BiometricEmpNo, LogDatetime),
                        INDEX idx_biolog_emp_date              (BiometricEmpNo, PunchDate),
                        INDEX idx_biolog_processed             (IsProcessed));";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    
    private async Task _01AttSchedWeeklyHdr(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.AttSchedWeeklyHdr (
                          Id                    INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          Code                  char(15)         DEFAULT '-',
                          Description           char(60)         DEFAULT 'R',
                          PRIMARY KEY(`Id`)
                        ) ENGINE = InnoDB DEFAULT CHARSET = latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01AttSchedWeeklyDtl(string schema, string connName)
    {
        var sql = $@"CREATE TABLE IF NOT EXISTS {schema}.AttSchedWeeklyDtl (
                            Id                      INTEGER UNSIGNED    NOT NULL AUTO_INCREMENT,
                            AttSchedWeeklyHdrId     INTEGER UNSIGNED    NOT NULL DEFAULT 0,
                            D1_DutyType             char(2)             DEFAULT 'RD',
                            D1_In                   int                 DEFAULT 0,
                            D1_HrsLength            int                 DEFAULT 0,
                            D1_Out                  int                 DEFAULT 0,
                            D2_DutyType             char(2)             DEFAULT 'R',
                            D2_In                   int                 DEFAULT 800,
                            D2_HrsLength            int                 DEFAULT 900,
                            D2_Out                  int                 DEFAULT 0,
                            D3_DutyType             char(2)             DEFAULT 'R',
                            D3_In                   int                 DEFAULT 800,
                            D3_HrsLength            int                 DEFAULT 900,
                            D3_Out                  int                 DEFAULT 0,
                            D4_DutyType             char(2)             DEFAULT 'R',
                            D4_In                   int                 DEFAULT 800,
                            D4_HrsLength            int                 DEFAULT 900,
                            D4_Out                  int                 DEFAULT 0,
                            D5_DutyType             char(2)             DEFAULT 'R',
                            D5_In                   int                 DEFAULT 800,
                            D5_HrsLength            int                 DEFAULT 900,
                            D5_Out                  int                 DEFAULT 0,
                            D6_DutyType             char(2)             DEFAULT 'R',
                            D6_In                   int                 DEFAULT 800,
                            D6_HrsLength            int                 DEFAULT 900,
                            D6_Out                  int                 DEFAULT 0,
                            D7_DutyType             char(2)             DEFAULT 'RN',
                            D7_In                   int                 DEFAULT 0,
                            D7_HrsLength            int                 DEFAULT 0,
                            D7_Out                  int                 DEFAULT 0,
                            PRIMARY KEY (`Id`),
                            KEY `idx_AttSchedWeeklyHdrId` (`AttSchedWeeklyHdrId`)
                        ) ENGINE = InnoDB DEFAULT CHARSET = latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01AttSchedDaily(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.AttSchedDaily (
                      Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                      Name          VARCHAR(45),
                      DutyType      CHAR(2)         DEFAULT 'R' COMMENT 'R, RD,RN',
                      PIn           INTEGER         DEFAULT 0,
                      Duration      INTEGER         DEFAULT 0,
                      POut          INTEGER         DEFAULT 0,
                      Nd            DOUBLE(5,2)     DEFAULT 0,
                      PRIMARY KEY (`Id`)
                    ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    
    private async Task _01AttAdvanceSchedule(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.`AttAdvanceSchedule` (
                        `Id`                INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        `Empnumber`         CHAR(5),
                        Date                DATE,
                        `attscheddailyId`   INTEGER         UNSIGNED,
                        `DutyType`          CHAR(2),
                        `PIn`               INTEGER         UNSIGNED,
                        `Duration`          INTEGER         UNSIGNED,
                        `Pout`              INTEGER         UNSIGNED,
                        `ND`                DOUBLE(5,2),
                        PRIMARY KEY (`Id`) ) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task _01Atttemplate_Schedule(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists  {schema}.`Atttemplate_Schedule` (
                        EmpmasId                int NOT NULL,
                        Effectivity             date,
                        IsImplemented           int         DEFAULT 0,
                        AttendanceTypeId        int         DEFAULT '1',
                        D1_In                   int         DEFAULT '8000',
                        D1_HrsLength            int         DEFAULT '8',
                        D1_DutyType             char(2)     DEFAULT 'R',
                        D2_In                   int         DEFAULT '8000',
                        D2_HrsLength            int         DEFAULT '8',
                        D2_DutyType             char(2)     DEFAULT 'R',
                        D3_In                   int         DEFAULT '8000',
                        D3_HrsLength            int         DEFAULT '8',
                        D3_DutyType             char(2)     DEFAULT 'R',
                        D4_In                   int         DEFAULT '8000',
                        D4_HrsLength            int         DEFAULT '8',
                        D4_DutyType             char(2)     DEFAULT 'R',
                        D5_In                   int         DEFAULT '8000',
                        D5_HrsLength            int         DEFAULT '8',
                        D5_DutyType             char(2)     DEFAULT 'R',
                        D6_In                   int         DEFAULT '0',
                        D6_HrsLength            int         DEFAULT '0',
                        D6_DutyType             char(2)     DEFAULT 'RD',
                        D7_In                   int         DEFAULT '0',
                        D7_HrsLength            int         DEFAULT '0',
                        D7_DutyType             char(2)     DEFAULT 'RD',
                        PRIMARY KEY (`EmpmasId`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    private async Task Ams_OTSettings(string schema, string connName)
    {
        var sql = $@"CREATE TABLE if not exists {schema}.Ams_OTSettings (
                          Empnumber        CHAR(5) NOT NULL,
                          NeedsOTFiling    INTEGER UNSIGNED DEFAULT 1,
                          MaxOTHour        DOUBLE(8,2) DEFAULT 0,
                          PRIMARY KEY (`Empnumber`)) ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01BioDailyPunches(string schema, string connName)
    {
        var sql = $@"CREATE TABLE IF NOT EXISTS {schema}.BioDailyPunches (
                    EmpNumber       VARCHAR(5)      NOT NULL,
                    Date            DATE            NOT NULL,
                    PayrollGrpId    INT             NULL,
                    DutyType        VARCHAR(2)      NULL,
                    DayType         VARCHAR(2)      NULL,
                    InSchedule      INT             NULL,
                    OutSchedule     INT             NULL,
                    PunchIn         DATETIME        NULL,
                    PunchOut        DATETIME        NULL,
                    DayWork         DOUBLE(8,2)     NULL,
                    Overtime        DOUBLE(8,2)     NULL,
                    DutyStatus      VARCHAR(10)     NULL,
                    ND              DOUBLE(8,2)     NULL,
                    Late            DOUBLE(8,2)     NULL,
                    Undertime       DOUBLE(8,2)     NULL,
                    Absent          DOUBLE(8,2)     NULL,
                    PRIMARY KEY (EmpNumber, Date),
                    INDEX idx_biodailypunches_payrollgrp (PayrollGrpId),
                    INDEX idx_biodailypunches_date       (Date));";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01BioManHourHdr(string schema, string connName)
    {
        var sql = $@"CREATE TABLE IF NOT EXISTS {schema}.BioManHourHdr (
                    Id              BIGINT          NOT NULL AUTO_INCREMENT,
                    PayrollGrpId    INT             NOT NULL,
                    CoverageStart   DATE            NOT NULL,
                    CoverageEnd     DATE            NOT NULL,
                    Remarks         VARCHAR(200)    NULL,
                    PRIMARY KEY (Id),
                    UNIQUE INDEX uidx_biomanhourHdr (PayrollGrpId, CoverageStart));";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01BioManHour(string schema, string connName)
    {
        var sql = $@"CREATE TABLE IF NOT EXISTS {schema}.BioManHour (
                    Id                  BIGINT          NOT NULL AUTO_INCREMENT,
                    BioManHourHdrId     BIGINT          NOT NULL,
                    EmpNumber           VARCHAR(5)      NOT NULL,
                    RdDays              DOUBLE(8,2)     NULL,
                    RdRd                DOUBLE(8,2)     NULL,
                    RdTardiness         DOUBLE(8,2)     NULL,
                    RdOt                DOUBLE(8,2)     NULL,
                    LhDays              DOUBLE(8,2)     NULL,
                    LhRd                DOUBLE(8,2)     NULL,
                    LhTardiness         DOUBLE(8,2)     NULL,
                    LhOt                DOUBLE(8,2)     NULL,
                    ShDays              DOUBLE(8,2)     NULL,
                    ShRd                DOUBLE(8,2)     NULL,
                    ShTardiness         DOUBLE(8,2)     NULL,
                    ShOt                DOUBLE(8,2)     NULL,
                    DhDays              DOUBLE(8,2)     NULL,
                    DhRd                DOUBLE(8,2)     NULL,
                    DhTardiness         DOUBLE(8,2)     NULL,
                    DhOt                DOUBLE(8,2)     NULL,
                    PRIMARY KEY (Id),
                    UNIQUE INDEX uidx_biomanhour (BioManHourHdrId, EmpNumber),
                    INDEX idx_biomanhour_hdr (BioManHourHdrId));";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01BioManHourMapping(string schema, string connName)
    {
        var sql = $@"
        CREATE TABLE IF NOT EXISTS {schema}.BioManHourMapping (
            Id              INT         NOT NULL AUTO_INCREMENT,
            RdDays          CHAR(5)     NULL,
            RdRd            CHAR(5)     NULL,
            RdTardiness     CHAR(5)     NULL,
            RdOt            CHAR(5)     NULL,
            LhDays          CHAR(5)     NULL,
            LhRd            CHAR(5)     NULL,
            LhTardiness     CHAR(5)     NULL,
            LhOt            CHAR(5)     NULL,
            ShDays          CHAR(5)     NULL,
            ShRd            CHAR(5)     NULL,
            ShTardiness     CHAR(5)     NULL,
            ShOt            CHAR(5)     NULL,
            DhDays          CHAR(5)     NULL,
            DhRd            CHAR(5)     NULL,
            DhTardiness     CHAR(5)     NULL,
            DhOt            CHAR(5)     NULL,
            PRIMARY KEY (Id)
        );

        INSERT INTO {schema}.BioManHourMapping (Id)
        SELECT 1 WHERE NOT EXISTS (SELECT 1 FROM {schema}.BioManHourMapping WHERE Id = 1 ); ";

        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01Sites(string schema, string connName)
    {
        var sql = $@"
        CREATE TABLE IF NOT EXISTS {schema}.Sites (
            Id              INT             NOT NULL AUTO_INCREMENT,
            Code            VARCHAR(20)     NOT NULL,
            Name            VARCHAR(100)    NOT NULL,
            Address         VARCHAR(255)    NULL,
            Latitude        DECIMAL(10,8)   NOT NULL,
            Longitude       DECIMAL(11,8)   NOT NULL,
            RadiusMeters    INT             NOT NULL DEFAULT 100,
            Status          CHAR(1)         NOT NULL DEFAULT 'A',
            PRIMARY KEY (Id),
            UNIQUE KEY UQ_Sites_Code (Code)
        );" ;
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = $@" CREATE TABLE IF NOT EXISTS {schema}.EmpSites (
                    Id          INT NOT NULL AUTO_INCREMENT,
                    EmpmasId    INT NOT NULL,
                    SiteId      INT NOT NULL,
                    PRIMARY KEY (Id),
                    UNIQUE KEY UQ_EmpSites (EmpmasId, SiteId));";  

        await _sql.ExecuteCmd(sql, new { }, connName);
    }

















    private async Task _03Atttemplate_Schedule(string schema, string connName)
    {
        var sqls = new[]
        {
            $@"ALTER TABLE {schema}.atttemplate ADD COLUMN AttschedweeklyhdrId      INT Default 0 ",
            $@"ALTER TABLE {schema}.atttemplate ADD COLUMN AttschedweeklyhdrIdAdv   INT Default 0 ",
            $@"ALTER TABLE {schema}.atttemplate ADD COLUMN ChangeSchedEffectivity   DATE ",
            $@"ALTER TABLE {schema}.atttemplate ADD COLUMN ChangeSchedEnd           DATE "
        };

        foreach (var sql in sqls)
        {
            try {   await _sql.ExecuteCmd(sql, new { }, connName); }
            catch (MySqlException ex) when (ex.Number == 1060) 
            {   
                // Existing column — ignore
            }
        }
    }
}


public interface IAMSTableMaker
{
    Task _01AMSTable(string schema, string connName = "MySqlConn");
}
