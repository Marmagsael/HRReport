
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using MySqlX.XDevAPI;
using System;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00MainPisTblMakerAccess : I_00MainPisTblMakerAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    private readonly I_20_002_PayTblMaker _payTblMaker;


    public _00MainPisTblMakerAccess(I_90_001_MySqlDataAccess sql, I_20_002_PayTblMaker payTblMaker)
    {
        _sql = sql;
        _payTblMaker = payTblMaker;
    }

    public async Task _01UserTable(string schema, string connName = "MySqlConn")
    {
        await _01Schema(schema, connName);
        await _01Mail(schema, connName);
        await _01Engagement(schema, connName);
        await _01MyMovement(schema, connName);
        await _01EmpmasFamilyRef(schema, connName);
        await _01EmpmasFamily(schema, connName);
        await _01EmpmasRelativesRef(schema, connName);
        await _01EmpmasRelatives(schema, connName);
        await _01EmpmasEmergencyContact(schema, connName);
        await _01EmpmasGovPh(schema, connName);
     

        await _01EmpmasEducation(schema, connName);

        await _01EmpmasCharRef(schema, connName);
        await _01EmpmasEmployment(schema, connName);
        await _01EmpmasTraining(schema, connName);
        await _01EmpmasClearancePh(schema, connName);

        await _01EmpmasFileUploadCategory(schema, connName);
        await _01EmpmasFileUpload(schema, connName);

        await _01RCivStat(schema, connName);

    }
    public async Task _01MainPisTable(string schema = "MainPis", string connName = "MySqlConn")
    {
        await _01MainPisSchema(schema, connName);

        if (schema == "MainPis") await _01Empmas(schema, connName);
        else
        {
            await _01EmpmasInternal(schema, connName);

            await _01EmpmasEducation(schema, connName);
            await _01EmpmasCharRef(schema, connName);
            await _01EmpmasEmergencyContact(schema, connName);

            await _01EmpmasFamilyRef(schema, connName);
            await _01EmpmasFamily(schema, connName);
            await _01EmpmasRelativesRef(schema, connName);
            await _01EmpmasRelatives(schema, connName);

            await _01EmpmasGrp(schema, connName);
            await _01EmpmasEmployment(schema, connName);
            await _01EmpmasGovPh(schema, connName);
            await _01EmpmasInsurance(schema, connName);
            await _01EmpmasSecLic(schema, connName);

            await _01EmpmasTraining(schema, connName);

        }

        await _01EmpmasPI(schema, connName);
        await _01EmpmasAddress(schema, connName);
    }

    public async Task _01MainPisTableInternal(string schema, string connName = "MySqlConn")
    {
        await _01MainPisSchema(schema, connName);
        await _payTblMaker._00_002_SystemUser(schema, connName);
        await _payTblMaker._00_003_SystemUserModuleAccess(schema, connName);
        await _payTblMaker._00_004_SystemUserOtherAccess(schema, connName);
       
        //--- Accountability Table ------------------------------------
        await _01Inv(schema, connName);
        await _01Inv_MasterTbl(schema, connName);
        await _01Inv_MasterTblCreateData(schema, connName);
        //=============================================================



       
        await _01RDep(schema, connName);
        await _01DeployMode(schema, connName);
        await _01EmployType(schema, connName);
        await _01EmploymentType(schema, connName);
        await _01RecreateEmploymentType(schema, connName);


        // -- Master Tables ------------------------------------
        await _01AttenanceTemplate(schema, connName);
        await _01AttendanceType(schema, connName);
        await _01AttendanceDutyType(schema, connName);
        await _01RCivStat(schema, connName);
        await _01RCoInfoPH(schema, connName);
        await _01RCollege(schema, connName);
        await _01RDeviation(schema, connName);
        await _01REmpStat(schema, connName);
        // _01RecreateRempstat(schema, connName);
        await _01RLanguageSpoken(schema, connName);
        await _01RDivision(schema, connName);
        await _01RDepartment(schema, connName);
        await _01RSection(schema, connName);
        await _01RPosition(schema, connName);
        await _01RDesignation(schema, connName);
        await _01RDeployment(schema, connName);
        await _01RDevdata(schema, connName);
        await _01RDevdataRecreate(schema, connName);

        await _01RPenalty(schema, connName);
        await _01RecreateRPenalty(schema, connName);

        await _01LeaveType(schema, connName);
        await _01LeaveGrp(schema, connName);
        await _01LeaveGrpApprover(schema, connName);
        await _01LeaveCredit(schema, connName);
        await _01LeaveDefaultApprover(schema, connName);
        await _01LeaveApplication(schema, connName);


        await _01PisSettings(schema, connName);
        


        await _01RempstatForDeployment(schema, connName);
        await _01Rempstat_fordeviation(schema, connName);
        await _01Rempstat_fordisciplinary(schema, connName);
        await _01Rempstat_forexonerate(schema, connName);
        await _01Rempstat_forexonerate(schema, connName);
        await _01Rempstat_forreinstatement(schema, connName);


        // -- Transaction Tables -------------------------------
        await _01AttenanceDaily(schema, connName);
        await _01AttenanceMonthly(schema, connName);
        await _01EmpMovement(schema, connName);
        await _01Companies(schema, connName);
        await _01Deviation(schema, connName);
        await _01EmpmasInternal(schema, connName);
        await _01EmpmasAddress(schema, connName);
        await _01EmpmasPI(schema, connName);
        await _01EmpmasTraining(schema, connName);


        await _01DepRec(schema, connName);
        await _01EmpBlockPost(schema, connName);


        await _01TranDeployment(schema, connName);
        await _01TranDeploymentApproval(schema, connName);
        await _01TranDeploymentApprovalHistory(schema, connName);
        await _01RecreateTranDeployment(schema, connName);
        await _01RecreateTranDeploymentApproval(schema, connName);


        await _01TranDeviation(schema, connName);
        await _01RecreateTranDeviation(schema, connName);
        await _01TranDeviationApproval(schema, connName);
        await _01TranDeviationHistory(schema, connName);
        await _01TrandeviationOther(schema, connName);
        await _01RecreateTrandeviationOther(schema, connName);


        await _01TranDisciplinary(schema, connName);
        await _01TranDisciplinaryAppr(schema, connName);
        await _01TranDisciplinaryApprHistory(schema, connName);


        await _01TranExonerate(schema, connName);

        await _01TranExonerateAppr(schema, connName);
        await _01TranExonerateOther(schema, connName);
        await _01TranExonerateApprHistory(schema, connName);



        await _01TranInvestigate(schema, connName);
        await _01TranInvestigateAppr(schema, connName);
        await _01TranInvestigateApprHistory(schema, connName);


        await _01TranReinstatement(schema, connName);
        await _01TranReinstatementAppr(schema, connName);
        await _01TranReinstatementApprHistory(schema, connName);


        await _01EmpTranMovement(schema, connName);
        await _01RecreateEmpTranMovement(schema, connName);


        await _01Para(schema, connName);
       
    }


    //********************************************************************
    //*** Private Functions **********************************************
    //********************************************************************
    
    
    
    // --- Schema User Profiles ------------------------------------------------------
    private async Task _01Schema(string schema, string connName)
    {
        var sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    // --- Schema Equipment ------------------------------------------------------
    
    private async Task _01Inv_MasterTbl(string equipdb, string connName)
    {
      
        var sql = $"""
                   CREATE TABLE if not exists {equipdb}.Inv_Status (
                     Id         INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                     `Name`     VARCHAR(45) ,
                   PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                    
                   CREATE TABLE if not exists {equipdb}.Inv_Type (
                        Id         INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        `Name`     VARCHAR(45) ,
                   PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                    
                   CREATE TABLE if not exists {equipdb}.Inv_Category (
                        Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        Inv_typeId      INTEGER UNSIGNED DEFAULT 0,
                        `Name`          VARCHAR(45) ,
                   PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                    
                   CREATE TABLE if not exists {equipdb}.Inv_Brand (
                        Id                  INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        Inv_CategoryId      INTEGER UNSIGNED DEFAULT 0,
                        `Name`     VARCHAR(45) ,
                   PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                    
                   CREATE TABLE if not exists {equipdb}.Inv_Make (
                        Id                  INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        Inv_CategoryId      INTEGER UNSIGNED DEFAULT 0,
                        `Name`     VARCHAR(45) ,
                   PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                   
                     
                   """; 
        await _sql.ExecuteCmd(sql, new { },connName);
    }
    
    private async Task _01Inv_MasterTblCreateData(string equipdb, string connName)
    {

        //--- Inv_Status -----------------------------------------------------------
        var cmd = $"select * from {equipdb}.Inv_Status limit 1 ";
        var resStatus = await _sql.FetchData<Inv_statusModel, dynamic>(cmd, new { }, connName);
        if (resStatus.Count == 0)
        {
            cmd = $@"insert into {equipdb}.Inv_Status (Name) values ('Available'),('Issued'), ('Damage'), ('Lost') ";
            await _sql.ExecuteCmd(cmd, new { }, connName);
        }

      

        //--- Inv_Type -----------------------------------------------------------
        cmd = $"select * from {equipdb}.Inv_Type limit 1 ";
        var resType = await _sql.FetchData<Inv_typeModel, dynamic>(cmd, new { }, connName);
        //Console.WriteLine($"Inv_Type Count : {resType.Count}.");
        if (resType.Count == 0)
        {
            cmd = $@"insert into {equipdb}.Inv_Type (Id, Name) values 
                         (1, 'Appliances'), 
                         (2, 'Equipment-ICT'),
                         (3, 'Equipment-Office Use'), 
                         (4, 'Equipment-Tools'), 
                         (5, 'Firearms'), 
                         (6, 'Furniture'), 
                         (8, 'Security Equipment'), 
                         (7, 'Vehicle') ";
            await _sql.ExecuteCmd(cmd, new { }, connName);
        }
        //Console.WriteLine($"Inv_Type written.");
       

        //--- Inv_Category -----------------------------------------------------------
        cmd = $"select * from {equipdb}.Inv_Category limit 1 ";
        var resCategory = await _sql.FetchData<Inv_categoryModel, dynamic>(cmd, new { }, connName);
        if (resCategory.Count == 0)
        {
            cmd = $@"insert into {equipdb}.Inv_Category (Inv_TypeId, Name) values 
                         (4, 'Car'), 
                         (4, 'Motorcycle'), 
                         
                         (4, 'Power Tool'), 
                         (4, 'Hand Tool'), 
                         
                         (2, 'Desktop Computer'), 
                         (2, 'Laptop'), 
                         (2, 'Mobile Device'), 
                         (2, 'Printer'), 
                         
                         (1, 'Airconditioner'), 
                         (1, 'Electric Fan'), 
                         (1, 'Refrigerator'), 
                         (1, 'TV'), 
                         (0, 'Undefined')  ";
            await _sql.ExecuteCmd(cmd, new { }, connName);
        }
      

    }
    
    private async Task _01Inv(string equipdb, string connName)
    {

        
        var sql = @$"
                   CREATE TABLE if not exists {equipdb}.Inv (
                         Id                     INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                         Name                   VARCHAR(80) NOT NULL,
                         TypeId                 INTEGER UNSIGNED DEFAULT 0,
                         MakeId                 INTEGER UNSIGNED DEFAULT 0,
                         ModelId                INTEGER UNSIGNED DEFAULT 0,
                         CategoryId             INTEGER UNSIGNED DEFAULT 0,
                         BrandId                INTEGER UNSIGNED DEFAULT 0,
                         Description            TEXT,
                         SerialNo               VARCHAR(25),
                         DatePurchased          DATE ,
                         DateWarrantyExpiration DATE ,
                         UnitCost               DOUBLE(10,2),
                         Status                 CHAR(1) DEFAULT 'A',
                         DeploymentId           INTEGER UNSIGNED,
                         EmpmasId               INTEGER UNSIGNED,
                         EmpNumber              CHAR(10),
                         DateAssignment         DATE,
                         AssignmentNo           CHAR(10),
                         DateEncoded            DATE , 
                         EncodedbyId            INTEGER UNSIGNED DEFAULT 0, 
                         PRIMARY KEY (`Id`)) ENGINE = InnoDB;
                   CREATE TABLE if not exists {equipdb}.InvDtl (
                       Id                     INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                       InvId                  INTEGER UNSIGNED Default 0,
                       Descr_                 VARCHAR(80) NOT NULL,
                       Value_                 VARCHAR(80) NOT NULL,
                       PRIMARY KEY (`Id`)) ENGINE = InnoDB; 
                        
                   "; 
        await _sql.ExecuteCmd(sql, new { }, connName);
    }
    
    // --- Tables ---------------------------------------------------------------
    private async Task _01AttenanceTemplate(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.AttTemplate (
                            EmpmasId            int         NOT NULL PRIMARY KEY,
                            AttendanceTypeId    int         default 1, 
                            D1_In               INTEGER     default 8000,
                            D1_HrsLength        INTEGER     default 8,
                            D1_DutyType         Char(2)     default 'R',
                            
                            D2_In               INTEGER     default 8000,
                            D2_HrsLength        INTEGER     default 8,
                            D2_DutyType         Char(2)     default 'R',
                            
                            D3_In               INTEGER     default 8000,
                            D3_HrsLength        INTEGER     default 8,
                            D3_DutyType         Char(2)     default 'R',
                            
                            D4_In               INTEGER     default 8000,
                            D4_HrsLength        INTEGER     default 8,
                            D4_DutyType         Char(2)     default 'R',
                            
                            D5_In               INTEGER     default 8000,
                            D5_HrsLength        INTEGER     default 8,
                            D5_DutyType         Char(2)     default 'R',
                            
                            D6_In               INTEGER     default 0,
                            D6_HrsLength        INTEGER     default 0,
                            D6_DutyType         Char(2)     default 'RD',
                            
                            D7_In               INTEGER     default 0,
                            D7_HrsLength        INTEGER     default 0,
                            D7_DutyType         Char(2)     default 'RD');";
        await _sql.ExecuteCmd(sql, new { });

        sql = @$"select * from {schema}.AttTemplate limit 1 ";
        var res = await _sql.FetchData<AtttemplateModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.AttTemplate (EmpmasId) values  (0) ";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

    }

    private async Task _01AttendanceType(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.AttType (
                            Id             INTEGER  UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                            Name           Varchar(45)     Default '',
                            Remarks        Varchar(150)     Default '');";
        await _sql.ExecuteCmd(sql, new { });

        sql = @$"select * from {schema}.AttType limit 1 ";
        var res = await _sql.FetchData<RCivStatModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.AttType
                            (Name,                  Remarks) values 
                            ('Fixed Attendance',    'Employees have specific start and end times each day'), 
                            ('Flexible Attendance', 'Employees do not have specific start and end times but required to complete the length of duty each day.'), 
                            ('Exempt Attendance',   'Employees are not required to have a punch in/out.')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

    }
    private async Task _01AttendanceDutyType(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.AttDutyType (
                            Id             INTEGER  UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                            Code           Char(2)          Default 'R',
                            Name           Varchar(45)      Default '');";
        await _sql.ExecuteCmd(sql, new { });

        sql = @$"select * from {schema}.AttDutyType limit 1 ";
        var res = await _sql.FetchData<RCivStatModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.AttDutyType
                            (Code,  Name) values 
                            ('R',  'Regulary Day'), 
                            ('RD', 'Restday')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }

    }


    private async Task _01AttenanceDaily(string schema, string connName)
    {
        var sql = @$"CREATE TABLE if not exists {schema}.AttDaily (
                            EmpmasId        INTEGER     UNSIGNED,
                            EmpNumber       Char(10),
                            PunchDate       DATE,
                            DayNo           INTEGER     UNSIGNED, 
                            TimeIn          DATETIME,
                            TimeInT         INTEGER     UNSIGNED,
                            TimeOut         DATETIME,
                            TimeOutT        INTEGER     UNSIGNED,
                            DutyTypeId      INTEGER     UNSIGNED,
                            InById          INTEGER     UNSIGNED,
                            OutById         INTEGER     UNSIGNED,
                        PRIMARY KEY (EmpmasId, PunchDate)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { });

        sql = @$"CREATE TABLE if not exists {schema}.AttDaily_Archived (
                            EmpmasId        INTEGER     UNSIGNED,
                            EmpNumber       Char(10),
                            PunchDate       DATE,
                            DayNo           INTEGER     UNSIGNED, 
                            TimeIn          DATETIME,
                            TimeInT         INTEGER     UNSIGNED,
                            TimeOut         DATETIME,
                            TimeOutT        INTEGER     UNSIGNED,
                            DutyTypeId      INTEGER     UNSIGNED,
                            InById          INTEGER     UNSIGNED,
                            OutById         INTEGER     UNSIGNED,
                        PRIMARY KEY (EmpmasId, PunchDate)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { });

        sql = @$"CREATE TABLE if not exists {schema}.AttPunches (
                            EmpmasId        INTEGER     UNSIGNED,
                            DayNo           Int, 
                            PunchDate       DATETIME,
                            Action          Char(5),
                            PunchT          Int, 
                            DutyTypeId      INTEGER     UNSIGNED,
                            TimeZoneId      INTEGER     UNSIGNED,
                            IpAddress       Char(20), 
                            MacAddress      Char(20),
                            UserId          BigInt, 
                        PRIMARY KEY (EmpmasId, PunchDate)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { });

    }

    private async Task _01AttenanceMonthly(string schema, string connName)
    {

        var flds = string.Empty;
        for (var i = 1; i < 32; i++)
        {
            flds += $@" d{i.ToString().Trim()}DayNo         INTEGER     UNSIGNED,
                        d{i.ToString().Trim()}TimeIn        DATETIME,
                        d{i.ToString().Trim()}TimeInT       INTEGER     UNSIGNED,
                        d{i.ToString().Trim()}TimeOut       DATETIME,
                        d{i.ToString().Trim()}TimeOutT      INTEGER     UNSIGNED,
                        d{i.ToString().Trim()}DayWkNo       INTEGER     UNSIGNED,
                        d{i.ToString().Trim()}DutyTypeId    INTEGER     UNSIGNED,";
        }
        string sql = @$"CREATE TABLE if not exists {schema}.AttMonthly (
                            EmpmasId        INTEGER     UNSIGNED,
                            Year            SMALLINT    UNSIGNED,
                            Month           SMALLINT    UNSIGNED,
                            {flds}
                        PRIMARY KEY (EmpmasId, Month, year)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { });
    }

    private async Task _01EmpMovement(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpMovement (
                            Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            EmpmasId        Int, 
                            Date            Date, 
                            RefNo           Char(10),                            
                            Mode            Char(10),                            
                            Dtls            Char(60),                            
                            UserId          Integer, 
                            Created         DateTime,
                        PRIMARY KEY (Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { });
    }


    private async Task _01Mail(string schema, string connName)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string sql = @$"CREATE TABLE if not exists {schema}.`Mail` (
                          Id                INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          UserCompanyId     INTEGER         UNSIGNED,
                          SenderId          INTEGER         UNSIGNED,
                          module            CHAR(10)                    Default '',
                          msg               VARCHAR(100)                Default ' ',
                          link              VARCHAR(80)                 Default ' ',
                          IsRead            SMALLINT                    DEFAULT 0,
                          DateSent          DATETIME,
                          DateRead          DATETIME,
                          PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01Engagement(string schema, string connName)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string sql = @$"CREATE TABLE if not exists {schema}.Engagement (
                          Id                INTEGER     UNSIGNED    NOT NULL AUTO_INCREMENT,
                          OwnerId           INTEGER     UNSIGNED        Default 0 ,
                          CompanyId         INTEGER     UNSIGNED        Default 0 ,
                          Module            Varchar(10)                 Default ' ',
                          RoleId            INTEGER     UNSIGNED        Default 0,
                          DateInvited       DATETIME,
                          DateApproved      DATETIME,
                          Status            Char(2)                     Default 'FA',
                          PRIMARY KEY (`Id`))   ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _01MyMovement(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.MyMovement (
                            Id              INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,                                            
                            Date            Date, 
                            CompanyId       Int,
                            RefNo           Char(10),                            
                            Mode            Char(10),                            
                            Dtls            Char(60),                            
                            Created         DateTime,
                        PRIMARY KEY (Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01RDep(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.RDep (
                            Id              BigInt UNSIGNED NOT NULL AUTO_INCREMENT,                                            
                            Trndate         Date, 
                            MovStart        Date, 
                            MovEnd          Date, 
                            DepmodeId       Int,
                            EmployTypeId    Int,
                            DivId           Int,
                            DepId           Int,
                            SecId           Int,
                            PosId           Int,
                            PayrollgrpId    Int,
                            ApprSystemId    Int,
                            UserId          Int,
                            Rstat           Char(2),
                            Datecreated     DateTime,
                            Dateapproved    DateTime,
                        PRIMARY KEY (Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        //--- RDep Approver ---------------------------------------
        sql = @$"CREATE TABLE if not exists {schema}.RDepApprover (
                            SystemId        BigInt UNSIGNED NOT NULL,  
                            Module          char(5),
                        PRIMARY KEY (SystemId, Module)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

    }

    private async Task _01DeployMode(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.DeployMode (
                            Id              BigInt UNSIGNED NOT NULL AUTO_INCREMENT,                                            
                            Name            Char(40),
                        PRIMARY KEY (Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"select * from {schema}.DeployMode  limit 1 ";
        var res = await _sql.FetchData<PositionModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.DeployMode 
                            (Name) values 
                            ('Deployment'), 
                            ('Promotion'),
                            ('Transfer'), 
                            ('Reinstatement'),
                            ('Exominaration and Reinstatement')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }

    private async Task _01EmployType(string schema, string connName)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmployType (
                            Id              BigInt UNSIGNED NOT NULL AUTO_INCREMENT,                                            
                            Name            Char(40),
                        PRIMARY KEY (Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, connName);

        sql = @$"select * from {schema}.EmployType  limit 1 ";
        var res = await _sql.FetchData<PositionModel, dynamic>(sql, new { }, connName);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.EmployType 
                            (Name) values 
                            ('Trainee'), 
                            ('Contractual'),
                            ('Probationary'), 
                            ('Regular'),
                            ('Permanent')";
            await _sql.ExecuteCmd(sql, new { }, connName);
        }
    }




    //***************************************************************************
    // --- Schema Main Pis ------------------------------------------------------
    private async Task _01MainPisSchema(string schema, string connName)
    {
        string sql = $"CREATE DATABASE IF NOT EXISTS {schema}";
        await _sql.ExecuteCmd(sql, new { }, connName);
    }

    // --- Tables ---------------------------------------------------------------
    private async Task _01Users(string schema, string connName)
    {
        // UserType => 0 = Ordinary Users, 1 = System Users 
        string sql = @$"CREATE TABLE if not exists {schema}.Users (
                          Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                          LoginName     VARCHAR(45),
                          Password      VARCHAR(150),
                          Email         VARCHAR(60),
                          Domain        VARCHAR(25),
                          UserType      INTEGER UNSIGNED NOT NULL DEFAULT 0,
                          Status        Char(1)         DEFAULT 'A',
                          DefaultCoId   INTEGER UNSIGNED NOT NULL DEFAULT 1,
                          PRIMARY KEY(`Id`))ENGINE = InnoDB;";
        _sql.ExecuteCmd(sql, new { }, connName);
    }

    private async Task _201_11_Empmas(string schema)
    {
        string sql = @$"CREATE TABLE if not exists  secpis.empmas (
                        
                        CLIENT           varchar(5)                         DEFAULT NULL,
                        CLIENT_          varchar(5)                         DEFAULT NULL,
                        BASICRATE        double(10,4)           NOT NULL    DEFAULT '0.0000',
                        PAYTYPE          int(10)    unsigned    NOT NULL    DEFAULT '0',
                        ADMIN            varchar(1)                         DEFAULT NULL,
                        CASHBOND         double(7,2)            NOT NULL    DEFAULT '50.00',
                        WORKDAYS         double(3,0)                        DEFAULT NULL,
                        ALLOWRATE        double(9,2)            NOT NULL    DEFAULT '0.00',
                        ALLOWTYPE        varchar(1)                         DEFAULT NULL,
                        ALLOWFIX         varchar(1)                         DEFAULT NULL,
                        ALLOW2RATE       double(9,2)                        DEFAULT NULL,
                        ALLOW2TYPE       varchar(1)                         DEFAULT NULL,
                        ALLOW2FIX        varchar(1)                         DEFAULT NULL,
                        ALLOW3RATE       double(9,2)                        DEFAULT NULL,
                        ALLOW3TYPE       varchar(1)                         DEFAULT NULL,
                        ALLOW3FIX        varchar(1)                         DEFAULT NULL,
                        ALLOW4RATE       double(9,2)                        DEFAULT NULL,
                        ALLOW4TYPE       varchar(1)                         DEFAULT NULL,
                        ALLOW4FIX        varchar(1)                         DEFAULT NULL,
                        MOVNUMBER        varchar(10)                        DEFAULT NULL,
                        MOVMODE          varchar(1)                         DEFAULT NULL,
                        MOVDATE          date                               DEFAULT NULL,
                        MOVEND           date NOT NULL                      DEFAULT '0000-00-00',
                        DUTYDATE         date                               DEFAULT NULL,
                        TEL2             varchar(15)                        DEFAULT NULL,
                        
                        SPOUSE           varchar(25)                        DEFAULT NULL,
                        OCCUPATION       varchar(75)                        DEFAULT NULL,
                        NOCHILDREN       double(2,0)                        DEFAULT NULL,
                        DATEHIRED        date                               DEFAULT NULL,
                        SEPARATE         date                               DEFAULT NULL,
                        Position_        varchar(5)                         DEFAULT NULL,

                        EMPSTAT_         varchar(1)                         DEFAULT NULL,
                        STATUSDATE       date                               DEFAULT NULL,
                        TRAINAT          varchar(30)                        DEFAULT NULL,
                        DATETRAIN        date                               DEFAULT NULL,
                        EXMILITARY       varchar(1)                         DEFAULT NULL,
                        CSP              varchar(1)                         DEFAULT NULL,
                        CPP              varchar(1)                         DEFAULT NULL,
                        ROTC             varchar(1)                         DEFAULT NULL,
                        ELLEVEL          varchar(8)                         DEFAULT NULL,
                        HSLEVEL          varchar(8)                         DEFAULT NULL,
                        COLLEGE_         varchar(8)                         DEFAULT NULL,
                        COURSE           varchar(25)                        DEFAULT NULL,
                        VOLEVEL          varchar(8)                         DEFAULT NULL,
                        VOCOURSE         varchar(25)                        DEFAULT NULL,
                        LANGUAGE         varchar(15)                        DEFAULT NULL,
                        SKILL1           varchar(2)                         DEFAULT NULL,
                        SKILL2           varchar(2)                         DEFAULT NULL,
                        SKILL3           varchar(2)                         DEFAULT NULL,
                        SKILL4           varchar(2)                         DEFAULT NULL,
                        ACCTCODE         varchar(21)                        DEFAULT ' ',
                        AWOL             varchar(6)                         DEFAULT NULL,
                        DISMISS          varchar(6)                         DEFAULT NULL,
                        ASTART           date                               DEFAULT NULL,
                        AEND             date                               DEFAULT NULL,
                        ADAYS            double(2,0)                        DEFAULT NULL,
                        DSTART           date                               DEFAULT NULL,
                        DEND             date                               DEFAULT NULL,
                        DDAYS            double(2,0)                        DEFAULT NULL,
                        EMRNAME          varchar(25)                        DEFAULT NULL,
                        EMRTEL           varchar(15)                        DEFAULT NULL,
                        EMRADDR          varchar(60)                        DEFAULT NULL,
                        GUARDEXP         double(5,2)                        DEFAULT NULL,
                        COMTAXNO         varchar(10)                        DEFAULT NULL,
                        COMTAXDATE       date                               DEFAULT NULL,
                        COMTAX_AT        varchar(30)                        DEFAULT NULL,
                        
                        W_BIRTHC         varchar(1)                         DEFAULT NULL,
                        W_CLOSINGR       varchar(1)                         DEFAULT NULL,
                        W_TRNCERT        varchar(1)                         DEFAULT NULL,
                        W_PRELIC         varchar(1)                         DEFAULT NULL,
                        W_CERTEMP        varchar(1)                         DEFAULT NULL,
                        W_MEDEXAM        varchar(1)                         DEFAULT NULL,
                        GKERATE          double(6,2)                        DEFAULT NULL,
                        CLNAME           varchar(254)                       DEFAULT NULL,
                        MLANAME          varchar(20)                        DEFAULT NULL,
                        AGE              double(2,0)                        DEFAULT NULL,
                        MBRANCH          varchar(20)                        DEFAULT NULL,
                        MYEAR            varchar(20)                        DEFAULT NULL,
                        MNATURE          varchar(20)                        DEFAULT NULL,
                        REMARKS          varchar(150)                       DEFAULT NULL,
                        GUARDNOYRS       varchar(5)                         DEFAULT NULL,
                        MILITARYNOYR     varchar(5)                         DEFAULT NULL,
                        EXPMED           date                               DEFAULT NULL,
                        regref           date                   NOT NULL    DEFAULT '0000-00-00' COMMENT 'regularship reference',
                        empBasicRate     double(10,4)           NOT NULL    DEFAULT '0.0000',
                        rateID           int(10)    unsigned    NOT NULL    DEFAULT '2',
                        empEcola         double(10,4)           NOT NULL    DEFAULT '0.0000',
                        xmark            int(10)    unsigned    NOT NULL    DEFAULT '0',
                        suretybondquota  double(10,2)           NOT NULL    DEFAULT '0.00',
                        isTaxable        int(10) unsigned       NOT NULL    DEFAULT '0',
                        isconfi          int(10) unsigned       NOT NULL    DEFAULT '0',
                        iswithSSS        int(10) unsigned                   DEFAULT '1',
                        iswithGSIS       int(10) unsigned       NOT NULL    DEFAULT '0',
                        iswithPHIC       int(10) unsigned                   DEFAULT '1',
                        iswithPagibig    int(10) unsigned                   DEFAULT '1',
                        ismaxsss         int(10) unsigned                   DEFAULT '0',
                        passwd           varchar(60)            NOT NULL,
                        sgcode           char(10)               NOT NULL    DEFAULT '',
                        
                        dpclient         char(5)                            DEFAULT '',
                        PRIMARY KEY (EMPNUMBER)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='InnoDB free: 9216 kB; InnoDB free: 13312 kB';";

        await _sql.ExecuteCmd(sql, new { });
    }

    private async Task _01Empmas(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.Empmas (
                        Id              INTEGER UNSIGNED    NOT NULL,
                        SystemId        int                 DEFAULT 0,
                        EmpLastNm       varchar(25)         DEFAULT NULL,
                        EmpFirstNm      varchar(25)         DEFAULT NULL,
                        EmpMidNm        varchar(15)         DEFAULT NULL,
                        Suffix          char(3)             DEFAULT '',
                        EmpAlias        varchar(15)         DEFAULT NULL,
                        PRIMARY KEY(Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }
    private async Task _01EmpmasPI(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.empmasPI (
                            Id              INTEGER UNSIGNED            NOT NULL,
                            EmpBirth         date                       DEFAULT NULL,
                            BirthPlace       Varchar(75)                DEFAULT NULL,
                            Sex_             Varchar(1)                 DEFAULT NULL,
                            CivStat_         Varchar(1)                 DEFAULT NULL,
                            Citizen          Varchar(15)                DEFAULT NULL,
                            Religion         Varchar(35)                DEFAULT NULL,
                            Height           Int                        DEFAULT NULL,
                            HeightInch       Int                        DEFAULT NULL,
                            Weight           double(6,2)                DEFAULT NULL,
                            Hair             Varchar(15)                DEFAULT NULL,
                            Eyes             Varchar(15)                DEFAULT NULL,
                            Complexion       Varchar(10)                DEFAULT NULL,
                            Marks            Varchar(60)                DEFAULT NULL,
                            BloodType        Varchar(10)                DEFAULT NULL,
                            Spouse           Varchar(25)                DEFAULT NULL,
                            Occupation       Varchar(75)                DEFAULT NULL,
                            NoChildren       int                        DEFAULT NULL,
                            PRIMARY KEY     (Id)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    private async Task _01EmpmasAddress(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasAddress (
                        Id                  INTEGER UNSIGNED                  NOT NULL,
                        PresAddStreet       Varchar(45)                       DEFAULT NULL,
                        PresAddVillage      Varchar(45)                       DEFAULT NULL,
                        PresAddBrgy         Varchar(45)                       DEFAULT NULL,
                        PresAddCityId       INT                               DEFAULT NULL,
                        PresAddCity         Varchar(45)                       DEFAULT NULL,
                        PresAddProvId       INT                               DEFAULT NULL,
                        PresAddProv         Varchar(45)                       DEFAULT NULL,
                        PresAddStateId      INT                               DEFAULT NULL,
                        PresAddState        Varchar(45)                       DEFAULT NULL,
                        PresAddCountryId    INT                               DEFAULT NULL,
                        PresAddCountry      Varchar(45)                       DEFAULT NULL,
                        PresAddZipCode      Varchar(10)                       DEFAULT NULL,
                        PresAdd             Varchar(200)                      DEFAULT NULL,
                        PresAddTelNo        Varchar(45)                       DEFAULT NULL,
                        
                        ProvAddStreet       Varchar(45)                       DEFAULT NULL,
                        ProvAddVillage      Varchar(45)                       DEFAULT NULL,
                        ProvAddBrgy         Varchar(45)                       DEFAULT NULL,
                        ProvAddCityId       INT                               DEFAULT NULL,
                        ProvAddCity         Varchar(45)                       DEFAULT NULL,
                        ProvAddProvId       INT                               DEFAULT NULL,
                        ProvAddProv         Varchar(45)                       DEFAULT NULL,
                        ProvAddStateId      Varchar(10)                       DEFAULT NULL,
                        ProvAddState        Varchar(45)                       DEFAULT NULL,
                        ProvAddCountryId    INT                               DEFAULT NULL,
                        ProvAddCountry      Varchar(45)                       DEFAULT NULL,
                        ProvAddZipCode      Varchar(10)                       DEFAULT NULL,
                        ProvAdd             Varchar(200)                      DEFAULT NULL,
                        ProvAddTelNo        Varchar(45)                       DEFAULT NULL,

                        Countrycode         char(3)                           DEFAULT NULL,
                        
                        EmailAdd            Varchar(45)                       DEFAULT NULL,
                        EmailAdd1           Varchar(45)                       DEFAULT NULL,
                        CellNo              Varchar(45)                       DEFAULT NULL,
                        CellNo1             Varchar(45)                       DEFAULT NULL,

                        PRIMARY KEY (Id)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }




    // Multi-Entry --------------------------------------------------------------------
    private async Task _01EmpmasFamilyRef(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.EmpmasFamilyRef (
                      Code              CHAR(2)         NOT NULL,
                      Name              VARCHAR(80)                 Default NULL,
                      PRIMARY KEY(Code)
                    )ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);


        sql = @$"select * from {schema}.EmpmasFamilyRef limit 1 ";
        var res = await _sql.FetchData<EmpmasFamilyRefModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.EmpmasFamilyRef
                            (Code,  Name) values 
                            ('F',   'Father'), 
                            ('M',   'Mother'),
                            ('SF',  'Step-Father'), 
                            ('SM',  'Step-Mother'),
                            ('SP',  'Spouse'), 
                            ('LP',  'Live-in-Partner'),
                            ('S',   'Son'), 
                            ('D',   'Daughter'), 
                            ('AS',  'Adopted Son'), 
                            ('AD',  'Adopted Daughter') 
                            ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

    }
    private async Task _01EmpmasFamily(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.empmasFamily (
                            Id          INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId    INTEGER UNSIGNED                DEFAULT NULL,
                            Name                Varchar(80)             DEFAULT NULL,
                            Birth               Date                    DEFAULT NULL,
                            Sex                 Char(1)                 DEFAULT NULL,
                            RelationCode        Char(2)                 DEFAULT NULL, 
                            primary key(Id) 
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";
        await _sql.ExecuteCmd(sql, new { });

    }


    private async Task _01EmpmasRelativesRef(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasRelativesRef (
                            Code            Char(5)        NOT NULL, 
                            Name            Varchar(80)                DEFAULT NULL,
                            PRIMARY KEY (Code)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";
        await _sql.ExecuteCmd(sql, new { }, conn);


        sql = @$"select * from {schema}.EmpmasRelativesRef limit 1";
        var res = await _sql.FetchData<EmpmasRelativesRefModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.EmpmasRelativesRef 
                            (Code,  Name) values 
                            ('Bro',  'Brother'), 
                            ('Sis',  'Sister'),
                            ('GF',   'Grand Father'), 
                            ('GM',   'Grand Mother'),
                            ('GS',   'Grandson'), 
                            ('GD',   'Granddaughter'), 
                            ('HBro', 'Half Brother'), 
                            ('HSis', 'Half Sister'),  
                            ('U',    'Uncle'),  
                            ('A',    'Aunt'), 
                            ('C',    'Cousin'),
                            ('Nep',  'Nephew'),
                            ('Nie',  'Niece'),
                            ('FIL',  'Father-in-Law'),
                            ('MIL',  'Mother-in-Law'),
                            ('SIL',  'Son-in-Law'),
                            ('DIL',  'Cousin-in-Law'),
                            ('BroL',  'Brother-in-Law'),
                            ('SisL',  'Sister-in-Law')
                            ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }


    }


    private async Task _01EmpmasRelatives(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasRelatives (
                            Id          INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId    INTEGER UNSIGNED                DEFAULT NULL,
                            Name                Varchar(80)             DEFAULT NULL,
                            Birth               Date                    DEFAULT NULL,
                            Sex                 Char(1)                 DEFAULT NULL,
                            RelativesRefCode    Varchar(5)              DEFAULT NULL, 
                            primary key(Id) 
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01EmpmasEmergencyContact(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.EmpmasEmergencyContact (
                        Id              INTEGER         unsigned            NOT NULL AUTO_INCREMENT,
                        EmpmasId        INTEGER UNSIGNED            DEFAULT NULL,
                        Name            varchar(25)                 DEFAULT NULL,
                        Addr            varchar(60)                 DEFAULT NULL,
                        Relationship    varchar(20)                 DEFAULT NULL,
                        TelNo           varchar(15)                 DEFAULT NULL,
                        PRIMARY KEY     (Id))  ENGINE=InnoDB DEFAULT CHARSET=latin1;";


        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01EmpmasGovPh(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasGovPh (
                            Id              INTEGER UNSIGNED                    NOT NULL,
                            Sss              varchar(15)                        DEFAULT NULL,
                            Tin              varchar(15)                        DEFAULT NULL,
                            PagibigNo        varchar(15)                        DEFAULT NULL,
                            Phic             varchar(15)                        DEFAULT NULL,
                            Hdmf             varchar(15)                        DEFAULT NULL,
                            BankNo           varchar(15)                        DEFAULT NULL,
                            BankName         varchar(45)                        DEFAULT NULL,
                            Drv_License      varchar(25)                        DEFAULT NULL,
                            Drv_Exp          date                               DEFAULT NULL,
                            dpadate          date                               DEFAULT NULL,
                            TaxCode          Char(3)                            DEFAULT NULL,
                            PRIMARY KEY     (Id)) ENGINE=InnoDB DEFAULT CHARSET=latin1 ;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    private async Task _01EmpmasSecLic(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasSecLic (
                            Id              INTEGER UNSIGNED            NOT NULL,
                            SecLicense      varchar(15)                 DEFAULT NULL,
                            LicExpire       date                        DEFAULT NULL,
                            BadgeNo         varchar(15)                 DEFAULT NULL,
                            SbrNo           char(25)                    DEFAULT NULL,
                            OpNo            char(25)                    DEFAULT NULL,
                            Validated       date                        DEFAULT NULL,
                            VFee            double(18,2)                DEFAULT '0.00',
                            Revalidated     date                        DEFAULT NULL,
                            ValStatus       char(15)                    DEFAULT ' ',
                            PRIMARY KEY     (Id)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Education ----------------------------------------------------------

    private async Task _01EmpmasEducation(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasEducate (
                            Id          INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId    INTEGER UNSIGNED            DEFAULT NULL,
                            Code            varchar(1)                  DEFAULT NULL,
                            School          varchar(45)                 DEFAULT NULL,
                            FROM_           date                        DEFAULT NULL,
                            TO_             date                        DEFAULT NULL,
                            COURSE          varchar(75)                 DEFAULT NULL,
                            LEVEL           varchar(8)                  DEFAULT NULL, 
                            primary key(Id) 
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"CREATE TABLE if not exists {schema}.EmpmasEducateRef (
                    Code            varchar(3)      DEFAULT NULL,
                    Name            varchar(45)     DEFAULT NULL
               ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        // --- Insert Reference values ---------------------------------------
        sql = $@"select * from {schema}.EmpmasEducateRef limit 1";

        var result = await _sql.FetchData<EmpmasEducateRefModel, dynamic>(sql, new { }, conn);
        if (result.Count == 0)
        {
            sql = @$"insert into {schema}.EmpmasEducateRef 
                  (Code,    Name) values 
                  ('ELM', 'Elementary'), 
                  ('HS',  'High School'), 
                  ('JHS', 'Junior High'), 
                  ('SHS', 'Senior High'), 
                  ('VOC', 'Vocational'),
                  ('COL', 'College'), 
                  ('MSR', 'Master'), 
                  ('PHD', 'Doctorate')";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }
    // ************************************************************************


    // --- Insurance ----------------------------------------------------------
    private async Task _01EmpmasInsurance(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasInsurance (
                            Id              INTEGER UNSIGNED                    NOT NULL,
                            INSURANCE        varchar(60)                        DEFAULT NULL,
                            PolicyNo         varchar(15)                        DEFAULT NULL,
                            FaceValue        double(9,2)                        DEFAULT NULL,
                            Premium          double(6,2)                        DEFAULT NULL,
                            InsExpire        date                               DEFAULT NULL,
                        PRIMARY KEY     (Id)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    // ************************************************************************

    // --- Character References (Refer in SecPis) ----------------------------------------------------------
    private async Task  _01EmpmasCharRef(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasCharRef (
                            Id          INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId    INTEGER UNSIGNED            DEFAULT NULL,
                            Name        varchar(50)                 DEFAULT NULL,
                            Addr        varchar(60)                 DEFAULT NULL,
                            Tel         varchar(45)                 DEFAULT NULL,
                            Position    varchar(75)                 DEFAULT NULL, 
                            primary key(Id) 
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    // ************************************************************************

    // --- Character Employment (Employ in SecPis) ----------------------------------------------------------
    private async Task _01EmpmasEmployment(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasEmployment (
                            Id          INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId    INTEGER UNSIGNED            DEFAULT NULL,
                            CompName        varchar(60)                 DEFAULT NULL,
                            Address         varchar(120)                DEFAULT NULL,
                            Tel             varchar(25)                 DEFAULT NULL,
                            Pos             varchar(75)                 DEFAULT NULL,
                            From_           DATE                        DEFAULT NULL,
                            To_             DATE                        DEFAULT NULL,
                            Sal             double(10,2)                DEFAULT NULL,
                            Rem             varchar(120)                 DEFAULT NULL,
                            primary key(Id) 
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    // ************************************************************************

    private async Task _01EmpmasTraining(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasTraining (
                            Id              INTEGER unsigned NOT NULL AUTO_INCREMENT,
                            EmpmasId        INTEGER UNSIGNED            DEFAULT NULL,
                            ProgramName     varchar(60)                 DEFAULT NULL,
                            DateTaken       varchar(20)                 DEFAULT NULL,
                            DateStart       DATE                        DEFAULT NULL,
                            DateEnd         DATE                        DEFAULT NULL,
                            TotalHrs        INTEGER                     DEFAULT NULL,
                            TotalDays       varchar(20)                 DEFAULT NULL,
                            School          varchar(50)                 DEFAULT NULL,
                            Trainor         varchar(30)                 DEFAULT NULL,
                            PRIMARY KEY (Id)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    // --- Uploadables ----------------------------------------------------------

    private async Task _01EmpmasFileUploadCategory(string schema, string conn)
    {
        string sql = @$" CREATE TABLE if not exists {schema}.EmpmasFileUploadCategory(
                        Id      INTEGER         UNSIGNED    NOT NULL        AUTO_INCREMENT,
                        Name    Varchar(60)                 DEFAULT NULL,
                        PRIMARY KEY (Id)
                      ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01EmpmasFileUpload(string schema, string conn)
    {
        string sql = @$" CREATE TABLE if not exists {schema}.EmpmasFileUpload(
                        Id                      INTEGER         UNSIGNED    NOT NULL        AUTO_INCREMENT,
                        EmpmasId                INTEGER         UNSIGNED    DEFAULT 0,
                        FileUploadCategotyId    INTEGER         UNSIGNED    DEFAULT 0,
                        ValidityStart           DATE,
                        Expiration              DATE,
                        ImageLink               VARCHAR(100)                DEFAULT NULL,
                        Remarks                 VARCHAR(150)                DEFAULT NULL,
                        PRIMARY KEY (Id)
                      ) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // --- Insurance ----------------------------------------------------------
    private async Task _01EmpmasClearancePh(string schema, string conn)
    {
        string sql = @$"CREATE TABLE if not exists {schema}.EmpmasClearancePh (
                            Id              INTEGER UNSIGNED            NOT NULL,
                            Nbi_Taken       date                        DEFAULT NULL,
                            Nbi_Exp         date                        DEFAULT NULL,
                            Nbi_Remarks     Varchar(60)                 DEFAULT NULL,
                            Nbi_Link        Varchar(60)                 DEFAULT NULL,

                            Police_Taken    date                        DEFAULT NULL,
                            Police_Exp      date                        DEFAULT NULL,
                            Police_Remarks  Varchar(60)                 DEFAULT NULL,
                            Police_Link     Varchar(60)                 DEFAULT NULL,
                            
                            Pnp_Taken       date                        DEFAULT NULL,
                            Pnp_Exp         date                        DEFAULT NULL,
                            Pnp_Remarks     Varchar(60)                 DEFAULT NULL,
                            Pnp_Link        Varchar(60)                 DEFAULT NULL,

                            Brgy_Taken       date                       DEFAULT NULL,
                            Brgy_Exp         date                       DEFAULT NULL,
                            Brgy_Remarks     Varchar(60)                DEFAULT NULL,
                            Brgy_Link        Varchar(60)                DEFAULT NULL,
                            
                            Court_Taken       date                      DEFAULT NULL,
                            Court_Exp         date                      DEFAULT NULL,
                            Court_Remarks     Varchar(60)               DEFAULT NULL,
                            Court_Link        Varchar(60)               DEFAULT NULL,

                            Neuro_Taken       date                      DEFAULT NULL,
                            Neuro_Exp         date                      DEFAULT NULL,
                            Neuro_Remarks     Varchar(60)               DEFAULT NULL,
                            Neuro_Link        Varchar(60)               DEFAULT NULL,
                            
                            Drug_Taken       date                       DEFAULT NULL,
                            Drug_Exp         date                       DEFAULT NULL,
                            Drug_Remarks     Varchar(60)                DEFAULT NULL,
                            Drug_Link        Varchar(60)                DEFAULT NULL,
                            
                        PRIMARY KEY     (Id)) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    // ************************************************************************


    // ******************************************************************************************//
    // --- Master Tables ---------------------------------------------------------------------------
    // ******************************************************************************************//
    private async Task _01RCivStat(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.RCivStat (
                          Id    Int             unsigned NOT NULL AUTO_INCREMENT,
                          Code  varchar(1)  DEFAULT NULL,
                          Name  varchar(15) DEFAULT NULL,
                          PRIMARY KEY (Id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {schema}.RCivStat limit 1 ";
        var res = await _sql.FetchData<RCivStatModel, dynamic>(sql, new { }, conn);

        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.RCivStat
                            (Code, Name) values 
                            ('S', 'Single'), 
                            ('M', 'Married'), 
                            ('D', 'Divorced'), 
                            ('W', 'Widowed'), 
                            ('T', 'Separated'), 
                            ('P', 'Single Parent'); ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }
    private async Task _01RCoInfoPH(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS   {schema}.RCoInfo (
                            Id          Int unsigned NOT NULL AUTO_INCREMENT,
                            Add1        varchar(250)    Default Null,
                            Add2        varchar(250)    Default Null,
                            TelNo       varchar(100)    Default Null,
                            RegNo       varchar(60)    Default Null,
                            RegPeriod   Int             Default Null,
                            CoLogo      varchar(60)     Default Null,
                            AcctNo      varchar(45)     Default Null DEFAULT ' ',
                            SssNo       varchar(25)     Default Null,
                            PhicNo      varchar(25)     Default Null,
                            TinNo       varchar(25)     Default Null,
                            PagibigNo   varchar(25)     Default Null,
                            SssMemType  Int unsigned    DEFAULT '0' COMMENT '1-private, 2 2-local, 3-gcc, 4-nga',
                            SssDocNo   char(6) DEFAULT NULL,
                            SssBrCode  Char(3) DEFAULT NULL,
                            PRIMARY KEY (Id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    private async Task _01RCollege(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS   {schema}.RCollege (
                            Id Int  unsigned NOT NULL AUTO_INCREMENT,
                            Code    char(10)        DEFAULT NULL, 
                            Name    varchar(25)     DEFAULT NULL, 
                            PRIMARY KEY (`id`))     ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    private async Task _01RDeviation(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS   {schema}.RDeviation (
                            Id Int  unsigned NOT NULL AUTO_INCREMENT,
                            Dev_No      Char(10)    DEFAULT NULL,
                            Dev_Name    Char(40)    DEFAULT NULL,
                            Dev_Type    Char(1)     DEFAULT NULL,
                            Dev_Level   Char(1)     DEFAULT NULL,
                            Dev_Parent  Char(10)    DEFAULT NULL,
                            PRIMARY KEY (Id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }
    private async Task _01REmpStat(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.REmpStat (
                        Id           INT UNSIGNED NOT NULL AUTO_INCREMENT,
                        Code         CHAR(1) NOT NULL,
                        Name         VARCHAR(30) DEFAULT NULL,
                        IsResigned   INT DEFAULT 0,
                        IsOnLeaved    INT DEFAULT 0,
                        IsFloating   INT DEFAULT 0,
                        IsSuspended  INT DEFAULT 0,
                        PRIMARY KEY (Id)
                    ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);




        sql = $"select * from {schema}.REmpStat limit 1";
        var res = await _sql.FetchData<RempstatModel, dynamic>(sql, new { }, conn);

        if (res.Count == 0)
        {
            sql = @$"INSERT INTO {schema}.REmpStat   
                (CODE, NAME, IsResigned, IsOnLeaved, IsFloating, IsSuspended) VALUES 
                ('A', 'Active', 0, 0, 0, 0),
                ('W', 'AWOL', 0, 0, 0, 0),
                ('J', 'Back out', 1, 0, 0, 0),
                ('2', 'Backout', 0, 0, 0, 0),
                ('C', 'Clearancing', 0, 0, 0, 0),
                ('K', 'Continuous Service', 0, 0, 0, 0),
                ('D', 'Deceased', 1, 0, 0, 0),
                ('O', 'Dormant', 0, 0, 0, 0),
                ('E', 'Emergency Leave', 0, 1, 0, 0),
                ('G', 'Escort', 0, 0, 0, 0),
                ('B', 'Floating', 0, 0, 1, 0),
                ('0', 'For Deployment', 0, 0, 0, 0),
                ('X', 'Labor', 1, 0, 0, 0),
                ('M', 'Maternity Leave', 0, 1, 0, 0),
                ('_', 'Newly Hired', 0, 0, 0, 0),
                ('N', 'On process', 0, 0, 0, 0),
                ('R', 'Resigned', 1, 0, 0, 0),
                ('1', 'Resigned - Pending Clearance', 1, 0, 0, 0),
                ('U', 'Resigned - Under Clearance', 1, 0, 0, 0),
                ('Z', 'Retired', 1, 0, 0, 1),
                (' ', 'RTU', 0, 0, 0, 0),
                ('L', 'Sick Leave', 0, 1, 0, 0),
                ('S', 'Staff', 0, 0, 0, 0),
                ('H', 'Suspended', 1, 0, 0, 1),
                ('Y', 'Suspended & Relieved', 1, 0, 0, 1),
                ('T', 'Terminated', 1, 0, 0, 0),
                ('I', 'Training', 0, 0, 0, 0),
                ('F', 'Transferred', 0, 0, 0, 0),
                ('V', 'Vacation Leave', 0, 1, 0, 0);";

            await _sql.ExecuteCmd(sql, new { }, conn);
        }


    }

    private async Task _01Rempstat_fordeviation(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.rempstat_fordeviation(
                         `RempstatId` int(10) unsigned NOT NULL,
                          PRIMARY KEY (`RempstatId`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Rempstat_fordisciplinary(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.rempstat_fordisciplinary (
                         `RempstatId` int(10) unsigned NOT NULL,
                          PRIMARY KEY (`RempstatId`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Rempstat_forexonerate(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.rempstat_forexonerate (
                         `RempstatId` int(10) unsigned NOT NULL,
                          PRIMARY KEY (`RempstatId`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01Rempstat_forreinstatement(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.rempstat_forreinstatement (
                         `RempstatId` int(10) unsigned NOT NULL,
                          PRIMARY KEY (`RempstatId`) USING BTREE
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01RempstatForDeployment(string schema, string conn)
    {

        string sql = $@" CREATE TABLE IF NOT EXISTS {schema}.rempstat_fordeployment (
                          `RempstatId` int(10) unsigned NOT NULL,
                          PRIMARY KEY (`REmpstatId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01RLanguageSpoken(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.RLanguageSpoken (
                          Id Int unsigned NOT NULL AUTO_INCREMENT,
                          Name    varchar(45) Default NULL, 
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01RDivision(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.RDivision (
                          Id Int unsigned NOT NULL AUTO_INCREMENT,
                          SName         Char(10)      Default NULL, 
                          Name          varchar(65)   Default NULL, 
                          SupervisorId  Int           Default 0, 
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        //--- Division ----------------------------------------------------------------
        sql = $"select * from {schema}.RDivision limit 1";
        var res = await _sql.FetchData<RdivisionModel, dynamic>(sql, new { }, conn);
        if (res.Count == 0)
        {
            sql = @$"insert into {schema}.RDivision 
                  (SName,   Name) values ('HO',    'Head Office')";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }


    }

    private async Task _01RDepartment(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.RDepartment (
                          Id Int unsigned NOT NULL AUTO_INCREMENT,
                          SName         Char(10)      Default NULL, 
                          Name          varchar(65)   Default NULL, 
                          SupervisorId  Int           Default 0, 
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        //--- Department ----------------------------------------------------------------
        sql = $"select * from {schema}.RDepartment limit 1";
        var dep = await _sql.FetchData<RdepartmentModel, dynamic>(sql, new { }, conn);
        if (dep.Count == 0)
        {
            sql = @$"insert into {schema}.RDepartment 
                  (SName,       Name) values 
                  ('ACCT',      'Accounting Department'), 
                  ('OPS',       'Operation Department'), 
                  ('MKT',       'Marketing Department'), 
                  ('SAL',       'Sales Department'), 
                  ('HRD',       'Human Resource Department'), 
                  ('FIN',       'Finance Department'), 
                  ('ITD',       'Information Technology Department'), 
                  ('CSD',       'Customer Service Department') 
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }


    }

    private async Task _01RSection(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.RSection (
                          Id            Int unsigned NOT NULL AUTO_INCREMENT,
                          DepartmentId  Int           Default 0,
                          SName         Char(10)      Default NULL, 
                          Name          varchar(65)   Default NULL, 
                          PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
        //--- Section ----------------------------------------------------------------
        sql = $"select * from {schema}.RSection limit 1";
        var sec = await _sql.FetchData<RsectionModel, dynamic>(sql, new { }, conn);
        if (sec.Count == 0)
        {
            //--- Accounting (1) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (1,               'BK',      'Bookkeeping Section'), 
                  (1,               'AP',      'Accounts Payable Section'), 
                  (1,               'AR',      'Accounts Receivable Section'), 
                  (1,               'TC',      'Tax and Compliance Section'), 
                  (1,               'FR',      'Financial Reporting Section'), 
                  (1,               'CFM',      'Cash Flow Management Section'), 
                  (1,               'CTA',      'Capital Tracking and Allocation Section') 
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- Marketing (3) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (2,               'LSS',      'Leadership and Strategy Section'), 
                  (2,               'PMS',      'Production and Manufacturing Section'), 
                  (2,               'PIS',      'Process Improvement Section'), 
                  (2,               'CSOS',     'Customer Service Operations Section'), 
                  (2,               'PM',       'Project Management Section'), 
                  (2,               'IT',       'Information Technology and SystemsSection') 
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- Marketing (3) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (3,               'SMS',      'Social Media Section'), 
                  (3,               'CMS',      'Content Marketing Section'), 
                  (3,               'MRS',      'Market Research Section') 
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- Sales (4) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (4,               'ACS',      'Account Management Section'), 
                  (4,               'BDS',      'Business Development Section')
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- HR (5) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (5,               'RS',       'Recruitment Section'), 
                  (5,               'OS',      'Onboarding Section'), 
                  (5,               'TDS',      'Traininng and  Development Section')
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- Fiance (6) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (6,               'APS',      'Accounts Payable Section'), 
                  (6,               'ARS',      'Accounts Receivable Section'), 
                  (6,               'FRS',      'Financial Reporting Section')
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

            //--- IT (7) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (7,               'HDS',      'Help Desk Section'), 
                  (7,               'NAS',      'Network Administrator Section'), 
                  (7,               'SSS',      'Software Support Section'), 
                  (7,               'CTS',      'Computer Tech Section'), 
                  (7,               'SAS',      'System Administration Section')
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);


            //--- Customer Support (8) ----------------------------------------
            sql = @$"insert into {schema}.RSection 
                  (Departmentid,    SName,      Name) values 
                  (8,               'CSS',      'Customer Section Section'), 
                  (8,               'TSS',      'Tech Support Section'), 
                  (8,               'OPS',      'Order Processing Section')
                  ";
            await _sql.ExecuteCmd(sql, new { }, conn);

        }
    }

    private async Task _01RPosition(string schema, string conn)
    {
        string sql = $@"CREATE TABLE   if not exists {schema}.Position (
                            Id          INTEGER UNSIGNED    NOT NULL AUTO_INCREMENT,
                            CODE        varchar(10)             Default '',
                            NAME        varchar(60)             DEFAULT NULL,
                            ISGUARD     varchar(1)              DEFAULT '',
                            sort        varchar(2)         NOT NULL DEFAULT '99',
                            PRIMARY KEY (`Id`) ) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select Id from {schema}.Position limit 1 ";
        var res = await _sql.FetchData<PositionModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.Position
                            (Name,                                  Code) values 
                            ( 'Chief Executive Officer',            'CEO'),
                            ( 'Chief Operating Officer',            'COO'), 
                            ( 'Chief Financial Officer',            'CFO'), 
                            ( 'Chief Technology Officer',           'CTO'),
                            ( 'Chief Human Resources Officer',      'CHRO'),
                            ( 'Chief Information Officer',          'CIO'),
                            ( 'Vice President of Sales',            'VPSales'),
                            ( 'Vice President of Marketing',        'VPMktg'),
                            ( 'Vice President of Finance',          'VPFin'),
                            ( 'Vice President of Human Resources',  'VPHR'),
                            ( 'Vice President of Operations',       'VPOps'),
                            ( 'Director of Sales',                  'DirSales'),
                            ( 'Director of Marketing',              'DirMrktg'),
                            ( 'Director of Finance',                'DirFin'),
                            ( 'Director of Human Resources',        'DirHR'),
                            ( 'Director of Operations',             'DirOps'),
                            ( 'Manager of Sales',                   'MgrSales'),
                            ( 'Manager of Marketing',               'MgrMrktg'),
                            ( 'Manager of Finance',                 'MgrFin'),
                            ( 'Manager of Human Resources',         'MgrHR'),
                            ( 'Manager of Operations',              'MgrOps'), 
                            ( 'Chief Product Officer',              'CPO'),
                            ( 'Chief Risk Officer',                 'CRO'),
                            ( 'Chief Compliance Officer',           'CCO'),
                            ( 'Manager of Customer Service',        'MgrCS'),
                            ( 'Software Engineer',                  'SE'),
                            ( 'Senior Software Engineer',           'SSE'),
                            ( 'Data Scientist',                     'DS'),
                            ( 'Senior Data Scientist',              'SDS'),
                            ( 'Product Manager',                    'PM'),
                            ( 'Marketing Specialist',               'MS'),
                            ( 'Sales Representative',               'SR'),
                            ( 'Human Resources Specialist',         'HRS'),
                            ( 'Financial Analyst',                  'FA'),
                            ( 'Business Analyst',                   'BA'),
                            ( 'IT Support Specialist',              'ITSS'), 
                            ('Content Writer',                      'CW'),
                            ('Senior Content Writer',               'SCW'),
                            ('Project Manager',                     'PM'),
                            ('Senior Project Manager',              'SPM'),
                            ('Account Manager',                     'AM'),
                            ('Office Manager',                      'OM'),
                            ('Procurement Specialist',              'PS'),
                            ('Facilities Manager',                  'FM'),
                            ('Health and Safety Officer',           'HSO'),
                            ('Training and Development Manager',    'TDM'),
                            ('Benefits Coordinator',                'BC'),
                            ('Payroll Specialist',                  'PS'), 
                            ('Recruitment Specialist',              'RS'),
                            ('Talent Acquisition Manager',          'TAM'),
                            ('Security Manager',                    'SMgr'),
                            ('Security Guard',                      'SG'),
                            ('Lady Guard',                          'LG'),
                            ('IT Manager',                          'ITM'),
                            ('IT Officer',                          'ITO'),
                            ('Network Administrator',               'NA'),
                            ('Systems Administrator',               'SysAdmin'),
                            ('Database Administrator',              'DBA'),
                            ('Cloud Engineer',                      'CE'),
                            ('DevOps Engineer',                     'DevOps'),
                            ('Cybersecurity Analyst',               'CSA'),
                            ('Information Security Manager',        'ISM'),
                            ('Web Developer',                       'WD'),
                            ('Front-End Developer',                 'FED'); ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    private async Task _01RDesignation(string schema, string conn)
    {
        string sql = $@"CREATE TABLE   if not exists {schema}.Designation (
                            Id          INTEGER UNSIGNED    NOT NULL AUTO_INCREMENT,
                            CODE        varchar(10)             Default '',
                            NAME        varchar(60)             DEFAULT NULL,
                            sort        varchar(2)         NOT NULL DEFAULT '99',
                            PRIMARY KEY (`Id`) ) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select Id from {schema}.Designation limit 1 ";
        var res = await _sql.FetchData<DesignationModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.Designation
                            (Name,                                  Code) values 
                            ( 'Administrator',                      'ADMIN'),
                            ( 'Director',                           'DIR'),
                            ( 'Executive Officer',                  'EO'),
                            ( 'Officer',                            'OFFR'),
                            ( 'Officer In Charge',                  'OIC'),
                            ( 'Supervisor',                         'SUP'),
                            ( 'Manager',                            'MGR'),
                            ('Employee',                            'EMP') ; ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    private async Task _01RDeployment(string schema, string conn)
    {
        string sql = $@"CREATE TABLE   if not exists {schema}.rDeployment (
                            Id          INTEGER UNSIGNED    NOT NULL AUTO_INCREMENT,
                            Sname       varchar(10)             Default '',
                            Name        varchar(60)             DEFAULT NULL,
                            PRIMARY KEY (`Id`) ) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01RDevdata(string schema, string conn)
    {
        string sql = $@"CREATE TABLE   if not exists {schema}.`rdevdata` (
                      `DEV_NO` varchar(10) DEFAULT NULL,
                      `DEV_NAME` varchar(60) DEFAULT NULL,
                      `DEV_TYPE` varchar(1) DEFAULT NULL,
                      `DEV_LEVEL` varchar(1) DEFAULT NULL,
                      `DEV_PARENT` varchar(10) DEFAULT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select DEV_NO from {schema}.rdevdata limit 1 ";
        var res = await _sql.FetchData<RdevdataModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.rdevdata
                           (DEV_NO, DEV_NAME, DEV_TYPE, DEV_LEVEL, DEV_PARENT) VALUES
                                (1,   'AGAINST PERSONAL/COMPANY PROPERTY', 'G', 1, 0),
                                (2,   'AGAINTS COMPANY INTEREST (ACI)', 'G', 1, 0),
                                (3,   'AGAINST PUBLIC MORAL', 'G', 1, 0),
                                (12,  'AGAINST FIREARM LAW', 'G', 2, 1),
                                (201, 'ABANDONING POST', 'D', 2, 2),
                                (202, 'SLEEPING WHILE ON DUTY', 'D', 2, 2),
                                (203, 'NOT WEARING UNIFORM', 'D', 2, 2),
                                (301, 'REEKING W/ LIQUOR', 'D', 2, 3),
                                (302, 'POSITIVE- DRUG', 'D', 2, 3),
                                (303, 'BRINGING DRUGS TO POST', 'D', 2, 3),
                                (304, 'BRINGING ALCOHOLIC BEVERAGE', 'D', 2, 3),
                                (204, 'COMMITTING A CRIME', 'D', 2, 2),
                                (143, 'INDISCRIMINATE FIRING', 'D', 3, 12),
                                (144, 'CRRYING F/A OUTSIDE PRIMISES', 'D', 3, 12),
                                (151, 'LENDING OF FIRE ARMS', 'D', 3, 12),
                                (152, 'CARRYING DEADLY WEAPON PRIMISE', 'D', 3, 12),
                                (153, 'LOSING/DAMAGE F/A UNJUSTIFIABL', 'D', 3, 12),
                                (205, 'TARDINESS', 'D', 2, 2),
                                (206, 'AWOL', 'D', 1, 0),
                                (207, 'POOR SERVICE', 'D', 2, 2),
                                (208, 'ABSENTEEISM', 'D', 2, 2),
                                (209, 'INSUBORDINATION', 'D', 2, 2),
                                (210, 'VIOLATION OF OFFICE MEMORANDUM', 'D', 2, 2),
                                (211, 'LAXITY', 'D', 2, 2),
                                (212, 'FALSE TESTIMONY', 'D', 2, 2),
                                (213, 'ANOMALOUS ACTIVITY', 'D', 2, 2),
                                (214, 'PRINCIPAL OF ANOMALOUS ACT.', 'D', 2, 2),
                                (215, 'FRAUDULENT PUNCHING OF T/C', 'D', 2, 2),
                                (216, 'INSTIGATING MASS WALK', 'D', 2, 2),
                                (217, 'INSTIGATING A PETITION DRIVE', 'D', 2, 2),
                                (218, 'PARTICIPATING PETITION', 'D', 2, 2),
                                (219, 'FALSIFYING DOCUMENT', 'D', 2, 2),
                                (220, 'FALSEHOOD OR MISREPRESENTATION', 'D', 2, 2),
                                (221, 'ACCEPTING MONEY', 'D', 2, 2),
                                (222, 'OFFERING EXCHANGING 4 FAVOR', 'D', 2, 2),
                                (223, 'DISOBEYING PRESCRIBE HAIR CUT', 'D', 2, 2),
                                (224, 'ENTERTAINING VISOTORS AT HOLID', 'D', 2, 2),
                                (225, 'GIVING CONFIDENTIAL INFO', 'D', 2, 2),
                                (226, 'MOONLIGHTING', 'D', 2, 2),
                                (227, 'NOT REPORT OFFENSE OF OTHER', 'D', 2, 2),
                                (228, 'MALINGERING', 'D', 2, 2),
                                (229, 'DISCOURTESY', 'D', 2, 2),
                                (230, 'FAILURE TO SALUTE', 'D', 2, 2),
                                (231, 'OVER FRATERNIZATION', 'D', 2, 2),
                                (232, 'NON ATTENDANCE IN TROP-FORMATI', 'D', 2, 2),
                                (233, 'IMPROPER/INCOMPLETE JOURNAL', 'D', 2, 2),
                                (234, 'PERSONAL LOAN ANY CONTRACTOR', 'D', 2, 2),
                                (240, 'NOT CARRYING ID/DDO/SBR', 'D', 2, 2),
                                (235, 'FAILURE TO NOTIFY VIOLATIONS', 'D', 2, 2),
                                (236, 'REFUSAL TO SIGN VIOLATIONS', 'D', 2, 2),
                                (238, 'ASSUMING DUTY WITH EXPIRED SBR', 'D', 2, 2),
                                (237, 'STRAIGHT DUTY W/O REASON', 'D', 2, 2),
                                (239, 'DISLOYALTY', 'D', 2, 2),
                                (241, 'GRAVE OFFENSES', 'D', 2, 2),
                                (305, 'USING INDECENT LANGUAGE', 'D', 2, 3),
                                (306, 'GAMBLING IN AGENCY PREMISES', 'D', 2, 3),
                                (308, 'EXHIBITING PORNOGRAPHIC MATERI', 'D', 2, 3),
                                (309, 'UNALERT,SMOKING,TXTING ON POST', 'D', 2, 3),
                                (310, 'CONDUCT SCANDAL', 'D', 2, 3),
                                (311, 'ILLICIT LOVE AFFAIR', 'D', 2, 3),
                                (312, 'IMMORALITY', 'D', 2, 3),
                                (101, 'PROVOKING A FIGHT', 'D', 2, 1),
                                (102, 'TRREATENING', 'D', 2, 1),
                                (103, 'CAUSING INJURY', 'D', 2, 3),
                                (111, 'ABANDONING POST', 'D', 2, 3),
                                (112, 'NEGLIGENCE', 'D', 2, 3),
                                (113, 'LOSING OR ALLOWING DAMAGE', 'D', 2, 3),
                                (114, 'LOSING DAMAGE NEGLIGENCE', 'G', 2, 3),
                                (115, 'DAMAGING CLIENT OR AGENCY', 'D', 2, 3),
                                (116, 'STEALING', 'D', 2, 3),
                                (117, 'MALVERSATION', 'D', 2, 3),
                                (118, 'DEFACING COMPANY ISSUED ID', 'D', 2, 3),
                                (307, 'CREATING UNSAFE CON TO CLIENT', 'D', 2, 3);

                            ";

            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    private async Task _01RDevdataRecreate(string schema, string conn)
    {
        string desc = $@"SELECT * FROM  {schema}.rdevdata WHERE  Dev_Name like '%ACI - 004%' ;";
        var existingColumn = await _sql.FetchData<RdevdataModel, dynamic>(desc, new { }, conn);

        if (existingColumn.Count > 0)
        {
            var sql = $@"Drop table if exists {schema}.rdevdata  ";
            await _sql.ExecuteCmd<dynamic>(sql, new { }, conn);


            await _01RDevdata(schema, conn);
        }
    }

    private async Task _01RPenalty(string schema, string conn)
    {
        string sql = $@"CREATE TABLE   if not exists {schema}.`rpenalty` (
                          `DEV_NO` varchar(10) DEFAULT NULL,
                          `FREQ` varchar(1) DEFAULT NULL,
                          `PENALTY_NO` varchar(2) DEFAULT NULL,
                          `DESC_` varchar(30) DEFAULT NULL,
                          `resetregref` varchar(1) NOT NULL DEFAULT '0' COMMENT '1true, 2false reset regularization period',
                          `isterminated` varchar(1) NOT NULL DEFAULT '0',
                          `days` double(6,0) NOT NULL DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select DEV_NO from {schema}.rpenalty limit 1 ";
        var res = await _sql.FetchData<RpenaltyModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.rpenalty
                     (DEV_NO,	FREQ,	PENALTY_NO,	DESC_,	resetregref,	isterminated,	days) VALUES
                     (1,	2,	'7D',	'7 DAYS SUSPENSION',	1,	0,	7),
                     (1,	1,	'WR',	'WRITTEN REPRIMAND',	0,	0,	0),
                     (1,	5,	'DM',	'DISMISSAL',	        1,	1,	0),
                     (2,	3,	'SL',	'SUSPENDED L/ POST',	0,	0,	0),
                     (2,	4,	'DM',	'DISMISSAL',	        1,	1,	0),
                     (2,	5,	'DP',	'DISMISSAL/PAY OF COST',0,	1,	0),
                     (3,	3,	'SL',	'SUSPENDED L/ POST',	0,	0,	0),
                     (3,	4,	'DM',	'DISMISSAL',	        1,	1,	0),
                     (3,	5,	'DP',	'DISMISSAL/PAY OF COST',0,	1,	0)
             ; ";


            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    private async Task _01RecreateRPenalty(string schema, string conn)
    {
        string desc = $@"DESC {schema}.rpenalty;";
        var existingColumns = await _sql.FetchData<dynamic, dynamic>(desc, new { }, conn);

        bool hasId = existingColumns.Any(c => c.Field == "Id");

        if (!hasId)
        {
            string sql = $@"
            ALTER TABLE {schema}.rpenalty
            ADD COLUMN Id INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY FIRST;
        ";

            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }


    private async Task _01EmploymentType(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.Employmenttype (
                            Id      INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            Name    VARCHAR(45),
                            PRIMARY KEY (`Id`))ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select Id from {schema}.Employmenttype limit 1 ";
        var res = await _sql.FetchData<LeavetypeModel, dynamic>(sql, new { }, conn);

        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.Employmenttype
                            (Name) values 
                            ('Trainee'), ('Contractual'),('Probationary'), ('Regular'); ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

    }

    private async Task _01RecreateEmploymentType(string schema, string conn)
    {

        string desc = $@" DESC {schema}.Employmenttype;";
        var existingColumns = await _sql.FetchData<dynamic, dynamic>(desc, new { }, conn);
        var newColumns = new List<string> { "IsVisible", "ShowDeploymentEnd", "CanbeDeleted" };


        foreach (var column in newColumns)
        {
            bool isExist = existingColumns.Any(c => c.Field == column);
            if (!isExist)
            {
                string columnDesc = "";

                if (column == "IsVisible") columnDesc = "IsVisible SMALLINT DEFAULT 0";
                if (column == "ShowDeploymentEnd") columnDesc = "ShowDeploymentEnd SMALLINT DEFAULT 0";
                if (column == "CanbeDeleted") columnDesc = "CanbeDeleted SMALLINT DEFAULT 1";

                string sql = $@"Alter table {schema}.employmenttype Add Column {columnDesc}";
                await _sql.ExecuteCmd(sql, new { columnDesc }, conn);

            }
        }
    }


    private async Task _01LeaveType(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveType (
                        Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                        Code          CHAR(5) NOT NULL,
                        LeaveName     VARCHAR(45),
                        AnivStart     DATE,
                        AnivEnd       DATE,
                        DefValue      Integer,
                        PRIMARY KEY (`Id`))ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);


        sql = @$"select * from {schema}.LeaveType limit 1 ";
        var res = await _sql.FetchData<LeavetypeModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.LeaveType
                            (Id, Code,  LeaveName,                 AnivStart,                          AnivEnd, DefValue) values 
                            (1,  'SIL', 'Service Incentive Leave', date(concat(year(now()),'-01-01')), date(concat(year(now()),'-12-31')), 5), 
                            (2,  'SL', 'Sick Leave',               date(concat(year(now()),'-01-01')), date(concat(year(now()),'-12-31')), 15),
                            (3,  'VL', 'Vacation Leave',           date(concat(year(now()),'-01-01')), date(concat(year(now()),'-12-31')), 15) ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }
    }

    private async Task _01LeaveGrp(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveGrp (
                            Id        INTEGER         UNSIGNED    NOT NULL AUTO_INCREMENT,
                            Name      VARCHAR(60)                 NOT NULL,
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;  ";
        await _sql.ExecuteCmd(sql, new { }, conn);

        sql = @$"select * from {schema}.LeaveGrp limit 1 ";
        var res = await _sql.FetchData<LeavegrpModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.LeaveGrp
                            (Name) values 
                            ('Executive'), 
                            ('Supervisor'), 
                            ('Manager'), 
                            ('Tenured'), 
                            ('Tenured - Lvl 1'), 
                            ('Tenured - Lvl 2'), 
                            ('Tenured - Lvl 3'), 
                            ('Regular Employee'), 
                            ('Contract Employee'), 
                            ('Part-time Employee'); 
                    insert into {schema}.LeaveGrp (Name) 
                        select Name from {schema}.rdepartment; ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

    }

    private async Task _01LeaveGrpCredit(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveGrpCredit (
                          LeaveGrpId    INTEGER         UNSIGNED NOT NULL,
                          LeaveTypeId   INTEGER         UNSIGNED        DEFAULT 0,
                          Credit`       DOUBLE(12,2)                    DEFAULT 0,
                        PRIMARY KEY(`LeaveGrpId`, `LeaveTypeId`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01LeaveGrpApprover(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveGrpApprover (
                            Id              INTEGER     UNSIGNED            NOT NULL AUTO_INCREMENT,
                            LeaveGrpId      INTEGER     UNSIGNED DEFAULT 0,
                            ApproverId      INTEGER     UNSIGNED DEFAULT 0,
                            ApproverLevel   INTEGER     UNSIGNED DEFAULT 0,
                        PRIMARY KEY (`Id`))ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01LeaveCredit(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveCredit (
                          Year          INTEGER UNSIGNED    NOT NULL AUTO_INCREMENT,
                          EmpmasId      CHAR(5)             NOT NULL,
                          LeaveTypeId   Integer         Default 0,
                          AnnivStart    DATE,
                          AnnivEnd      DATE,
                          Credit        DOUBLE(12,2)    Default 0,
                          Consumed      DOUBLE(12,2)    Default 0,
                        PRIMARY KEY(`Year`, `EmpmasId`)) ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01LeaveDefaultApprover(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveDefaultApprover (
                              Id            INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                              Lvl           INTEGER UNSIGNED,
                              EmpmasId      INTEGER UNSIGNED,
                              Designation   VARCHAR(45),
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01LeaveApplication(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LeaveApplication (
                              Id            INTEGER         UNSIGNED NOT NULL AUTO_INCREMENT,
                              Yr            INTEGER         UNSIGNED    DEFAULT 0,
                              EmpmasId      INTEGER         UNSIGNED    DEFAULT 0,
                              DateApplied   DATE,
                              LeaveTypeId   INTEGER         UNSIGNED    DEFAULT 0,
                              LvBalance     DOUBLE(12,2)                DEFAULT 0,
                              DaysCnt       DOUBLE(12,2)                DEFAULT 0,
                              LvTime        VARCHAR(45),
                              DaysWithPay   DOUBLE(12,2)                DEFAULT 0,
                              Urgency       VARCHAR(15)                 DEFAULT 'Normal',
                              LvStart       DATE,
                              LvEnd         DATE,
                              Reason        VARCHAR(150),
                              Address       VARCHAR(150),
                              TelNo         VARCHAR(45),
                              Approver1Id   INTEGER         UNSIGNED    DEFAULT 0,
                              Approver2Id   INTEGER         UNSIGNED    DEFAULT 0,
                              Approver3Id   INTEGER         UNSIGNED    DEFAULT 0,
                              Status        VARCHAR(10)                 DEFAULT 'New',
                        PRIMARY KEY(`Id`)) ENGINE = InnoDB;
";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01PisSettings(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.PisSettings (
                            Id                      INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            LeaveYrImplementation   int, 
                            LeaveAnniversaryStart   DATE,
                            LeaveAnniversaryEnd     DATE,
                        PRIMARY KEY(`Id`))ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);


        sql = @$"select * from {schema}.PisSettings limit 1 ";
        var res = await _sql.FetchData<PissettingsModel, dynamic>(sql, new { }, conn);
        if (res == null || res.Count == 0)
        {
            sql = $@"insert into {schema}.PisSettings
                            (Id, LeaveYrImplementation, LeaveAnniversaryStart,              LeaveAnniversaryEnd) values 
                            (1,  year(now()),           date(concat(year(now()),'-01-01')), date(concat(year(now()),'-12-31')) ); ";
            await _sql.ExecuteCmd(sql, new { }, conn);
        }

        // --- Trail ----------------------------------------------------------------
        sql = $@"CREATE TABLE if not exists {schema}.PisSettingsTrail (
                            Id                      INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
                            LeaveYrImplementation   int, 
                            LeaveAnniversaryStart   DATE,
                            LeaveAnniversaryEnd     DATE,
                            UserId                  int, 
                            Changed                 DateTime,
                        PRIMARY KEY(`Id`))ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    // ******************************************************************************************//
    // --- Transaction Tables ---------------------------------------------------------------------
    // ******************************************************************************************//
    private async Task _01Companies(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.Companies (
                            Id          int unsigned NOT NULL AUTO_INCREMENT,
                            Name        varchar(60)     DEFAULT NULL,
                            Add1        varchar(150)    DEFAULT NULL,
                            Addr2       varchar(150)    DEFAULT NULL,
                            AreaId      varchar(5)      DEFAULT NULL,
                            TelNo       varchar(35)     DEFAULT NULL,
                            FaxNo       varchar(10)     DEFAULT NULL,
                            ParentId    Int             DEFAULT NULL,
                            Status      varchar(2)      DEFAULT 'A',
                            RegNo       Varchar(25)     DEFAULT NULL,
                            ContPerson  varchar(25)     DEFAULT NULL,
                            ContNo      varchar(45)     DEFAULT NULL,
                            Remarks     varchar(200)    DEFAULT NULL,
                            ContStart   date            DEFAULT Null,
                            ContEnd     date            DEFAULT Null,
                            RegionId    varchar(15)     DEFAULT '  ',
                            PRIMARY KEY (Id)) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }
    private async Task _01Deviation(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.Deviation (
                            Id          int unsigned NOT NULL AUTO_INCREMENT,
                            Control_No  varchar(12) DEFAULT NULL,
                            Prep_ById   Int         DEFAULT 0,
                            Prep_Dt     date        DEFAULT NULL,
                            CoId        int         DEFAULT NULL,
                            EmpNumber   varchar(10) DEFAULT NULL,
                            Dev_NO      varchar(10) DEFAULT NULL,
                            Occur_Dt    date        DEFAULT NULL,
                            Freq_No     Char(1)     DEFAULT NULL,
                            Penalty_No  Char(2)     DEFAULT NULL,
                            Appr_BYId   Int         DEFAULT NULL,
                            Appr_DT     date        DEFAULT NULL,
                            DevStart    date        DEFAULT NULL,
                            DevEnd      date        DEFAULT NULL,
                        PRIMARY KEY (id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

    }
    private async Task _01Emergency(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  if not exists {schema}.Emergency (
                            Id          Int unsigned NOT NULL AUTO_INCREMENT,
                            EmpNumber   Varchar(10) DEFAULT NULL,
                            Name        Varchar(25) DEFAULT NULL,
                            Addr        varchar(60) DEFAULT NULL,
                            Rel         varchar(20) DEFAULT NULL,
                            Tel         varchar(45) DEFAULT NULL,
                        PRIMARY KEY (id)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01EmpmasInternal(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.Empmas (
                        Id              Int unsigned NOT NULL AUTO_INCREMENT,
                        SystemId        Int                 DEFAULT NULL,
                        EmpNumber       Varchar(10)         DEFAULT NULL,
                        EmpLastNm       varchar(45)         DEFAULT NULL,
                        EmpFirstNm      varchar(45)         DEFAULT NULL,
                        EmpMidNm        varchar(45)         DEFAULT NULL,
                        Suffix          char(3)             DEFAULT '',
                        EmpAlias        varchar(15)         DEFAULT NULL,
                        PRIMARY KEY(Id)) ENGINE = InnoDB;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01EmpmasGrp(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.EmpmasGrp (
                          EmpmasId      INTEGER NOT NULL,
                          SecId         INTEGER UNSIGNED DEFAULT 0,
                          DepId         INTEGER UNSIGNED DEFAULT 0,
                          DivId         INTEGER UNSIGNED DEFAULT 0,
                          LeaveGrpId    INTEGER UNSIGNED DEFAULT 0,
                          EmpstatId     INTEGER UNSIGNED DEFAULT 0,
                            
                          PRIMARY KEY(`EmpmasId`)) ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01DepRec(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.DepRec (
                            EmpmasId                INTEGER UNSIGNED NOT NULL DEFAULT 0,
                            DivId                   INTEGER UNSIGNED DEFAULT 0,
                            DepId                   INTEGER UNSIGNED DEFAULT 0,
                            SecId                   INTEGER UNSIGNED DEFAULT 0,
                            PositionId              INTEGER UNSIGNED DEFAULT 0,
                            LeavegrpId              INTEGER UNSIGNED DEFAULT 0,
                            PayrollgrpId            INTEGER UNSIGNED DEFAULT 0,
                            EmploymentTypeId        INTEGER UNSIGNED DEFAULT 0,
                            EmpStatusId             INTEGER UNSIGNED DEFAULT 15,
                            DHired                  DATE,
                            DRegularization         DATE,
                            DTraineeStart           DATE,
                            DTraineeEnd             DATE,
                            DContractualStart       DATE,
                            DContractualEnd         DATE,
                            DProbationaryStart      DATE,
                            DProbationaryEnd        DATE,
                            DRegularizationStart    DATE,
                            DRegularizationEnd      DATE,
                            DPermanentStart         DATE,
                            DResigned               DATE,
                            DTerminated             DATE,
                            DSeparated              DATE,
                            Remarks                 VARCHAR(100),
                            PRIMARY KEY(`EmpmasId`)) ENGINE = InnoDB; ";
        await _sql.ExecuteCmd(sql, new { }, conn);


        string descPara = $@"DESC {schema}.DepRec";
        var columns = await _sql.FetchData<dynamic, dynamic>(descPara, new { }, conn);

        var columnNames = new List<string> { "IsOnDeviation", "IdDeviation", "IsOnDiciplinary", "IsOnInvestigation", "IdDeployment", "DepDate", "TranNumber" , "IdInvestigate"};

        foreach (var columnName in columnNames)
        {
            // Check if the column exists in the table
            bool columnExist = columns.Any(c => c.Field == columnName);

            // If the column doesn't exist, add it
            if (!columnExist)
            {
                string fieldDesc = "";

                // Define the column description based on the column name
                if (columnName == "IsOnDeviation")      fieldDesc = "`IsOnDeviation`     smallint(5)    DEFAULT '0'";
                if (columnName == "IdDeviation")        fieldDesc = "`IdDeviation`       int(10)        DEFAULT '0'";
                if (columnName == "IsOnDiciplinary")    fieldDesc = "`IsOnDiciplinary`   smallint(5)    DEFAULT '0'";
                if (columnName == "IsOnInvestigation")  fieldDesc = "`IsOnInvestigation` smallint(5)    DEFAULT '0'";
                if (columnName == "IdDeployment")       fieldDesc = "`IdDeployment`      INT            DEFAULT '0'     AFTER PayrollgrpId";
                if (columnName == "DepDate")            fieldDesc = "`DepDate`           DATE                           AFTER EmpStatusId";
                if (columnName == "TranNumber")         fieldDesc = "`TranNumber`        CHAR(12)       DEFAULT ''      AFTER EmpmasId";
                if (columnName == "IdInvestigate")      fieldDesc = "`IdInvestigate`     int(10)        DEFAULT '0'     AFTER IsOnInvestigation";


                // Construct the SQL query
                sql = $@"ALTER TABLE {schema}.DepRec ADD COLUMN {fieldDesc}";

                await _sql.ExecuteCmd(sql, new { }, conn);
            }
        }




    }

    private async Task _01EmpBlockPost(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.empBlockPost (
                          `EmpmasId` int(11) NOT NULL,
                          `DeploymentId` int(11) NOT NULL,
                          PRIMARY KEY (`EmpmasId`,`DeploymentId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1";

        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    private async Task _01TranDeployment(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS  {schema}.TranDeployment (
                      `Id`                  int(10)     unsigned NOT NULL  AUTO_INCREMENT,
                      `IdEmpmas`            int(10)     unsigned    DEFAULT '0',
                      `TranNumber`          char(12)                DEFAULT NULL,
                      `PrepDate`            date                    DEFAULT NULL,
                      `DepStart`            date                    DEFAULT NULL,
                      `DepEnd`              date                    DEFAULT NULL,
                      `DateApproved`        datetime                DEFAULT NULL,
                      `Mode`                char(3)                 DEFAULT NULL,
                      `IdEmploymentType`    int(10)     unsigned    DEFAULT '0',
                      `IdDivision`          int(10)     unsigned    DEFAULT '0',
                      `IdSection`           int(10)     unsigned    DEFAULT '0',
                      `IdDepartment`        int(10)     unsigned    DEFAULT '0',
                      `IdPosition`          int(10)     unsigned    DEFAULT '0',
                      `IdDesignation`       int(10)     unsigned    DEFAULT '0',
                      `IdPayrollGrp`        int(10)     unsigned    DEFAULT '0',
                      `IdDeployment`        int(10)     unsigned    DEFAULT '0',
                      `IdApprover`          int(10)     unsigned    DEFAULT '0',
                      `MarkApprove`         int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1; ";

        await _sql.ExecuteCmd(sql, new { }, conn);

    }

    private async Task _01RecreateTranDeployment(string schema, string conn)
    {
        string descPara = $@"DESC {schema}.TranDeployment";
        var columns = await _sql.FetchData<dynamic, dynamic>(descPara, new { }, conn);

        var columnNames = new List<string> { "IdDeployment" };

        foreach (var columnName in columnNames)
        {

            bool columnExist = columns.Any(c => c.Field == columnName);

            if (!columnExist)
            {
                string fieldDesc = "";

                if (columnName == "IdDeployment") fieldDesc = "`IdDeployment`        int(10)     unsigned    DEFAULT '0' AFTER IdPayrollGrp";

                string sql = $@"ALTER TABLE {schema}.TranDeployment ADD COLUMN {fieldDesc}";

                await _sql.ExecuteCmd(sql, new { }, conn);
            }
        }
    }

    private async Task _01RecreateTranDeploymentApproval(string schema, string conn)
    {
        string descPara = $@"DESC {schema}.trandeploymentapproval";
        var columns = await _sql.FetchData<dynamic, dynamic>(descPara, new { }, conn);

        var columnNames = new List<string> { "IdDeployment" };

        foreach (var columnName in columnNames)
        {

            bool columnExist = columns.Any(c => c.Field == columnName);

            if (!columnExist)
            {
                string fieldDesc = "";

                if (columnName == "IdDeployment") fieldDesc = "`IdDeployment`        int(10)     unsigned    DEFAULT '0' AFTER IdPayrollGrp";

                string sql = $@"ALTER TABLE {schema}.trandeploymentapproval ADD COLUMN {fieldDesc}";

                await _sql.ExecuteCmd(sql, new { }, conn);
            }
        }
    }

    private async Task _01TranDeploymentApproval(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS  {schema}.TranDeploymentApproval (
                      `Id`                  int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `IdEmpmas`            int(10)     unsigned    DEFAULT '0',
                      `TranNumber`          char(12)                DEFAULT NULL,
                      `PrepDate`            date                    DEFAULT NULL,
                      `DepStart`            date                    DEFAULT NULL,
                      `DepEnd`              date                    DEFAULT NULL,
                      `DateApproved`        datetime                DEFAULT NULL,
                      `Mode`                char(3)                 DEFAULT NULL,
                      `IdEmploymentType`    int(10)     unsigned    DEFAULT '0',
                      `IdDivision`          int(10)     unsigned    DEFAULT '0',
                      `IdSection`           int(10)     unsigned    DEFAULT '0',
                      `IdDepartment`        int(10)     unsigned    DEFAULT '0',
                      `IdPosition`          int(10)     unsigned    DEFAULT '0',
                      `IdDesignation`       int(10)     unsigned    DEFAULT '0',
                      `IdPayrollGrp`        int(10)     unsigned    DEFAULT '0',
                      `IdDeployment`        int(10)     unsigned    DEFAULT '0',
                      `IdApprover`          int(10)     unsigned    DEFAULT '0',
                      `MarkApprove`         int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1; ";

        await _sql.ExecuteCmd(sql, new { }, conn);


    }

    private async Task _01TranDeploymentApprovalHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.Trandeploymentapprovalhistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }



    private async Task _01TranDeviation(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trandeviation` (
                              `Id`              int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
                              `IdEmpmas`        int(10) UNSIGNED,
                              `TranNumber`      CHAR(12),
                              `PrepDate`        DATE,
                              `Mode`            CHAR(3),
                              `ReportDate`      DATETIME,
                              `OccurDate`      DATETIME,
                              `Allegation`      VARCHAR(10),
                              `EmpStatusId`     int(10),
                              `IdApprover`      int(10)     unsigned    DEFAULT '0',
                              `MarkApprove`     int(10)     unsigned    DEFAULT '0',
                              PRIMARY KEY (`Id`)) ENGINE = InnoDB; ;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }



    private async Task _01RecreateTranDeviation(string schema, string conn)
    {
        string desc = $@" DESC {schema}.Trandeviation;";
        var existingColumns = await _sql.FetchData<dynamic, dynamic>(desc, new { }, conn);


        var newColumns = new List<string> { "DateReported" };


        foreach (var column in newColumns)
        {
            bool isExist = existingColumns.Any(c => c.Field == column);
            if (!isExist)
            {
                string columnDesc = "";

                if (column == "DateReported") columnDesc = "DateReported VARCHAR(180) DEFAULT NULL AFTER ReportDate ";

                string sql = $@"Alter table {schema}.Trandeviation Add Column {columnDesc}";
                await _sql.ExecuteCmd(sql, new { columnDesc }, conn);

            }
        }
    }

    private async Task _01TranDeviationApproval(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trandeviationapproval` (
                              `Id`              int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
                              `IdEmpmas`        int(10) UNSIGNED,
                              `TranNumber`      CHAR(12),
                              `PrepDate`        DATE,
                              `Mode`            CHAR(3),
                              `ReportDate`      DATETIME,
                              `OccurDate`       DATETIME,
                              `Allegation`      VARCHAR(10),
                              `Freq_No`         CHAR(1),
                              `EmpStatusId`     int(10),
                              `IdApprover`      int(10)     unsigned    DEFAULT '0',
                              `MarkApprove`     int(10)     unsigned    DEFAULT '0',
                              PRIMARY KEY (`Id`)) ENGINE = InnoDB; ;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01TranDeviationHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.Trandeviationapprovalhistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01TrandeviationOther(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`Trandeviationother` (
                      `Remarks` varchar(180) DEFAULT NULL
                   ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01RecreateTrandeviationOther(string schema, string conn)
    {
        string desc = $@" DESC {schema}.Trandeviationother;";
        var existingColumns = await _sql.FetchData<dynamic, dynamic>(desc, new { }, conn);

        bool hasId = existingColumns.Any(c => c.Field == "Id");
        if (hasId)
        {
            string dropSql = $@"ALTER TABLE {schema}.Trandeviationother DROP COLUMN Id;";
            await _sql.ExecuteCmd(dropSql, new { }, conn);
        }

        var newColumns = new List<string> { "Link", "TranNumber" };


        foreach (var column in newColumns)
        {
            bool isExist = existingColumns.Any(c => c.Field == column);
            if (!isExist)
            {
                string columnDesc = "";

                if (column == "Link") columnDesc = "Link VARCHAR(180) DEFAULT NULL";
                if (column == "TranNumber") columnDesc = "TranNumber char(12) DEFAULT NULL";


                string sql = $@"Alter table {schema}.Trandeviationother Add Column {columnDesc}";
                await _sql.ExecuteCmd(sql, new { columnDesc }, conn);

            }
        }

    }

    private async Task _01TranDisciplinary(string schema, string conn)
    {
        string sql = $@"  CREATE TABLE IF NOT EXISTS {schema}.`Trandisciplinary`(
                          `Id`          int(10)     unsigned NOT NULL AUTO_INCREMENT,
                          `IdEmpmas`    int(10)     unsigned DEFAULT NULL,
                          `TranNumber`  char(12)    DEFAULT NULL,
                          `PrepDate`    date        DEFAULT NULL,
                          `Mode`        char(3)     DEFAULT NULL,
                          `Penalty_No`  char(2)     DEFAULT NULL,
                          `StartDate`   date        DEFAULT NULL,
                          `EndDate`     date        DEFAULT NULL,
                          `NoOfDays`    double(6, 0) DEFAULT '0',
                          `EmpStatusId` int(10)      DEFAULT '0',
                          PRIMARY KEY(`Id`)
                         ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";




        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01TranDisciplinaryAppr(string schema, string conn)
    {
        string sql = $@"  CREATE TABLE IF NOT EXISTS {schema}.`TrandisciplinaryApproval`(
                          `Id`          int(10)     unsigned NOT NULL AUTO_INCREMENT,
                          `IdEmpmas`    int(10)     unsigned DEFAULT NULL,
                          `TranNumber`  char(12)    DEFAULT NULL,
                          `PrepDate`    date        DEFAULT NULL,
                          `Mode`        char(3)     DEFAULT NULL,
                          `Penalty_No`  char(2)     DEFAULT NULL,
                          `StartDate`   date        DEFAULT NULL,
                          `EndDate`     date        DEFAULT NULL,
                          `NoOfDays`    double(6, 0) DEFAULT '0',
                          `EmpStatusId` int(10)      DEFAULT '0',
                          PRIMARY KEY(`Id`)
                         ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01TranDisciplinaryApprHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.TrandisciplinaryApprovalHistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01TranExonerate(string schema, string conn)
    {
        string sql = $@" CREATE TABLE IF NOT EXISTS {schema}.`Tranexonerate` (
                      `Id`          int(10) unsigned NOT NULL AUTO_INCREMENT,
                      `IdEmpmas`    int(10) unsigned DEFAULT NULL,
                      `TranNumber`  char(12) DEFAULT NULL,
                      `PrepDate`    date DEFAULT NULL,
                      `Prep_ById`   int(11) DEFAULT '0',
                      `Mode`        char(3) DEFAULT NULL,
                      `EmpStatusId` int(10) DEFAULT '0',
                      `IdApprover`  int(10)     unsigned    DEFAULT '0',
                      `MarkApprove` int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY(`Id`)
                    ) ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01TranExonerateAppr(string schema, string conn)
    {
        string sql = $@" CREATE TABLE IF NOT EXISTS {schema}.`TranexonerateApproval` (
                      `Id`          int(10) unsigned NOT NULL AUTO_INCREMENT,
                      `IdEmpmas`    int(10) unsigned DEFAULT NULL,
                      `TranNumber`  char(12) DEFAULT NULL,
                      `PrepDate`    date DEFAULT NULL,
                      `Prep_ById`   int(11) DEFAULT '0',
                      `Mode`        char(3) DEFAULT NULL,
                      `EmpStatusId` int(10) DEFAULT '0',
                      `IdApprover`  int(10)     unsigned    DEFAULT '0',
                      `MarkApprove` int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY(`Id`)
                    ) ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }


    private async Task _01TranExonerateOther(string schema, string conn)
    {
        string sql = $@" CREATE TABLE IF NOT EXISTS {schema}.`TranexonerateOther` (
                      `TranNumber`  char(12) DEFAULT NULL,
                      `Remarks`    VARCHAR(180) DEFAULT NULL
                    ) ENGINE = InnoDB AUTO_INCREMENT = 1 DEFAULT CHARSET = latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }



    private async Task _01TranExonerateApprHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.TranexonerateApprovalHistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01TranInvestigate(string schema, string conn)
    {
        string sql = $@"  
                        CREATE TABLE IF NOT EXISTS {schema}.`Traninvestigate` (
                          `Id`          int(10) unsigned NOT NULL AUTO_INCREMENT,
                          `IdEmpmas`    int(10) unsigned DEFAULT NULL,
                          `TranNumber`  char(12) DEFAULT NULL,
                          `PrepDate`    date DEFAULT NULL,
                          `Prep_ById`   int(11) DEFAULT '0',
                          `Mode`        char(3) DEFAULT NULL,
                          `StartDate`   date DEFAULT NULL,
                          `EndDate`     date DEFAULT NULL,
                          `Remarks`    VARCHAR(180) DEFAULT NULL,
                          `EmpStatusId` int(10) DEFAULT '0',
                           `IdApprover` int(10)     unsigned    DEFAULT '0',
                           `MarkApprove` int(10)     unsigned    DEFAULT '0',
                          PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;";


        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01TranInvestigateAppr(string schema, string conn)
    {
        string sql = $@"  
                        CREATE TABLE IF NOT EXISTS {schema}.`TraninvestigateApproval` (
                          `Id`          int(10) unsigned NOT NULL AUTO_INCREMENT,
                          `IdEmpmas`    int(10) unsigned DEFAULT NULL,
                          `TranNumber`  char(12) DEFAULT NULL,
                          `PrepDate`    date DEFAULT NULL,
                          `Prep_ById`   int(11) DEFAULT '0',
                          `Mode`        char(3) DEFAULT NULL,
                          `StartDate`   date DEFAULT NULL,
                          `EndDate`     date DEFAULT NULL,
                          `Remarks`    VARCHAR(180) DEFAULT NULL,
                          `EmpStatusId` int(10) DEFAULT '0',
                           `IdApprover` int(10)     unsigned    DEFAULT '0',
                           `MarkApprove` int(10)     unsigned    DEFAULT '0',
                          PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;";


        await _sql.ExecuteCmd(sql, new { }, conn);
    }



    private async Task _01TranInvestigateApprHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.TraninvestigateApprovalHistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }

    private async Task _01TranReinstatement(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS  {schema}.TranReinstatement (
                      `Id`                  int(10)     unsigned NOT NULL  AUTO_INCREMENT,
                      `IdEmpmas`            int(10)     unsigned    DEFAULT '0',
                      `TranNumber`          char(12)                DEFAULT NULL,
                      `PrepDate`            date                    DEFAULT NULL,
                      `DepStart`            date                    DEFAULT NULL,
                      `DepEnd`              date                    DEFAULT NULL,
                      `DateApproved`        datetime                DEFAULT NULL,
                      `Mode`                char(3)                 DEFAULT NULL,
                      `IdEmploymentType`    int(10)     unsigned    DEFAULT '0',
                      `IdDivision`          int(10)     unsigned    DEFAULT '0',
                      `IdSection`           int(10)     unsigned    DEFAULT '0',
                      `IdDepartment`        int(10)     unsigned    DEFAULT '0',
                      `IdPosition`          int(10)     unsigned    DEFAULT '0',
                      `IdDesignation`       int(10)     unsigned    DEFAULT '0',
                      `IdPayrollGrp`        int(10)     unsigned    DEFAULT '0',
                      `IdDeployment`        int(10)     unsigned    DEFAULT '0',
                      `IdApprover`          int(10)     unsigned    DEFAULT '0',
                      `MarkApprove`         int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1; ";

        await _sql.ExecuteCmd(sql, new { }, conn);

    }

  

    private async Task _01TranReinstatementAppr(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS  {schema}.TranReinstatementApproval (
                      `Id`                  int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `IdEmpmas`            int(10)     unsigned    DEFAULT '0',
                      `TranNumber`          char(12)                DEFAULT NULL,
                      `PrepDate`            date                    DEFAULT NULL,
                      `DepStart`            date                    DEFAULT NULL,
                      `DepEnd`              date                    DEFAULT NULL,
                      `DateApproved`        datetime                DEFAULT NULL,
                      `Mode`                char(3)                 DEFAULT NULL,
                      `IdEmploymentType`    int(10)     unsigned    DEFAULT '0',
                      `IdDivision`          int(10)     unsigned    DEFAULT '0',
                      `IdSection`           int(10)     unsigned    DEFAULT '0',
                      `IdDepartment`        int(10)     unsigned    DEFAULT '0',
                      `IdPosition`          int(10)     unsigned    DEFAULT '0',
                      `IdDesignation`       int(10)     unsigned    DEFAULT '0',
                      `IdPayrollGrp`        int(10)     unsigned    DEFAULT '0',
                      `IdDeployment`        int(10)     unsigned    DEFAULT '0',
                      `IdApprover`          int(10)     unsigned    DEFAULT '0',
                      `MarkApprove`         int(10)     unsigned    DEFAULT '0',
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1; ";

        await _sql.ExecuteCmd(sql, new { }, conn);


    }

    private async Task _01TranReinstatementApprHistory(string schema, string conn)
    {
        string sql = $@"CREATE TABLE  IF NOT EXISTS {schema}.Tranreinstatementapprovalhistory (
                      `Id`                      int(10)     unsigned NOT NULL AUTO_INCREMENT,
                      `TranNumber`              char(12)             DEFAULT NULL,
                      `Date`                    datetime             DEFAULT NULL,
                      `UserId`                  int(10)     unsigned DEFAULT NULL,
                      `Status`                  char(12)             DEFAULT NULL,
                      `ApproverId`              int(10)     unsigned DEFAULT NULL,
                      `ApproverRemarks`         char(150)            DEFAULT NULL,
                      PRIMARY KEY (`Id`)
                    ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }




    private async Task _01EmpTranMovement(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`emptranmovement` (
                          `id`          int(10) unsigned NOT NULL AUTO_INCREMENT,
                          `EmpmasId`    int(10) unsigned NULL,
                          `MovDate`     date         NULL,
                          `MovNumber`   varchar(12)  NULL,
                          `TranStart`   date         NULL,
                          `TranEnd`     date         NULL,
                          `Remarks`     varchar(180) NULL,
                          `EmpStatusId` int(10) unsigned  NULL,
                          PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }



    private async Task _01RecreateEmpTranMovement(string schema, string conn)
    {
        string desc = $@"DESC {schema}.emptranmovement;";
        var existingColumn = await _sql.FetchData<dynamic, dynamic>(desc, new { }, conn);

        var newColumn = new List<string> { "UserId", "DateRecorded" };

        foreach (var item in newColumn)
        {
            bool isColumnExists = existingColumn.Any(c => c.Field == item);
            if (!isColumnExists)
            {
                string columnDesc = "";

                if (item == "UserId") columnDesc = "UserId int(10) unsigned DEFAULT NULL AFTER MovNumber";
                if (item == "DateRecorded") columnDesc = "DateRecorded datetime default null AFTER UserId";


                string sql = $@"ALTER TABLE {schema}.emptranmovement ADD COLUMN {columnDesc}";
                await _sql.ExecuteCmd<dynamic>(sql, new { }, conn);
            }
        }


    }

    private async Task _01Para(string schema, string conn)
    {
        string sql = $@"CREATE TABLE IF NOT EXISTS {schema}.Para (
                        Id      INT     UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
                        Year    CHAR(2) DEFAULT NULL,
                        Month   CHAR(2) DEFAULT NULL
                    ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";

        await _sql.ExecuteCmd(sql, new { }, conn);
    }




    private async Task _01LanguageSpoken(string schema, string conn)
    {
        string sql = $@"CREATE TABLE if not exists {schema}.LanguageSpoken (
                        Id      Int         unsigned NOT NULL AUTO_INCREMENT,
                        Name    varchar(45)             Default NULL,
                        Level   Int         unsigned    Default  NULL COMMENT '1 - Knowledgeable, 2 - Fluent, 3 - Expert',
                        PRIMARY KEY (`Id`)) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);
    }



}
