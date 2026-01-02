using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmploytypeDataAccess
    {
        Task<EmploytypeModel?> _01(EmploytypeModel employtype, string schema, string conn);
        Task<EmploytypeModel?> _02(int id, string schema, string conn);
        Task<List<EmploytypeModel?>?> _02(string schema, string conn);
        Task<EmploytypeModel?> _03(int id, EmploytypeModel employtype, string schema, string conn);
        Task<EmploytypeModel?> _04(int id, string schema, string conn);
    }
}