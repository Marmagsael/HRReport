namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00MainDataMakerAccess
    {
        Task _01MainDefaultDatas(string schema = "Main", string connName = "MySqlConn");
        void _01UsersCompany_DefaultDatas(int userId, string companyCode, string companyName, int currencyId, string schema = "Main", string connName = "MySqlConn");
    }
}