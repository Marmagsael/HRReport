using HRApiLibrary.Models._10_Pis;
using System.Threading.Tasks;

public interface ITraninvestigateapprovalDataAccess
{
    Task<TraninvestigateapprovalModel?> _01(TraninvestigateapprovalModel Traninvestigateapproval, string schema, string conn);
    Task<TraninvestigateapprovalModel?> _02(int id, string schema, string conn);
    Task<TraninvestigateapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TraninvestigateapprovalModel?> _03(int id, TraninvestigateapprovalModel Traninvestigateapproval, string schema, string conn);
    Task<TraninvestigateapprovalModel?> _04(int id, string schema, string conn);
}