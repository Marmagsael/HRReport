namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface IOldPis_TblMakerDataAccess
    {
        Task _01OldPisTable(string? schema, string? connName = "MySqlConn");
        Task _01OldPisDefaults(string? schema, string? sourceSchema, string? connName = "MySqlConn");
    }
}
