namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainPisMasterfileMaker
    {
        Task _01MainPisMasterfile(string? schema = "MainPis", string? connName = "MySqlConn");
    }
}
