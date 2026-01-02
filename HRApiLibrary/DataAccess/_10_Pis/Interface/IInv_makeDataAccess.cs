using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInv_makeDataAccess
    {
        Task<Inv_makeModel?> _01(Inv_makeModel inv_make, string schema, string conn);
        Task<Inv_makeModel?> _02(int id, string schema, string conn);
        Task<Inv_makeModel?> _03(int id, Inv_makeModel inv_make, string schema, string conn);
        Task<Inv_makeModel?> _04(int id, string schema, string conn);
    }
}