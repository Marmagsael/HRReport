using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace HRApiLibrary.DataAccess._90_Utils;

public class _90_001_MySqlDataAccess : I_90_001_MySqlDataAccess
{
    private readonly IConfiguration _config;

    public _90_001_MySqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> FetchData<T, U>(string? sql, U parameters, string? connName = "MySqlConn")
    {
        var connString = _config.GetConnectionString(connName);
        using (IDbConnection conn = new MySqlConnection(connString))
        {
            var list = await conn.QueryAsync<T>(sql, parameters);
            return list.ToList();
        }
    }
    
    public async Task<List<T>> FetchData<T, U>(string? sql, U parameters, string? db, string? connName)
    {
        var defConn = _config.GetConnectionString(connName);
        var connString = $"{defConn}; database={db}";
        using IDbConnection conn = new MySqlConnection(connString);
        var list = await conn.QueryAsync<T>(sql, parameters);
        return list.ToList();
    }
    
    public async Task ExecuteCmd<T>(string? sql, T parameters, string? connName = "MySqlConn")
    {
        var connString = _config.GetConnectionString(connName);
        using (IDbConnection conn = new MySqlConnection(connString))
        {
            await conn.ExecuteAsync(sql, parameters);
        }
    }
    public async Task ExecuteCmd<T>(string? sql, T parameters, string? db, string? connName)
    {
        var defConn = _config.GetConnectionString(connName);
        var connString = $"{defConn}; database={db}";
        using IDbConnection conn = new MySqlConnection(connString);
        await conn.ExecuteAsync(sql, parameters);
    }
    
    public IDbConnection GetConnString(string? connName = "MySqlConn")
    {
        var connString = _config.GetConnectionString(connName);
        return new MySqlConnection(connString);
    }

    //--- Legacy schemas (OPis / OPay) : the same calls run in LegacySqlMode on their own connection ----------------------
    public Task ExecuteLegacyCmd<T>(string? sql, T parameters, string? connName = "MySqlConn")
        => Legacy(_config.GetConnectionString(connName), cn => cn.ExecuteAsync(sql, parameters));

    public Task ExecuteLegacyCmd<T>(string? sql, T parameters, string? db, string? connName)
        => Legacy($"{_config.GetConnectionString(connName)}; database={db}", cn => cn.ExecuteAsync(sql, parameters));

    public async Task<List<T>> FetchLegacy<T, U>(string? sql, U parameters, string? connName = "MySqlConn")
        => (await Legacy(_config.GetConnectionString(connName), cn => cn.QueryAsync<T>(sql, parameters))).ToList();

    public async Task<List<T>> FetchLegacy<T, U>(string? sql, U parameters, string? db, string? connName)
        => (await Legacy($"{_config.GetConnectionString(connName)}; database={db}", cn => cn.QueryAsync<T>(sql, parameters))).ToList();

    private static async Task<TR> Legacy<TR>(string? connString, Func<IDbConnection, Task<TR>> work)
    {
        using var cn = new MySqlConnection(connString);
        await cn.OpenAsync();
        await cn.ExecuteAsync(LegacySqlMode.Set);
        try     { return await work(cn); }
        finally { await cn.ExecuteAsync(LegacySqlMode.Restore); }
    }
}
