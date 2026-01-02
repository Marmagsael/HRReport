using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmploymenttypeDataAccess
    {
        Task<EmploymenttypeModel?>          _01(EmploymenttypeModel employmenttype, string schema, string conn);
        Task<EmploymenttypeModel?>          _02(int id, string schema, string conn);
        Task<List<EmploymenttypeModel?>?>   _02(string schema, string conn);
        Task<EmploymenttypeModel?>          _03(int id, EmploymenttypeModel employmenttype, string schema, string conn);
        Task<EmploymenttypeModel?>          _04(int id, string schema, string conn);
    }
}