using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInv_statusDataAccess
    {
        Task<Inv_statusModel?> _01(Inv_statusModel inv_status, string schema, string conn);
        Task<Inv_statusModel?> _02(int id, string schema, string conn);
        Task<Inv_statusModel?> _03(int id, Inv_statusModel inv_status, string schema, string conn);
        Task<Inv_statusModel?> _04(int id, string schema, string conn);
    }
}