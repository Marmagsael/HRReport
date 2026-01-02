using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IMatrixwtaxDataAccess
    {
        Task<MatrixwtaxModel?> _01(MatrixwtaxModel matrixwtax, string schema, string conn);
        Task<MatrixwtaxModel?> _02(int id, string schema, string conn);
        Task<List<MatrixwtaxModel?>?> _02Revisions(string schema, string conn);
        Task<MatrixwtaxModel?> _03(int id, MatrixwtaxModel matrixwtax, string schema, string conn);
        Task<MatrixwtaxModel?> _04(int id, string schema, string conn);
    }
}