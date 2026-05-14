using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IUserpayinprocessDataAccess
    {
        Task<UserpayinprocessModel?> _01(UserpayinprocessModel userpayinprocess, string schema, string conn);
        Task<UserpayinprocessModel?> _02(int id, string schema, string conn);
        Task<UserpayinprocessModel?> _03(int id, UserpayinprocessModel userpayinprocess, string schema, string conn);
        Task<UserpayinprocessModel?> _04(int id, string schema, string conn);
    }
}