using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00_CurrencyDataAccess
    {
        Task<CompanyUserTypeModel?> _01(CompanyUserTypeModel companyusertype, string schema, string conn);
        Task<CompanyUserTypeModel?> _02(int id, string schema, string conn);
        Task<CompanyUserTypeModel?> _03(int id, CompanyUserTypeModel companyusertype, string schema, string conn);
        Task<CompanyUserTypeModel?> _04(int id, string schema, string conn);
    }
}