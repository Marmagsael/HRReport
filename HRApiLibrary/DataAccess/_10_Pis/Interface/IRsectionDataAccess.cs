using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRsectionDataAccess
    {
        Task<RsectionModel?> _01(RsectionModel rsection, string schema, string conn);
        Task<RsectionModel?> _02(int id, string schema, string conn);
        Task<List<RsectionModel?>?> _02(string schema, string conn);
        Task<List<RsectionModel?>?> _02ByDepartmentId(int departmentId, string schema, string conn);
        Task<RsectionModel?> _03(int id, RsectionModel rsection, string schema, string conn);
        Task<RsectionModel?> _04(int id, string schema, string conn);
    }
}