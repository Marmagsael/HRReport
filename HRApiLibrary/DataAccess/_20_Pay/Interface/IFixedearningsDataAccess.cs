using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IFixedearningsDataAccess
    {
        Task<FixedearningsModel?>           _01(FixedearningsModel fixedearnings, string schema, string conn);
        Task<FixedearningsModel?>           _02(int id, string schema, string conn);
        Task<List<FixedearningsModel?>?>    _02ByEmpnumber(string empnumber, string paydb, string conn);
        Task<List<FixedearningsModel?>?>    _02By_PayTrnPrd(string trn, string paydb, string conn); 
        Task<FixedearningsModel?>           _03(int id, FixedearningsModel fixedearnings, string schema, string conn);
        Task<FixedearningsModel?>           _04(int id, string schema, string conn);
    }
}