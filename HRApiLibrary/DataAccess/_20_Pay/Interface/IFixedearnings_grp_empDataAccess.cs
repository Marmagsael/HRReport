using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IFixedearnings_grp_empDataAccess
    {
        Task<Fixedearnings_grp_empModel?> _01(Fixedearnings_grp_empModel fixedearnings_grp_emp, string schema, string conn);
        Task<List<Fixedearnings_grp_empModel?>?> _02(int id, string schema, string conn);
        Task<Fixedearnings_grp_empModel?> _04_ByFixedEarnings_grpId(int id, string schema, string conn); 
        Task<Fixedearnings_grp_empModel?> _04_PerEmployee(int fixedEarnings_grpId, int empmasId, string schema,
            string conn); 
        
    }
}