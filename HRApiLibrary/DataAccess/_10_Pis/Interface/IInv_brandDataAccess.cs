using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInv_brandDataAccess
    {
        Task<Inv_brandModel?> _01(Inv_brandModel inv_brand, string schema, string conn);
        Task<Inv_brandModel?> _02(int id, string schema, string conn);
        Task<Inv_brandModel?> _03(int id, Inv_brandModel inv_brand, string schema, string conn);
        Task<Inv_brandModel?> _04(int id, string schema, string conn);
    }
}