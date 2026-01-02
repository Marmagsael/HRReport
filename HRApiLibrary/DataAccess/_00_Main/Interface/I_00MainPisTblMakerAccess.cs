namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainPisTblMakerAccess
    {
        Task _01MainPisTable(string schema = "MainPis", string connName = "MySqlConn");
        Task _01MainPisTableInternal(string schema, string connName = "MySqlConn");
        Task _01UserTable(string schema, string connName = "MySqlConn");
    }
}