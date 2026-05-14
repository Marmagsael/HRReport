using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IMatrixpagibigDataAccess
    {
        Task<MatrixpagibigModel?> _01(MatrixpagibigModel matrixpagibig, string schema, string conn);
        Task<MatrixpagibigModel?> _02(int id, string schema, string conn);
        Task<List<MatrixpagibigModel?>?> _02Revisions(string schema, string conn);
        Task<MatrixpagibigModel?> _03(int id, MatrixpagibigModel matrixpagibig, string schema, string conn);
        Task<MatrixpagibigModel?> _04(int id, string schema, string conn);
    }
}