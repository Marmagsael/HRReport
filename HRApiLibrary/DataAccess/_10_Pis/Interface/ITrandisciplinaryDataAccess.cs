using HRApiLibrary.Models._10_Pis;

public interface ITrandisciplinaryDataAccess
{
    Task<TrandisciplinaryModel?> _01(TrandisciplinaryModel Trandisciplinary, string schema, string conn);
    Task<TrandisciplinaryModel?> _02(int id, string schema, string conn);
    Task<TrandisciplinaryModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TrandisciplinaryModel?> _03(int id, TrandisciplinaryModel Trandisciplinary, string schema, string conn);
    Task<TrandisciplinaryModel?> _04(int id, string schema, string conn);
}