using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay
{
    public interface ICoaDataAccess
    {
        Task<CoaModel?>             _01(CoaModel coa, string schema, string conn);
        Task<CoaModel?>             _02(int id, string schema, string conn);
        Task<List<CoaModel?>?>      _02(string schema, string conn);
        Task<List<CoaModel?>?>      _02_CheckInTbltran(string acctNumber, string schema, string conn);
        Task<List<CoaModel?>?>      _02_Earnings_HasROB(string schema, string conn);
        Task<List<CoaModel?>?>      _02_Earnings_HasNoROB(string schema, string conn);
        Task<List<CoaModel?>?>      _02_Earnings(string schema, string conn);
        Task<List<CoaModel?>?>      _02_Earnings_TaxMap(string schema, string conn); 
        Task<List<CoaModel?>?>      _02_Earnings_SSSMap(string schema, string conn); 
        Task<List<CoaModel?>?>      _02_Earnings_PHICMap(string schema, string conn); 
        Task<List<CoaModel?>?>      _02_Deductions(string schema, string conn);
        Task<List<CoaModel?>?>      _02ByType(string acctType, string schema, string conn);
        Task<List<CoaModel?>?>      _02ByTypeNotLocked(string acctType, string schema, string conn); 
        Task<CoaModel?>             _03(int id, CoaModel coa, string schema, string conn);
        Task<CoaModel?>             _03Basic(CoaModel coa, string schema, string conn); 
        Task<CoaModel?>             _04(string acctNumber, string schema, string conn);

        Task _0104_Earnings_TaxMap(CoaModel coa, string paydb, string conn); 
        Task _0104_Earnings_SSSMap(CoaModel coa, string paydb, string conn); 
        Task _0104_Earnings_PHICMap(CoaModel coa, string paydb, string conn); 
    }
}