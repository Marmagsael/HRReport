namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface I_20_002_PayTblMaker
    {
        Task _00_002_SystemUser(string schema, string connName);
        Task _00_003_SystemUserModuleAccess(string schema, string connName);
        Task _00_004_SystemUserOtherAccess(string schema, string connName);
        Task _01(string schema, string country = "PH", string connName = "MySqlConn");
        Task _01PH(string schema, string country = "PH", string connName = "MySqlConn");
    }
}