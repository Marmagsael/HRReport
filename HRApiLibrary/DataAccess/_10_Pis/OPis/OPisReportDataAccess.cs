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



}

public interface IOPisReportDataAccess
{
    Task<List<OClientModel>> _02Client(string schema, string conn);
    Task<List<OClientModel>> _02ClientByStatus(string status, string schema, string conn);
    Task<List<OEmpstatModel>> _02Empstats(string status, string schema, string conn); 
}
