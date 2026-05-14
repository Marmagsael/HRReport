using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface ILoansDataAccess
    {
        Task<LoansModel?> _01(LoansModel loans, string schema, string conn);
        Task<LoansModel?> _02(int id, string schema, string conn);
        Task<List<LoansModel?>?> _02ByEmpNumbers(string empNumber, string schema, string conn); 
        Task<LoansModel?> _03(int id, LoansModel loans, string schema, string conn);
        Task<LoansModel?> _03ChangeStatus(int id, LoansModel loans, string schema, string conn); 
        Task<LoansModel?> _04(int id, string schema, string conn);
    }
}