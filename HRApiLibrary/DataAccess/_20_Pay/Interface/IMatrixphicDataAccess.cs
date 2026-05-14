using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IMatrixphicDataAccess
    {
        Task<MatrixphicModel?> _01(MatrixphicModel matrixphic, string schema, string conn);
        Task<MatrixphicModel?> _02(int id, string schema, string conn);
        Task<List<MatrixphicModel?>?> _02Revisions(string schema, string conn);
        Task<MatrixphicModel?> _03(int id, MatrixphicModel matrixphic, string schema, string conn);
        Task<MatrixphicModel?> _04(int id, string schema, string conn);
    }
}