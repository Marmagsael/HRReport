using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00ClientDataAccess
    {
        Task<ClientModel?> _01(ClientModel client, string schema, string conn);
        Task<ClientModel?> _02(int id, string schema, string conn);
        Task<ClientModel?> _03(int id, ClientModel client, string schema, string conn);
        Task<ClientModel?> _04(int id, string schema, string conn);
    }
}