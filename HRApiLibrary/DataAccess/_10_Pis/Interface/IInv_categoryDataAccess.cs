using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInv_categoryDataAccess
    {
        Task<Inv_categoryModel?> _01(Inv_categoryModel inv_category, string schema, string conn);
        Task<Inv_categoryModel?> _02(int id, string schema, string conn);
        Task<Inv_categoryModel?> _03(int id, Inv_categoryModel inv_category, string schema, string conn);
        Task<Inv_categoryModel?> _04(int id, string schema, string conn);
    }
}