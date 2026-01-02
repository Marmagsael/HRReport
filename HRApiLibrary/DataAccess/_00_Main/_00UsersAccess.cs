using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00UsersAccess : I_00UsersAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00UsersAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<UsersModel?> _01(UsersModel user, string schema = "Main", string connName = "MySqlConn")
    {
        var newuser = await _sql.FetchData<UsersModel?, dynamic>($"select * from {schema}.users where LoginName = @LoginName", new { LoginName = user.LoginName }, connName);
        if (newuser == null) { return newuser?.FirstOrDefault(); }

        string sql = $@"Insert into {schema}.users (LoginName, Password, Email, Domain, UserType, Status, DefaultCoId)  Values (	@LoginName, sha2(@Password,512), @Email, @Domain, @UserType, @Status, @DefaultCoId);";
        await _sql.ExecuteCmd<dynamic>(sql, user, connName);

        sql = $@" select  * from {schema}.Users e where e.LoginName = @LoginName and e.Email = @Email";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = user.LoginName, Email = user.Email }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _02ById(int id, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where Id = @Id";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = id }, connName);
        return data?.FirstOrDefault();
    }
    public async Task<UsersModel?> _02ByEmail(string email, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where Email = @Email";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _02ByLoginName(string loginname, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where LoginName = @LoginName";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = loginname }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _02LoginEmail(string email, string password, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where e.Email = @Email and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Email = email, Password = password }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _02LoginLoginName(string loginName, string password, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@" select  * from {schema}.Users e where e.LoginName = @LoginName and Password = sha2(@Password,512)";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { LoginName = loginName, Password = password }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _03(int id, UsersModel user, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@"Update {schema}.users set 
                            LoginName   = @LoginName, 
                            Email       = @Email, 
                            Domain      = @Domain, 
                            UserType    = @UserType, 
                            Status      = @Status, 
                            DefaultCoId = @DefaultCoId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, user, connName);

        sql = $@" select  * from {schema}.Users e where e.Id = @Id ;";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = id }, connName);
        return data?.FirstOrDefault();
    }

    public async Task<UsersModel?> _03ChangeDefaultCompany(int userId, int newDefaultCoId, string schema = "Main", string connName = "MySqlConn")
    {
        string sql = $@"Update          {schema}.users set DefaultCoId = @DefaultCoId where Id = @Id;
                     Select  * from  {schema}.Users e where e.Id = @Id ;";
        var data = await _sql.FetchData<UsersModel?, dynamic>(sql, new { Id = userId, DefaultCoId = newDefaultCoId }, connName);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema = "Main", string connName = "MySqlConn")
    {
        string msql = @" Delete from " + schema + @".Users where Id = @Id; ";

        await _sql.ExecuteCmd<dynamic>(msql, new { Id = id }, connName);
    }
}
