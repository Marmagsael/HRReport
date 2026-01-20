using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OempmasDataAccess : IOempmasDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

	public OempmasDataAccess(I_90_001_MySqlDataAccess sql)
	{
			_sql = sql;
	}

	public async Task<OempmasModel?> _01(OempmasModel empmas, string schema, string conn )
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

		var res = await _sql.FetchData<OempmasModel?,dynamic>(sql,new {Empnumber=empmas.EmpNumber },conn);

		return res.FirstOrDefault();
	}

	
	public async Task<List<OempmasModel?>?> _02(string empnumber, string schema, string conn)
	{
		var  sql = $@"select  * from {schema}.Empmas where Empnumber = @Empnumber" ; 
		var data = await _sql.FetchData<OempmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn); 
		return data;
	}
	
	public async Task<List<OempmasModel?>?> _02ByEmail(string email, string schema, string conn)
	{
		var sql = $@"select  * from {schema}.Empmas where Email = @Email" ; 
		var data = await _sql.FetchData<OempmasModel?, dynamic>(sql, new { Email = email }, conn); 
		return data;
	}
	

	
	public async Task<OempmasModel?> _03(int id,OempmasModel empmas, string schema, string conn)
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
		var data = await _sql.FetchData<OempmasModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}

	public async Task<OempmasModel?> _04(int id, string schema, string conn)
	{
		string sql = $@"Delete from {schema}.Empmas where Id = @Id;";
		// await _sql.ExecuteCmd<dynamic>(sql, new {Id=id},conn);

		sql = $@" select  * from {schema}.Empmas x where x.Id = @Id ;";
		var data = await _sql.FetchData<OempmasModel?, dynamic>(sql, new { Id = id }, conn);
		return data?.FirstOrDefault();
	}
	

}


public interface IOempmasDataAccess
{
	Task<OempmasModel?> _01(OempmasModel empmas, string schema, string conn);
	Task<List<OempmasModel?>?> _02(string empnumber, string schema, string conn); 
	Task<List<OempmasModel?>?> _02ByEmail(string email, string schema, string conn); 
	
}
