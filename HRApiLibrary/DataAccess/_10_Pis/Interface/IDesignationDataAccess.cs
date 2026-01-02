using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IDesignationDataAccess
    {
        Task<DesignationModel?> _01(DesignationModel designation, string schema, string conn);
        Task<DesignationModel?> _02(int id, string schema, string conn);
        Task<List<DesignationModel?>?> _02(string schema, string conn);
        Task<DesignationModel?> _03(int id, DesignationModel designation, string schema, string conn);
        Task<DesignationModel?> _04(int id, string schema, string conn);
    }
}