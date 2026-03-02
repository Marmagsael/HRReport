using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OEmpmasDataAccess : IOEmpmasDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

	public OEmpmasDataAccess(I_90_001_MySqlDataAccess sql)
	{
			_sql = sql;
	}

	public async Task<OEmpmasModel?> _01(OEmpmasModel empmas, string schema, string conn )
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


    public async Task<List<OEmpmasModel?>?> _02(string empnumber, string schema, string conn)
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
    
    public async Task<List<OEmpmasModel?>?> _02ByClNumbers(string clnumber, string schema, string conn)
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


    public async Task<List<OEmpmasModel?>?> _02ByEmail(string email, string schema, string conn)
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
	

	
	public async Task<OEmpmasModel?> _03(int id,OEmpmasModel empmas, string schema, string conn)
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
                               sgcode = @sgcode, dpadate = @dpadate, dpclient = @dpclient where Id = @Id;"; 
		await _sql.ExecuteCmd<dynamic>(sql, empmas, conn);
		
		sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
		var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

	public async Task<OEmpmasModel?> _04(int id, string schema, string conn)
	{
		string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
		// await _sql.ExecuteCmd<dynamic>(sql, new {Id=id},conn);

		sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
		var data = await _sql.FetchData<OEmpmasModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

        // --- Private Functions ------------------------------------------------------------------------------------------------

    private string EmpmasFields()
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
	Task<OEmpmasModel?>         _01(OEmpmasModel empmas, string schema, string conn);
	Task<List<OEmpmasModel?>?>  _02(string empnumber, string schema, string conn); 
    Task<List<OEmpmasModel?>?>  _02ByClNumbers(string clnumber, string schema, string conn); 
	Task<List<OEmpmasModel?>?>  _02ByEmail(string email, string schema, string conn); 
	
}
