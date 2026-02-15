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
    
    public async Task<List<OEmpstatModel>> _02Empstats(string status, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empstat order by Name ";
        var data = await _sql.FetchData<OEmpstatModel, dynamic>(sql, new {  }, conn);
        return data ?? [];
    }

    public async Task<List<OEmpmasModel>> _02Empmas_By_Clnumbers(string clnumber, string schema, string conn)
    {
        
        await _03Empmas_Remove_0_Dates(schema, conn); 

        string              mclnumber   = clnumber; 
        List<OEmpmasModel>  data        = []; 
        
        if(clnumber=="-")
        {
            string sql = $@"select  CONCAT_WS(' ', NULLIF(TRIM(EmpLastNm),', '), NULLIF(TRIM(EmpFirstNm), ' '), NULLIF(TRIM(EmpMidNm), ' ')) AS EmpName, 
                                e.*, s.Name EmpStatus from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            where e.Empstat_ in 
                                    (select code from {schema}.EmpStat where isResigned = 0 ) 
                            order by empLastNm, EmpFirstNm ";
            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Clnumber = mclnumber }, conn);
    
        }  else
        {
            string sql = $@"select  CONCAT_WS(' ', NULLIF(TRIM(EmpLastNm),', '), NULLIF(TRIM(EmpFirstNm), ' '), NULLIF(TRIM(EmpMidNm), ' ')) AS EmpName, 
                                e.*, s.Name EmpStatus from {schema}.Empmas e 
                            left join {schema}.EmpStat s on s.Code = e.Empstat_  
                            where e.Client_ = @Clnumber 
                            order by empLastNm, EmpFirstNm ";
            data = await _sql.FetchData<OEmpmasModel, dynamic>(sql, new { Clnumber = mclnumber }, conn);
        }

        
        return data ?? [];

    }


    // --- Private Functions ------------------------------------------------------------------------------------------------

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
                        OR dpadate       < '1800-01-01'
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
    Task<List<OClientModel>>    _02Client(string schema, string conn);
    Task<List<OClientModel>>    _02ClientByStatus(string status, string schema, string conn);
    Task<List<OEmpstatModel>>   _02Empstats(string status, string schema, string conn); 
    Task<List<OEmpmasModel>>    _02Empmas_By_Clnumbers(string clnumber, string schema, string conn); 
}
