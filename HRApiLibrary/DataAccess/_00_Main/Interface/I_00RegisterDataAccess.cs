using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00RegisterDataAccess
    {
        Task<RegisterOwnerModel> _01Owner(RegisterOwnerModel owner, string? schema, string? mainPisSchema, string? connName = "MySqlConn");
        Task<RegisterOwnerModel> _01Company(RegisterOwnerModel owner, string? schema, string? connName = "MySqlConn");
        Task _01OwnerEmpmas(RegisterOwnerModel owner, string? schema, string? connName = "MySqlConn");
        Task _01OwnerOldEmpmas(RegisterOwnerModel owner, string? connName = "MySqlConn");
        Task<RegisterEmployeeModel> _01Employee(RegisterEmployeeModel emp, string? schema, string? mainPisSchema, string? connName = "MySqlConn");

        Task<bool>                  _02EmailExists(string email, string? schema, string? connName = "MySqlConn");
        Task<V01CompanyLookup?>     _02CompanyByCode(string companyCode, string? schema, string? connName = "MySqlConn");
        Task<V01EmpmasLookup?>      _02OldEmpmasForRegister(string empNumber, string lastName, string oldPisSchema, string? connName = "MySqlConn");
        Task<V01EmpmasLookup?>      _02PisEmpmasByEmpNumber(string empNumber, string pisSchema, string? connName = "MySqlConn");
        Task<V01EmpmasLookup?>      _02OldEmpStatus(string empNumber, string oldPisSchema, string? connName = "MySqlConn");
        Task<bool>                  _02PendingInvite(int companyId, int empmasId, string? schema, string? connName = "MySqlConn");

        Task<bool>                  _02CodeTaken(string companyCode, string? schema, string? connName = "MySqlConn");
        Task<RegisterOwnerModel?>   _02OwnerByUserId(int userId, string? schema, string? mainPisSchema, string? connName = "MySqlConn");
        Task<V01CompanyLookup?>     _02CurrentAdminCompany(int userId, string? schema, string? connName = "MySqlConn");
    }
}
