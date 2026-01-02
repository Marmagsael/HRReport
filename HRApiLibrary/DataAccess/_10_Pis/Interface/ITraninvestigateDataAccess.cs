using HRApiLibrary.Models._10_Pis;

public interface ITraninvestigateDataAccess
{
    Task<TraninvestigateModel?> _01(TraninvestigateModel Traninvestigate, string schema, string conn);
    Task<TraninvestigateModel?> _02(int id, string schema, string conn);

    Task<TraninvestigateModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TraninvestigateModel?> _03(int id, TraninvestigateModel Traninvestigate, string schema, string conn);
    Task<TraninvestigateModel?> _04(int id, string schema, string conn);
}