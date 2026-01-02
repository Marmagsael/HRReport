using HRApiLibrary.Models._10_Pis;

public interface ITranexonerateDataAccess
{
    Task<TranexonerateModel?> _01(TranexonerateModel Tranexonerate, string schema, string conn);
    Task<TranexonerateModel?> _02(int id, string schema, string conn);
    Task<TranexonerateModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TranexonerateModel?> _03(int id, TranexonerateModel Tranexonerate, string schema, string conn);
    Task<TranexonerateModel?> _04(int id, string schema, string conn);
}