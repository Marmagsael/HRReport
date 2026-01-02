using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface ITbltranDataAccess
    {
        Task<TbltranModel?> _01(string tbl, TbltranModel tbltran, string schema, string conn);
        Task<TbltranModel?> _02(string tbl, string trn, string acctNumber, int empmasId, string schema, string conn);
        Task<List<TbltranModel?>?> _02ByTrn(string tbl, string trn, string schema, string conn);
        Task<TbltranModel?> _03(string tbl, TbltranModel tbltran, string schema, string conn);
        Task<TbltranModel?> _04(string tbl, string trn, string acctNumber, int empmasId, string schema, string conn);
        Task _04TmpByTrn(string trn, string schema, string conn);
    }
}