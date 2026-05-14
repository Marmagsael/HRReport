using HRApiLibrary.Models._10_Pis;
using System.Threading.Tasks;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IDeviationDataAccess
    {
        Task<DeviationModel?> _01(DeviationModel deviation, string schema, string conn);
        Task<DeviationModel?> _02(int id, string schema, string conn);
        Task<List<DeviationModel>?> _02ByEmpNumber(string empno, string schema, string conn);
        Task<DeviationModel?> _02ByControlNo(string controlNo, string schema, string conn);
        Task<DeviationModel?> _03(int id, DeviationModel deviation, string schema, string conn);
        Task<DeviationModel?> _04(int id, string schema, string conn);
    }
}