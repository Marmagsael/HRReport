using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._90_Utils;
using System.Security.Claims;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainDA
    {
        Task<CityModel?>                            _01City(CityModel city, string schema, string conn);
        Task<CompanyUsersModel?>                    _01CompanyUsers(CompanyUsersModel companyusers, string schema, string conn);
        Task<CountryModel?>                         _01Country(CountryModel country, string schema, string conn);
        Task<MenusModel?>                           _01Menu(string menuName, MenusModel Menus, string schema, string conn);
        Task<ProvinceStateModel?>                   _01ProvinceState(ProvinceStateModel ProvinceState, string schema, string conn);
        Task<UserCompanyModel?>                     _01UserCompany(UserCompanyModel userscompany, string schema, string conn);
        Task<UsersModel?>                           _01Users(UsersModel user, string schema = "Main", string conn = "MySqlConn");
        Task<CityModel?>                            _02City(int id, string schema, string conn);
        Task<List<CityModel?>>                      _02CityPerRegion(int regionId, string schema, string conn);
        Task<List<CompaniesAssignedToUserModel?>>   _02CompaniesAssignedToUser(int empmasId, string schema, string conn);
        Task<CompanyUsersModel?>                    _02CompanyUsers(int userId, int companyId, string schema, string conn);
        Task<List<CompanyUsersModel?>>              _02CompanyUsersPerCompany(int companyId, string schema, string conn);
        Task<List<CompaniesAssignedToUserModel?>>   _02CompanyUsers_Employment_WTypeWCoDtls_List(int userId, string schema, string conn);
        Task<List<CompaniesAssignedToUserModel?>>   _02CompanyUsers_Owned_WTypeWCoDtls_List(int userId, string schema, string conn);
        Task<CompaniesAssignedToUserModel?>         _02CompanyUsers_WTypeWCoDtls(int userId, int companyId, string schema, string conn);
        Task<List<CompaniesAssignedToUserModel?>>   _02CompanyUsers_WTypeWCoDtls_List(int userId, string schema, string conn);
        Task<CountryModel?>                         _02Country(int id, string schema, string conn);
        Task<List<CountryModel?>>                   _02CountryList(string schema, string conn);
        Task<MenusModel?>                           _02Menu(string menuName, int id, string schema, string conn);
        Task<List<MenusModel?>>                     _02MenuList(string menuName, int id, string schema, string conn);
        Task<List<MenusModel?>>                     _02MenuList_ByModule(string menuName, string moduleNo, string schema, string conn);
        Task<ProvinceStateModel?>                   _02ProvinceState(int id, string schema, string conn);
        Task<List<ProvinceStateModel?>>             _02ProvinceStatePerCountry(int countryId, string schema, string conn);
        UserClaimsModel                             _02UserClaimsContent(IEnumerable<Claim?> claims);
        Task<UserCompanyModel?>                     _02UserCompany(int id, string schema = "Main", string conn = "MySqlConn");
        Task<List<UserCompanyModel?>>               _02UserCompanyPerUser(int id, string schema = "Main", string conn = "MySqlConn");
        Task<List<UserCompanyModel?>>               _02UserCompanyPerOwnerId(int id, string schema = "Main", string conn = "MySqlConn");
        Task<List<CompaniesAssignedToUserModel?>>   _02UserEmploymentList(int empmasId, string schema, string conn);
        Task<UsersModel?>                           _02UsersByEmail(string email, string schema = "Main", string conn = "MySqlConn");
        Task<List<UsersModel?>?>                    _02UsersByEmailLst(string email, string schema = "Main", string conn = "MySqlConn");
        Task<UsersModel?>                           _02UsersById(int id, string schema = "Main", string conn = "MySqlConn");
        Task<List<UsersModel?>?>                    _02UsersByIdAndEmailLst(int id, string email, string schema = "Main", string conn = "MySqlConn"); 
        Task<UsersModel?>                           _02UsersByLoginName(string loginname, string schema = "Main", string conn = "MySqlConn");
        Task<UsersModel?>                           _02UsersLoginEmail(string email, string password, string schema = "Main", string conn = "MySqlConn");
        Task<UsersModel?>                           _02UsersLoginLoginName(string loginName, string password, string schema = "Main", string conn = "MySqlConn");
        Task<CityModel?>                            _03City(int id, CityModel city, string schema, string conn);
        Task<CompanyUsersModel?>                    _03CompanyUsers(int userId, int companyId, CompanyUsersModel companyusers, string schema, string conn);
        Task<CountryModel?>                         _03Country(int id, CountryModel country, string schema, string conn);
        Task<MenusModel?>                           _03Menu(string menuName, int id, MenusModel Menus, string schema, string conn);
        Task<MenusModel?>                           _03MenuSpecificIsSelected(string menuName, int id, MenusModel Menus, string schema, string conn);
        Task<ProvinceStateModel?>                   _03ProvinceState(int id, ProvinceStateModel ProvinceState, string schema, string conn);
        Task<UserCompanyModel?>                     _03UserCompany(int id, UserCompanyModel userscompany, string schema, string conn);
        Task<UsersModel?>                           _03Users(int id, UsersModel user, string schema = "Main", string conn = "MySqlConn");
        Task<CityModel?>                            _04City(int id, string schema, string conn);
        Task<CompanyUsersModel?>                    _04CompanyUsers(int userId, int companyId, string schema, string conn);
        Task<CountryModel?>                         _04Country(int id, string schema, string conn);
        Task<MenusModel?>                           _04Menu(string menuName, int id, string schema, string conn);
        Task<ProvinceStateModel?>                   _04ProvinceState(int id, string schema, string conn);
        Task<UserCompanyModel?>                     _04UserCompany(int id, string schema, string conn);
        Task                                        _04Users(int id, string schema = "Main", string conn = "MySqlConn");
        Task<List<Uc_accessreqModel?>>              _02Uc_Access_ByEmpmasId(int empmasId, string schema, string conn);
        Task<List<Uc_accessreqModel?>>              _02Uc_Access_ProfileAccessRequest(int empmasId, string schema, string conn); 
        Task<List<Uc_accessreqModel?>>              _02Uc_Access_NotAddressed(int empmasId, string schema, string conn);
        Task<List<Uc_accessreqModel?>>              _01Uc_Access(Uc_accessreqModel uca, string schema, string conn);
        Task<Uc_accessreqModel?>                    _03Uc_Access_Approve(Uc_accessreqModel uc_accessreq, string schema, string conn);
        Task<List<Uc_accessreqModel?>?>             _03Uc_Access_Deny(Uc_accessreqModel uc_accessreq, string schema, string conn);

        Task<Uc_accessreqModel?>                    _04Uc_Access_Request(int empmasId, string schema, string conn);
        Task<List<SystemuserModel?>?>               _02SystemUsers(int systemId, string schema, string conn);
        Task<List<AtttypeModel?>>                   _02AttTypes(string schema, string conn);
        Task<List<AttdutytypeModel?>>               _02AttDutyTypes(string schema, string conn);
        Task<List<Uc_accessreqModel?>> _02Uc_Accessreq_ById(int id, string schema, string conn);
    }
}