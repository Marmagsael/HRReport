using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OPisReportDataAccess : IOPisReportDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OPisReportDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async Task<List<OClientModel>> _02Client(string schema, string conn)
    {
        string sql = $@"select  CLNUMBER, CLNAME, ADDR1, ADDR2, AREACODE, TEL1, FAXNO, PARENT, RATE, BILLRATE, ASSIST, STATUS, COLARATE, 
                            ND_RATE, RETIRATE, UNIFRATE, FDIRATE, OTRATE, TIN, CONT, USED, CONTACT, POSTPERIOD, BATCHX, FSSSEE, FSSSER, 
                            FECC, FMEDEE, FMEDER, Remarks, 
                            IF(contStart    IN ('0000-00-00','0000-00-00 00:00:00'), NULL, contStart)   AS contStart,
                            IF(contEnd      IN ('0000-00-00','0000-00-00 00:00:00'), NULL, contEnd)     AS contEnd,
                            parentcd, maxsss, maxphic, 
                            IF(ContExp      IN ('0000-00-00','0000-00-00 00:00:00'), NULL, ContExp)     AS ContExp,
                            HavTax, MinRate, MealAllow, 
                            withUniform, withRetirement, region, ecolaRevised, ctpaRate, withCTPA, seaRate, withSEA, payprd, sgcode, isTrucking, isLumpsum
                        from {schema}.Client order by ClName ";
        var data = await _sql.FetchData<OClientModel, dynamic>(sql, new { }, conn);
        return data ?? [];
    }

    public async Task<List<OEmpmasModel>> _02ByClNumbersByStatus(List<string> clnumbers, List<string> statuses, string schema, string conn)
    {
        if (clnumbers == null || clnumbers.Count == 0 ||  statuses == null || statuses.Count == 0) return [];
        var flds = EmpmasFields();

        if (clnumbers?.FirstOrDefault() == "-")
        {
            clnumbers = [];
        }

        string sql = $@"SELECT {flds}, s.Name AS EmpStatus, c.ClName
                        FROM {schema}.Empmas e
                        LEFT JOIN {schema}.EmpStat s ON s.Code = e.Empstat_
                        LEFT JOIN {schema}.Client  c ON c.ClNumber = e.Client_
                        WHERE e.Empstat_ IN @Statuses
                        {(clnumbers != null && clnumbers.Count > 0 ? "AND e.Client_ IN @ClNumbers" : "")}
                        ORDER BY e.EmpLastNm, e.EmpFirstNm;";
        var data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { ClNumbers=clnumbers, Statuses = statuses }, conn);
        return data ?? [];
    }

    public async Task<List<OClientModel>> _02ClientByStatus(string status, string schema, string conn)
    {
        string sql = $@"select  CLNUMBER, CLNAME, ADDR1, ADDR2, AREACODE, TEL1, FAXNO, PARENT, RATE, BILLRATE, ASSIST, STATUS, COLARATE, 
                            ND_RATE, RETIRATE, UNIFRATE, FDIRATE, OTRATE, TIN, CONT, USED, CONTACT, POSTPERIOD, BATCHX, FSSSEE, FSSSER, 
                            FECC, FMEDEE, FMEDER, Remarks, 
                            IF(contStart    IN ('0000-00-00','0000-00-00 00:00:00'), NULL, contStart)   AS contStart,
                            IF(contEnd      IN ('0000-00-00','0000-00-00 00:00:00'), NULL, contEnd)     AS contEnd,
                            parentcd, maxsss, maxphic, 
                            IF(ContExp      IN ('0000-00-00','0000-00-00 00:00:00'), NULL, ContExp)     AS ContExp,
                            HavTax, MinRate, MealAllow, 
                            withUniform, withRetirement, region, ecolaRevised, ctpaRate, withCTPA, seaRate, withSEA, payprd, sgcode, isTrucking, isLumpsum 
                        from {schema}.Client where Status = @Status order by ClName ";
        var data = await _sql.FetchData<OClientModel, dynamic>(sql, new { Status = status }, conn);
        return data ?? [];
    }
    
    public async Task<List<OEmpstatModel>> _02Empstats(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empstat order by Name ";
        var data = await _sql.FetchData<OEmpstatModel, dynamic>(sql, new {  }, conn);
        return data ?? [];
    }

    public async Task<List<OEmpmasModel>> _02Empmas_By_Clnumbers(string clnumber, string schema, string conn)
    {
        
        //await _03Empmas_Remove_0_Dates(schema, conn); 
        var flds = EmpmasFields(); 

        string              mclnumber   = clnumber; 
        List<OEmpmasModel>  data        = []; 
        
        if(clnumber=="-")
        {
            string sql = $@"select  {flds}, s.Name EmpStatus, c.ClName  
                            from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            left join {schema}.client c on c.ClNumber = e.Client_  
                            where e.Empstat_ in 
                                    (select code from {schema}.EmpStat where isResigned = 0 ) 
                            order by empLastNm, EmpFirstNm ";
            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Clnumber = mclnumber }, conn);
    
        }  else
        {
            string sql = $@"select  {flds}, s.Name EmpStatus from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            where e.Client_ = @Clnumber 
                            order by empLastNm, EmpFirstNm ";
            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Clnumber = mclnumber }, conn);
        }  
        return data ?? [];
    }
   
    public async Task<List<OEmpmasModel>> _02Empmas_By_Status_And_DateRange(string empstat, DateTime? startDate, DateTime? endDate, string schema, string conn)
    {

        var flds = EmpmasFields();

        DateTime mstartDate = startDate ?? DateTime.Today;
        DateOnly xendDate   = endDate.HasValue
            ? DateOnly.FromDateTime(endDate.Value)
            : DateOnly.FromDateTime(DateTime.Today);

        DateTime mendDate = xendDate.ToDateTime(TimeOnly.MaxValue);

        List<OEmpmasModel> data = [];
    
            string sql = $@"select  CONCAT_WS(' ', NULLIF(TRIM(EmpLastNm),', '), NULLIF(TRIM(EmpFirstNm), ' '), NULLIF(TRIM(EmpMidNm), ' ')) AS EmpName, 
                                {flds}, s.Name EmpStatus from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            where e.empstat_ = @Empstat and e.movdate >= @StartDate and  e.movend <= @EndDate
                            order by empLastNm, EmpFirstNm ";

            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Empstat = empstat, StartDate = mstartDate, EndDate = mendDate, }, conn);
        
        return data ?? [];
    }

    public async Task<List<OEmpmasModel>> _02Empmas_ByLicenseExpiry( DateTime? startDate, DateTime? endDate, List<string> statuses,string schema, string conn)
    {

        var flds = EmpmasFields();

        DateTime mstartDate = startDate ?? DateTime.Today;
        DateOnly xendDate   = endDate.HasValue
            ? DateOnly.FromDateTime(endDate.Value)
            : DateOnly.FromDateTime(DateTime.Today);

        DateTime mendDate = xendDate.ToDateTime(TimeOnly.MaxValue);

        List<OEmpmasModel> data = [];

        string sql = $@"select  CONCAT_WS(' ', NULLIF(TRIM(EmpLastNm),', '), NULLIF(TRIM(EmpFirstNm), ' '), NULLIF(TRIM(EmpMidNm), ' ')) AS EmpName, 
                                {flds}, s.Name EmpStatus from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            where e.LicExpire between @StartDate and @EndDate and e.empstat_ IN @Statuses
                            order by empLastNm, EmpFirstNm ";

        data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new {  StartDate = mstartDate, EndDate = mendDate, Statuses=statuses }, conn);

        return data ?? [];
    }

    public async Task<List<OEmpmasModel>> _02Empmas_ByMonth_And_Year( string fld, int month, int year, string schema, string conn)
    {
        var flds = EmpmasFields();
        List<OEmpmasModel> data = [];

        string sql = $@"SELECT concat_ws(' ', Concat(Nullif(trim(emplastnm), ''),', '), Nullif(trim(empfirstnm), ''), Nullif(trim(empmidnm), '') ) EmpName,  
                        p.name PositionName,  s.Name EmpStatus,
                        {flds} FROM  {schema}.empmas e
                        LEFT JOIN {schema}.position p on p.code = e.position_
                        LEFT JOIN {schema}.empstat s on s.code = e.empstat_
                        WHERE e.empstat_ IN @Statuses AND Month(e.{fld}) = @Month AND Year(e.{fld}) = @Year
                        order by emplastnm, empfirstnm
                        ";

        data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Month = month, Year = year}, conn);
        return data ?? [];
    }
    public async Task<List<OEmpmasModel>> _02Empmas_ByInsurance_And_Status(int filterValue,  List<string> statuses,  DateTime? startDate, DateTime? endDate, string schema, string conn)
    {
        var flds                = EmpmasFields();
        List<OEmpmasModel> data = [];
        string sql              = "";

        if (filterValue == 1)
        {
            DateTime mstartDate = startDate ?? DateTime.Today;
            DateOnly xendDate = endDate.HasValue
                ? DateOnly.FromDateTime(endDate.Value)
                : DateOnly.FromDateTime(DateTime.Today);

            DateTime mendDate = xendDate.ToDateTime(TimeOnly.MaxValue);

            sql = $@"SELECT concat_ws(' ', Concat(Nullif(trim(emplastnm), ''),', '), Nullif(trim(empfirstnm), ''), Nullif(trim(empmidnm), '') ) EmpName,
                    {flds}, s.Name EmpStatus
                    FROM {schema}.empmas e
                    LEFT JOIN {schema}.empstat s on s.code = e.empstat_
                    WHERE e.insexpire between @StartDate and @EndDate AND empstat_ IN @statuses
                    order by e.emplastnm, e.empfirstnm";

            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { StartDate = mstartDate, EndDate = mendDate, Statuses = statuses }, conn);
        }
        else if (filterValue == 2) {
            sql = $@"SELECT concat_ws(' ', Concat(Nullif(trim(emplastnm), ''),', '), Nullif(trim(empfirstnm), ''), Nullif(trim(empmidnm), '') ) EmpName,
                    {flds}, s.Name EmpStatus
                    FROM {schema}.empmas e
                    LEFT JOIN {schema}.empstat s on s.code = e.empstat_
                    WHERE (e.insurance IS NULL or TRIM(e.insurance) = '') AND empstat_ IN @statuses
                    order by e.emplastnm, e.empfirstnm";

            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Statuses = statuses }, conn);
        } else
        {
            sql = $@"SELECT concat_ws(' ', Concat(Nullif(trim(emplastnm), ''),', '), Nullif(trim(empfirstnm), ''), Nullif(trim(empmidnm), '') ) EmpName,
                    {flds}, s.Name EmpStatus
                    FROM {schema}.empmas e
                    LEFT JOIN {schema}.empstat s on s.code = e.empstat_
                    WHERE e.insurance IS NOT NULL AND TRIM(e.insurance) <> '' AND empstat_ IN @statuses
                    order by e.emplastnm, e.empfirstnm";
            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Statuses = statuses }, conn);
        }
        return data ?? [];
    }





    public async Task<List<OCompanyInfoModel?>> _02CoInfo(string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.Coinfo ";
        var data    = await _sql.FetchData<OCompanyInfoModel?, dynamic>(sql, new { }, conn);
        return data??[];
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

    private async Task _03Empmas_Remove_0_Dates(string schema, string conn) 
    {
        var sql = $@"UPDATE {schema}.empmas SET
                        AEND        = IF(AEND       < '1800-01-01', '1900-01-01', AEND),
                        ASTART      = IF(ASTART     < '1800-01-01', '1900-01-01', ASTART),
                        COMTAXDATE  = IF(COMTAXDATE < '1800-01-01', '1900-01-01', COMTAXDATE),
                        DATEHIRED   = IF(DATEHIRED  < '1800-01-01', '1900-01-01', DATEHIRED),
                        DATETRAIN   = IF(DATETRAIN  < '1800-01-01', '1900-01-01', DATETRAIN),
                        DEND        = IF(DEND       < '1800-01-01', '1900-01-01', DEND),
                        dpadate     = IF(dpadate    < '1800-01-01', '1900-01-01', dpadate),
                        DRV_EXP     = IF(DRV_EXP    < '1800-01-01', '1900-01-01', DRV_EXP),
                        DSTART      = IF(DSTART     < '1800-01-01', '1900-01-01', DSTART),
                        DUTYDATE    = IF(DUTYDATE   < '1800-01-01', '1900-01-01', DUTYDATE),
                        EMPBIRTH    = IF(EMPBIRTH   < '1800-01-01', '1900-01-01', EMPBIRTH),
                        EXP_BRGY    = IF(EXP_BRGY   < '1800-01-01', '1900-01-01', EXP_BRGY),
                        EXP_COURT   = IF(EXP_COURT  < '1800-01-01', '1900-01-01', EXP_COURT),
                        EXP_DRUG    = IF(EXP_DRUG   < '1800-01-01', '1900-01-01', EXP_DRUG),
                        EXP_NBI     = IF(EXP_NBI    < '1800-01-01', '1900-01-01', EXP_NBI),
                        EXP_NEURO   = IF(EXP_NEURO  < '1800-01-01', '1900-01-01', EXP_NEURO),
                        EXP_PNP     = IF(EXP_PNP    < '1800-01-01', '1900-01-01', EXP_PNP),
                        EXP_POLICE  = IF(EXP_POLICE < '1800-01-01', '1900-01-01', EXP_POLICE),
                        EXPMED      = IF(EXPMED     < '1800-01-01', '1900-01-01', EXPMED),
                        INSEXPIRE   = IF(INSEXPIRE  < '1800-01-01', '1900-01-01', INSEXPIRE),
                        LICEXPIRE   = IF(LICEXPIRE  < '1800-01-01', '1900-01-01', LICEXPIRE),
                        MOVDATE     = IF(MOVDATE    < '1800-01-01', '1900-01-01', MOVDATE),
                        MOVEND      = IF(MOVEND     < '1800-01-01', '1900-01-01', MOVEND),
                        regref      = IF(regref     < '1800-01-01', '1900-01-01', regref),
                        SEPARATE    = IF(SEPARATE   < '1800-01-01', '1900-01-01', SEPARATE),
                        STATUSDATE  = IF(STATUSDATE < '1800-01-01', '1900-01-01', STATUSDATE)
                    WHERE
                        AEND            < '1800-01-01'
                        OR ASTART       < '1800-01-01'
                        OR COMTAXDATE   < '1800-01-01'
                        OR DATEHIRED    < '1800-01-01'
                        OR DATETRAIN    < '1800-01-01'
                        OR DEND         < '1800-01-01'
                        OR dpadate      < '1800-01-01'
                        OR DRV_EXP      < '1800-01-01'
                        OR DSTART       < '1800-01-01'
                        OR DUTYDATE     < '1800-01-01'
                        OR EMPBIRTH     < '1800-01-01'
                        OR EXP_BRGY     < '1800-01-01'
                        OR EXP_COURT    < '1800-01-01'
                        OR EXP_DRUG     < '1800-01-01'
                        OR EXP_NBI      < '1800-01-01'
                        OR EXP_NEURO    < '1800-01-01'
                        OR EXP_PNP      < '1800-01-01'
                        OR EXP_POLICE   < '1800-01-01'
                        OR EXPMED       < '1800-01-01'
                        OR INSEXPIRE    < '1800-01-01'
                        OR LICEXPIRE    < '1800-01-01'
                        OR MOVDATE      < '1800-01-01'
                        OR MOVEND       < '1800-01-01'
                        OR regref       < '1800-01-01'
                        OR SEPARATE     < '1800-01-01'
                        OR STATUSDATE   < '1800-01-01';"; 
        await _sql.ExecuteCmd<dynamic>(sql, new{}, conn); 
    }
}

public interface IOPisReportDataAccess
{
    Task<List<OClientModel>>        _02Client(string schema, string conn);
    Task<List<OClientModel>>        _02ClientByStatus(string status, string schema, string conn);
    Task<List<OEmpstatModel>>       _02Empstats(string schema, string conn); 
    Task<List<OEmpmasModel>>        _02Empmas_By_Clnumbers(string clnumber, string schema, string conn); 
    Task<List<OCompanyInfoModel?>>  _02CoInfo(string schema, string conn); 
    Task<List<OEmpmasModel>>        _02ByClNumbersByStatus(List<string> clnumbers, List<string> statuses, string schema, string conn);
    Task<List<OEmpmasModel>>        _02Empmas_By_Status_And_DateRange(string empstat, DateTime? startdate, DateTime? endDate, string schema, string conn);
    Task<List<OEmpmasModel>>        _02Empmas_ByMonth_And_Year(string fld, int month, int year, string schema, string conn);
    Task<List<OEmpmasModel>>        _02Empmas_ByLicenseExpiry( DateTime? startDate, DateTime? endDate, List<string> statuses,string schema, string conn);
}
