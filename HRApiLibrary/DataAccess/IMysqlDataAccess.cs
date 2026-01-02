using System.Data;

namespace MysqlApiLibrary.DataAccess
{
    public interface IMysqlDataAccess
    {
        Task ExecuteCmd<T>(string sql, T parameters, string connName = "MySqlConn");
        Task<List<T>> FetchData<T, U>(string sql, U parameters, string connName = "MySqlConn");
        IDbConnection GetConnString(string connName = "MySqlConn");
    }
}