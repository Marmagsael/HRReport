using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class MatrixsssDataAccess : IMatrixsssDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MatrixsssDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MatrixsssModel?> _01(MatrixsssModel matrixsss, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Matrixsss (DateStart, DateEnd, FStart, FEnd, Ee, Er, Ecc, Compensation, Revision) values (@DateStart, @DateEnd, @FStart, @FEnd, @Ee, @Er, @Ecc, @Compensation, @Revision)";
        await _sql.ExecuteCmd<dynamic>(sql, matrixsss, conn);

        sql = $@"SELECT * FROM {schema}.Matrixsss WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MatrixsssModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MatrixsssModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, DateStart, DateEnd, FStart, FEnd, Ee, Er, Ecc, Compensation, Revision from {schema}.Matrixsss where Id = @Id";
        var data = await _sql.FetchData<MatrixsssModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<MatrixsssModel?>?> _02Revisions(string schema, string conn)
    {
        string sql = $@"select distinct Revision from {schema}.Matrixsss order by Revision ";
        var data = await _sql.FetchData<MatrixsssModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<MatrixsssModel?> _03(int id, MatrixsssModel matrixsss, string schema, string conn)
    {
        string sql = $@"Update {schema}.Matrixsss set DateStart = @DateStart, DateEnd = @DateEnd, FStart = @FStart, FEnd = @FEnd, Ee = @Ee, Er = @Er, Ecc = @Ecc, Compensation = @Compensation, Revision = @Revision where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, matrixsss, conn);

        sql = $@" select  * from {schema}.Matrixsss x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixsssModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MatrixsssModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Matrixsss where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Matrixsss x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixsssModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}