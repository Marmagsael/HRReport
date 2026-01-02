using HRApiLibrary.Models._10_Pis;

public interface ITranexonerateapprovalDataAccess
{
    Task<TranexonerateapprovalModel?> _01(TranexonerateapprovalModel Tranexonerateapproval, string schema, string conn);
    Task<TranexonerateapprovalModel?> _02(int id, string schema, string conn);
    Task<TranexonerateapprovalModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TranexonerateapprovalModel?> _03(int id, TranexonerateapprovalModel Tranexonerateapproval, string schema, string conn);
    Task<TranexonerateapprovalModel?> _04(int id, string schema, string conn);
}