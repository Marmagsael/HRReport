using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IDedmandatoryDataAccess
    {
        Task<DedmandatoryModel?> _01(DedmandatoryModel dedmandatory, string schema, string conn);
        Task<DedmandatoryModel?> _02(int id, string schema, string conn);
        Task<List<DedmandatoryModel?>?> _02(string schema, string conn);
        Task<List<DedmandatoryModel?>?> _02ByStatus(string status, string schema, string conn);
        Task<DedmandatoryModel?> _03(int id, DedmandatoryModel dedmandatory, string schema, string conn);
        Task<DedmandatoryModel?> _03Stop(int id, string schema, string conn);
        Task<DedmandatoryModel?> _04(int id, string schema, string conn);
    }
}