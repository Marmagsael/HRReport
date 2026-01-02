using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IInvdtlDataAccess
    {
        Task<InvdtlModel?> _01(InvdtlModel invdtl, string schema, string conn);
        Task<InvdtlModel?> _02(int id, string schema, string conn);
        Task<InvdtlModel?> _03(int id, InvdtlModel invdtl, string schema, string conn);
        Task<InvdtlModel?> _04(int id, string schema, string conn);
    }
}