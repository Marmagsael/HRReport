using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IMatrixsssDataAccess
    {
        Task<MatrixsssModel?> _01(MatrixsssModel matrixsss, string schema, string conn);
        Task<MatrixsssModel?> _02(int id, string schema, string conn);
        Task<List<MatrixsssModel?>?> _02Revisions(string schema, string conn);
        Task<MatrixsssModel?> _03(int id, MatrixsssModel matrixsss, string schema, string conn);
        Task<MatrixsssModel?> _04(int id, string schema, string conn);
    }
}