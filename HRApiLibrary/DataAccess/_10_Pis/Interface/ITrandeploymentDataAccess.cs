using HRApiLibrary.Models._10_Pis;

public interface ITrandeploymentDataAccess
{
    Task<TrandeploymentModel?> _01(TrandeploymentModel tranmovement, string schema, string conn);
    Task<TrandeploymentModel?> _02(int id, string schema, string conn);
    Task<TrandeploymentModel?> _02ByEmpmasId(int empmasId, string schema, string conn);
    Task<TrandeploymentModel?> _02ByTrnNumber(string trnNumber, string schema, string conn);
    Task<TrandeploymentModel?> _03(int id, TrandeploymentModel tranmovement, string schema, string conn);
    Task<TrandeploymentModel?> _04(int id, string schema, string conn);
}