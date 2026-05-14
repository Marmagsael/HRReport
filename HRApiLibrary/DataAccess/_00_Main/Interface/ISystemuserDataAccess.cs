using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface ISystemuserDataAccess
    {
        Task<SystemuserModel?>              _01(SystemuserModel user, string schema, string conn);
        Task<SystemuserModel?>              _02(int systemId, string schema, string conn);
        Task<List<SystemuserModel?>?>       _02ByStatus(string status, string schema, string conn); 
        Task<List<SystemuserModel?>?>       _02List(int systemId, string schema, string conn);
        Task<List<SystemuserModel?>?> _02Lst(int systemId, string schema, string conn);
        Task<List<SystemuserModel?>?> _02Lst_WName(int systemId, string schemaModule, string schemaPis, string conn);
        Task<SystemuserModel?>              _03(int systemId, SystemuserModel user, string schema, string conn);
        Task<SystemuserModel?>              _04(int systemId, string schema, string conn);
    }
}