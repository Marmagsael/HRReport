using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainTblMakerAccess
    {
        Task _01MainTable(string schema = "Main", string connName = "MySqlConn");
        Task<SchemaStructureModel?> _02_TableExists(string schema, string tableName, string connName);
    }
}