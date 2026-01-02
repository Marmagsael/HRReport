using HRApiLibrary.Models._10_Pis;

public interface ITrandeviationapprovalDataAccess
{
    Task<TrandeviationapprovalModel?> _01(TrandeviationapprovalModel Trandeviationapproval, string schema, string conn);
    Task<List<TrandeviationapprovalModel?>> _02(string schema, string conn);
    Task<TrandeviationapprovalModel?> _02(int id, string schema, string conn);
    Task<List<TrandeviationapprovalModel?>> _02DistinctId(string schema, string conn);
    Task<TrandeviationapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TrandeviationapprovalModel?> _03(int id, TrandeviationapprovalModel Trandeviationapproval, string schema, string conn);
    Task<TrandeviationapprovalModel?> _04(int id, string schema, string conn);
}