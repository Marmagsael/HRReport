namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface IOldPis_TblMakerDataAccess
    {
        Task _01OldPisTable(string? schema, string? connName = "MySqlConn");
    }
}
