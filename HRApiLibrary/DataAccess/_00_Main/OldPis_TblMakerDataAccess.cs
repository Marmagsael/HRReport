using System.Text.RegularExpressions;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._00_Main;

public class OldPis_TblMakerDataAccess : IOldPis_TblMakerDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OldPis_TblMakerDataAccess(I_90_001_MySqlDataAccess sql)
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

    // -- Masterfile tables whose rows are copied from MainPis into a new old PIS schema --
    private static readonly string[] DefaultTables =
    [
        "sex", "civstat", "empstat", "clientstatus", "licverstatus", "insurancestatus", "empremarksstatus",
        "dpastatusincluded", "empstat_statmgt", "empmas_nbi_clearance_empstat", "movehdr_status", "employmentmode",
        "empremarkscategory", "empcompliancecategory", "empcompliancelist", "college", "traincategory", "trainprogram",
        "skills", "regions", "area", "position", "designation", "levtbl", "devdata"
    ];

    // -- Default rows : MainPis -> {Code}_OPis, run after _01OldPisTable ------------------------------
    // -- Only empty tables are filled, and only the columns both tables have, so it is safe to run again --
    public async Task _01OldPisDefaults(string? schema, string? sourceSchema, string? connName = "MySqlConn")
    {
        if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(sourceSchema)) return;
        if (!Regex.IsMatch(schema, @"^\w+$") || !Regex.IsMatch(sourceSchema, @"^\w+$")) return;

        var columns = await _sql.FetchData<LegacyColumn, dynamic>(
            @"select lower(table_schema) as SchemaName, lower(table_name) as TableName, column_name as ColumnName
              from   information_schema.columns
              where  lower(table_schema) in (lower(@Schema), lower(@Source)) and lower(table_name) in @Tables
              order by ordinal_position",
            new { Schema = schema, Source = sourceSchema, Tables = DefaultTables }, connName);

        foreach (var table in DefaultTables)
        {
            var target  = columns.Where(c => c.SchemaName == schema.ToLower()       && c.TableName == table).Select(c => c.ColumnName).ToList();
            var source  = columns.Where(c => c.SchemaName == sourceSchema.ToLower() && c.TableName == table).Select(c => c.ColumnName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var shared  = target.Where(source.Contains).Select(c => $"`{c}`").ToList();
            if (shared.Count == 0) continue;                                            // table missing on either side

            var rows = await _sql.FetchData<long, dynamic>($"select count(*) from {schema}.`{table}`", new { }, connName);
            if (rows.FirstOrDefault() > 0) continue;                                    // already has data

            // -- Zero dates allowed, and an Id of 0 stays 0 (some masterfiles use it) --
            string cols = string.Join(", ", shared);
            string mode = "CONCAT_WS(',', TRIM(BOTH ',' FROM REPLACE(REPLACE(@@SESSION.sql_mode, 'NO_ZERO_IN_DATE', ''), 'NO_ZERO_DATE', '')), 'NO_AUTO_VALUE_ON_ZERO')";
            await _sql.ExecuteCmd($@"SET SESSION sql_mode = {mode};
                                     insert into {schema}.`{table}` ({cols}) select {cols} from {sourceSchema}.`{table}`;
                                     SET SESSION sql_mode = @@GLOBAL.sql_mode;", new { }, connName);
        }
    }

    private class LegacyColumn
    {
        public string SchemaName    { get; set; } = string.Empty;
        public string TableName     { get; set; } = string.Empty;
        public string ColumnName    { get; set; } = string.Empty;
    }

    public async Task _01OldPisTable(string? schema, string? connName = "MySqlConn")
    {
        await _sql.ExecuteCmd($"CREATE SCHEMA IF NOT EXISTS {schema} DEFAULT CHARACTER SET latin1;", new { }, connName);

        await _01Area(schema, connName);
        await _01Bi(schema, connName);
        await _01Children(schema, connName);
        await _01Civstat(schema, connName);
        await _01Client(schema, connName);
        await _01Clientarchieved(schema, connName);
        await _01Clientcontrep(schema, connName);
        await _01Clientstatus(schema, connName);
        await _01Coinfo(schema, connName);
        await _01College(schema, connName);
        await _01Compliancerec(schema, connName);
        await _01Counter(schema, connName);
        await _01Databags(schema, connName);
        await _01Datadict(schema, connName);
        await _01Dataords(schema, connName);
        await _01Deprec(schema, connName);
        await _01Designation(schema, connName);
        await _01Devdata(schema, connName);
        await _01Devnum(schema, connName);
        await _01Devtran(schema, connName);
        await _01Dpastatusincluded(schema, connName);
        await _01Drugtest(schema, connName);
        await _01Educate(schema, connName);
        await _01Emergenc(schema, connName);
        await _01Empcompliance(schema, connName);
        await _01Empcompliancecategory(schema, connName);
        await _01Empcompliancelist(schema, connName);
        await _01Empentryflds(schema, connName);
        await _01Employ(schema, connName);
        await _01Employmentmode(schema, connName);
        await _01Empmas(schema, connName);
        await _01EmpmasNbiClearance(schema, connName);
        await _01EmpmasNbiClearanceEmpstat(schema, connName);
        await _01Empmasaccess(schema, connName);
        await _01Empmasarchieved(schema, connName);
        await _01Empremarks(schema, connName);
        await _01Empremarkscategory(schema, connName);
        await _01Empremarksstatus(schema, connName);
        await _01Empstat(schema, connName);
        await _01EmpstatIn(schema, connName);
        await _01EmpstatOut(schema, connName);
        await _01EmpstatStatmgt(schema, connName);
        await _01EmpstatUserAccess(schema, connName);
        await _01Empstatforarchive(schema, connName);
        await _01Empstatmovement(schema, connName);
        await _01EmpstatmovementBak(schema, connName);
        await _01Family(schema, connName);
        await _01Filedata(schema, connName);
        await _01Floatnotice(schema, connName);
        await _01Freeuniform(schema, connName);
        await _01Govexam(schema, connName);
        await _01Grdbirth(schema, connName);
        await _01Idissuance(schema, connName);
        await _01Info(schema, connName);
        await _01Insurance(schema, connName);
        await _01InsuranceAccident(schema, connName);
        await _01Insurancestatus(schema, connName);
        await _01Language(schema, connName);
        await _01Lastrefused(schema, connName);
        await _01Leave1(schema, connName);
        await _01Levmonletter(schema, connName);
        await _01Levmonpara(schema, connName);
        await _01Levtbl(schema, connName);
        await _01Licverstatus(schema, connName);
        await _01Loghist(schema, connName);
        await _01Mdr(schema, connName);
        await _01MdrClient(schema, connName);
        await _01Mdrcaption(schema, connName);
        await _01Mdrfirearms(schema, connName);
        await _01Mdrhdr(schema, connName);
        await _01Menubars(schema, connName);
        await _01Mlacode(schema, connName);
        await _01Movedtl(schema, connName);
        await _01Movehdr(schema, connName);
        await _01MovehdrStatus(schema, connName);
        await _01MovehdrStatusTrail(schema, connName);
        await _01Neuroexm(schema, connName);
        await _01Notice(schema, connName);
        await _01Ofstaff(schema, connName);
        await _01Ofstaff2(schema, connName);
        await _01Ofstaff3(schema, connName);
        await _01Ofstaff4(schema, connName);
        await _01Para(schema, connName);
        await _01Parent(schema, connName);
        await _01Penalty(schema, connName);
        await _01Position(schema, connName);
        await _01Procode(schema, connName);
        await _01Qrynum(schema, connName);
        await _01Query(schema, connName);
        await _01Refer(schema, connName);
        await _01Regions(schema, connName);
        await _01Seclic(schema, connName);
        await _01SeclicSignatory(schema, connName);
        await _01Secliclogs(schema, connName);
        await _01Sex(schema, connName);
        await _01Skills(schema, connName);
        await _01Ssstbl(schema, connName);
        await _01Status(schema, connName);
        await _01Statusremarks(schema, connName);
        await _01Sysmenu(schema, connName);
        await _01Tbloperators(schema, connName);
        await _01Tblqueries(schema, connName);
        await _01Train(schema, connName);
        await _01Traincategory(schema, connName);
        await _01Traindtl(schema, connName);
        await _01Trainhdr(schema, connName);
        await _01Trainingconfig(schema, connName);
        await _01Trainprogram(schema, connName);
        await _01Traintransno(schema, connName);
        await _01Trainvaliduser(schema, connName);
        await _01TransCtr(schema, connName);
        await _01Useraccess(schema, connName);
        await _01Usr(schema, connName);
        await _01Usrmenu(schema, connName);
        await _01Usrrights(schema, connName);
        await _01Version(schema, connName);

        // -- Added from the reference (192.168.0.103 secpis) --
        await _01ClientRateSettings(schema, connName);
        await _01ClientRateatrail(schema, connName);
        await _01ClientRateatrailEmpmas(schema, connName);
        await _01Domainaccess(schema, connName);
        await _01Domainusr(schema, connName);
        await _01EmpmasVisibleStatus(schema, connName);
    }

    private async Task _01Area(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`area` (
                        `AREACODE` varchar(5) NOT NULL,
                        `AREANAME` varchar(20) DEFAULT NULL,
                        PRIMARY KEY (`AREACODE`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Bi(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`bi` (
                        `empnumber` char(5) NOT NULL,
                        `NB_DateConducted` date DEFAULT NULL,
                        `NB_DateEncoded` date DEFAULT NULL,
                        `NB_Feedback` int unsigned DEFAULT NULL,
                        `NB_PersonInterviewed` varchar(60) DEFAULT NULL,
                        `NB_Relation` varchar(45) DEFAULT NULL,
                        `NB_VerifiedBy` varchar(60) DEFAULT NULL,
                        `NB_DateVerified` date DEFAULT NULL,
                        `NB_Remarks` varchar(200) DEFAULT NULL,
                        `Brgy_DateConducted` date DEFAULT NULL,
                        `Brgy_DateEncoded` date DEFAULT NULL,
                        `Brgy_Record` int unsigned DEFAULT NULL,
                        `Brgy_PersonInterviewed` varchar(60) DEFAULT NULL,
                        `Brgy_PersonPostion` varchar(45) DEFAULT NULL,
                        `Brgy_DateVerified` date DEFAULT NULL,
                        `Brgy_VerifiedBy` varchar(60) DEFAULT NULL,
                        `Brgy_Remarks` varchar(200) DEFAULT NULL,
                        `Emp_DateConducted` date DEFAULT NULL,
                        `Emp_DateEncoded` date DEFAULT NULL,
                        `Emp_Accountability` int unsigned DEFAULT NULL,
                        `Emp_PersonInterviewed` varchar(60) DEFAULT NULL,
                        `Emp_PersonPostion` varchar(45) DEFAULT NULL,
                        `Emp_VerifiedBy` varchar(60) DEFAULT NULL,
                        `Emp_DateVerified` date DEFAULT NULL,
                        `Emp_Remarks` varchar(200) DEFAULT NULL,
                        `Sch_DateConducted` date DEFAULT NULL,
                        `Sch_Status` char(10) DEFAULT NULL,
                        `Sch_Course` varchar(80) DEFAULT NULL,
                        `Sch_LevelCompletion` int unsigned DEFAULT NULL,
                        `Sch_PersonInterviewed` varchar(60) DEFAULT NULL,
                        `Sch_PersonPostion` varchar(45) DEFAULT NULL,
                        `Sch_VerifiedBy` varchar(60) DEFAULT NULL,
                        `Sch_DateVerified` date DEFAULT NULL,
                        `Sch_Remarks` varchar(200) DEFAULT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Children(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`children` (
                        `empnumber` varchar(5) NOT NULL,
                        `name` varchar(60) NOT NULL,
                        `bday` date NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Civstat(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`civstat` (
                        `CODE` varchar(1) DEFAULT NULL,
                        `NAME` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Client(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`client` (
                        `CLNUMBER` varchar(5) NOT NULL,
                        `CLNAME` varchar(60) DEFAULT NULL,
                        `ADDR1` varchar(150) DEFAULT NULL,
                        `ADDR2` varchar(150) DEFAULT NULL,
                        `AREACODE` varchar(5) DEFAULT ' ',
                        `TEL1` varchar(10) DEFAULT NULL,
                        `FAXNO` varchar(10) DEFAULT NULL,
                        `PARENT` varchar(5) DEFAULT NULL,
                        `RATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `BILLRATE` double(9,2) DEFAULT NULL,
                        `ASSIST` double(6,2) DEFAULT NULL,
                        `STATUS` varchar(15) DEFAULT NULL,
                        `COLARATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `ND_RATE` double(5,3) DEFAULT NULL,
                        `RETIRATE` double(5,3) DEFAULT NULL,
                        `UNIFRATE` double(6,2) DEFAULT NULL,
                        `FDIRATE` double(5,3) DEFAULT NULL,
                        `OTRATE` double(6,2) DEFAULT NULL,
                        `TIN` varchar(15) DEFAULT NULL,
                        `CONT` varchar(1) DEFAULT NULL,
                        `USED` varchar(1) DEFAULT NULL,
                        `CONTACT` varchar(25) DEFAULT NULL,
                        `POSTPERIOD` varchar(8) DEFAULT NULL,
                        `BATCHX` varchar(2) DEFAULT NULL,
                        `FSSSEE` double(7,2) DEFAULT NULL,
                        `FSSSER` double(7,2) DEFAULT NULL,
                        `FECC` double(5,2) DEFAULT NULL,
                        `FMEDEE` double(7,2) DEFAULT NULL,
                        `FMEDER` double(7,2) DEFAULT NULL,
                        `Remarks` varchar(254) DEFAULT NULL,
                        `contStart` date NOT NULL DEFAULT '0000-00-00',
                        `contEnd` date NOT NULL DEFAULT '0000-00-00',
                        `parentcd` varchar(5) NOT NULL DEFAULT '',
                        `maxsss` double(10,4) NOT NULL DEFAULT '0.0000',
                        `maxphic` double(10,4) NOT NULL DEFAULT '0.0000',
                        `ContExp` date NOT NULL DEFAULT '0000-00-00',
                        `HavTax` double NOT NULL DEFAULT '1',
                        `MinRate` double(10,2) NOT NULL DEFAULT '0.00',
                        `MealAllow` double(6,2) NOT NULL DEFAULT '0.00',
                        `withUniform` tinyint unsigned NOT NULL DEFAULT '0',
                        `withRetirement` tinyint unsigned NOT NULL DEFAULT '0',
                        `region` varchar(15) DEFAULT 'NCR',
                        `ecolaRevised` int unsigned NOT NULL DEFAULT '0',
                        `ctpaRate` double(18,2) DEFAULT '0.00',
                        `withCTPA` int DEFAULT '0',
                        `seaRate` double(15,2) DEFAULT '0.00',
                        `withSEA` int DEFAULT '0',
                        `payprd` char(15) NOT NULL DEFAULT '',
                        `sgcode` char(10) NOT NULL DEFAULT '',
                        `isTrucking` int unsigned DEFAULT '0',
                        `isLumpsum` int unsigned DEFAULT '0',
                        `withSIL` int unsigned DEFAULT '1',
                        PRIMARY KEY (`CLNUMBER`)
                        ) ENGINE=MyISAM DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Clientarchieved(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`clientarchieved` (
                        `CLNUMBER` varchar(5) NOT NULL,
                        `CLNAME` varchar(60) DEFAULT NULL,
                        `ADDR1` varchar(60) DEFAULT NULL,
                        `ADDR2` varchar(60) DEFAULT NULL,
                        `AREACODE` varchar(5) DEFAULT NULL,
                        `TEL1` varchar(10) DEFAULT NULL,
                        `FAXNO` varchar(10) DEFAULT NULL,
                        `PARENT` varchar(5) DEFAULT NULL,
                        `RATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `BILLRATE` double(9,2) DEFAULT NULL,
                        `ASSIST` double(6,2) DEFAULT NULL,
                        `STATUS` varchar(15) DEFAULT NULL,
                        `COLARATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `ND_RATE` double(5,3) DEFAULT NULL,
                        `RETIRATE` double(5,3) DEFAULT NULL,
                        `UNIFRATE` double(6,2) DEFAULT NULL,
                        `FDIRATE` double(5,3) DEFAULT NULL,
                        `OTRATE` double(6,2) DEFAULT NULL,
                        `TIN` varchar(15) DEFAULT NULL,
                        `CONT` varchar(1) DEFAULT NULL,
                        `USED` varchar(1) DEFAULT NULL,
                        `CONTACT` varchar(25) DEFAULT NULL,
                        `POSTPERIOD` varchar(8) DEFAULT NULL,
                        `BATCHX` varchar(2) DEFAULT NULL,
                        `FSSSEE` double(7,2) DEFAULT NULL,
                        `FSSSER` double(7,2) DEFAULT NULL,
                        `FECC` double(5,2) DEFAULT NULL,
                        `FMEDEE` double(7,2) DEFAULT NULL,
                        `FMEDER` double(7,2) DEFAULT NULL,
                        `Remarks` varchar(254) DEFAULT NULL,
                        `contStart` date NOT NULL DEFAULT '0000-00-00',
                        `contEnd` date NOT NULL DEFAULT '0000-00-00',
                        `parentcd` varchar(5) NOT NULL DEFAULT '',
                        `maxsss` double(10,4) NOT NULL DEFAULT '0.0000',
                        `maxphic` double(10,4) NOT NULL DEFAULT '0.0000',
                        `ContExp` date NOT NULL DEFAULT '0000-00-00',
                        `HavTax` double NOT NULL DEFAULT '1',
                        `MinRate` double(10,2) NOT NULL DEFAULT '0.00',
                        `MealAllow` double(6,2) NOT NULL DEFAULT '0.00',
                        `withUniform` tinyint unsigned NOT NULL DEFAULT '0',
                        `withRetirement` tinyint unsigned NOT NULL DEFAULT '0',
                        `region` char(15) NOT NULL DEFAULT '',
                        `ecolaRevised` int unsigned NOT NULL DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Clientcontrep(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`clientcontrep` (
                        `clNumber` char(5) NOT NULL,
                        `Job` varchar(80) DEFAULT NULL,
                        `WorkerPos` varchar(120) DEFAULT NULL,
                        `MaleCnt` int unsigned DEFAULT NULL,
                        `FemaleCnt` int unsigned DEFAULT NULL,
                        `OtherCnt` int unsigned DEFAULT NULL,
                        PRIMARY KEY (`clNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Clientstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`clientstatus` (
                        `ClStatus` varchar(15) NOT NULL,
                        PRIMARY KEY (`ClStatus`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Coinfo(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`coinfo` (
                        `CoName` varchar(150) NOT NULL,
                        `CoAdd` varchar(300) NOT NULL,
                        `TelNo` varchar(100) NOT NULL,
                        `RegPeriod` int unsigned NOT NULL,
                        `CurrBasicRate` double(6,2) NOT NULL DEFAULT '0.00',
                        `min` double(10,4) NOT NULL DEFAULT '0.0000',
                        `SDOSource` varchar(100) NOT NULL DEFAULT '',
                        `SDOSaveTo` varchar(100) NOT NULL DEFAULT '',
                        `shortname` varchar(45) NOT NULL,
                        `schemapis` varchar(45) NOT NULL,
                        `schemapay` varchar(45) NOT NULL,
                        `schemaequip` varchar(45) NOT NULL,
                        `cologo` varchar(45) DEFAULT NULL,
                        `acctno` varchar(45) NOT NULL DEFAULT ' ',
                        `TIN` varchar(25) NOT NULL,
                        `SSS` varchar(25) NOT NULL,
                        `PHIC` varchar(25) NOT NULL,
                        `PAGIBIG` varchar(25) NOT NULL,
                        `sssMemType` int unsigned NOT NULL DEFAULT '1' COMMENT '1 - Private, 2 - Loca, 3-GCC, 4 - NGA',
                        `sssdocno` char(6) DEFAULT NULL,
                        `sssLocCode` char(3) DEFAULT NULL,
                        `schemaams` varchar(45) DEFAULT NULL,
                        `schemalumpsum` varchar(45) DEFAULT NULL,
                        `schematruc` varchar(45) DEFAULT NULL,
                        `schemaipay` varchar(45) DEFAULT NULL,
                        `isHeadOffice` tinyint unsigned DEFAULT '0',
                        PRIMARY KEY (`CoName`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01College(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`college` (
                        `CODE` varchar(8) DEFAULT NULL,
                        `NAME` varchar(15) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Compliancerec(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`compliancerec` (
                        `empnumber` char(5) NOT NULL,
                        `CategoryId` int unsigned DEFAULT NULL,
                        `EmploymentModeId` int unsigned DEFAULT NULL,
                        `Source` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Counter(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`counter` (
                        `idcounter` int unsigned NOT NULL AUTO_INCREMENT,
                        `keyFld` varchar(45) NOT NULL,
                        `ctr` int unsigned NOT NULL,
                        PRIMARY KEY (`idcounter`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Databags(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`databags` (
                        `BAGNAME` varchar(8) DEFAULT NULL,
                        `OBJECT` varchar(25) DEFAULT NULL,
                        `RDD` varchar(6) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Datadict(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`datadict` (
                        `FLD_NAME` varchar(10) DEFAULT NULL,
                        `FLD_TYPE` varchar(1) DEFAULT NULL,
                        `FLD_WIDTH` double(5,0) DEFAULT NULL,
                        `FLD_DEC` double(2,0) DEFAULT NULL,
                        `FLD_DESC` varchar(30) DEFAULT NULL,
                        `FLD_BRWMSG` varchar(60) DEFAULT NULL,
                        `HIDDEN` varchar(1) DEFAULT NULL,
                        `BAGNAME` varchar(8) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Dataords(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dataords` (
                        `BAGNAME` varchar(8) DEFAULT NULL,
                        `TAGNAME` varchar(8) DEFAULT NULL,
                        `TAGKEY` varchar(60) DEFAULT NULL,
                        `TAGDESCEND` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Deprec(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`deprec` (
                        `EmpNumber` varchar(5) NOT NULL,
                        `TranNumber` char(12) DEFAULT '',
                        `DivId` int unsigned DEFAULT '0',
                        `DepId` int unsigned DEFAULT '0',
                        `SecId` int unsigned DEFAULT '0',
                        `PositionId` int unsigned DEFAULT '0',
                        `LeavegrpId` int unsigned DEFAULT '0',
                        `PayrollgrpId` int unsigned DEFAULT '0',
                        `IdDeployment` int DEFAULT '0',
                        `EmploymentTypeId` int unsigned DEFAULT '0',
                        `EmpStatusId` int unsigned DEFAULT '15',
                        `DepDate` date DEFAULT NULL,
                        `DHired` date DEFAULT NULL,
                        `DRegularization` date DEFAULT NULL,
                        `DTraineeStart` date DEFAULT NULL,
                        `DTraineeEnd` date DEFAULT NULL,
                        `DContractualStart` date DEFAULT NULL,
                        `DContractualEnd` date DEFAULT NULL,
                        `DProbationaryStart` date DEFAULT NULL,
                        `DProbationaryEnd` date DEFAULT NULL,
                        `DRegularizationStart` date DEFAULT NULL,
                        `DRegularizationEnd` date DEFAULT NULL,
                        `DPermanentStart` date DEFAULT NULL,
                        `DResigned` date DEFAULT NULL,
                        `DTerminated` date DEFAULT NULL,
                        `DSeparated` date DEFAULT NULL,
                        `Remarks` varchar(100) DEFAULT NULL,
                        `IsOnDeviation` smallint DEFAULT '0',
                        `IdDeviation` int DEFAULT '0',
                        `IsOnDiciplinary` smallint DEFAULT '0',
                        `IsOnInvestigation` smallint DEFAULT '0',
                        `IdInvestigate` int DEFAULT '0',
                        PRIMARY KEY (`EmpNumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Designation(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`designation` (
                        `Code` char(5) NOT NULL,
                        `Name` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`Code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
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
        await ExecLegacy(sql, conn);
    }

    private async Task _01Devnum(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`devnum` (
                        `CONTROL_NO` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Devtran(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`devtran` (
                        `CONTROL_NO` varchar(10) DEFAULT NULL,
                        `PREP_BY` varchar(10) DEFAULT NULL,
                        `PREP_DT` date DEFAULT NULL,
                        `CLNUMBER` varchar(5) DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DEV_NO` varchar(10) DEFAULT NULL,
                        `OCCUR_DT` date DEFAULT NULL,
                        `FREQ_NO` varchar(1) DEFAULT NULL,
                        `PENALTY_NO` varchar(2) DEFAULT NULL,
                        `APPR_BY` varchar(10) DEFAULT NULL,
                        `APPR_DT` date DEFAULT NULL,
                        `devstart` date NOT NULL DEFAULT '0000-00-00',
                        `devend` date NOT NULL DEFAULT '0000-00-00'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Dpastatusincluded(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`dpastatusincluded` (
                        `dpaStatusIncluded` char(2) NOT NULL DEFAULT '',
                        PRIMARY KEY (`dpaStatusIncluded`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Drugtest(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`drugtest` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `DRUTAKEN` date DEFAULT NULL,
                        `DRUEXPIRE` date DEFAULT NULL,
                        `DRUCENTER` varchar(30) DEFAULT NULL,
                        `DRUREMARK` varchar(20) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Educate(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`educate` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `CODE` varchar(1) DEFAULT NULL,
                        `SCHOOL` varchar(30) DEFAULT NULL,
                        `FROM_` varchar(10) DEFAULT NULL,
                        `TO_` varchar(10) DEFAULT NULL,
                        `COURSE` varchar(75) DEFAULT NULL,
                        `LEVEL` varchar(8) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Emergenc(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`emergenc` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `NAME` varchar(25) DEFAULT NULL,
                        `ADDR` varchar(60) DEFAULT NULL,
                        `RELA` varchar(20) DEFAULT NULL,
                        `TEL` varchar(15) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empcompliance(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empcompliance` (
                        `Empnumber` char(5) NOT NULL,
                        `EmpComplianceListId` int unsigned NOT NULL,
                        `DateCompleted` date DEFAULT NULL,
                        `DateValidated` date DEFAULT NULL,
                        `RefNo` char(25) DEFAULT NULL,
                        `Status` char(10) DEFAULT NULL,
                        `Remarks` varchar(80) DEFAULT NULL,
                        `CompDeadline` date DEFAULT NULL,
                        PRIMARY KEY (`Empnumber`,`EmpComplianceListId`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empcompliancecategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empcompliancecategory` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(60) DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empcompliancelist(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empcompliancelist` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `EmpComplianceCategoryId` int unsigned DEFAULT '0',
                        `Name` varchar(45) DEFAULT ' ',
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empentryflds(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empentryflds` (
                        `grp` char(45) NOT NULL,
                        `fldname` char(80) NOT NULL,
                        `Description` char(100) NOT NULL,
                        PRIMARY KEY (`grp`,`fldname`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Employ(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`employ` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `COMP` varchar(60) DEFAULT NULL,
                        `ADDR1` varchar(60) DEFAULT NULL,
                        `ADDR2` varchar(60) DEFAULT NULL,
                        `TEL` varchar(15) DEFAULT NULL,
                        `POSI` varchar(75) DEFAULT NULL,
                        `FROM_` varchar(10) DEFAULT NULL,
                        `TO_` varchar(10) DEFAULT NULL,
                        `SAL` double(10,2) DEFAULT NULL,
                        `REM1` varchar(60) DEFAULT NULL,
                        `REM2` varchar(60) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Employmentmode(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`employmentmode` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(60) NOT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empmas(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmas` (
                        `EMPNUMBER` varchar(5) NOT NULL,
                        `EMPLASTNM` varchar(25) DEFAULT NULL,
                        `EMPFIRSTNM` varchar(30) DEFAULT NULL,
                        `EMPMIDNM` varchar(15) DEFAULT NULL,
                        `suffix` char(3) DEFAULT '',
                        `EMPALIAS` varchar(15) DEFAULT NULL,
                        `CLIENT` varchar(5) DEFAULT NULL,
                        `CLIENT_` varchar(5) DEFAULT NULL,
                        `BASICRATE` double(10,4) NOT NULL DEFAULT '0.0000',
                        `PAYTYPE` int unsigned NOT NULL DEFAULT '0',
                        `ADMIN` varchar(1) DEFAULT NULL,
                        `CASHBOND` double(7,2) NOT NULL DEFAULT '50.00',
                        `WORKDAYS` double(3,0) DEFAULT NULL,
                        `ALLOWRATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `ALLOWTYPE` varchar(1) DEFAULT NULL,
                        `ALLOWFIX` varchar(1) DEFAULT NULL,
                        `ALLOW2RATE` double(9,2) DEFAULT NULL,
                        `ALLOW2TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW2FIX` varchar(1) DEFAULT NULL,
                        `ALLOW3RATE` double(9,2) DEFAULT NULL,
                        `ALLOW3TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW3FIX` varchar(1) DEFAULT NULL,
                        `ALLOW4RATE` double(9,2) DEFAULT NULL,
                        `ALLOW4TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW4FIX` varchar(1) DEFAULT NULL,
                        `MOVNUMBER` varchar(10) DEFAULT NULL,
                        `MOVMODE` varchar(1) DEFAULT NULL,
                        `MOVDATE` date DEFAULT NULL,
                        `MOVEND` date DEFAULT NULL,
                        `DUTYDATE` date DEFAULT NULL,
                        `ADDR1` varchar(150) DEFAULT NULL,
                        `MLACODE_` varchar(5) DEFAULT NULL,
                        `TEL1` varchar(15) DEFAULT NULL,
                        `ADDR2` varchar(150) DEFAULT NULL,
                        `PROCODE_` varchar(5) DEFAULT NULL,
                        `TEL2` varchar(15) DEFAULT NULL,
                        `EMPBIRTH` date DEFAULT NULL,
                        `BIRTHPLACE` varchar(75) DEFAULT NULL,
                        `SEX_` varchar(1) DEFAULT NULL,
                        `CIVSTAT_` varchar(1) DEFAULT NULL,
                        `CITIZEN` varchar(15) DEFAULT NULL,
                        `HEIGHT` double(6,4) DEFAULT NULL,
                        `WEIGHT` double(6,2) DEFAULT NULL,
                        `TIN` varchar(11) DEFAULT NULL,
                        `SSS` varchar(12) DEFAULT NULL,
                        `HDMF` varchar(12) DEFAULT NULL,
                        `RELIGION` varchar(35) DEFAULT NULL,
                        `HAIR` varchar(5) DEFAULT NULL,
                        `EYES` varchar(5) DEFAULT NULL,
                        `SPOUSE` varchar(25) DEFAULT NULL,
                        `OCCUPATION` varchar(75) DEFAULT NULL,
                        `NOCHILDREN` double(2,0) DEFAULT NULL,
                        `DATEHIRED` date DEFAULT NULL,
                        `SEPARATE` date DEFAULT NULL,
                        `POSITION_` varchar(5) DEFAULT NULL,
                        `EMPSTAT_` varchar(1) DEFAULT NULL,
                        `STATUSDATE` date DEFAULT NULL,
                        `SECLICENSE` varchar(15) DEFAULT NULL,
                        `LICEXPIRE` date DEFAULT NULL,
                        `TRAINAT` varchar(30) DEFAULT NULL,
                        `DATETRAIN` date DEFAULT NULL,
                        `INSURANCE` varchar(20) DEFAULT NULL,
                        `POLICYNO` varchar(15) DEFAULT NULL,
                        `FACEVALUE` double(9,2) DEFAULT NULL,
                        `PREMIUM` double(6,2) DEFAULT NULL,
                        `INSEXPIRE` date DEFAULT NULL,
                        `EXMILITARY` varchar(1) DEFAULT NULL,
                        `CSP` varchar(1) DEFAULT NULL,
                        `CPP` varchar(1) DEFAULT NULL,
                        `ROTC` varchar(1) DEFAULT NULL,
                        `ELLEVEL` varchar(8) DEFAULT NULL,
                        `HSLEVEL` varchar(8) DEFAULT NULL,
                        `COLLEGE_` varchar(8) DEFAULT NULL,
                        `COURSE` varchar(25) DEFAULT NULL,
                        `VOLEVEL` varchar(8) DEFAULT NULL,
                        `VOCOURSE` varchar(25) DEFAULT NULL,
                        `LANGUAGE` varchar(15) DEFAULT NULL,
                        `SKILL1` varchar(2) DEFAULT NULL,
                        `SKILL2` varchar(2) DEFAULT NULL,
                        `SKILL3` varchar(2) DEFAULT NULL,
                        `SKILL4` varchar(2) DEFAULT NULL,
                        `TAXCODE` varchar(3) DEFAULT NULL,
                        `ACCTCODE` varchar(21) DEFAULT ' ',
                        `AWOL` varchar(6) DEFAULT NULL,
                        `DISMISS` varchar(6) DEFAULT NULL,
                        `ASTART` date DEFAULT NULL,
                        `AEND` date DEFAULT NULL,
                        `ADAYS` double(2,0) DEFAULT NULL,
                        `DSTART` date DEFAULT NULL,
                        `DEND` date DEFAULT NULL,
                        `DDAYS` double(2,0) DEFAULT NULL,
                        `EMRNAME` varchar(25) DEFAULT NULL,
                        `EMRTEL` varchar(15) DEFAULT NULL,
                        `EMRADDR` varchar(60) DEFAULT NULL,
                        `GUARDEXP` double(5,2) DEFAULT NULL,
                        `COMTAXNO` varchar(10) DEFAULT NULL,
                        `COMTAXDATE` date DEFAULT NULL,
                        `COMTAX_AT` varchar(30) DEFAULT NULL,
                        `BLOODTYPE` varchar(10) DEFAULT NULL,
                        `MARKS` varchar(60) DEFAULT NULL,
                        `COMPLEXION` varchar(10) DEFAULT NULL,
                        `EXP_NBI` date DEFAULT NULL,
                        `EXP_POLICE` date DEFAULT NULL,
                        `EXP_PNP` date DEFAULT NULL,
                        `EXP_BRGY` date DEFAULT NULL,
                        `EXP_COURT` date DEFAULT NULL,
                        `EXP_NEURO` date DEFAULT NULL,
                        `EXP_DRUG` date DEFAULT NULL,
                        `W_BIRTHC` varchar(1) DEFAULT NULL,
                        `W_CLOSINGR` varchar(1) DEFAULT NULL,
                        `W_TRNCERT` varchar(1) DEFAULT NULL,
                        `W_PRELIC` varchar(1) DEFAULT NULL,
                        `W_CERTEMP` varchar(1) DEFAULT NULL,
                        `W_MEDEXAM` varchar(1) DEFAULT NULL,
                        `GKERATE` double(6,2) DEFAULT NULL,
                        `CLNAME` varchar(254) DEFAULT NULL,
                        `MLANAME` varchar(20) DEFAULT NULL,
                        `AGE` double(2,0) DEFAULT NULL,
                        `MBRANCH` varchar(20) DEFAULT NULL,
                        `MYEAR` varchar(20) DEFAULT NULL,
                        `MNATURE` varchar(20) DEFAULT NULL,
                        `REMARKS` varchar(150) DEFAULT NULL,
                        `BADGENO` varchar(10) DEFAULT NULL,
                        `GUARDNOYRS` varchar(5) DEFAULT NULL,
                        `MILITARYNOYR` varchar(5) DEFAULT NULL,
                        `PAGIBIGNO` varchar(15) DEFAULT NULL,
                        `PHIC` varchar(15) DEFAULT NULL,
                        `BANK` varchar(10) DEFAULT NULL,
                        `EXPMED` date DEFAULT NULL,
                        `regref` date DEFAULT NULL COMMENT 'regularship reference',
                        `empBasicRate` double(10,4) NOT NULL DEFAULT '0.0000',
                        `rateID` int unsigned NOT NULL DEFAULT '2',
                        `empEcola` double(10,4) NOT NULL DEFAULT '0.0000',
                        `xmark` int unsigned NOT NULL DEFAULT '0',
                        `suretybondquota` double(10,2) NOT NULL DEFAULT '0.00',
                        `DRV_LICENSE` varchar(25) DEFAULT NULL,
                        `DRV_EXP` date DEFAULT NULL,
                        `isTaxable` int unsigned NOT NULL DEFAULT '0',
                        `isconfi` int unsigned NOT NULL DEFAULT '0',
                        `iswithSSS` int unsigned DEFAULT '1',
                        `iswithGSIS` int unsigned NOT NULL DEFAULT '0',
                        `iswithPHIC` int unsigned DEFAULT '1',
                        `iswithPagibig` int unsigned NOT NULL DEFAULT '1',
                        `ismaxsss` int unsigned DEFAULT '0',
                        `email` varchar(50) DEFAULT ' ',
                        `passwd` char(25) DEFAULT '',
                        `Countrycode` char(3) DEFAULT 'PHL',
                        `sgcode` char(10) NOT NULL DEFAULT '',
                        `dpadate` date DEFAULT NULL,
                        `dpclient` char(5) DEFAULT '',
                        `desig_` char(5) DEFAULT 'EMP',
                        `IsMigrated` int DEFAULT '0',
                        PRIMARY KEY (`EMPNUMBER`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1 COMMENT='InnoDB free: 9216 kB';";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpmasNbiClearance(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmas_nbi_clearance` (
                        `empnumber` char(5) NOT NULL,
                        `nbi_verified` date DEFAULT NULL,
                        `nbi_forVerification` tinyint unsigned NOT NULL,
                        `nbi_noDerogatoryReord` tinyint unsigned NOT NULL,
                        `nbi_withDerogatoryRecord` tinyint unsigned NOT NULL,
                        `nbi_case` char(250) NOT NULL,
                        `bi_dateConducted` date DEFAULT NULL,
                        `bi_forNeighborhoodCheck` tinyint unsigned NOT NULL,
                        `bi_noDerogatoryRecord` tinyint unsigned NOT NULL,
                        `bi_withderogatoryRecord` tinyint unsigned NOT NULL,
                        `bi_case` char(250) NOT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpmasNbiClearanceEmpstat(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmas_nbi_clearance_empstat` (
                        `code` char(1) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empmasaccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmasaccess` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `LogName` char(25) NOT NULL,
                        `code` char(15) NOT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empmasarchieved(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmasarchieved` (
                        `EMPNUMBER` varchar(5) NOT NULL,
                        `EMPLASTNM` varchar(15) DEFAULT NULL,
                        `EMPFIRSTNM` varchar(17) DEFAULT NULL,
                        `EMPMIDNM` varchar(15) DEFAULT NULL,
                        `suffix` char(3) DEFAULT '',
                        `EMPALIAS` varchar(15) DEFAULT NULL,
                        `CLIENT` varchar(5) DEFAULT NULL,
                        `CLIENT_` varchar(5) DEFAULT NULL,
                        `BASICRATE` double(10,4) NOT NULL DEFAULT '0.0000',
                        `PAYTYPE` int unsigned NOT NULL DEFAULT '0',
                        `ADMIN` varchar(1) DEFAULT NULL,
                        `CASHBOND` double(7,2) NOT NULL DEFAULT '50.00',
                        `WORKDAYS` double(3,0) DEFAULT NULL,
                        `ALLOWRATE` double(9,2) NOT NULL DEFAULT '0.00',
                        `ALLOWTYPE` varchar(1) DEFAULT NULL,
                        `ALLOWFIX` varchar(1) DEFAULT NULL,
                        `ALLOW2RATE` double(9,2) DEFAULT NULL,
                        `ALLOW2TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW2FIX` varchar(1) DEFAULT NULL,
                        `ALLOW3RATE` double(9,2) DEFAULT NULL,
                        `ALLOW3TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW3FIX` varchar(1) DEFAULT NULL,
                        `ALLOW4RATE` double(9,2) DEFAULT NULL,
                        `ALLOW4TYPE` varchar(1) DEFAULT NULL,
                        `ALLOW4FIX` varchar(1) DEFAULT NULL,
                        `MOVNUMBER` varchar(10) DEFAULT NULL,
                        `MOVMODE` varchar(1) DEFAULT NULL,
                        `MOVDATE` date DEFAULT NULL,
                        `MOVEND` date NOT NULL DEFAULT '0000-00-00',
                        `DUTYDATE` date DEFAULT NULL,
                        `ADDR1` varchar(120) DEFAULT NULL,
                        `MLACODE_` varchar(5) DEFAULT NULL,
                        `TEL1` varchar(15) DEFAULT NULL,
                        `ADDR2` varchar(120) DEFAULT NULL,
                        `PROCODE_` varchar(5) DEFAULT NULL,
                        `TEL2` varchar(15) DEFAULT NULL,
                        `EMPBIRTH` date DEFAULT NULL,
                        `BIRTHPLACE` varchar(75) DEFAULT NULL,
                        `SEX_` varchar(1) DEFAULT NULL,
                        `CIVSTAT_` varchar(1) DEFAULT NULL,
                        `CITIZEN` varchar(15) DEFAULT NULL,
                        `HEIGHT` double(6,4) DEFAULT NULL,
                        `WEIGHT` double(6,2) DEFAULT NULL,
                        `TIN` varchar(11) DEFAULT NULL,
                        `SSS` varchar(12) DEFAULT NULL,
                        `HDMF` varchar(12) DEFAULT NULL,
                        `RELIGION` varchar(35) DEFAULT NULL,
                        `HAIR` varchar(5) DEFAULT NULL,
                        `EYES` varchar(5) DEFAULT NULL,
                        `SPOUSE` varchar(25) DEFAULT NULL,
                        `OCCUPATION` varchar(75) DEFAULT NULL,
                        `NOCHILDREN` double(2,0) DEFAULT NULL,
                        `DATEHIRED` date DEFAULT NULL,
                        `SEPARATE` date DEFAULT NULL,
                        `POSITION_` varchar(5) DEFAULT NULL,
                        `EMPSTAT_` varchar(1) DEFAULT NULL,
                        `STATUSDATE` date DEFAULT NULL,
                        `SECLICENSE` varchar(15) DEFAULT NULL,
                        `LICEXPIRE` date DEFAULT NULL,
                        `TRAINAT` varchar(30) DEFAULT NULL,
                        `DATETRAIN` date DEFAULT NULL,
                        `INSURANCE` varchar(20) DEFAULT NULL,
                        `POLICYNO` varchar(15) DEFAULT NULL,
                        `FACEVALUE` double(9,2) DEFAULT NULL,
                        `PREMIUM` double(6,2) DEFAULT NULL,
                        `INSEXPIRE` date DEFAULT NULL,
                        `EXMILITARY` varchar(1) DEFAULT NULL,
                        `CSP` varchar(1) DEFAULT NULL,
                        `CPP` varchar(1) DEFAULT NULL,
                        `ROTC` varchar(1) DEFAULT NULL,
                        `ELLEVEL` varchar(8) DEFAULT NULL,
                        `HSLEVEL` varchar(8) DEFAULT NULL,
                        `COLLEGE_` varchar(8) DEFAULT NULL,
                        `COURSE` varchar(25) DEFAULT NULL,
                        `VOLEVEL` varchar(8) DEFAULT NULL,
                        `VOCOURSE` varchar(25) DEFAULT NULL,
                        `LANGUAGE` varchar(15) DEFAULT NULL,
                        `SKILL1` varchar(2) DEFAULT NULL,
                        `SKILL2` varchar(2) DEFAULT NULL,
                        `SKILL3` varchar(2) DEFAULT NULL,
                        `SKILL4` varchar(2) DEFAULT NULL,
                        `TAXCODE` varchar(3) DEFAULT NULL,
                        `ACCTCODE` varchar(15) DEFAULT NULL,
                        `AWOL` varchar(6) DEFAULT NULL,
                        `DISMISS` varchar(6) DEFAULT NULL,
                        `ASTART` date DEFAULT NULL,
                        `AEND` date DEFAULT NULL,
                        `ADAYS` double(2,0) DEFAULT NULL,
                        `DSTART` date DEFAULT NULL,
                        `DEND` date DEFAULT NULL,
                        `DDAYS` double(2,0) DEFAULT NULL,
                        `EMRNAME` varchar(25) DEFAULT NULL,
                        `EMRTEL` varchar(15) DEFAULT NULL,
                        `EMRADDR` varchar(60) DEFAULT NULL,
                        `GUARDEXP` double(5,2) DEFAULT NULL,
                        `COMTAXNO` varchar(10) DEFAULT NULL,
                        `COMTAXDATE` date DEFAULT NULL,
                        `COMTAX_AT` varchar(30) DEFAULT NULL,
                        `BLOODTYPE` varchar(10) DEFAULT NULL,
                        `MARKS` varchar(60) DEFAULT NULL,
                        `COMPLEXION` varchar(10) DEFAULT NULL,
                        `EXP_NBI` date DEFAULT NULL,
                        `EXP_POLICE` date DEFAULT NULL,
                        `EXP_PNP` date DEFAULT NULL,
                        `EXP_BRGY` date DEFAULT NULL,
                        `EXP_COURT` date DEFAULT NULL,
                        `EXP_NEURO` date DEFAULT NULL,
                        `EXP_DRUG` date DEFAULT NULL,
                        `W_BIRTHC` varchar(1) DEFAULT NULL,
                        `W_CLOSINGR` varchar(1) DEFAULT NULL,
                        `W_TRNCERT` varchar(1) DEFAULT NULL,
                        `W_PRELIC` varchar(1) DEFAULT NULL,
                        `W_CERTEMP` varchar(1) DEFAULT NULL,
                        `W_MEDEXAM` varchar(1) DEFAULT NULL,
                        `GKERATE` double(6,2) DEFAULT NULL,
                        `CLNAME` varchar(254) DEFAULT NULL,
                        `MLANAME` varchar(20) DEFAULT NULL,
                        `AGE` double(2,0) DEFAULT NULL,
                        `MBRANCH` varchar(20) DEFAULT NULL,
                        `MYEAR` varchar(20) DEFAULT NULL,
                        `MNATURE` varchar(20) DEFAULT NULL,
                        `REMARKS` varchar(150) DEFAULT NULL,
                        `BADGENO` varchar(10) DEFAULT NULL,
                        `GUARDNOYRS` varchar(5) DEFAULT NULL,
                        `MILITARYNOYR` varchar(5) DEFAULT NULL,
                        `PAGIBIGNO` varchar(15) DEFAULT NULL,
                        `PHIC` varchar(15) DEFAULT NULL,
                        `BANK` varchar(10) DEFAULT NULL,
                        `EXPMED` date DEFAULT NULL,
                        `regref` date NOT NULL DEFAULT '0000-00-00' COMMENT 'regularship reference',
                        `empBasicRate` double(10,4) NOT NULL DEFAULT '0.0000',
                        `rateID` int unsigned NOT NULL DEFAULT '2',
                        `empEcola` double(10,4) NOT NULL DEFAULT '0.0000',
                        `xMark` int unsigned NOT NULL DEFAULT '0',
                        `suretybondquota` double(10,2) NOT NULL DEFAULT '0.00',
                        `DRV_LICENSE` varchar(25) DEFAULT NULL,
                        `DRV_EXP` date NOT NULL DEFAULT '0000-00-00',
                        `isTaxable` int unsigned NOT NULL,
                        `isconfi` int unsigned NOT NULL DEFAULT '0',
                        `iswithSSS` int unsigned DEFAULT '1',
                        `iswithGSIS` int unsigned NOT NULL DEFAULT '0',
                        `iswithPHIC` int unsigned DEFAULT '1',
                        `iswithPagibig` int unsigned DEFAULT '1',
                        `ismaxsss` int unsigned NOT NULL DEFAULT '0',
                        `email` varchar(50) NOT NULL DEFAULT ' ',
                        `passwd` char(25) NOT NULL DEFAULT '',
                        `Countrycode` char(3) NOT NULL DEFAULT 'PH',
                        `sgcode` char(10) NOT NULL DEFAULT '',
                        `dpadate` date NOT NULL DEFAULT '0000-00-00',
                        `dpclient` char(5) NOT NULL DEFAULT '',
                        `desig_` char(5) DEFAULT 'EMP',
                        PRIMARY KEY (`EMPNUMBER`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empremarks(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empremarks` (
                        `idEmpRemarks` int unsigned NOT NULL AUTO_INCREMENT,
                        `empno` varchar(5) NOT NULL,
                        `filed` datetime NOT NULL,
                        `remarks` longtext NOT NULL,
                        `resolution` longtext,
                        `empRemCat` varchar(45) NOT NULL,
                        `resolutionDate` datetime DEFAULT NULL,
                        `remstatus` varchar(45) NOT NULL,
                        PRIMARY KEY (`idEmpRemarks`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empremarkscategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empremarkscategory` (
                        `idempRemarksCategory` int unsigned NOT NULL AUTO_INCREMENT,
                        `category` varchar(45) NOT NULL,
                        PRIMARY KEY (`idempRemarksCategory`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empremarksstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empremarksstatus` (
                        `idempRemarksStatus` int unsigned NOT NULL AUTO_INCREMENT,
                        `RemStatus` varchar(45) NOT NULL,
                        PRIMARY KEY (`idempRemarksStatus`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
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
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpstatIn(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat_in` (
                        `code` char(1) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpstatOut(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat_out` (
                        `code` char(1) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpstatStatmgt(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat_statmgt` (
                        `code` varchar(1) NOT NULL,
                        `name` varchar(30) DEFAULT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpstatUserAccess(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstat_user_access` (
                        `empno` char(5) NOT NULL,
                        `empstat_` char(1) NOT NULL,
                        PRIMARY KEY (`empno`,`empstat_`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empstatforarchive(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstatforarchive` (
                        `code` varchar(5) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Empstatmovement(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstatmovement` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` char(5) NOT NULL,
                        `user` varchar(45) DEFAULT NULL,
                        `datemove` datetime DEFAULT NULL,
                        `stat_from` char(15) DEFAULT NULL,
                        `stat_to` char(15) DEFAULT NULL,
                        `module` varchar(30) DEFAULT NULL,
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01EmpstatmovementBak(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empstatmovement_bak` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` char(5) NOT NULL,
                        `user` varchar(45) DEFAULT NULL,
                        `datemove` datetime DEFAULT NULL,
                        `stat_` char(5) DEFAULT NULL,
                        `module` varchar(30) DEFAULT NULL,
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Family(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`family` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `NAME` varchar(30) DEFAULT NULL,
                        `BIRTH` date DEFAULT NULL,
                        `RELATION` varchar(15) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Filedata(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`filedata` (
                        `Filename` varchar(25) NOT NULL,
                        `empnumber` varchar(5) NOT NULL,
                        `dateCREATED` date NOT NULL,
                        `user` varchar(25) NOT NULL,
                        PRIMARY KEY (`Filename`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Floatnotice(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`floatnotice` (
                        `transno` varchar(10) NOT NULL,
                        `empnumber` varchar(5) NOT NULL,
                        `date1` date NOT NULL DEFAULT '0000-00-00',
                        `date2` date NOT NULL DEFAULT '0000-00-00',
                        `date3` date NOT NULL DEFAULT '0000-00-00',
                        `date4` date NOT NULL DEFAULT '0000-00-00',
                        `d1no` varchar(10) NOT NULL DEFAULT '',
                        `d2no` varchar(10) NOT NULL DEFAULT '',
                        `d3no` varchar(10) NOT NULL DEFAULT '',
                        `d4no` varchar(10) NOT NULL DEFAULT '',
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Freeuniform(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`freeuniform` (
                        `FUkey` int unsigned NOT NULL AUTO_INCREMENT,
                        `empnumber` varchar(5) NOT NULL,
                        `dateGiven` date NOT NULL,
                        PRIMARY KEY (`FUkey`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Govexam(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`govexam` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `EXAM` varchar(50) DEFAULT NULL,
                        `RATING` double(6,2) DEFAULT NULL,
                        `TAKEN` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Grdbirth(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`grdbirth` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `LASTNAME` varchar(15) DEFAULT NULL,
                        `FIRSTNAME` varchar(15) DEFAULT NULL,
                        `MIDDLENAME` varchar(15) DEFAULT NULL,
                        `BIRTHDATE` date DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Idissuance(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`idissuance` (
                        `empnumber` varchar(10) NOT NULL,
                        `requested` date DEFAULT '0000-00-00',
                        `dateissued` date DEFAULT '0000-00-00',
                        `remarks` varchar(50) DEFAULT ' ',
                        `expiration` date DEFAULT '0000-00-00',
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Info(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`info` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `CODE` varchar(1) DEFAULT NULL,
                        `YES` varchar(1) DEFAULT NULL,
                        `REM` varchar(60) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Insurance(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`insurance` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Name` varchar(45) DEFAULT NULL,
                        `PolicyNo` varchar(25) DEFAULT NULL,
                        `InsuranceType` varchar(15) CHARACTER SET latin1 NOT NULL DEFAULT 'Life' COMMENT 'Life and Accident',
                        `FaceValue` double(12,2) NOT NULL COMMENT '0',
                        `Premiums` double(12,2) NOT NULL,
                        `InsExpire` date DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01InsuranceAccident(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`insurance_accident` (
                        `EMPNUMBER` char(5) NOT NULL DEFAULT '0',
                        `INSURANCE` varchar(45) DEFAULT NULL,
                        `POLICYNO` varchar(25) DEFAULT NULL,
                        `FACEVALUE` double(8,2) NOT NULL DEFAULT '0.00',
                        `PREMIUM` double(8,2) NOT NULL DEFAULT '0.00',
                        `INSEXPIRE` date DEFAULT NULL,
                        PRIMARY KEY (`EMPNUMBER`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Insurancestatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`insurancestatus` (
                        `code` char(2) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Language(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`language` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `CODE` varchar(1) DEFAULT NULL,
                        `READ_` varchar(9) DEFAULT NULL,
                        `SPOKEN_` varchar(9) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Lastrefused(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`lastrefused` (
                        `ClCode` varchar(10) NOT NULL,
                        `ClAreaCd` varchar(10) NOT NULL,
                        `EmpNumber` varchar(10) NOT NULL DEFAULT '',
                        PRIMARY KEY (`ClCode`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Leave1(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`leave1` (
                        `LVNUMBER` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT '',
                        `LVDATE` date DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `LVCODE` varchar(2) DEFAULT NULL,
                        `START` date DEFAULT NULL,
                        `END` date DEFAULT NULL,
                        `NO_DAYS` double(3,0) DEFAULT NULL,
                        `WITHPAY` varchar(1) DEFAULT NULL,
                        `ENCODEBY` varchar(10) DEFAULT NULL,
                        `ENCODEDT` varchar(10) DEFAULT NULL,
                        `CHANGEBY` varchar(10) DEFAULT NULL,
                        `CHANGEDT` varchar(10) DEFAULT NULL,
                        `POSTED` varchar(1) DEFAULT NULL,
                        `mode` varchar(1) NOT NULL DEFAULT 'L'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Levmonletter(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`levmonletter` (
                        `lvnumber` char(10) NOT NULL,
                        `empnumber` char(10) NOT NULL,
                        `letter1` date DEFAULT '0000-00-00',
                        `letter2` date DEFAULT '0000-00-00',
                        `letter3` date DEFAULT '0000-00-00',
                        `letter4` date DEFAULT '0000-00-00',
                        `letter1by` char(15) DEFAULT '',
                        `letter2by` char(15) DEFAULT '',
                        `letter3by` char(15) DEFAULT '',
                        `letter4by` char(15) DEFAULT '',
                        PRIMARY KEY (`lvnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Levmonpara(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`levmonpara` (
                        `idlevMonpara` int unsigned NOT NULL AUTO_INCREMENT,
                        `day1duration` int unsigned NOT NULL,
                        `day2Duration` int unsigned NOT NULL,
                        `day3Duration` int unsigned NOT NULL,
                        `day4Duration` int unsigned NOT NULL,
                        PRIMARY KEY (`idlevMonpara`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Levtbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`levtbl` (
                        `LVCODE` varchar(2) DEFAULT NULL,
                        `LVNAME` varchar(20) DEFAULT NULL,
                        `EARNCODE` varchar(2) DEFAULT NULL,
                        `NO_INCDATE` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Licverstatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`licverstatus` (
                        `licverStatus` char(15) NOT NULL,
                        PRIMARY KEY (`licverStatus`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Loghist(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`loghist` (
                        `ctrlNo` int unsigned NOT NULL AUTO_INCREMENT,
                        `logcd` varchar(25) NOT NULL,
                        `location` varchar(30) NOT NULL,
                        `date` datetime NOT NULL,
                        PRIMARY KEY (`ctrlNo`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mdr` (
                        `empnumber` char(5) NOT NULL,
                        `clnumber` char(5) NOT NULL,
                        `trn` char(4) NOT NULL DEFAULT '',
                        `mrk` int unsigned DEFAULT '0',
                        `education` char(80) DEFAULT '',
                        `secLicense` char(15) DEFAULT '',
                        `licExpire` date DEFAULT '0000-00-00',
                        `PolicyNo` char(20) DEFAULT '',
                        `InsAmt` double(18,2) DEFAULT '0.00',
                        PRIMARY KEY (`empnumber`,`clnumber`,`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01MdrClient(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mdr_client` (
                        `trn` char(10) NOT NULL,
                        `mrk` int unsigned DEFAULT '0',
                        PRIMARY KEY (`trn`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mdrcaption(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mdrcaption` (
                        `idmdrCaption` int unsigned NOT NULL AUTO_INCREMENT,
                        `coname` varchar(60) NOT NULL,
                        `address` varchar(100) NOT NULL,
                        `LicDaysExp` int unsigned NOT NULL,
                        `telno` varchar(60) NOT NULL,
                        `shdr1` varchar(60) NOT NULL,
                        `sss_tin` varchar(60) NOT NULL,
                        `email` varchar(60) NOT NULL,
                        PRIMARY KEY (`idmdrCaption`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mdrfirearms(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mdrfirearms` (
                        `mrk` int unsigned NOT NULL,
                        `yr` int unsigned NOT NULL,
                        `mo` int unsigned NOT NULL,
                        `clnumber` char(5) NOT NULL,
                        `serialno` char(15) NOT NULL,
                        `kind` char(15) DEFAULT NULL,
                        `make` char(15) DEFAULT NULL,
                        `odr` int DEFAULT NULL,
                        PRIMARY KEY (`yr`,`mo`,`serialno`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mdrhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mdrhdr` (
                        `yr` int unsigned NOT NULL,
                        `month` int unsigned NOT NULL,
                        `date` date NOT NULL,
                        `secLicExp` date NOT NULL,
                        PRIMARY KEY (`yr`,`month`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Menubars(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`menubars` (
                        `code` varchar(25) NOT NULL,
                        `command_` varchar(254) NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Mlacode(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`mlacode` (
                        `CODE` varchar(5) DEFAULT NULL,
                        `NAME` varchar(30) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Movedtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`movedtl` (
                        `NUMBER` varchar(10) DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `MODE` varchar(1) DEFAULT NULL,
                        `DATE` date DEFAULT NULL,
                        `TIME` varchar(4) DEFAULT NULL,
                        `SADATE` date DEFAULT NULL,
                        `REASON` varchar(254) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Movehdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`movehdr` (
                        `NUMBER` varchar(10) DEFAULT NULL,
                        `PREPDATE` date DEFAULT NULL,
                        `REQNUMBER` varchar(10) DEFAULT NULL,
                        `CLNUMBER` varchar(5) DEFAULT NULL,
                        `MODE` varchar(1) DEFAULT NULL,
                        `REPORT` varchar(30) DEFAULT NULL,
                        `EXPDATE` date DEFAULT NULL,
                        `grpNo` varchar(10) NOT NULL DEFAULT ''
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01MovehdrStatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`movehdr_status` (
                        `Code` char(1) NOT NULL,
                        PRIMARY KEY (`Code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01MovehdrStatusTrail(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`movehdr_status_trail` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `usr` varchar(55) DEFAULT NULL,
                        `code` char(1) DEFAULT NULL,
                        `action` char(1) DEFAULT NULL,
                        `activity` datetime DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Neuroexm(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`neuroexm` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `NEUTAKEN` date DEFAULT NULL,
                        `NEUEXPIRE` date DEFAULT NULL,
                        `NEUCENTER` varchar(30) DEFAULT NULL,
                        `NEURATING` double(6,2) DEFAULT NULL,
                        `NEUREMARK` varchar(20) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Notice(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`notice` (
                        `control_no` varchar(10) NOT NULL,
                        `empnumber` varchar(5) DEFAULT NULL,
                        `from` varchar(60) DEFAULT NULL,
                        `cc` varchar(60) DEFAULT NULL,
                        `date` date DEFAULT NULL,
                        `subj` varchar(15) DEFAULT NULL,
                        `msg` longtext,
                        PRIMARY KEY (`control_no`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ofstaff(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ofstaff` (
                        `staff_id` int unsigned NOT NULL AUTO_INCREMENT,
                        `staff_nm` varchar(60) NOT NULL,
                        `staff_pos` varchar(60) NOT NULL,
                        `defa` int unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`staff_id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ofstaff2(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ofstaff2` (
                        `staff_id` int unsigned NOT NULL AUTO_INCREMENT,
                        `staff_nm` varchar(60) NOT NULL,
                        `staff_pos` varchar(60) NOT NULL,
                        `defa` int unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`staff_id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ofstaff3(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ofstaff3` (
                        `staff_id` int unsigned NOT NULL AUTO_INCREMENT,
                        `staff_nm` varchar(60) NOT NULL,
                        `staff_pos` varchar(60) NOT NULL,
                        `defa` int unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`staff_id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ofstaff4(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ofstaff4` (
                        `staff_id` int unsigned NOT NULL AUTO_INCREMENT,
                        `staff_nm` varchar(60) NOT NULL,
                        `staff_pos` varchar(60) NOT NULL,
                        `defa` int unsigned NOT NULL,
                        PRIMARY KEY (`staff_id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Para(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`para` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `desigTblCreated` smallint unsigned NOT NULL DEFAULT '0',
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Parent(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`parent` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `CODE` varchar(1) DEFAULT NULL,
                        `NAME` varchar(30) DEFAULT NULL,
                        `AGE` double(2,0) DEFAULT NULL,
                        `OCC` varchar(20) DEFAULT NULL,
                        `ADDR` varchar(60) DEFAULT NULL,
                        `dob` datetime NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Penalty(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`penalty` (
                        `DEV_NO` varchar(10) DEFAULT NULL,
                        `FREQ` varchar(1) DEFAULT NULL,
                        `PENALTY_NO` varchar(2) DEFAULT NULL,
                        `DESC_` varchar(20) DEFAULT NULL,
                        `resetregref` varchar(1) NOT NULL DEFAULT '0' COMMENT '1true, 2false reset regularization period',
                        `isterminated` varchar(1) NOT NULL DEFAULT '0',
                        `days` double(6,0) NOT NULL DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
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
        await ExecLegacy(sql, conn);
    }

    private async Task _01Procode(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`procode` (
                        `CODE` varchar(5) DEFAULT NULL,
                        `NAME` varchar(30) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Qrynum(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`qrynum` (
                        `NUMBER` varchar(8) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Query(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`query` (
                        `QRYCODE` varchar(8) DEFAULT NULL,
                        `DBFNAME` varchar(8) DEFAULT NULL,
                        `TITLE` varchar(50) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Refer(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`refer` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `NAME` varchar(50) DEFAULT NULL,
                        `ADDR` varchar(60) DEFAULT NULL,
                        `TEL` varchar(15) DEFAULT NULL,
                        `POSITION` varchar(75) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Regions(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`regions` (
                        `code` char(10) NOT NULL,
                        `name` varchar(60) NOT NULL,
                        `odr` int NOT NULL,
                        PRIMARY KEY (`code`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Seclic(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`seclic` (
                        `empnumber` char(5) NOT NULL,
                        `sbrno` char(25) DEFAULT '',
                        `opno` char(25) DEFAULT '',
                        `validated` date DEFAULT NULL,
                        `vfee` double(18,2) DEFAULT '0.00',
                        `revalidated` date DEFAULT NULL,
                        `valStatus` char(15) DEFAULT '',
                        `user` char(25) DEFAULT NULL,
                        `dateModified` datetime DEFAULT NULL,
                        `licDateIssued` date DEFAULT NULL,
                        PRIMARY KEY (`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);

        // -- Existing tables : the dates default to NULL, as in the reference (192.168.0.103 secpis) --
        sql = $@"ALTER TABLE {schema}.`seclic`
                    ALTER COLUMN `validated`     SET DEFAULT NULL,
                    ALTER COLUMN `revalidated`   SET DEFAULT NULL,
                    ALTER COLUMN `licDateIssued` SET DEFAULT NULL;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01SeclicSignatory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`seclic_signatory` (
                        `sig1` varchar(80) NOT NULL,
                        `desig1` varchar(80) DEFAULT NULL,
                        `sig2` varchar(80) DEFAULT NULL,
                        `desig2` varchar(80) DEFAULT NULL,
                        `sig3` varchar(80) DEFAULT NULL,
                        `desig3` varchar(80) DEFAULT NULL,
                        PRIMARY KEY (`sig1`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Secliclogs(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`secliclogs` (
                        `empnumber` char(5) NOT NULL,
                        `sbrno` char(25) DEFAULT NULL,
                        `opno` char(25) DEFAULT NULL,
                        `validated` date DEFAULT NULL,
                        `vfee` double(18,2) DEFAULT '0.00',
                        `revalidated` date DEFAULT NULL,
                        `valStatus` char(15) DEFAULT '',
                        `user` char(25) DEFAULT NULL,
                        `dateModified` datetime DEFAULT NULL,
                        `licDateIssued` date DEFAULT '0000-00-00'
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Sex(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`sex` (
                        `CODE` varchar(1) DEFAULT NULL,
                        `NAME` varchar(10) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Skills(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`skills` (
                        `CODE` varchar(2) DEFAULT NULL,
                        `NAME` varchar(20) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Ssstbl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`ssstbl` (
                        `FSTART` double(8,2) DEFAULT NULL,
                        `FEND` double(8,2) DEFAULT NULL,
                        `FSSSEE` double(6,2) DEFAULT NULL,
                        `FSSSER` double(6,2) DEFAULT NULL,
                        `FECC` double(6,2) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Status(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`status` (
                        `DATE` date DEFAULT NULL,
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `EMPLOYSTAT` varchar(1) DEFAULT NULL,
                        `START` date DEFAULT NULL,
                        `END` date DEFAULT NULL,
                        `NO_DAYS` double(2,0) DEFAULT NULL,
                        `ENCODEBY` varchar(10) DEFAULT NULL,
                        `ENCODEDT` varchar(10) DEFAULT NULL,
                        `CHANGEBY` varchar(10) DEFAULT NULL,
                        `CHANGEDT` varchar(10) DEFAULT NULL,
                        `POSTED` varchar(1) DEFAULT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Statusremarks(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`statusremarks` (
                        `EmpNumber` char(5) NOT NULL,
                        `Remarks` char(200) NOT NULL,
                        `ResignedRem` char(200) DEFAULT '',
                        PRIMARY KEY (`EmpNumber`)
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

    private async Task _01Tbloperators(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tbloperators` (
                        `Desc_` varchar(45) NOT NULL,
                        `OP` varchar(5) NOT NULL,
                        PRIMARY KEY (`Desc_`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Tblqueries(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`tblqueries` (
                        `nQueryID` int unsigned NOT NULL AUTO_INCREMENT,
                        `cDesc` varchar(45) NOT NULL,
                        `cContent` longtext NOT NULL,
                        `logname` varchar(25) DEFAULT ' ',
                        PRIMARY KEY (`nQueryID`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Train(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`train` (
                        `EMPNUMBER` varchar(5) DEFAULT NULL,
                        `PROGRAM` varchar(60) DEFAULT NULL,
                        `TAKEN` varchar(20) DEFAULT NULL,
                        `SCHOOL` varchar(50) DEFAULT NULL,
                        `TRAINOR` varchar(30) DEFAULT NULL,
                        `TYPE` varchar(9) DEFAULT NULL,
                        `idtrainhdr` int unsigned NOT NULL DEFAULT '0'
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Traincategory(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`traincategory` (
                        `idtraincategory` int unsigned NOT NULL AUTO_INCREMENT,
                        `category` varchar(80) NOT NULL,
                        PRIMARY KEY (`idtraincategory`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Traindtl(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`traindtl` (
                        `IdTrainHdr` int unsigned NOT NULL,
                        `empnumber` char(5) NOT NULL,
                        `remarks` varchar(80) DEFAULT '',
                        PRIMARY KEY (`IdTrainHdr`,`empnumber`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trainhdr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trainhdr` (
                        `idtrainhdr` int unsigned NOT NULL AUTO_INCREMENT,
                        `transno` int unsigned DEFAULT NULL,
                        `idtraincategory` int unsigned DEFAULT NULL,
                        `idtrainProgram` int unsigned DEFAULT NULL,
                        `location` varchar(80) DEFAULT NULL,
                        `trainor` varchar(60) DEFAULT NULL,
                        `batch` char(10) DEFAULT NULL,
                        `dateStart` date DEFAULT NULL,
                        `dateEnd` date DEFAULT NULL,
                        `timeStart` datetime DEFAULT NULL,
                        `timeEnd` datetime DEFAULT NULL,
                        `Description` varchar(80) DEFAULT NULL,
                        `host` varchar(60) DEFAULT NULL,
                        `isFinalized` int unsigned DEFAULT '0',
                        `Type` char(10) DEFAULT NULL,
                        `Remarks` varchar(80) DEFAULT NULL,
                        PRIMARY KEY (`idtrainhdr`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trainingconfig(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trainingconfig` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `DispCnt` int unsigned NOT NULL,
                        `DefTimeStart` int unsigned NOT NULL,
                        `DefMinStart` int unsigned NOT NULL,
                        `DefTimeEnd` int unsigned NOT NULL,
                        `DefMinEnd` int unsigned NOT NULL,
                        `DefType` varchar(10) DEFAULT 'Internal',
                        PRIMARY KEY (`id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trainprogram(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trainprogram` (
                        `idtrainProgram` int unsigned NOT NULL AUTO_INCREMENT,
                        `Program` varchar(100) NOT NULL,
                        PRIMARY KEY (`idtrainProgram`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Traintransno(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`traintransno` (
                        `idtraintransno` int unsigned NOT NULL AUTO_INCREMENT,
                        `transno` int unsigned NOT NULL,
                        PRIMARY KEY (`idtraintransno`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Trainvaliduser(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trainvaliduser` (
                        `idtrainvaliduser` int unsigned NOT NULL AUTO_INCREMENT,
                        `logname` varchar(25) NOT NULL,
                        PRIMARY KEY (`idtrainvaliduser`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01TransCtr(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`trans_ctr` (
                        `id` int unsigned NOT NULL AUTO_INCREMENT,
                        `type` char(12) DEFAULT '',
                        `yr` int unsigned DEFAULT '0',
                        `mo` int unsigned DEFAULT '0',
                        `ctr` bigint unsigned DEFAULT '0',
                        PRIMARY KEY (`id`)
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
                        `LogName` varchar(25) NOT NULL,
                        `UserName` varchar(45) NOT NULL,
                        `Password` varchar(60) NOT NULL,
                        `status` varchar(1) NOT NULL DEFAULT 'A',
                        `withInsuranceAccess` int unsigned DEFAULT '0',
                        `Email` varchar(45) DEFAULT NULL,
                        PRIMARY KEY (`LogName`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Usrmenu(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`usrmenu` (
                        `user` varchar(25) NOT NULL,
                        `menucode_` varchar(5) NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Usrrights(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`usrrights` (
                        `idusrRights` int unsigned NOT NULL AUTO_INCREMENT,
                        `logname` varchar(25) NOT NULL,
                        `rDesc` varchar(45) NOT NULL,
                        PRIMARY KEY (`idusrRights`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01Version(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`version` (
                        `version` int unsigned NOT NULL AUTO_INCREMENT,
                        PRIMARY KEY (`version`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    // -- Tables added from the reference (192.168.0.103 secpis) --------------------
    private async Task _01ClientRateSettings(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`client_rate_settings` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `Pursuant` varchar(150) DEFAULT 'Article ...',
                        `Effectivity` date DEFAULT NULL,
                        `SignatoryName` varchar(60) DEFAULT 'Signatory',
                        `SignatoryDesignation` varchar(120) DEFAULT 'Designation',
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01ClientRateatrail(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`client_rateatrail` (
                        `Id` int unsigned NOT NULL AUTO_INCREMENT,
                        `ClNumber` char(5) DEFAULT '',
                        `SGCodeOld` char(10) DEFAULT NULL,
                        `SGCodeNew` char(10) DEFAULT NULL,
                        `RateOld` double(9,2) DEFAULT '0.00',
                        `RateNew` double(9,2) DEFAULT '0.00',
                        `DateChanged` datetime DEFAULT NULL,
                        `ChangedBy` varchar(25) DEFAULT NULL,
                        PRIMARY KEY (`Id`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }

    private async Task _01ClientRateatrailEmpmas(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`client_rateatrail_empmas` (
                        `CRTId` int NOT NULL DEFAULT '0',
                        `EmpNumber` char(5) NOT NULL DEFAULT '',
                        PRIMARY KEY (`CRTId`,`EmpNumber`)
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

    // -- Statuses shown in Employee Entry (2052); empty = all statuses --
    private async Task _01EmpmasVisibleStatus(string? schema, string? conn)
    {
        string? sql = $@"CREATE TABLE IF NOT EXISTS {schema}.`empmas_visible_status` (
                        `empstat_` char(2) NOT NULL,
                        PRIMARY KEY (`empstat_`)
                        ) ENGINE=InnoDB DEFAULT CHARSET=latin1;";
        await ExecLegacy(sql, conn);
    }
}
