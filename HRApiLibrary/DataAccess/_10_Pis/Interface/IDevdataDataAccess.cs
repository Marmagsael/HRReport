using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IDevdataDataAccess
    {
        Task<RdevdataModel?> _01(RdevdataModel devdata, string schema, string conn);
        Task<RdevdataModel?> _02(int id, string schema, string conn);
        Task<List<RdevdataModel?>?> _02(string schema, string conn);
        Task<RdevdataModel?> _03(int id, RdevdataModel devdata, string schema, string conn);
        Task<RdevdataModel?> _04(int id, string schema, string conn);
    }
}