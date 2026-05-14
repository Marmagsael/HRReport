using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main
{
    public interface I_00UserscompanyDataAccess
    {
        Task<UserCompanyModel?>         _01(UserCompanyModel userscompany, string schema="Main", string conn="MySqlConn");
        Task<List<UserCompanyModel?>?> _02Lst(int id, string schema = "Main", string conn = "MySqlConn"); 
        Task<UserCompanyModel?>         _02(int id, string schema="Main", string conn="MySqlConn");
        Task<List<UserCompanyModel?>?>  _02ByCompanySName(string companySName, string schema="Main", string conn="MySqlConn");
        Task<UserCompanyModel?>         _02ByUserDefaultCoId(int userId, string schema = "Main", string conn = "MySqlConn");
        Task<List<UserCompanyModel?>?>  _02ByUserId(int userId, string schema = "Main", string conn = "MySqlConn"); 
        Task<UserCompanyModel?>         _03(int id, UserCompanyModel userscompany, string schema, string conn);
        Task<UserCompanyModel?>         _04(int id, string schema, string conn);
    }
}