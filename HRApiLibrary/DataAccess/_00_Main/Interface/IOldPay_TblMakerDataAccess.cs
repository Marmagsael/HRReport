namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface IOldPay_TblMakerDataAccess
    {
        Task _01OldPayTable(string? schema, string? connName = "MySqlConn");
    }
}
