using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OPaymainhdrDataAccess : IOPaymainhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPaymainhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<GPaymainhdrModel?> _01(GPaymainhdrModel paymainhdr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Paymainhdr 
						(trn, clrate, minrate, withSEA, SEARate, withCTPA, CTPARate, ecolaRevised, billrate, user, status, datecreated, dateposted, attStart, attEnd) values 
						(@trn, @clrate, @minrate, @withSEA, @SEARate, @withCTPA, @CTPARate, @ecolaRevised, @billrate, @user, @status, @datecreated, @dateposted, @attStart, @attEnd); 
						SELECT * FROM {schema}.Paymainhdr WHERE Trn = @Trn; ";
        var res = await _sql.FetchData<GPaymainhdrModel?, dynamic>(sql, paymainhdr, conn);
        return res.FirstOrDefault();
    }


    public async Task<List<GPaymainhdrModel?>?> _02ByYYMMPP(string trn, string paydb, string pisdb, string conn)
    {
        string sql = $@"select  c.ClNumber, c.ClName, h.*
						from {paydb}.Paymainhdr h 
						left join {pisdb}.Client c on c.clNumber = right(trim(h.trn),5) 
						where Left(t.Trn,6) = left(@Trn,6) ";
        var data = await _sql.FetchData<GPaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }

    public async Task<GPaymainhdrModel?> _02Trn(string trn, string paydb, string pisdb, string conn)
    {
        string sql = $@"select  c.ClNumber, c.ClName, h.*
						from {paydb}.Paymainhdr h  
						left join {pisdb}.Client c on c.clNumber = right(trim(h.trn),5) 
						where Trn = @Trn ";
        var data = await _sql.FetchData<GPaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data.FirstOrDefault();
    }

    public async Task<GPaymainhdrModel?> _03(int id, GPaymainhdrModel paymainhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paymainhdr set 
							Clrate 			= @ClRate, 
							minrate 		= @MinRate, 
							withSEA 		= @withSea, 
							SEARate 		= @SEARate, 
							withCTPA 		= @withCtpa, 
							CTPARate 		= @CtpaRate, 
							ecolaRevised 	= @EcolaRevised, 
							billrate 		= @BillRate, 
							user 			= @User, 
							status 			= @Status, 
							datecreated 	= @DateCreated, 
							dateposted 		= @DatePosted, 
							attStart 		= @AttStart, 
							attEnd 			= @AttEnd 
						where Trn 			= @Trn; 
						select  * from {schema}.Paymainhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<GPaymainhdrModel?, dynamic>(sql, paymainhdr, conn);
        return data?.FirstOrDefault();
    }

    public async Task<GPaymainhdrModel?> _04(string trn, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paymainhdr where Trn = @Trn;
		 				select  * from {schema}.Paymainhdr where Trn = @Trn ;";
        var data = await _sql.FetchData<GPaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOPaymainhdrDataAccess
{
    Task<GPaymainhdrModel?> _01(GPaymainhdrModel paymainhdr, string schema, string conn);
    Task<List<GPaymainhdrModel?>?> _02ByYYMMPP(string trn, string paydb, string pisdb, string conn);
    Task<GPaymainhdrModel?> _02Trn(string trn, string paydb, string pisdb, string conn);
    Task<GPaymainhdrModel?> _03(int id, GPaymainhdrModel paymainhdr, string schema, string conn);
    Task<GPaymainhdrModel?> _04(string trn, string schema, string conn);
}
