using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00UsercompanyaddDataAccess
    {
        Task<UserCompanyModel?> _01(UserCompanyModel userscompany, string schema, string conn);
        Task<UserCompanyModel?> _02(int id, string schema, string conn);
        Task<UserCompanyModel?> _03(int id, UserCompanyModel userscompany, string schema, string conn);
        Task<UserCompanyModel?> _04(int id, string schema, string conn);
    }
}