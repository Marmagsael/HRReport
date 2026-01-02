using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IParaDataAccess
    {
        Task<ParaModel?> _01(ParaModel para, string schema, string conn);
        Task<ParaModel?> _02(string schema, string conn);
        Task<ParaModel?> _02(string mode, string schema, string conn);
        Task<ParaModel?> _03(int id, ParaModel para, string schema, string conn);
        Task<ParaModel?> _04(int id, string schema, string conn);
    }
}