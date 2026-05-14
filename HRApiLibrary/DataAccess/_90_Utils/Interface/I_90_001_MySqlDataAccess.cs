using System.Data;

namespace HRApiLibrary.DataAccess._90_Utils.Interface
{
    public interface I_90_001_MySqlDataAccess
    {
        Task ExecuteCmd<T>(string sql, T parameters, string connName = "MySqlConn");
        Task ExecuteCmd<T>(string sql, T parameters, string db, string connName); 
        Task<List<T>> FetchData<T, U>(string sql, U parameters, string connName = "MySqlConn");
        Task<List<T>> FetchData<T, U>(string sql, U parameters, string db, string connName); 
        IDbConnection GetConnString(string connName = "MySqlConn");
        
    }
}