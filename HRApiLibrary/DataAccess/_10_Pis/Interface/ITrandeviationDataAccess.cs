using HRApiLibrary.Models._10_Pis;

public interface ITrandeviationDataAccess
{
    Task<TrandeviationModel?> _01(TrandeviationModel trandeviation, string schema, string conn);
    Task<TrandeviationModel?> _02(int id, string schema, string conn);
    Task<TrandeviationModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TrandeviationModel?> _02ByTrn(string trn, string schema, string conn);
    Task<TrandeviationModel?> _03(int id, TrandeviationModel trandeviation, string schema, string conn);
    Task<TrandeviationModel?> _04(int id, string schema, string conn);
}