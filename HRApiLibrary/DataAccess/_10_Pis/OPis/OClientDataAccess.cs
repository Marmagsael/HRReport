using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OClientDataAccess : IOClientDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OClientDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(OClientModel client, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Client 
							(CLNUMBER, CLNAME, ADDR1, ADDR2, AREACODE, TEL1, FAXNO, PARENT, RATE, 
							 BILLRATE, ASSIST, STATUS, COLARATE, ND_RATE, RETIRATE, UNIFRATE, FDIRATE, 
							 OTRATE, TIN, CONT, USED, CONTACT, POSTPERIOD, BATCHX, FSSSEE, FSSSER, FECC, FMEDEE, 
							 FMEDER, Remarks, contStart, contEnd, parentcd, maxsss, maxphic, ContExp, HavTax, MinRate, 
							 MealAllow, withUniform, withRetirement, region, ecolaRevised, ctpaRate, withCTPA, seaRate, 
							 withSEA, payprd, sgcode, isTrucking, isLumpsum) values 
							(@CLNUMBER, @CLNAME, @ADDR1, @ADDR2, @AREACODE, @TEL1, @FAXNO, @PARENT, @RATE, 
							 @BILLRATE, @ASSIST, @STATUS, @COLARATE, @ND_RATE, @RETIRATE, @UNIFRATE, @FDIRATE, 
							 @OTRATE, @TIN, @CONT, @USED, @CONTACT, @POSTPERIOD, @BATCHX, @FSSSEE, @FSSSER, @FECC, @FMEDEE, 
							 @FMEDER, @Remarks, @contStart, @contEnd, @parentcd, @maxsss, @maxphic, @ContExp, @HavTax, @MinRate, 
							 @MealAllow, @withUniform, @withRetirement, @region, @ecolaRevised, @ctpaRate, @withCTPA, @seaRate, 
							 @withSEA, @payprd, @sgcode, @isTrucking, @isLumpsum)";
        await _sql.ExecuteCmd<dynamic>(sql, client, conn);


    }


    public async Task<List<OClientModel?>?> _02ByClNumbers(string clnumber, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Client where ClNumber = @ClNumber order by ClName ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { ClNumber = clnumber }, conn);
        return data;
    }

    public async Task<List<OClientModel?>?> _02ByStatuss(string status, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Client where ClNumber = @ClNumber order by ClName ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { Status = status }, conn);
        return data;
    }


    public async Task<OClientModel?> _03(OClientModel client, string schema, string conn)
    {
        string sql = $@"Update {schema}.Client set CLNUMBER = @CLNUMBER, CLNAME = @CLNAME, ADDR1 = @ADDR1, ADDR2 = @ADDR2, 
							AREACODE = @AREACODE, TEL1 = @TEL1, FAXNO = @FAXNO, PARENT = @PARENT, RATE = @RATE, 
							BILLRATE = @BILLRATE, ASSIST = @ASSIST, STATUS = @STATUS, COLARATE = @COLARATE, 
							ND_RATE = @ND_RATE, RETIRATE = @RETIRATE, UNIFRATE = @UNIFRATE, FDIRATE = @FDIRATE, 
							OTRATE = @OTRATE, TIN = @TIN, CONT = @CONT, USED = @USED, CONTACT = @CONTACT, POSTPERIOD = @POSTPERIOD, 
							BATCHX = @BATCHX, FSSSEE = @FSSSEE, FSSSER = @FSSSER, FECC = @FECC, FMEDEE = @FMEDEE, 
							FMEDER = @FMEDER, Remarks = @Remarks, contStart = @contStart, 
							contEnd = @contEnd, parentcd = @parentcd, maxsss = @maxsss, maxphic = @maxphic, 
							ContExp = @ContExp, HavTax = @HavTax, MinRate = @MinRate, MealAllow = @MealAllow, 
							withUniform = @withUniform, withRetirement = @withRetirement, region = @region, 
							ecolaRevised = @ecolaRevised, ctpaRate = @ctpaRate, withCTPA = @withCTPA, seaRate = @seaRate, 
							withSEA = @withSEA, payprd = @payprd, sgcode = @sgcode, isTrucking = @isTrucking, 
							isLumpsum = @isLumpsum where ClNumber = @ClNumber;
						select  * from {schema}.Client  where ClNumber = @ClNumber ;";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, client, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OClientModel?> _04(string clNumber, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Client where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { ClNumber = clNumber }, conn);

        sql = $@" select  * from {schema}.Client x where x.Id = @Id ;";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { ClNumber = clNumber }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOClientDataAccess
{
    Task _01(OClientModel client, string schema, string conn);
    Task<List<OClientModel?>?> _02ByClNumbers(string clnumber, string schema, string conn);
    Task<List<OClientModel?>?> _02ByStatuss(string status, string schema, string conn);
    Task<OClientModel?> _03(OClientModel client, string schema, string conn);
    Task<OClientModel?> _04(string clNumber, string schema, string conn);
}
