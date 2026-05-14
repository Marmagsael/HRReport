using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface;
public interface IPisEmpmasDataAccess
{
    Task<PisEmpmasModel?>           _02(int id, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02(string schema, string conn);
    Task<PisEmpmasModel?>           _02ByEmpnumber(string empnumber, string schema, string conn);
    Task<List<PisEmpmasModel?>?>   _02EmpByStatus(List<int> empstatusId, string schema, string conn);
    Task<List<PisEmpmasModel?>?>   _02ByStatus(List<int> empstatusId, string schema, string conn); // Detailed
    Task<PisEmpmasModel?>           _02BySystemId(int systemId, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02BySystemIds(int systemId, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02BySystemIdLst(int systemId, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02FilterByName(string name, int approverlvl, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02ByEmpnumbers(string empnumber, string schema, string conn);
    Task<List<PisEmpmasModel?>?>   _02ByEmpIds(List<int> ids, string schema, string conn);
    Task<List<PisEmpmasModel?>?>    _02FilterByName(string name, string schema, string conn);
}
