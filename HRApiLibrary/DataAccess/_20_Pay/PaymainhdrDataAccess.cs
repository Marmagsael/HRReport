using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PaymainhdrDataAccess : IPaymainhdrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PaymainhdrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PaymainhdrModel?> _01(PaymainhdrModel paymainhdr, string schema, string conn)
    {
        var query = @$"select * from {schema}.Paymainhdr where Trn = @Trn ";
        var output = await _sql.FetchData<PaymainhdrModel?, dynamic>(query, new{Trn=paymainhdr.Trn}, conn);

        var sql = $@"Insert into {schema}.Paymainhdr 
							(Trn,  ClRate,  MinRate,  UserId,  Status,  DateCreated,  DatePosted,  AttStart,  AttEnd) values 
							(@Trn, @ClRate, @MinRate, @UserId, @Status, @DateCreated, @DatePosted, @AttStart, @AttEnd); ";
        if (output.Count < 1) await _sql.ExecuteCmd<dynamic>(sql!, paymainhdr, conn);

        sql = $@"SELECT * FROM {schema}.Paymainhdr WHERE Trn = @Trn";
        var res = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = paymainhdr.Trn }, conn);
        return res.FirstOrDefault();

    }


    public async Task<PaymainhdrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Trn, ClRate, MinRate, UserId, Status, DateCreated, DatePosted, AttStart, AttEnd 
							from {schema}.Paymainhdr where Id = @Id";
        var data = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PaymainhdrModel?> _02ByTrn(string trn, string schema, string conn)
    {
        string sql = $@"select  Trn, ClRate, MinRate, UserId, Status, DateCreated, DatePosted, AttStart, AttEnd 
							from {schema}.Paymainhdr where Trn = @trn";
        var data = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<PaymainhdrModel?>?> _02ByTrns(string trn, string schema, string conn)
    {
        string sql = $@"select  Trn, ClRate, MinRate, UserId, Status, DateCreated, DatePosted, AttStart, AttEnd 
							from {schema}.Paymainhdr where Trn = @trn";
        var data = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }
    
    public async Task<List<PaymainhdrModel?>?> _02ByPeriodTrns(string periodTrn, string schema, string conn)
    {
        string sql  = $@"select  Trn, ClRate, MinRate, UserId, Status, DateCreated, DatePosted, AttStart, AttEnd 
							from {schema}.Paymainhdr where left(Trn,6) = Left(@trn,6) ; ";
        var data    = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Trn = periodTrn }, conn);
        return data;
    }


    public async Task<PaymainhdrModel?> _03(int id, PaymainhdrModel paymainhdr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paymainhdr set 
								Trn 	= @Trn, 
								ClRate 	= @ClRate, 
								MinRate = @MinRate, 
								UserId 	= @UserId, 
								Status 	= @Status, 
								DateCreated = @DateCreated, 
								DatePosted 	= @DatePosted, 
								AttStart 	= @AttStart, 
								AttEnd 		= @AttEnd where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, paymainhdr, conn);

        sql = $@" select  * from {schema}.Paymainhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PaymainhdrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paymainhdr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Paymainhdr x where x.Id = @Id ;";
        var data = await _sql.FetchData<PaymainhdrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}