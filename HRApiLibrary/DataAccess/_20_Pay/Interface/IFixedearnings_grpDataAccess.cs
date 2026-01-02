using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IFixedearnings_grpDataAccess
    {
        Task<Fixedearnings_grpModel?>        _01(Fixedearnings_grpModel fixedearnings_grp, string schema, string conn);
        Task<Fixedearnings_grpModel?>        _02(int id, string schema, string conn);
        Task<List<Fixedearnings_grpModel?>?> _02_Active(string schema, string conn);
        Task<List<Fixedearnings_grpModel?>?> _02_Active(string fld, string schema, string conn);
        Task<List<Fixedearnings_grpModel?>?> _02_ByPgrpId_Active(int pgrpId, string schema, string conn);
        
        Task<Fixedearnings_grpModel?>        _03(int id, Fixedearnings_grpModel fixedearnings_grp, string schema, string conn);
        
        Task<Fixedearnings_grpModel?>        _03_Terminate(int id, int terminatedbyId, string schema, string conn); 
        Task<Fixedearnings_grpModel?>        _04(int id, string schema, string conn);
    }
}