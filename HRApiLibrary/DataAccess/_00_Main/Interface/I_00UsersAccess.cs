using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00UsersAccess
    {
        Task<UsersModel?> _01(UsersModel user, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _02ByEmail(string email, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _02ById(int id, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _02ByLoginName(string loginname, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _02LoginEmail(string email, string password, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _02LoginLoginName(string loginName, string password, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _03(int id, UsersModel user, string schema = "Main", string connName = "MySqlConn");
        Task<UsersModel?> _03ChangeDefaultCompany(int userId, int newDefaultCoId, string schema = "Main", string connName = "MySqlConn");
        Task _04(int id, string schema = "Main", string connName = "MySqlConn");
    }
}