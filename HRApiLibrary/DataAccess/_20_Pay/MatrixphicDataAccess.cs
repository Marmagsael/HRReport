using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class MatrixphicDataAccess : IMatrixphicDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MatrixphicDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MatrixphicModel?> _01(MatrixphicModel matrixphic, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Matrixphic (DateStart, DateEnd, FStart, FEnd, Ee, Er, Percent, Revision) values (@DateStart, @DateEnd, @FStart, @FEnd, @Ee, @Er, @Percent, @Revision)";
        await _sql.ExecuteCmd<dynamic>(sql, matrixphic, conn);

        sql = $@"SELECT * FROM {schema}.Matrixphic WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MatrixphicModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MatrixphicModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, DateStart, DateEnd, FStart, FEnd, Ee, Er, Percent, Revision from {schema}.Matrixphic where Id = @Id";
        var data = await _sql.FetchData<MatrixphicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<MatrixphicModel?>?> _02Revisions(string schema, string conn)
    {
        string sql = $@"select  distinct Revision from {schema}.Matrixphic order by Revision; ";
        var data = await _sql.FetchData<MatrixphicModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<MatrixphicModel?> _03(int id, MatrixphicModel matrixphic, string schema, string conn)
    {
        string sql = $@"Update {schema}.Matrixphic set DateStart = @DateStart, DateEnd = @DateEnd, FStart = @FStart, FEnd = @FEnd, Ee = @Ee, Er = @Er, Percent = @Percent, Revision = @Revision where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, matrixphic, conn);

        sql = $@" select  * from {schema}.Matrixphic x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixphicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MatrixphicModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Matrixphic where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Matrixphic x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixphicModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}