using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInv_typeDataAccess
    {
        Task<Inv_typeModel?> _01(Inv_typeModel inv_type, string schema, string conn);
        Task<Inv_typeModel?> _02(int id, string schema, string conn);
        Task<List<Inv_typeModel?>?> _02(string schema, string conn);
        Task<Inv_typeModel?> _03(int id, Inv_typeModel inv_type, string schema, string conn);
        Task<Inv_typeModel?> _04(int id, string schema, string conn);
    }
}