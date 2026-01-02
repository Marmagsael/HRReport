using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class MatrixpagibigDataAccess : IMatrixpagibigDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MatrixpagibigDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MatrixpagibigModel?> _01(MatrixpagibigModel matrixpagibig, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Matrixpagibig (Revision, DateStart, DateEnd, FStart, FEnd, Ee, Er) values (@Revision, @DateStart, @DateEnd, @FStart, @FEnd, @Ee, @Er)";
        await _sql.ExecuteCmd<dynamic>(sql, matrixpagibig, conn);

        sql = $@"SELECT * FROM {schema}.Matrixpagibig WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MatrixpagibigModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MatrixpagibigModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Revision, DateStart, DateEnd, FStart, FEnd, Ee, Er from {schema}.Matrixpagibig where Id = @Id";
        var data = await _sql.FetchData<MatrixpagibigModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<MatrixpagibigModel?>?> _02Revisions(string schema, string conn)
    {
        string sql = $@"select  Revision from {schema}.Matrixpagibig order by Revision ";
        var data = await _sql.FetchData<MatrixpagibigModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<MatrixpagibigModel?> _03(int id, MatrixpagibigModel matrixpagibig, string schema, string conn)
    {
        string sql = $@"Update {schema}.Matrixpagibig set Revision = @Revision, DateStart = @DateStart, DateEnd = @DateEnd, FStart = @FStart, FEnd = @FEnd, Ee = @Ee, Er = @Er where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, matrixpagibig, conn);

        sql = $@" select  * from {schema}.Matrixpagibig x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixpagibigModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MatrixpagibigModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Matrixpagibig where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Matrixpagibig x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixpagibigModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}