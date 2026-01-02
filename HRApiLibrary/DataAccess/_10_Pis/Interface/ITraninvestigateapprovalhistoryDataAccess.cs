using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis.Interface
{
    public interface ITraninvestigateapprovalhistoryDataAccess
    {
        Task<TraninvestigateapprovalhistoryModel?> _01(TraninvestigateapprovalhistoryModel Traninvestigateapprovalhistory, string schema, string conn);
        Task<TraninvestigateapprovalhistoryModel?> _02(int id, string schema, string conn);
        Task<TraninvestigateapprovalhistoryModel?> _02ByTrn(string trannumber, string schema, string conn);
        Task<TraninvestigateapprovalhistoryModel?> _03(int id, TraninvestigateapprovalhistoryModel Traninvestigateapprovalhistory, string schema, string conn);
        Task<TraninvestigateapprovalhistoryModel?> _04(int id, string schema, string conn);
    }
}