using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRdepapproverDataAccess
    {
        Task<RdepapproverModel?> _01(RdepapproverModel rdepapprover, string schema, string conn);
        Task<List<RdepapproverModel?>?> _02(string schema, string conn);
        Task<List<RdepapproverModel?>?> _02ByModule(string module, string schema, string conn);
        Task<RdepapproverModel?> _04(int systemid, string module, string schema, string conn);
    }
}