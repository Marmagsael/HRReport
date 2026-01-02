using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface ILoanhdrDataAccess
    {
        Task<LoanhdrModel?> _01(LoanhdrModel loanhdr, string schema, string conn);
        Task<LoanhdrModel?> _02(int id, string schema, string conn);
        Task<LoanhdrModel?> _03(int id, LoanhdrModel loanhdr, string schema, string conn);
        Task<LoanhdrModel?> _04(int id, string schema, string conn);
    }
}