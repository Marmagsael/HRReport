using HRApiLibrary.Models._10_Pis;

public interface ITrandisciplinaryapprovalDataAccess
{
    Task<TrandisciplinaryapprovalModel?> _01(TrandisciplinaryapprovalModel Trandisciplinary, string schema, string conn);
    Task<TrandisciplinaryapprovalModel?> _02(int id, string schema, string conn);
    Task<TrandisciplinaryapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TrandisciplinaryapprovalModel?> _03(int id, TrandisciplinaryapprovalModel Trandisciplinary, string schema, string conn);
    Task<TrandisciplinaryapprovalModel?> _04(int id, string schema, string conn);
}