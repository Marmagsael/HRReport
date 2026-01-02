using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IDutyrenderedDataAccess
    {
        Task<DutyrenderedModel?> _01(DutyrenderedModel dutyrendered, string schema, string conn);
        Task<DutyrenderedModel?> _02(string acctNumber, string schema, string conn);
        Task<List<DutyrenderedModel?>?> _02s(string schema, string conn);
        Task<DutyrenderedModel?> _03(DutyrenderedModel dutyrendered, string schema, string conn);
        Task<DutyrenderedModel?> _04(string acctNumber, string schema, string conn);
    }
}