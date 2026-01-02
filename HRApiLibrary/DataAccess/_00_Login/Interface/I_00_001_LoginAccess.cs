using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._00_Login.Interface
{
    public interface I_00_001_LoginAccess
    {
        Task<string> CreateToken(int id, string loginName, string schema = "Main");
        Task<LoginOutputModel?> _001_LoginByEmpNumber_CentralizedSchema(string schema, string empNumber, string password);
        Task<UsersModel?> _011_LoginByLoginName_Main(string loginname, string password, string schema = "Main");
        Task<UsersModel?> _012_LoginByEmail_Main(string email, string password, string schema = "Main");
    }
}