using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00CompanyusersDataAccess
    {
        Task<CompanyUsersModel?>            _01(CompanyUsersModel companyusers, string schema, string conn);
        Task<CompanyUsersModel?>            _02(int id, string schema, string conn);
        Task<List<CompanyUsersModel?>?>     _02ByUserId(int userId, string schema="Main", string conn="MySqlConn");
        Task<List<CompanyUsersModel?>?> _02ByUseridCompanyid(int userId, int companyId, string schema = "Main",
            string conn = "MySqlConn");
        Task<CompanyUsersModel?>            _03(int id, CompanyUsersModel companyusers, string schema, string conn);
        Task<CompanyUsersModel?>            _04(int id, string schema, string conn);
    }
}