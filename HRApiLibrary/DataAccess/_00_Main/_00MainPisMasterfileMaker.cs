using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._00_Main;

// -- MainPis masterfile tables : copied into each new {Code}_OPis by OldPis_TblMakerDataAccess._01OldPisDefaults --
// -- Structure and rows from the reference old PIS (192.168.0.103 secpis), plus the generic positions / designations --
// -- Rows are added only when a table is empty, so it is safe to run again ------------------------------------------
public class _00MainPisMasterfileMaker : I_00MainPisMasterfileMaker
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00MainPisMasterfileMaker(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01MainPisMasterfile(string? schema = "MainPis", string? connName = "MySqlConn")
    {
        await _sql.ExecuteCmd($"CREATE SCHEMA IF NOT EXISTS {schema} DEFAULT CHARACTER SET latin1;", new { }, connName);

        await _01Sex(schema, connName);
        await _01Civstat(schema, connName);
        await _01Empstat(schema, connName);
        await _01Clientstatus(schema, connName);
        await _01Licverstatus(schema, connName);
        await _01Insurancestatus(schema, connName);
        await _01Empremarksstatus(schema, connName);
        await _01Dpastatusincluded(schema, connName);
        await _01EmpstatStatmgt(schema, connName);
        await _01EmpmasNbiClearanceEmpstat(schema, connName);
        await _01MovehdrStatus(schema, connName);
        await _01Employmentmode(schema, connName);
        await _01Empremarkscategory(schema, connName);
        await _01Empcompliancecategory(schema, connName);
        await _01Empcompliancelist(schema, connName);
        await _01College(schema, connName);
        await _01Traincategory(schema, connName);
        await _01Trainprogram(schema, connName);
        await _01Skills(schema, connName);
        await _01Regions(schema, connName);
        await _01Area(schema, connName);
        await _01Position(schema, connName);
        await _01Designation(schema, connName);
        await _01Levtbl(schema, connName);
        await _01Devdata(schema, connName);
    }

    // -- Insert the default rows when the table has none; an Id of 0 stays 0 (some masterfiles use it) --
    private async Task SeedIfEmpty(string? schema, string table, string values, string? conn)
    {
        var rows = await _sql.FetchData<long, dynamic>($"select count(*) from {schema}.`{table}`", new { }, conn);
        if (rows.FirstOrDefault() > 0) return;

        string mode = "CONCAT_WS(',', TRIM(BOTH ',' FROM REPLACE(REPLACE(@@SESSION.sql_mode, 'NO_ZERO_IN_DATE', ''), 'NO_ZERO_DATE', '')), 'NO_AUTO_VALUE_ON_ZERO')";
        await _sql.ExecuteCmd($@"SET SESSION sql_mode = {mode};
                                 INSERT INTO {schema}.`{table}` {values};
                                 SET SESSION sql_mode = @@GLOBAL.sql_mode;", new { }, conn);
    }


    private async Task _01Sex(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sex` (
                        `CODE` varchar(1) DEFAULT NULL,
                        `NAME` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "sex", @"VALUES ('F','Female    '),('M','Male      ')", conn);
    }

    private async Task _01Civstat(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`civstat` (
                        `CODE` varchar(1) DEFAULT NULL,
                        `NAME` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "civstat", @"VALUES ('S','Single    '),('M','Married   '),('W','Widow/er  '),('T','Separated '),('P','S/Parent  ')", conn);
    }

    private async Task _01Empstat(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat` (
                        `CODE` varchar(1) NOT NULL,
                        `NAME` varchar(30) DEFAULT NULL,
                        `ISRESIGNED` varchar(1) DEFAULT NULL,
                        `isonleaved` varchar(1) DEFAULT '0',
                        `isfloating` varchar(1) DEFAULT '0',
                        `issuspended` varchar(1) DEFAULT '0',
                        `isInPayroll` int unsigned NOT NULL DEFAULT '0',
                        `inLicVer` int DEFAULT '0',
                        `inOE` int unsigned DEFAULT '1',
                        `isDeviation` smallint unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`CODE`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empstat", @"VALUES (' ','RTU','0','0','0','0',1,0,1,0),('0','For Deployment','0','0','0','0',0,0,1,0),('1','Resigned - Pending Clearance','1','0','0','0',0,0,1,0),('2','Backout','0','0','0','0',0,0,0,0),('3','Pending Quit Claim','1','0','0','0',0,0,1,0),('4','Negative Quit Claim','1','0','0','0',0,0,1,0),('A','Active         ','0','0','0','0',1,1,1,0),('B','Floating ','0','0','1','0',1,1,1,0),('C','Clearancing    ','0','0','0','0',0,0,1,0),('D','Deceased       ','1','0','0','0',0,0,0,0),('E','Emergency Leave','0','1','0','0',1,1,1,0),('F','Transferred','0','0','0','0',0,0,1,0),('G','Escort         ','0','0','0','0',0,0,1,0),('H','Suspended      ','0','0','0','1',1,0,1,1),('I','Training','0','0','0','0',0,0,1,0),('J','Back out       ','1','0','0','0',0,0,0,0),('K','Continuous Sevice','0','0','0','0',0,0,1,0),('L','Sick Leave     ','0','1','0','0',1,1,1,0),('M','Maternity Leave','0','1','0','0',0,1,1,0),('N','On process     ','0','0','0','0',0,0,1,0),('O','Dormant','0','0','0','0',0,0,1,0),('R','Resigned       ','1','0','0','0',0,0,1,0),('S','Staff          ','0','0','0','0',0,0,1,0),('T','Terminated     ','1','0','0','0',0,0,1,1),('U','Resigned - Under Clearance','1','0','0','0',1,0,1,0),('V','Vacation Leave ','0','1','0','0',1,1,1,0),('W','AWOL           ','1','0','0','0',0,0,1,0),('X','Labor          ','1','0','0','0',0,0,1,0),('Y','Suspended & Relieved','0','0','0','1',0,0,1,1),('Z','Retired','1','0','0','0',0,0,1,0),('_','Newly Hired','0','0','0','0',0,1,1,0)", conn);
    }

    private async Task _01Clientstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`clientstatus` (
                        `ClStatus` varchar(15) NOT NULL,
                        PRIMARY KEY (`ClStatus`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "clientstatus", @"VALUES ('Active'),('Inactive'),('Terminated')", conn);
    }

    private async Task _01Licverstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`licverstatus` (
                        `licverStatus` char(15) NOT NULL,
                        PRIMARY KEY (`licverStatus`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "licverstatus", @"VALUES ('ForVerification'),('Genuine'),('No Record'),('Rec. Exprd')", conn);
    }

    private async Task _01Insurancestatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`insurancestatus` (
                        `code` char(2) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "insurancestatus", @"VALUES (''),('A'),('B'),('E'),('H'),('I'),('L'),('M'),('S'),('V')", conn);
    }

    private async Task _01Empremarksstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empremarksstatus` (
                        `idempRemarksStatus` int unsigned NOT NULL AUTO_INCREMENT,
                        `RemStatus` varchar(45) NOT NULL,
                        PRIMARY KEY (`idempRemarksStatus`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empremarksstatus", @"VALUES (1,'Active'),(2,'Resolved')", conn);
    }

    private async Task _01Dpastatusincluded(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dpastatusincluded` (
                        `dpaStatusIncluded` char(2) NOT NULL DEFAULT '',
                        PRIMARY KEY (`dpaStatusIncluded`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "dpastatusincluded", @"VALUES ('2'),('A'),('C'),('J'),('W')", conn);
    }

    private async Task _01EmpstatStatmgt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat_statmgt` (
                        `code` varchar(1) NOT NULL,
                        `name` varchar(30) DEFAULT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empstat_statmgt", @"VALUES ('',''),('0','For Deployment'),('1','Pending4Clearan'),('2','Backout'),('3','Pending Quit Claim'),('4','Negative Quit Claim'),('D','Deceased       '),('F','Transferred'),('K','Continuous Sevice'),('O','Dormant'),('R','Resigned       '),('T','Terminated'),('U','Under Clearance'),('W','AWOL           '),('Y','Suspended & Relieved'),('Z','Retired')", conn);
    }

    private async Task _01EmpmasNbiClearanceEmpstat(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmas_nbi_clearance_empstat` (
                        `code` char(1) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empmas_nbi_clearance_empstat", @"VALUES ('A'),('B'),('F'),('O')", conn);
    }

    private async Task _01MovehdrStatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`movehdr_status` (
                        `Code` char(1) NOT NULL,
                        PRIMARY KEY (`Code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "movehdr_status", @"VALUES (''),('0'),('1'),('A'),('B'),('E'),('F'),('H'),('K'),('L'),('M'),('O'),('U'),('V'),('Y'),('_')", conn);
    }

    private async Task _01Employmentmode(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`employmentmode` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(60) NOT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=66 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "employmentmode", @"VALUES (1,'Direct Application'),(2,'Referral by a Friend'),(3,'Recruitment Agencies'),(4,'Job Fairs'),(5,'Online Job Boards'),(6,'Company Websites'),(7,'Social Media'),(8,'Internships'),(9,'Freelancing'),(10,'Temp Agencies'),(11,'Government Job Centers'),(12,'Networking'),(13,'Cold Calling'),(14,'Volunteering'),(15,'Job Referral Programs'),(16,'Alumni Networks'),(17,'Professional Associations'),(18,'Career Counseling Centers'),(19,'Company Events'),(20,'Trade Shows'),(21,'Job Search Apps'),(22,'Company Newsletters'),(23,'Cold Emails'),(24,'Industry-Specific Websites'),(25,'Freelance Platforms'),(26,'Job Alerts'),(27,'Company Research'),(28,'Consulting Firms'),(29,'Remote Work Platforms'),(30,'Job Shadowing'),(31,'Company Internship Programs'),(32,'Freelance Marketplaces'),(33,'Job Placement Agencies'),(34,'LinkedIn Networking'),(35,'Company Conferences'),(36,'Job Search Engines'),(37,'Company Open Houses'),(38,'Job Coaches'),(39,'Temporary-to-Permanent Roles'),(40,'Job Portals'),(41,'Company Intranets'),(42,'Professional Certifications'),(43,'Job Aggregators'),(44,'Job Application Tracking Systems'),(45,'Freelance Networking Events'),(46,'Company Alumni Networks'),(47,'Job Search Clubs'),(48,'Company Blogs'),(49,'Industry Forums'),(50,'Company Talent Pools'),(51,'Job Search Consultants'),(52,'Company Webinars'),(53,'Job Search Seminars'),(54,'Company Diversity Programs'),(55,'Job Search Podcasts'),(56,'Company Employee Referral Bonuses'),(57,'Job Search '),(58,'Company Initiatives'),(59,'Absorbed'),(60,'Facebook'),(61,'Online Advertisement'),(62,'Recommended'),(63,'Referred'),(64,'Train Now, Pay Later Program'),(65,'Walk in')", conn);
    }

    private async Task _01Empremarkscategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empremarkscategory` (
                        `idempRemarksCategory` int unsigned NOT NULL AUTO_INCREMENT,
                        `category` varchar(45) NOT NULL,
                        PRIMARY KEY (`idempRemarksCategory`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empremarkscategory", @"VALUES (1,'Awards'),(2,'Violation'),(3,'Expiration'),(4,'Security Issue'),(5,'Clearance Issue'),(6,'Sample                                       '),(7,'Medical Result                           '),(8,'Neuro Result                             '),(9,'Not Accredited Center (Neuro)            '),(10,'Not Accredited Center (Drug Test)        '),(11,'Declared as Fit but incomplete documents '),(12,'No fit-to-work remarks & Incomplete doc. '),(14,'Complete doc. but no fit-to-work remark      '),(15,'Not Accredited Center (Medical)          ')", conn);
    }

    private async Task _01Empcompliancecategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empcompliancecategory` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(60) DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empcompliancecategory", @"VALUES (1,'Security Officer'),(2,'CCTV Operator'),(3,'Security Guard'),(4,'Lady Guard'),(5,'Security Driver'),(6,'Office Personnel'),(7,'Admin Nurse')", conn);
    }

    private async Task _01Empcompliancelist(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empcompliancelist` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `EmpComplianceCategoryId` int unsigned DEFAULT '0',
                        `Name` varchar(45) DEFAULT ' ',
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "empcompliancelist", @"VALUES (0,0,''),(1,0,'Applicant Tracking Form'),(2,0,'Written Examination'),(3,0,'Interview Form'),(4,0,'Employment Contract'),(5,0,'PVC ID'),(6,0,'Orientation Attendance'),(7,0,'Consent Form'),(8,0,'Non Disclosure Agreement'),(9,0,'Training Passport'),(10,0,'H&R Requirements                             '),(11,0,'ATM Application'),(12,0,'NBI Verification'),(13,0,'License Verification'),(14,0,'Approval Letter                              '),(15,0,'Orientation Exam                             ')", conn);
    }

    private async Task _01College(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`college` (
                        `CODE` varchar(8) DEFAULT NULL,
                        `NAME` varchar(15) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "college", @"VALUES ('1st year','1st yr college '),('2nd year','2nd yr college '),('3rd year','3rd yr college '),('4th year','4th yr college '),('5th year','5th yr college '),('Graduate','Coll Graduate  '),('Technica','Vocational     ')", conn);
    }

    private async Task _01Traincategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`traincategory` (
                        `idtraincategory` int unsigned NOT NULL AUTO_INCREMENT,
                        `category` varchar(80) NOT NULL,
                        PRIMARY KEY (`idtraincategory`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "traincategory", @"VALUES (1,'Specialized'),(2,'Re-training Program')", conn);
    }

    private async Task _01Trainprogram(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trainprogram` (
                        `idtrainProgram` int unsigned NOT NULL AUTO_INCREMENT,
                        `Program` varchar(100) NOT NULL,
                        PRIMARY KEY (`idtrainProgram`)
                        ) ENGINE=InnoDB AUTO_INCREMENT=425 DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "trainprogram", @"VALUES (1,'Basic and Advance ROTC'),(2,'Basic Gun Handling'),(3,'Basic Language Skill & Communication'),(4,'Basic Language Skills'),(6,'Basic Supervisory Course'),(10,'Bomb Threat'),(12,'CDP'),(15,'Civil Disturvance & Crowd Control'),(17,'Fire Prevention'),(19,'First Aid Training'),(20,'Freght Security Escort'),(24,'Gun Safety & Hadling With Live Firing'),(25,'Gun Safety & Handling'),(68,'In-Service Traning Course'),(79,'ISO 14001'),(80,'ISO 18001'),(82,'ISO 9001:2000 Awareness'),(83,'ISO 9001:2000 Internal Quality'),(84,'Leadership Award'),(85,'Motivational Techniques'),(89,'Pre-Licensing Course'),(94,'Private Security Supervisory Course'),(123,'Proper Handling of Radio Communication'),(290,'Safety Induction Program'),(291,'Safety Procedures'),(293,'Scty Services NC I-Basic Scty Guard Course'),(298,'Security Investigation'),(415,'SK Leadership'),(416,'Supervisory Training Course'),(417,'TASIC for Kaligtasan'),(418,'Traffic Management Seminar'),(423,'Unleash Training Seminar'),(424,'Youth Development Program')", conn);
    }

    private async Task _01Skills(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`skills` (
                        `CODE` varchar(2) DEFAULT NULL,
                        `NAME` varchar(20) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "skills", @"VALUES ('DR','DRIVING             '),('CL','COMPUTER LITERATE   '),('TY','TYPING              '),('WL','WELDER              '),('EL','ELECTRICIAN         '),('CK','COOKING             '),('EC','ELECTRONICS         '),('DW','DRAWING/SKETCHING   '),('PT','PAINTING            '),('PO','PORKLIFT OPERATOR   '),('HA','HANDICRAFT          '),('AM','AUTO MECHANIC       '),('MA','MARTIAL ARTS        '),('CA','CARPENTER           '),('RA','REF. AIRCON TECHN.  '),('LT','LETTERING           '),('ME','MECHANIC            '),('SW','SEWING              '),('PL','PLUMBING            '),('DF','DRAFTING            '),('CM','COSMETOLOGY         '),('MY','MASONRY             '),('PR','PAINTER             '),('SW','SWIMMING            '),('RO','RADIO OPERATOR      ')", conn);
    }

    private async Task _01Regions(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`regions` (
                        `code` char(10) NOT NULL,
                        `name` varchar(60) NOT NULL,
                        `odr` int NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "regions", @"VALUES ('BARMM','Bangsamoro Autonomous Region in Muslim Mindanao',3),('CAR','Cordillera Administrative Region',1),('I','Ilocos Region',4),('II','Cagayan Valley',5),('III','Central Luzon',6),('IV-A','CALABARZON',7),('IV-B','MIMAROPA',8),('IX','Zamboanga Peninsula',13),('NCR','National Capital Region',2),('V','Bicol Region',9),('VI','Western Visayas',10),('VII','Central Visayas',11),('VIII','Eastern Visayas',12),('X','Northern Mindanao',14),('XI','Davao Region',15),('XII','SOCCSKARGEN',16),('XIII','Caraga',17)", conn);
    }

    private async Task _01Area(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`area` (
                        `AREACODE` varchar(5) NOT NULL,
                        `AREANAME` varchar(20) DEFAULT NULL,
                        PRIMARY KEY (`AREACODE`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "area", @"VALUES ('00010','MAKATI CITY         '),('00020','MANDALUYONG CITY    '),('00030','PASIG CITY          '),('00040','VALENZUELA CITY     '),('00050','QUEZON CITY         '),('00060','MANILA CITY         '),('00070','PARANAQUE CITY      '),('00080','LAGUNA              '),('00090','CAVITE CITY         '),('00100','PASAY CITY          '),('00110','BULACAN             '),('00120','SAN JUAN M.M.       '),('00130','BATANGAS CITY       '),('00140','QUEZON PROVINCE     '),('00150','ANTIPOLO CITY       '),('00160','LIPA CITY           '),('00170','RIZAL               '),('00180','ORTIGAS             '),('00190','TAGAYTAY CITY       '),('00200','BATAAN              '),('00201','MUNTINLUPA CITY'),('00202','PAMPANGA'),('00203','TAGUIG CITY'),('00204','PANGASINAN'),('00205','LAS PIÑAS CITY'),('00206','NUEVA ECIJA'),('00207','ALBAY'),('00208','Rizal'),('00209','ZAMBALES'),('00210','TARLAC'),('04301','Lucena City'),('CAG','CAGAYAN VALLEY'),('DAVAO','DAVAO'),('ILO','ILO-ILO CITY'),('ILOCO','ILOCOS NORTE'),('MINDA','MINDANAO'),('NEGOC','NEGROS OCCIDENTAL'),('NEGOR','NEGROS ORIENTAL'),('PALAW','PALAWAN')", conn);
    }

    private async Task _01Position(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`position` (
                        `CODE` varchar(5) NOT NULL,
                        `NAME` varchar(25) DEFAULT NULL,
                        `ISGUARD` varchar(1) DEFAULT NULL,
                        `sort` varchar(2) NOT NULL DEFAULT '99',
                        PRIMARY KEY (`CODE`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "position", @"VALUES ('     ','               ','0','99'),('AA','Admin Asst.','0','99'),('ACCA','Accounting Assistant','0','99'),('ACCT','Accountant','0','99'),('ACCTE','Account Executive','0','99'),('ACMGR','Accounting Manager','0','99'),('ADC  ','Assist DC      ','1','99'),('ADMMG','Admin Manager','0','99'),('ADMNA','Admin Assistant','0','99'),('ADMO','Admin Officer','0','99'),('AGM','Asst. General Manager','0','99'),('ARCH','Architect','0','99'),('AS','Account Supervisor','0','99'),('ASUP','Assistant Supervisor','0','99'),('AUD','Internal Auditor','0','99'),('BAR','Barista','0','99'),('BCA','Billing & Collection Assi','0','99'),('BCO','Billing & Collection Offi','0','99'),('BILL','Billing Clerk','0','99'),('BKPR','Bookkeeper','0','99'),('BO','Benefits Officer','0','99'),('C','Cashier','0','99'),('CARP','Carpenter','0','99'),('CBO','Comp. & Benefits Officer','0','99'),('CCTV ','CCTV Operator  ','1','99'),('CE','Civil Engineer','0','99'),('CEO','Chief Executive Officer','0','99'),('CFO','Chief Financial Officer','0','99'),('CHEF','Chef','0','99'),('CLERK','Clerk','0','99'),('COMPL','Compliance Officer','0','99'),('CONS','Consultant','0','99'),('COO','Chief Operating Officer','0','99'),('COOK','Cook','0','99'),('CRDTO','Credit & Collection Ofcr','0','99'),('CREW','Service Crew','0','99'),('CSR','Customer Service Rep.','0','99'),('DBA','Database Administrator','0','99'),('DC   ','Detachment Comm','1','1'),('DEPHD','Department Head','0','99'),('DEV','Software Developer','0','99'),('DIR','Director','0','99'),('DISP','Dispatcher','0','99'),('DOC','Document Controller','0','99'),('DOO','Deputy Operation Officer','0','99'),('DR   ','Driver         ','1','99'),('DRAFT','Draftsman','0','99'),('DS','Disbursement Staff','0','99'),('EA','Executive Assistant','0','99'),('EE','Electrical Engineer','0','99'),('ELEC','Electrician','0','99'),('ENC','Encoder','0','99'),('ENGR','Engineer','0','99'),('ER','Employee Relations Ofcr','0','99'),('FC','Finance Comptroller','0','99'),('FDO','Front Desk Officer','0','99'),('FI','Field Inspectors','0','99'),('FLT','Forklift Operator','0','99'),('FMAN','Foreman','0','99'),('GC','Guidance Councilor','0','99'),('GM','General Manager','0','99'),('GRAPH','Graphic Artist','0','99'),('HELP','Helper','0','99'),('HG   ','Head Guard     ','1','2'),('HK','Housekeeper','0','99'),('HRA','HR Assistant','0','99'),('HRMGR','HR Manager','0','99'),('HRO','HR Officer','0','99'),('HRSUP','HR Supervisor','0','99'),('INSP','Field Inspector','0','99'),('INTRN','Intern / OJT','0','99'),('INV','Inventory Controller','0','99'),('IT','IT','0','99'),('ITMGR','IT Manager','0','99'),('ITS','IT Support Technician','0','99'),('JACCT','Junior Accountant','0','99'),('JAN','Janitor','0','99'),('LA','Liason Assistant','0','99'),('LAB','Laborer','0','99'),('LEGAL','Legal Officer','0','99'),('LG   ','Lady Guard     ','1','99'),('LO','Logistic Assistant','0','99'),('MAINT','Maintenance Technician','0','99'),('MASON','Mason','0','99'),('MD','Company Physician','0','99'),('ME','Mechanical Engineer','0','99'),('MECH','Mechanic','0','99'),('MERCH','Merchandiser','0','99'),('MGR  ','Manager        ','0','99'),('MKMGR','Marketing Manager','0','99'),('MKTG','Marketing Officer','0','99'),('MR','Management Rep.','0','99'),('MSG','Messenger / Liason','0','99'),('NETAD','Network Administrator','0','99'),('NR','Admin Nurse','0','99'),('NURSE','Company Nurse','0','99'),('OIC  ','Officer in Char','1','3'),('OPMGR','Operations Manager','0','99'),('OPR','Machine Operator','0','99'),('OS','Operations Supervisor','0','99'),('PASST','Payroll Asistant','0','99'),('PAYO','Payroll Officer','0','99'),('PLANR','Production Planner','0','99'),('PLMB','Plumber','0','99'),('PM','Personnel Manager','0','99'),('PMAS','Payroll Master','0','99'),('PMGR','Production Manager','0','99'),('PRES','President','0','99'),('PROB','Probationary Staff','0','99'),('PROG','Programmer','0','99'),('PROJE','Project Engineer','0','99'),('PROJM','Project Manager','0','99'),('PSUP','Production Supervisor','0','99'),('PUASS','Purchasing Assistant','0','99'),('PURCH','Purchasing Officer','0','99'),('PW','Production Worker','0','99'),('QA','QA Officer','0','99'),('QC','QC Inspector','0','99'),('RE   ','Receptionist   ','1','99'),('RECRU','Recruitment Specialist','0','99'),('RIDER','Delivery Rider','0','99'),('SA','Service Auditor','0','99'),('SAFE','Safety Officer','0','99'),('SAGT','Sales Agent','0','99'),('SCLK','Sales Clerk','0','99'),('SD','Security Driver','1','99'),('SE','VIP Security Escort','0','99'),('SEC','Secretary','0','99'),('SG   ','Security Guard ','1','99'),('SIC  ','Shift-In-Charge','1','99'),('SMGR','Sales Manager','0','99'),('SO   ','Security Offcr ','1','4'),('SP   ','Special Project','0','99'),('SREP','Sales Representative','0','99'),('SSUP','Sales Supervisor','0','99'),('STF  ','Staff          ','0','99'),('STMGR','Store Manager','0','99'),('STOCK','Stock Clerk','0','99'),('SUP','Supervisor','0','99'),('SYSAD','System Administrator','0','99'),('TAX','Tax Specialist','0','99'),('TECH','Technician','0','99'),('TL','Team Leader','0','99'),('TO','Telephone Operator','0','99'),('TOE','Training Officer (Externa','0','99'),('TOI','Training Officer (Interna','0','99'),('TRAIN','Trainee','0','99'),('TREAS','Treasury Officer','0','99'),('TRS','Training Specialist','0','99'),('UTIL','Utility Worker','0','99'),('VP','Vice President','0','99'),('VPFIN','Vice President for Financ','0','99'),('WAIT','Waiter','0','99'),('WEB','Web Developer','0','99'),('WELD','Welder','0','99'),('WHM','Warehouseman','0','99'),('WHMGR','Warehouse Manager','0','99'),('WHSUP','Warehouse Supervisor','0','99'),('WRIT','Content Writer','0','99')", conn);
    }

    private async Task _01Designation(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`designation` (
                        `Code` char(5) NOT NULL,
                        `Name` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`Code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "designation", @"VALUES ('ADC','Assistant Detachment Commander'),('ADV','Adviser'),('AMGR','Assistant Manager'),('CONS','Consultant'),('CONSL','Consultant (Retainer)'),('CONTR','Contractual'),('CREWL','Crew Leader'),('DC','Detachment Commander'),('EMP','Employee'),('EXEC','Executive'),('FMAN','Foreman'),('HEAD','Department Head'),('HG','Head Guard'),('INTRN','Intern / OJT'),('JRSTF','Junior Staff'),('LEAD','Lead'),('LEADM','Leadman'),('LG','Lady Guard'),('MGR','Manager'),('OIC','Officer-In-Charge'),('PIC','Person-In-Charge'),('PROB','Probationary'),('PROJ','Project-Based'),('RF','Rank and File'),('SEAS','Seasonal'),('SG','Security Guard'),('SHL','Shift Leader'),('SIC','Shift-In-Charge'),('SPEC','Specialist'),('SRMGR','Senior Manager'),('SRSTF','Senior Staff'),('SRSUP','Senior Supervisor'),('STF','Staff'),('SUP','Supervisor'),('TL','Team Leader'),('TRN','Trainee')", conn);
    }

    private async Task _01Levtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`levtbl` (
                        `LVCODE` varchar(2) DEFAULT NULL,
                        `LVNAME` varchar(20) DEFAULT NULL,
                        `EARNCODE` varchar(2) DEFAULT NULL,
                        `NO_INCDATE` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "levtbl", @"VALUES ('VL','Vacation Leave      ','  ','0'),('SL','Sick Leave          ','  ','0'),('IL','Incentive Leave     ','  ','0'),('ML','Maternity Leave     ','  ','0'),('PL','Paternity Leave     ','  ','0'),('EL','Emergency Leave     ','  ','0')", conn);
    }

    private async Task _01Devdata(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`devdata` (
                        `DEV_NO` varchar(10) NOT NULL DEFAULT '-',
                        `DEV_NAME` varchar(40) DEFAULT NULL,
                        `DEV_TYPE` varchar(1) DEFAULT NULL,
                        `DEV_LEVEL` varchar(1) DEFAULT NULL,
                        `DEV_PARENT` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await _sql.ExecuteCmd(sql, new { }, conn);

        await SeedIfEmpty(schema, "devdata", @"VALUES ('1         ','AGAINST PERSONAL/COMPANY PROPERTY       ','G','1','          '),('101       ','AP - 001 PROVOKING A FIGHT              ','D','2','1         '),('102       ','AP - 002 TRREATENING                    ','D','2','1         '),('103       ','AP - 003 CAUSING INJURY                 ','D','2','3         '),('111       ','ACP - 001 ABANDONING POST               ','D','2','3         '),('112       ','ACP - 002 NEGLIGENCE                    ','D','2','3         '),('113       ','ACP - 003 LOSING OR ALLOWING DAMAGE     ','D','2','3         '),('114       ','ACP - 004 LOSING DAMAGE NEGLIGENCE      ','G','2','3         '),('115       ','ACP - 005 DAMAGING CLIENT OR AGENCY     ','D','2','3         '),('116       ','ACP - 006 STEALING                      ','D','2','3         '),('117       ','ACP - 007 MALVERSATION                  ','D','2','3         '),('118       ','ACP - 008 DEFACING COMPANY ISSUED ID    ','D','2','3         '),('12        ','AGAINST FIREARM LAW                     ','G','2','1         '),('143       ','AFL - 001 INDISCRIMINATE FIRING         ','D','3','12        '),('144       ','AFL - 002 CRRYING F/A OUTSIDE PRIMISES  ','D','3','12        '),('151       ','AFL - 003 LENDING OF FIRE ARMS          ','D','3','12        '),('152       ','AFL - 004 CARRYING DEADLY WEAPON PRIMISE','D','3','12        '),('153       ','AFL - 005 LOSING/DAMAGE F/A UNJUSTIFIABL','D','3','12        '),('2         ','AGAINTS COMPANY INTEREST (ACI)          ','G','1','          '),('201       ','ACI - 001 ABANDONING POST               ','D','2','2         '),('202       ','ACI - 002  SLEEPING WHILE ON DUTY       ','D','2','2         '),('203       ','ACI - 003 NOT WEARING UNIFORM           ','D','2','2         '),('204       ','ACI - 004 COMMITTING A CRIME            ','D','2','2         '),('205       ','ACI - 005 TARDINESS                     ','D','2','2         '),('206       ','ACI - 006  AWOL                         ','D','1','          '),('207       ','ACI - 007 POOR SERVICE                  ','D','2','2         '),('208       ','ACI - 008 ABSENTEEISM                   ','D','2','2         '),('209       ','ACI - 009 INSUBORDINATION               ','D','2','2         '),('210       ','ACI - 010 VIOLATION OF OFFICE MEMORANDUM','D','2','2         '),('211       ','ACI - 011 LAXITY                        ','D','2','2         '),('212       ','ACI - 012 FALSE TESTIMONY               ','D','2','2         '),('213       ','ACI - 013 ANOMALOUS ACTIVITY            ','D','2','2         '),('214       ','ACI - 014 PRINCIPAL OF ANOMALOUS ACT.   ','D','2','2         '),('215       ','ACI - 015 FRAUDULENT PUNCHING OF T/C    ','D','2','2         '),('216       ','ACI - 016 INSTIGATING MASS WALK         ','D','2','2         '),('217       ','ACI - 017 INSTIGATING A PETITION DRIVE  ','D','2','2         '),('218       ','ACI - 018 PARTICIPATING PETITION        ','D','2','2         '),('219       ','ACI - 019 FALSIFYING DOCUMENT           ','D','2','2         '),('220       ','ACI - 020 FALSEHOOD OR MISREPRESENTATION','D','2','2         '),('221       ','ACI - 021 ACCEPTING MONEY               ','D','2','2         '),('222       ','ACI - 022 OFFERING EXCHANGING 4 FAVOR   ','D','2','2         '),('223       ','ACI - 023 DISOBEYING PRESCRIBE HAIR CUT ','D','2','2         '),('224       ','ACI - 024 ENTERTAINING VISOTORS AT HOLID','D','2','2         '),('225       ','ACI - 025 GIVING CONFIDENTIAL INFO      ','D','2','2         '),('226       ','ACI - 026 MOONLIGHTING                  ','D','2','2         '),('227       ','ACI - 027 NOT REPORT OFFENSE OF OTHER   ','D','2','2         '),('228       ','ACI - 028 MALINGERING                   ','D','2','2         '),('229       ','ACI - 029 DISCOURTESY                   ','D','2','2         '),('230       ','ACI - 030 FAILURE TO SALUTE             ','D','2','2         '),('231       ','ACI - 031 OVER FAMILIARIZATION','D','2','2         '),('232       ','ACI - 032 NON ATTENDANCE IN TROP-FORMATI','D','2','2         '),('233       ','ACI - 033 IMPROPER/INCOMPLETE JOURNAL   ','D','2','2         '),('234       ','ACI - 034 PERSONAL LOAN ANY CONTRACTOR  ','D','2','2         '),('235       ','ACI - 035 FAILURE TO NOTIFY VIOLATIONS  ','D','2','2         '),('236       ','ACI - 036 REFUSAL TO SIGN VIOLATIONS    ','D','2','2         '),('237       ','ACI - 237 STRAIGHT DUTY W/O REASON      ','D','2','2         '),('238       ','ACI - 038 ASSUMING DUTY WITH EXPIRED SBR','D','2','2         '),('239       ','ACI - 039 DISLOYALTY                    ','D','2','2         '),('240       ','ACI - 040 NOT CARRYING ID/DDO/SBR       ','D','2','2         '),('241       ','ACI - 041 GRAVE OFFENSES                ','D','2','2         '),('3         ','AGAINST PUBLIC MORAL                    ','G','1','          '),('301       ','APM - 001 REEKING W/ LIQUOR             ','D','2','3         '),('302       ','APM - 002 POSITIVE- DRUG                ','D','2','3         '),('303       ','APM - 003 BRINGING DRUGS TO POST        ','D','2','3         '),('304       ','APM - 004 BRINGING ALCOHOLIC BEVERAGE   ','D','2','3         '),('305       ','APM - 005 USING INDECENT LANGUAGE       ','D','2','3         '),('306       ','APM - 006 GAMBLING IN AGENCY PREMISES   ','D','2','3         '),('307       ','APM - 007 CREATING UNSAFE CON TO CLIENT ','D','2','3         '),('308       ','APM - 008 EXHIBITING PORNOGRAPHIC MATERI','D','2','3         '),('309       ','APM - 009 UNALERT,SMOKING,TXTING ON POST','D','2','3         '),('310       ','APM - 010 CONDUCT SCANDAL               ','D','2','3         '),('311       ','APM - 011 ILLICIT LOVE AFFAIR           ','D','2','3         '),('312       ','APM - 012 IMMORALITY                    ','D','2','3         ')", conn);
    }
}
