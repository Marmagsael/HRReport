using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface IAtttemplateDataAccess
    {
        Task<AtttemplateModel?> _01(AtttemplateModel atttemplate, string schema, string conn);
        Task<AtttemplateModel?> _02(int empmasId, string schema, string conn);
        AtttemplateModel? _02NoSchedule(int empmasId);
        Task<AtttemplateModel?> _03(int empmasId, AtttemplateModel atttemplate, string schema, string conn);
        Task<AtttemplateModel?> _04(int empmasId, string schema, string conn);
    }
}