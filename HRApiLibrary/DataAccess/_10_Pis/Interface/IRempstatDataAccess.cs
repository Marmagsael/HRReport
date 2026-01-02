using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IRempstatDataAccess
    {
        Task<RempstatModel?> _01(RempstatModel rempstat, string schema, string conn);
        Task<RempstatModel?> _02(int id, string schema, string conn);
        Task<List<RempstatModel?>?> _02(string schema, string conn);
        Task<RempstatModel?> _03(int id, RempstatModel rempstat, string schema, string conn);
        Task<List<RempstatModel>> _03(List<int> ids, string fieldName, int fieldVal, string schema, string conn);
        Task<RempstatModel?> _04(int id, string schema, string conn);
    }
}