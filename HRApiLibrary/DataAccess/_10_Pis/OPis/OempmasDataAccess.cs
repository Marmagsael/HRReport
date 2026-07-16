using Google.Protobuf.WellKnownTypes;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OEmpmasDataAccess : IOEmpmasDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

	public OEmpmasDataAccess(I_90_001_MySqlDataAccess sql)
	{
			_sql = sql;
	}

    public async Task _00CreateEmpmasMigration_Marker(string? oPisDb, string? conn)
    {
        string? sql = $@"SELECT '1' Column_Name
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = 'empmas'
                            AND COLUMN_NAME = 'IsMigrated'
                            AND TABLE_SCHEMA = '{oPisDb}';"; 
        var res = await _sql.FetchData<TableModel?, dynamic>(sql, new { }, conn);
        
        if (res == null || res.Count == 0) { 
            string? usql = $@"ALTER TABLE {oPisDb}.Empmas ADD COLUMN IsMigrated int? Default 0;"; 
            await _sql.ExecuteCmd<dynamic>(usql, new { }, conn);
        }
    }
    public async Task _00MigrateData(string? pisDb, string? oPisDb, string? conn)
    {
        
        
        var sql = $@"  drop table if exists tmp_users;
                       create temporary table tmp_users as select LoginName empnumber from main.Users; 
                       insert into main.users (loginName, Password,             Email, Domain,             UserType, Status, DefaultCoId)  
                       
                       select                  empnumber, sha2(empnumber, 512), email, 'morpheusbox.info', 1,        empstat_, 2
                     from {oPisDb}.Empmas e
                     where  IsMigrated = 0 
                            and empstat_ = 'A' 
                            and email is not null 
                            and email != '' 
                            and empnumber not in (select empnumber from tmp_users) ;";
        await _sql.ExecuteCmd<dynamic>(sql, new { }, conn);

        sql = $@"drop table if exists tmp_empmas;

                 create temporary table tmp_empmas as 
                 select u.id SystemId, e.Empnumber, e.EmplastNm, e.EmpFirstnm, e.EmpMidNm, e.Suffix,e.EmpAlias 
                 from main.users u
                 left join {pisDb}.empmas e on e.empnumber = u.loginName;

                insert into {pisDb}.empmas (SystemId,      EmpNumber,   EmpLastNm,   EmpFirstNm,   EmpMidNm,   Suffix,   EmpAlias)
                select                      u.id SystemId, u.loginName, e.EmplastNm, e.EmpFirstnm, e.EmpMidNm, e.Suffix, e.EmpAlias
                from main.users u
                    left join {oPisDb}.empmas e on e.empnumber = u.loginName
                    left join {pisDb}.empmas e1 on e1.systemId = u.Id
                where e1.systemId is null; ";
        var res = await _sql.FetchData<UsersModel?, dynamic>(sql, new { }, conn);
        
        sql = $@"update {oPisDb}.Empmas set IsMigrated = 1 
                 where IsMigrated = 0 and empstat_ = 'A' and email is not null and email != '' ;";
        await _sql.ExecuteCmd<dynamic>(sql, new { }, conn);

        
    }

    public async Task<OEmpmasModel?> _01(OEmpmasModel empmas, string? schema, string? conn )
	{
		string sql = $@"Insert into {schema}.Empmas 
    					(EMPNUMBER, EMPLASTNM, EMPFIRSTNM, EMPMIDNM, suffix, EMPALIAS, CLIENT, CLIENT_, BASICRATE, PAYTYPE, ADMIN, CASHBOND, WORKDAYS, 
                         ALLOWRATE, ALLOWTYPE, ALLOWFIX, ALLOW2RATE, ALLOW2TYPE, ALLOW2FIX, ALLOW3RATE, ALLOW3TYPE, ALLOW3FIX, ALLOW4RATE, ALLOW4TYPE, 
                         ALLOW4FIX, MOVNUMBER, MOVMODE, MOVDATE, MOVEND, DUTYDATE, ADDR1, MLACODE_, TEL1, ADDR2, PROCODE_, TEL2, EMPBIRTH, BIRTHPLACE, 
                         SEX_, CIVSTAT_, CITIZEN, HEIGHT, WEIGHT, TIN, SSS, HDMF, RELIGION, HAIR, EYES, SPOUSE, OCCUPATION, NOCHILDREN, DATEHIRED, 
                         SEPARATE, POSITION_, EMPSTAT_, STATUSDATE, SECLICENSE, LICEXPIRE, TRAINAT, DATETRAIN, INSURANCE, POLICYNO, FACEVALUE, PREMIUM, 
                         INSEXPIRE, EXMILITARY, CSP, CPP, ROTC, ELLEVEL, HSLEVEL, COLLEGE_, COURSE, VOLEVEL, VOCOURSE, LANGUAGE, SKILL1, SKILL2, SKILL3, 
                         SKILL4, TAXCODE, ACCTCODE, AWOL, DISMISS, ASTART, AEND, ADAYS, DSTART, DEND, DDAYS, EMRNAME, EMRTEL, EMRADDR, GUARDEXP, COMTAXNO, 
    					 COMTAXDATE, COMTAX_AT, BLOODTYPE, MARKS, COMPLEXION, EXP_NBI, EXP_POLICE, EXP_PNP, EXP_BRGY, EXP_COURT, EXP_NEURO, EXP_DRUG, 
    					 W_BIRTHC, W_CLOSINGR, W_TRNCERT, W_PRELIC, W_CERTEMP, W_MEDEXAM, GKERATE, CLNAME, MLANAME, AGE, MBRANCH, MYEAR, MNATURE, REMARKS, 
    					 BADGENO, GUARDNOYRS, MILITARYNOYR, PAGIBIGNO, PHIC, BANK, EXPMED, regref, empBasicRate, rateID, empEcola, xmark, suretybondquota, 
    					 DRV_LICENSE, DRV_EXP, isTaxable, isconfi, iswithSSS, iswithGSIS, iswithPHIC, iswithPagibig, ismaxsss, email, passwd, Countrycode, 
    					 sgcode, dpadate, dpclient) values 
    					(@EMPNUMBER, @EMPLASTNM, @EMPFIRSTNM, @EMPMIDNM, @suffix, @EMPALIAS, @CLIENT, @CLIENT_, @BASICRATE, @PAYTYPE, @ADMIN, @CASHBOND, @WORKDAYS, 
    					 @ALLOWRATE, @ALLOWTYPE, @ALLOWFIX, @ALLOW2RATE, @ALLOW2TYPE, @ALLOW2FIX, @ALLOW3RATE, @ALLOW3TYPE, @ALLOW3FIX, @ALLOW4RATE, @ALLOW4TYPE, 
    					 @ALLOW4FIX, @MOVNUMBER, @MOVMODE, @MOVDATE, @MOVEND, @DUTYDATE, @ADDR1, @MLACODE_, @TEL1, @ADDR2, @PROCODE_, @TEL2, @EMPBIRTH, @BIRTHPLACE, 
    					 @SEX_, @CIVSTAT_, @CITIZEN, @HEIGHT, @WEIGHT, @TIN, @SSS, @HDMF, @RELIGION, @HAIR, @EYES, @SPOUSE, @OCCUPATION, @NOCHILDREN, @DATEHIRED, 
    					 @SEPARATE, @POSITION_, @EMPSTAT_, @STATUSDATE, @SECLICENSE, @LICEXPIRE, @TRAINAT, @DATETRAIN, @INSURANCE, @POLICYNO, @FACEVALUE, @PREMIUM, 
    					 @INSEXPIRE, @EXMILITARY, @CSP, @CPP, @ROTC, @ELLEVEL, @HSLEVEL, @COLLEGE_, @COURSE, @VOLEVEL, @VOCOURSE, @LANGUAGE, @SKILL1, @SKILL2, @SKILL3, 
    					 @SKILL4, @TAXCODE, @ACCTCODE, @AWOL, @DISMISS, @ASTART, @AEND, @ADAYS, @DSTART, @DEND, @DDAYS, @EMRNAME, @EMRTEL, @EMRADDR, @GUARDEXP, @COMTAXNO, 
    					 @COMTAXDATE, @COMTAX_AT, @BLOODTYPE, @MARKS, @COMPLEXION, @EXP_NBI, @EXP_POLICE, @EXP_PNP, @EXP_BRGY, @EXP_COURT, @EXP_NEURO, @EXP_DRUG, 
    					 @W_BIRTHC, @W_CLOSINGR, @W_TRNCERT, @W_PRELIC, @W_CERTEMP, @W_MEDEXAM, @GKERATE, @CLNAME, @MLANAME, @AGE, @MBRANCH, @MYEAR, @MNATURE, @REMARKS, 
    					 @BADGENO, @GUARDNOYRS, @MILITARYNOYR, @PAGIBIGNO, @PHIC, @BANK, @EXPMED, @regref, @empBasicRate, @rateID, @empEcola, @xmark, @suretybondquota, 
    					 @DRV_LICENSE, @DRV_EXP, @isTaxable, @isconfi, @iswithSSS, @iswithGSIS, @iswithPHIC, @iswithPagibig, @ismaxsss, @email, @passwd, @Countrycode, 
    					 @sgcode, @dpadate, @dpclient)" ; 
		await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);

		sql = $@"SELECT * FROM {schema}.Empmas WHERE EmpNumber = @Empnumber"; 

		var res = await _sql.FetchData<OEmpmasModel?,dynamic>(sql,new {Empnumber=empmas.EmpNumber },conn);

		return res.FirstOrDefault();
	}


    public async Task<List<OEmpmasModel?>?> _02ByLNameAndFNames(string? name, string? schema, string? conn)
    {
        name = name.Trim();
        var flds = EmpmasFields(); 
        var sql = $@"select   {flds}, s.name EmpStatus, p.name PositionName, c.ClName 
                        from {schema}.Empmas e
                     left join {schema}.position    p on p.code = e.position_
                     left join {schema}.empstat     s on s.code = e.empstat_                              
                     left join {schema}.Client      c  on c.ClNumber = e.Client_                              
                     where e.EmpLastNm like @Name or e.EmpFirstNm like @Name
                     order by e.EmpLastNm, e.EmpFirstNm;";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Name = $"{name}%"}, conn);
        return data;    
    }

    public async Task<List<OEmpmasModel?>?> _02By1stLetterRange(string? firstLetter, string? secondLetter, string? schema = "MainPis", string? conn = "MySqlConn")
    {

        string? sql = $@"select e.Empnumber, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                        from {schema}.Empmas e 
                        where left(trim(e.EmpLastNm),1) between @FirstLetter and @SecondLetter
                        order by e.EmplastNm, e.EmpFirstNm";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { FirstLetter = firstLetter, SecondLetter = secondLetter }, conn);
        return data;
    }

    public async Task<List<OEmpmasModel?>?> _02SearchName(string? skey, string? schema = "MainPis", string? conn = "MySqlConn")
    {
        string? searchKey = $"{skey}%";
        string? sql = $@"select  e.Empnumber, e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm, concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                        from {schema}.Empmas e 
                        where e.EmpLastNm like @SearchKey or e.EmpFirstNm like @SearchKey
                        order by e.EmplastNm, e.EmpFirstNm";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { SearchKey = searchKey }, conn);
        return data;
    }


    public async Task<List<OEmpmasModel?>?> _02Migrated( string? schema, string? conn)
    {
        var sql = $@"select count(*) as CntNotMigrated from {schema}.Empmas e 
                     where  IsMigrated = 0 
                            and empstat_ = 'A' 
                            and email is not null 
                            and email != '';";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { }, conn);
        return data;    
    }


    public async Task<List<OEmpmasModel?>?> _02(string? empnumber, string? schema, string? conn)
    {
        var flds = EmpmasFields(); 

        var sql = $@"select   {flds}, s.name EmpStatus, p.name PositionName, c.ClName 
                        from {schema}.Empmas e
                     left join {schema}.position    p on p.code = e.position_
                     left join {schema}.empstat     s on s.code = e.empstat_                              
                     left join {schema}.Client      c  on c.ClNumber = e.Client_                              
                     where e.Empnumber = @Empnumber";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data;
    }

    public async Task<List<OEmpmasModel?>?> _02EmpnumberAndEmailOwnedByOthers(string? empnumber, string? email, string? schema, string? conn)
    {
        bool hasEmail = !string.IsNullOrWhiteSpace(email);

        var condition = hasEmail
                                ? "(LOWER(TRIM(e.Empnumber)) = LOWER(TRIM(@Empnumber)) OR LOWER(TRIM(e.Email)) = LOWER(TRIM(@Email)))"
                                : "LOWER(TRIM(e.Empnumber)) = LOWER(TRIM(@Empnumber))";

        var sql = $@"SELECT e.empnumber, e.email, 
                           concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                            FROM {schema}.Empmas e 
                            WHERE {condition}
                            UNION
                            SELECT e.empnumber, e.email, 
                                   concat(trim(e.EmpLastNm),', ' , trim(e.EmpFirstNm),' ', trim(e.EmpMidNm)) FullName 
                            FROM {schema}.empmasarchieved e 
                            WHERE {condition}";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>( sql, new { Empnumber = empnumber, Email = email }, conn);

        return data;
    }


    public async Task<List<OEmpmasModel?>?> _02(string? empnumber, string? olddb, string? maindb,  string? newdb ,string? conn)
    {
        var flds = EmpmasFields();

        var sql = $@"select   {flds}, s.name EmpStatus, p.name PositionName, c.ClName, ne.SystemId UserId, ne.Id EmpmasId 
                        from {olddb}.Empmas e
                     left join {olddb}.position    p on p.code = e.position_
                     left join {olddb}.empstat     s on s.code = e.empstat_                              
                     left join {olddb}.Client      c on c.ClNumber = e.Client_                              
                     left join {newdb}.empmas      ne on ne.Empnumber = e.Empnumber                              
                     where e.Empnumber = @Empnumber";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data;
    }


    public async Task<List<OEmpmasModel?>?> _02ByClNumbers(string? clnumber, string? schema, string? conn)
    {
        var usql = $@"UPDATE {schema}.Empmas SET
                        MovDate    = IF(MovDate    < '1000-01-01', NULL, MovDate),
                        MovEnd     = IF(MovEnd     < '1000-01-01', NULL, MovEnd),
                        DutyDate   = IF(DutyDate   < '1000-01-01', NULL, DutyDate),
                        EmpBirth   = IF(EmpBirth   < '1000-01-01', NULL, EmpBirth),
                        DateHired  = IF(DateHired  < '1000-01-01', NULL, DateHired),
                        Separate   = IF(Separate   < '1000-01-01', NULL, Separate),
                        StatusDate = IF(StatusDate < '1000-01-01', NULL, StatusDate),
                        LicExpire  = IF(LicExpire  < '1000-01-01', NULL, LicExpire),
                        DateTrain  = IF(DateTrain  < '1000-01-01', NULL, DateTrain),
                        InsExpire  = IF(InsExpire  < '1000-01-01', NULL, InsExpire),
                        AStart     = IF(AStart     < '1000-01-01', NULL, AStart),
                        AEnd       = IF(AEnd       < '1000-01-01', NULL, AEnd),
                        DStart     = IF(DStart     < '1000-01-01', NULL, DStart),
                        DEnd       = IF(DEnd       < '1000-01-01', NULL, DEnd),
                        ComTaxDate = IF(ComTaxDate < '1000-01-01', NULL, ComTaxDate),
                        Exp_Nbi    = IF(Exp_Nbi    < '1000-01-01', NULL, Exp_Nbi),
                        Exp_Police = IF(Exp_Police < '1000-01-01', NULL, Exp_Police),
                        Exp_Pnp    = IF(Exp_Pnp    < '1000-01-01', NULL, Exp_Pnp),
                        Exp_Brgy   = IF(Exp_Brgy   < '1000-01-01', NULL, Exp_Brgy),
                        Exp_Court  = IF(Exp_Court  < '1000-01-01', NULL, Exp_Court),
                        Exp_Neuro  = IF(Exp_Neuro  < '1000-01-01', NULL, Exp_Neuro),
                        Exp_Drug   = IF(Exp_Drug   < '1000-01-01', NULL, Exp_Drug),
                        ExpMed     = IF(ExpMed     < '1000-01-01', NULL, ExpMed),
                        RegRef     = IF(RegRef     < '1000-01-01', NULL, RegRef),
                        Drv_Exp    = IF(Drv_Exp    < '1000-01-01', NULL, Drv_Exp),
                        DpaDate    = IF(DpaDate    < '1000-01-01', NULL, DpaDate)
                    WHERE Client_ = @ClNumber; "; 
        await _sql.ExecuteCmd<dynamic>(usql, new { ClNumber = clnumber }, conn);
        
        var sql = $@" select  s.name EmpStatus, p.name PositionName, c.ClName, 
                        e.* from {schema}.Empmas e
                     left join {schema}.position    p on p.code = e.position_
                     left join {schema}.empstat     s on s.code = e.empstat_                              
                     left join {schema}.Client      c  on c.ClNumber = e.Client_                              
                     where e.Client_ = @ClNumber; ";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { ClNumber = clnumber }, conn);
        return data;
    }


    public async Task<List<OEmpmasModel?>?> _02ByEmail(string? email, string? schema, string? conn)
	{

        var usql = $@"UPDATE {schema}.Empmas SET
                        MovDate    = IF(MovDate    < '0001-01-01', NULL, MovDate),
                        MovEnd     = IF(MovEnd     < '0001-01-01', NULL, MovEnd),
                        DUTYDATE   = IF(DUTYDATE   < '0001-01-01', NULL, DUTYDATE),
                        EmpBirth   = IF(EmpBirth   < '0001-01-01', NULL, EmpBirth),
                        DateHired  = IF(DateHired  < '0001-01-01', NULL, DateHired),
                        Separate   = IF(Separate   < '0001-01-01', NULL, Separate),
                        StatusDate = IF(StatusDate < '0001-01-01', NULL, StatusDate),
                        LicExpire  = IF(LicExpire  < '0001-01-01', NULL, LicExpire),
                        DateTrain  = IF(DateTrain  < '0001-01-01', NULL, DateTrain),
                        InsExpire  = IF(InsExpire  < '0001-01-01', NULL, InsExpire),
                        AStart     = IF(AStart     < '0001-01-01', NULL, AStart),
                        AEnd       = IF(AEnd       < '0001-01-01', NULL, AEnd),
                        DStart     = IF(DStart     < '0001-01-01', NULL, DStart),
                        DEnd       = IF(DEnd       < '0001-01-01', NULL, DEnd),
                        ComTaxDate = IF(ComTaxDate < '0001-01-01', NULL, ComTaxDate),
                        Exp_Nbi    = IF(Exp_Nbi    < '0001-01-01', NULL, Exp_Nbi),
                        Exp_Police = IF(Exp_Police < '0001-01-01', NULL, Exp_Police),
                        Exp_Pnp    = IF(Exp_Pnp    < '0001-01-01', NULL, Exp_Pnp),
                        Exp_Brgy   = IF(Exp_Brgy   < '0001-01-01', NULL, Exp_Brgy),
                        Exp_Court  = IF(Exp_Court  < '0001-01-01', NULL, Exp_Court),
                        Exp_Neuro  = IF(Exp_Neuro  < '0001-01-01', NULL, Exp_Neuro),
                        Exp_Drug   = IF(Exp_Drug   < '0001-01-01', NULL, Exp_Drug),
                        ExpMed     = IF(ExpMed     < '0001-01-01', NULL, ExpMed),
                        RegRef     = IF(RegRef     < '0001-01-01', NULL, RegRef),
                        Drv_Exp    = IF(Drv_Exp    < '0001-01-01', NULL, Drv_Exp),
                        DpaDate    = IF(DpaDate    < '0001-01-01', NULL, DpaDate)
                    WHERE Email = @Email;"; 
        await _sql.ExecuteCmd<dynamic>(usql, new { Email = email }, conn);

        var sql = $@"SELECT s.name AS EmpStatus, p.name AS PositionName, c.ClName,
                        e.* from {schema}.Empmas e
                     left join   {schema}.position    p on p.code = e.position_
                     left join   {schema}.empstat     s on s.code = e.empstat_                              
                     left join   {schema}.Client      c  on c.ClNumber = e.Client_
                     where e.Email = @Email" ; 
		var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Email = email }, conn); 
		return data;
	}


    public async Task<OEmpmasModel?>_02MaxEmpnumber( string? schema, string? conn)
    {

        var sql = $@"SELECT CASE
                                WHEN a.MaxEmp >= b.MaxEmp THEN a.MaxEmp
                                ELSE b.MaxEmp
                            END AS Empnumber
                        FROM
                            (SELECT MAX(cast(empnumber as signed)) AS MaxEmp FROM {schema}.empmas) a
                        CROSS JOIN
                            (SELECT MAX(cast(empnumber as signed)) AS MaxEmp FROM {schema}.empmasarchieved) b;";
      
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new {}, conn);
        return data.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03(string? empnumber, OEmpmasModel empmas, string? schema, string? conn)
	{
		string sql = $@"Update {schema}.Empmas set 
                               EMPNUMBER = @EMPNUMBER, 
                               EMPLASTNM = @EMPLASTNM, 
                               EMPFIRSTNM = @EMPFIRSTNM, 
                               EMPMIDNM = @EMPMIDNM, 
                               suffix = @suffix, 
                               EMPALIAS = @EMPALIAS, 
                               CLIENT = @CLIENT, 
                               CLIENT_ = @CLIENT_, BASICRATE = @BASICRATE, PAYTYPE = @PAYTYPE, ADMIN = @ADMIN, 
                               CASHBOND = @CASHBOND, WORKDAYS = @WORKDAYS, ALLOWRATE = @ALLOWRATE, 
                               ALLOWTYPE = @ALLOWTYPE, ALLOWFIX = @ALLOWFIX, ALLOW2RATE = @ALLOW2RATE, 
                               ALLOW2TYPE = @ALLOW2TYPE, ALLOW2FIX = @ALLOW2FIX, ALLOW3RATE = @ALLOW3RATE, 
                               ALLOW3TYPE = @ALLOW3TYPE, ALLOW3FIX = @ALLOW3FIX, ALLOW4RATE = @ALLOW4RATE, 
                               ALLOW4TYPE = @ALLOW4TYPE, ALLOW4FIX = @ALLOW4FIX, MOVNUMBER = @MOVNUMBER, 
                               MOVMODE = @MOVMODE, MOVDATE = @MOVDATE, MOVEND = @MOVEND, DUTYDATE = @DUTYDATE, 
                               ADDR1 = @ADDR1, MLACODE_ = @MLACODE_, TEL1 = @TEL1, ADDR2 = @ADDR2, PROCODE_ = @PROCODE_, 
                               TEL2 = @TEL2, EMPBIRTH = @EMPBIRTH, BIRTHPLACE = @BIRTHPLACE, SEX_ = @SEX_, 
                               CIVSTAT_ = @CIVSTAT_, CITIZEN = @CITIZEN, HEIGHT = @HEIGHT, WEIGHT = @WEIGHT, 
                               TIN = @TIN, SSS = @SSS, HDMF = @HDMF, RELIGION = @RELIGION, HAIR = @HAIR, 
                               EYES = @EYES, SPOUSE = @SPOUSE, OCCUPATION = @OCCUPATION, NOCHILDREN = @NOCHILDREN, 
                               DATEHIRED = @DATEHIRED, SEPARATE = @SEPARATE, POSITION_ = @POSITION_, 
                               EMPSTAT_ = @EMPSTAT_, STATUSDATE = @STATUSDATE, SECLICENSE = @SECLICENSE, 
                               LICEXPIRE = @LICEXPIRE, TRAINAT = @TRAINAT, DATETRAIN = @DATETRAIN, 
                               INSURANCE = @INSURANCE, POLICYNO = @POLICYNO, FACEVALUE = @FACEVALUE, PREMIUM = @PREMIUM, 
                               INSEXPIRE = @INSEXPIRE, EXMILITARY = @EXMILITARY, 
                               CSP = @CSP, CPP = @CPP, ROTC = @ROTC, ELLEVEL = @ELLEVEL, 
                               HSLEVEL = @HSLEVEL, COLLEGE_ = @COLLEGE_, COURSE = @COURSE, 
                               VOLEVEL = @VOLEVEL, VOCOURSE = @VOCOURSE, LANGUAGE = @LANGUAGE, 
                               SKILL1 = @SKILL1, SKILL2 = @SKILL2, SKILL3 = @SKILL3, SKILL4 = @SKILL4, 
                               TAXCODE = @TAXCODE, ACCTCODE = @ACCTCODE, AWOL = @AWOL, DISMISS = @DISMISS, 
                               ASTART = @ASTART, AEND = @AEND, ADAYS = @ADAYS, DSTART = @DSTART, DEND = @DEND, 
                               DDAYS = @DDAYS, EMRNAME = @EMRNAME, EMRTEL = @EMRTEL, EMRADDR = @EMRADDR, 
                               GUARDEXP = @GUARDEXP, COMTAXNO = @COMTAXNO, COMTAXDATE = @COMTAXDATE, 
                               COMTAX_AT = @COMTAX_AT, BLOODTYPE = @BLOODTYPE, MARKS = @MARKS, COMPLEXION = @COMPLEXION, 
                               EXP_NBI = @EXP_NBI, EXP_POLICE = @EXP_POLICE, EXP_PNP = @EXP_PNP, EXP_BRGY = @EXP_BRGY, 
                               EXP_COURT = @EXP_COURT, EXP_NEURO = @EXP_NEURO, EXP_DRUG = @EXP_DRUG, W_BIRTHC = @W_BIRTHC, 
                               W_CLOSINGR = @W_CLOSINGR, W_TRNCERT = @W_TRNCERT, W_PRELIC = @W_PRELIC, W_CERTEMP = @W_CERTEMP, 
                               W_MEDEXAM = @W_MEDEXAM, GKERATE = @GKERATE, CLNAME = @CLNAME, MLANAME = @MLANAME, 
                               AGE = @AGE, MBRANCH = @MBRANCH, MYEAR = @MYEAR, MNATURE = @MNATURE, REMARKS = @REMARKS, 
                               BADGENO = @BADGENO, GUARDNOYRS = @GUARDNOYRS, MILITARYNOYR = @MILITARYNOYR, PAGIBIGNO = @PAGIBIGNO, 
                               PHIC = @PHIC, BANK = @BANK, EXPMED = @EXPMED, regref = @regref, empBasicRate = @empBasicRate, 
                               rateID = @rateID, empEcola = @empEcola, xmark = @xmark, suretybondquota = @suretybondquota, 
                               DRV_LICENSE = @DRV_LICENSE, DRV_EXP = @DRV_EXP, isTaxable = @isTaxable, isconfi = @isconfi, 
                               iswithSSS = @iswithSSS, iswithGSIS = @iswithGSIS, iswithPHIC = @iswithPHIC, iswithPagibig = @iswithPagibig,
                               ismaxsss = @ismaxsss, email = @email, passwd = @passwd, Countrycode = @Countrycode, 
                               sgcode = @sgcode, dpadate = @dpadate, dpclient = @dpclient where Empnumber = @Empnumber;"; 
		await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);
		
		sql = $@" select  * from {schema}.Empmas x where x.Empnumber = @Empnumber ;";
		var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
		return data?.FirstOrDefault();
	}


    // Header: Employee Name
   public async Task<OEmpmasModel?> _03_EmployeeName(string selectedEmpnumber, OEmpmasModel empmas, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                    EMPLASTNM  = @EmpLastNm,
                    EMPFIRSTNM = @EmpFirstNm,
                    EMPMIDNM   = @EmpMidNm,
                    SUFFIX     = @Suffix,
                    EMPALIAS   = @EmpAlias,
                    EMPNUMBER  = @EmpNumber,
                    EMAIL      = @Email
                    WHERE EMPNUMBER = @SelEmpnumber";

        await _sql.ExecuteCmd<dynamic>(sql, new
        {
            SelEmpnumber = selectedEmpnumber,
            empmas.EmpLastNm,
            empmas.EmpFirstNm,
            empmas.EmpMidNm,
            empmas.Suffix,
            empmas.EmpAlias,
            empmas.EmpNumber,
            empmas.Email
        }, conn);

        sql = $@"SELECT EMPLASTNM, EMPFIRSTNM, EMPMIDNM, SUFFIX, EMPALIAS, EMPNUMBER, EMAIL
             FROM {schema}.Empmas
             WHERE Empnumber = @EmpNumber";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { empmas.EmpNumber }, conn);
        return data?.FirstOrDefault();
    }
    // Tab: Personal Data
    public async Task<OEmpmasModel?> _03_PersonalData(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                    EMPBIRTH = @EMPBIRTH, BIRTHPLACE = @BIRTHPLACE, SEX_ = @SEX_,
                    CIVSTAT_ = @CIVSTAT_, CITIZEN = @CITIZEN, HEIGHT = @HEIGHT, 
                    WEIGHT = @WEIGHT, BLOODTYPE = @BLOODTYPE, RELIGION = @RELIGION,
                    HAIR = @HAIR, EYES = @EYES, MARKS = @MARKS, COMPLEXION = @COMPLEXION,
                    AGE = @AGE, SPOUSE = @SPOUSE, OCCUPATION =@OCCUPATION, NOCHILDREN =@NOCHILDREN
                    WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT EMPBIRTH, BIRTHPLACE, SEX_, CIVSTAT_, CITIZEN, HEIGHT, 
        WEIGHT, BLOODTYPE, RELIGION, HAIR, EYES, MARKS, COMPLEXION, AGE, EMPNUMBER
        FROM {schema}.Empmas WHERE Empnumber = @Empnumber";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Address(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                        EMAIL = @EMAIL,
                        ADDR1 = @ADDR1, MLACODE_ = @MLACODE_, TEL1 = @TEL1,
                        ADDR2 = @ADDR2, PROCODE_ = @PROCODE_, TEL2 = @TEL2,
                        CLNAME = @CLNAME, MLANAME = @MLANAME, Countrycode = @Countrycode
                        WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT  EMAIL, ADDR1, MLACODE_, TEL1, ADDR2, PROCODE_, TEL2, 
                CLNAME, MLANAME, Countrycode, EMPNUMBER
                FROM {schema}.Empmas WHERE Empnumber = @Empnumber";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Deployment(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                    DATEHIRED = @DATEHIRED, POSITION_ = @POSITION_, EMPSTAT_ = @EMPSTAT_, REGREF = @REGREF
                    WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT  POSITION_, EMPSTAT_,  
                if(e.DATEHIRED  < '1000-01-01', null, DATEHIRED  )  as DATEHIRED  ,       
                if(e.REGREF  < '1000-01-01', null, REGREF  )  as REGREF 
                FROM {schema}.Empmas e WHERE Empnumber = @Empnumber";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Government(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                     SSS = @SSS, TIN = @TIN,  PAGIBIGNO = @PAGIBIGNO,  PHIC = @PHIC, HDMF = @HDMF,
                     Drv_License = @Drv_License, Drv_Exp =@Drv_Exp, Hdmf =@Hdmf, AcctCode =@AcctCode, 
                     TAXCODE = @TAXCODE, Bank =@Bank
                   
                    WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT  SSS, TIN, PAGIBIGNO, PHIC, HDMF, Drv_License, Drv_Exp, Hdmf, AcctCode,  TAXCODE, Bank, EMPNUMBER
                FROM {schema}.Empmas WHERE Empnumber = @Empnumber";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Insurance(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                INSURANCE = @INSURANCE, POLICYNO = @POLICYNO, FACEVALUE = @FACEVALUE,
                PREMIUM = @PREMIUM, INSEXPIRE = @INSEXPIRE
                WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT INSURANCE, POLICYNO, FACEVALUE, PREMIUM, INSEXPIRE, EMPNUMBER
                FROM {schema}.Empmas WHERE Empnumber = @Empnumber";
        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Education(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                ELLEVEL = @ELLEVEL, HSLEVEL = @HSLEVEL, COLLEGE_ = @COLLEGE_,
                COURSE = @COURSE, VOLEVEL = @VOLEVEL, VOCOURSE = @VOCOURSE,
                LANGUAGE = @LANGUAGE, SKILL1 = @SKILL1, SKILL2 = @SKILL2,
                SKILL3 = @SKILL3, SKILL4 = @SKILL4
                WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT ELLEVEL, HSLEVEL, COLLEGE_, COURSE, VOLEVEL, VOCOURSE,
            LANGUAGE, SKILL1, SKILL2, SKILL3, SKILL4, EMPNUMBER
            FROM {schema}.Empmas WHERE Empnumber = @Empnumber";

        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OEmpmasModel?> _03_Security(string empnumber, OEmpmasModel m, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Empmas SET
                    SECLICENSE = @SECLICENSE, LICEXPIRE = @LICEXPIRE,
                    GUARDEXP = @GUARDEXP, GUARDNOYRS = @GUARDNOYRS,
                    EXMILITARY = @EXMILITARY, MILITARYNOYR = @MILITARYNOYR,
                    CSP = @CSP, CPP = @CPP, ROTC = @ROTC,
                    EXP_NBI = @EXP_NBI, EXP_POLICE = @EXP_POLICE, EXP_PNP = @EXP_PNP,
                    EXP_BRGY = @EXP_BRGY, EXP_COURT = @EXP_COURT,
                    EXP_NEURO = @EXP_NEURO, EXP_DRUG = @EXP_DRUG,
                    BADGENO = @BADGENO, DRV_LICENSE = @DRV_LICENSE, DRV_EXP = @DRV_EXP,
                    W_BIRTHC = @W_BIRTHC, W_CLOSINGR = @W_CLOSINGR, W_TRNCERT = @W_TRNCERT,
                    W_PRELIC = @W_PRELIC, W_CERTEMP = @W_CERTEMP, W_MEDEXAM = @W_MEDEXAM
                    WHERE EMPNUMBER = @EMPNUMBER";
        await _sql.ExecuteCmd<dynamic>(sql, m, conn);

        sql = $@"SELECT SECLICENSE, LICEXPIRE, GUARDEXP, GUARDNOYRS, EXMILITARY, MILITARYNOYR,
                    CSP, CPP, ROTC, EXP_NBI, EXP_POLICE, EXP_PNP, EXP_BRGY, EXP_COURT,
                    EXP_NEURO, EXP_DRUG, BADGENO, DRV_LICENSE, DRV_EXP,
                    W_BIRTHC, W_CLOSINGR, W_TRNCERT, W_PRELIC, W_CERTEMP, W_MEDEXAM, EMPNUMBER
                    FROM {schema}.Empmas WHERE Empnumber = @Empnumber";


        var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }



    public async Task<OEmpmasModel?> _04(int? id, string? schema, string? conn)
	{
		string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
		// await _sql.ExecuteCmd<dynamic>(sql, new {Id=id},conn);

		sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
		var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

        // --- Private Functions ------------------------------------------------------------------------------------------------

    private string? EmpmasFields()
    {
        return @"CONCAT_WS(' ', NULLIF(TRIM(e.EmpLastNm), ', '), NULLIF(TRIM(e.EmpFirstNm), ' '), NULLIF(TRIM(e.EmpMidNm), ' ') ) AS EmpName,
                            e.Empnumber, 
                            e.Emplastnm, 
                            e.Empfirstnm, 
                            e.Empmidnm, 
                            e.Suffix, 
                            e.Empalias, 
                            e.Client, 
                            e.Client_, 
                            e.Basicrate, 
                            e.Paytype, 
                            e.Admin, 
                            e.Cashbond, 
                            e.Workdays, 
                            e.Allowrate, 
                            e.Allowtype, 
                            e.Allowfix, 
                            e.Allow2Rate, 
                            e.Allow2Type, 
                            e.Allow2Fix, 
                            e.Allow3Rate, 
                            e.Allow3Type, 
                            e.Allow3Fix, 
                            e.Allow4Rate, 
                            e.Allow4Type, 
                            e.Allow4Fix, 
                            e.Movnumber, 
                            e.Movmode, 
                            e.Addr1, 
                            e.Mlacode_, 
                            e.Tel1, 
                            e.Addr2, 
                            e.Procode_, 
                            e.Tel2, 
                            e.Birthplace, 
                            e.Sex_, 
                            e.Civstat_, 
                            e.Citizen, 
                            e.Height, 
                            e.Weight, 
                            e.Tin, 
                            e.Sss, 
                            e.Hdmf, 
                            e.Religion, 
                            e.Hair, 
                            e.Eyes, 
                            e.Spouse, 
                            e.Occupation, 
                            e.Nochildren, 
                            e.Position_, 
                            e.Empstat_, 
                            e.Seclicense, 
                            e.Trainat, 
                            e.Insurance, 
                            e.Policyno, 
                            e.Facevalue, 
                            e.Premium, 
                            e.Exmilitary, 
                            e.Csp, 
                            e.Cpp, 
                            e.Rotc, 
                            e.Ellevel, 
                            e.Hslevel, 
                            e.College_, 
                            e.Course, 
                            e.Volevel, 
                            e.Vocourse, 
                            e.Language, 
                            e.Skill1, 
                            e.Skill2, 
                            e.Skill3, 
                            e.Skill4, 
                            e.Taxcode, 
                            e.Acctcode, 
                            e.Awol, 
                            e.Dismiss, 
                            e.Adays, 
                            e.Ddays, 
                            e.Emrname, 
                            e.Emrtel, 
                            e.Emraddr, 
                            e.Guardexp, 
                            e.Comtaxno, 
                            e.Comtax_At, 
                            e.Bloodtype, 
                            e.Marks, 
                            e.Complexion, 
                            e.W_Birthc, 
                            e.W_Closingr, 
                            e.W_Trncert, 
                            e.W_Prelic, 
                            e.W_Certemp, 
                            e.W_Medexam, 
                            e.Gkerate, 
                            e.Clname, 
                            e.Mlaname, 
                            e.Age, 
                            e.Mbranch, 
                            e.Myear, 
                            e.Mnature, 
                            e.Remarks, 
                            e.Badgeno, 
                            e.Guardnoyrs, 
                            e.Militarynoyr, 
                            e.Pagibigno, 
                            e.Phic, 
                            e.Bank, 
                            e.Empbasicrate, 
                            e.Rateid, 
                            e.Empecola, 
                            e.Xmark, 
                            e.Suretybondquota, 
                            e.Drv_License, 
                            e.Istaxable, 
                            e.Isconfi, 
                            e.Iswithsss, 
                            e.Iswithgsis, 
                            e.Iswithphic, 
                            e.Iswithpagibig, 
                            e.Ismaxsss, 
                            e.Email, 
                            e.Passwd, 
                            e.Countrycode, 
                            e.Sgcode, 
                            e.Dpclient, 
                            e.Desig_, 

                            -- DATE NORMALIZATION
                            if(e.Movdate    < '1000-01-01', null, Movdate    )  as Movdate    ,                               
                            if(e.Movend     < '1000-01-01', null, Movend     )  as Movend     ,                           
                            if(e.Dutydate   < '1000-01-01', null, Dutydate   )  as Dutydate   ,                               
                            if(e.Empbirth   < '1000-01-01', null, Empbirth   )  as Empbirth   ,                               
                            if(e.Datehired  < '1000-01-01', null, Datehired  )  as Datehired  ,                               
                            if(e.Separate   < '1000-01-01', null, Separate   )  as Separate   ,                               
                            if(e.Statusdate < '1000-01-01', null, Statusdate )  as Statusdate ,                               
                            if(e.Licexpire  < '1000-01-01', null, Licexpire  )  as Licexpire  ,                               
                            if(e.Datetrain  < '1000-01-01', null, Datetrain  )  as Datetrain  ,                               
                            if(e.Insexpire  < '1000-01-01', null, Insexpire  )  as Insexpire  ,                               
                            if(e.Astart     < '1000-01-01', null, Astart     )  as Astart     ,                           
                            if(e.Aend       < '1000-01-01', null, Aend       )  as Aend       ,                           
                            if(e.Dstart     < '1000-01-01', null, Dstart     )  as Dstart     ,                           
                            if(e.Dend       < '1000-01-01', null, Dend       )  as Dend       ,                           
                            if(e.Comtaxdate < '1000-01-01', null, Comtaxdate )  as Comtaxdate ,                               
                            if(e.Exp_Nbi    < '1000-01-01', null, Exp_Nbi    )  as Exp_Nbi    ,                               
                            if(e.Exp_Police < '1000-01-01', null, Exp_Police )  as Exp_Police ,                               
                            if(e.Exp_Pnp    < '1000-01-01', null, Exp_Pnp    )  as Exp_Pnp    ,                               
                            if(e.Exp_Brgy   < '1000-01-01', null, Exp_Brgy   )  as Exp_Brgy   ,                               
                            if(e.Exp_Court  < '1000-01-01', null, Exp_Court  )  as Exp_Court  ,                               
                            if(e.Exp_Neuro  < '1000-01-01', null, Exp_Neuro  )  as Exp_Neuro  ,                               
                            if(e.Exp_Drug   < '1000-01-01', null, Exp_Drug   )  as Exp_Drug   ,                               
                            if(e.Expmed     < '1000-01-01', null, Expmed     )  as Expmed     ,                           
                            if(e.Regref     < '1000-01-01', null, Regref     )  as Regref     ,                           
                            if(e.Drv_Exp    < '1000-01-01', null, Drv_Exp    )  as Drv_Exp    ,                               
                            if(e.Dpadate    < '1000-01-01', null, Dpadate    )  as Dpadate " ; 

    }

	

}


public interface IOEmpmasDataAccess
{
    Task                        _00CreateEmpmasMigration_Marker(string? oPisDb, string? conn);
    Task                        _00MigrateData(string? pisDb, string? oPisDb, string? conn);
    Task<OEmpmasModel?>         _01(OEmpmasModel empmas, string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02(string? empnumber, string? schema, string? conn);
    Task<List<OEmpmasModel?>?> _02(string? empnumber, string? olddb, string? maindb, string? newdb, string? conn);
    Task<List<OEmpmasModel?>?>  _02Migrated(string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02ByLNameAndFNames(string? name, string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02By1stLetterRange(string? firstLetter, string? secondLetter, string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02SearchName(string? skey, string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02ByClNumbers(string? clnumber, string? schema, string? conn);
    Task<List<OEmpmasModel?>?>  _02ByEmail(string? email, string? schema, string? conn);
    Task<List<OEmpmasModel?>?> _02EmpnumberAndEmailOwnedByOthers(string? empnumber, string? email, string? schema, string? conn);
    Task<OEmpmasModel?>         _02MaxEmpnumber(string? schema, string? conn);

    Task<OEmpmasModel?>         _03(string? empnumber, OEmpmasModel empmas, string? schema, string? conn);

    Task<OEmpmasModel?>         _03_EmployeeName(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_PersonalData(string empnumber, OEmpmasModel mempmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Address(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Deployment(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Government(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Insurance(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Education(string empnumber, OEmpmasModel empmas, string schema, string conn);
    Task<OEmpmasModel?>         _03_Security(string empnumber, OEmpmasModel mempmas, string schema, string conn);

    Task<OEmpmasModel?>         _04(int? id, string? schema, string? conn);



}