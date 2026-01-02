using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IEmpmasInternalDataAccess
    {
        Task<EmpmasInternalModel?>              _01(EmpmasInternalModel empmas, string schema, string conn);
        Task<EmpmasInternalModel?>              _02(int id, string schema, string conn);
        Task<EmpmasInternalModel?>              _02(string empnumber, string schema, string conn);
        Task<List<EmpmasInternalModel?>?>       _02(string schema, string conn);
        Task<List<EmpmasInternalModel?>?>       _02byEmpnumber(string empnumber, string schema, string conn);
        Task<List<EmpmasInternalModel?>?>       _02byEmpnumber(string empnumber, string pisdb, string paydb, string conn);
        Task<List<EmpmasInternalModel?>?>       _02FilterByName(string name, string schema, string conn);
        Task<List<EmpmasInternalModel?>?>       _02FilterByName(string name, int approverlvl, string schema, string conn);
        Task<List<EmpmasInternalModel?>?> _02FilterByName(string name, string pisdb, string paydb, string conn);
        Task<EmpmasInternalModel?>              _03(int id, EmpmasInternalModel empmas, string schema, string conn);
        Task<EmpmasInternalModel?>              _03SystemId(int systemId, string schema, string conn);
        Task<EmpmasInternalModel?>              _03SystemId(int empmasId, int systemId, string schema, string conn);
        Task<EmpmasInternalModel?>              _04(int id, string schema, string conn);
    }
}