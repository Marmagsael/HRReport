using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;

namespace HRApiLibrary.DataAccess._00_MainTrans;

public class MailDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MailDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MailModel?> _01(MailModel mail, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Mail (UserCompanyId, SenderId, module, msg, link, IsRead, DateSent, DateRead) values (@UserCompanyId, @SenderId, @module, @msg, @link, @IsRead, @DateSent, @DateRead)";
        await _sql.ExecuteCmd<dynamic>(sql, mail, conn);

        sql = $@"SELECT * FROM {schema}.Mail WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MailModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MailModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, UserCompanyId, SenderId, module, msg, link, IsRead, DateSent, DateRead from {schema}.Mail where Id = @Id";
        var data = await _sql.FetchData<MailModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<MailModel?> _03(int id, MailModel mail, string schema, string conn)
    {
        string sql = $@"Update {schema}.Mail set UserCompanyId = @UserCompanyId, SenderId = @SenderId, module = @module, msg = @msg, link = @link, IsRead = @IsRead, DateSent = @DateSent, DateRead = @DateRead where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, mail, conn);

        sql = $@" select  * from {schema}.Mail x where x.Id = @Id ;";
        var data = await _sql.FetchData<MailModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MailModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Mail where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Mail x where x.Id = @Id ;";
        var data = await _sql.FetchData<MailModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
