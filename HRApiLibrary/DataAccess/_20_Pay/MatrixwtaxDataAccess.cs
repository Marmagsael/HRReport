using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class MatrixwtaxDataAccess : IMatrixwtaxDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public MatrixwtaxDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<MatrixwtaxModel?> _01(MatrixwtaxModel matrixwtax, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Matrixwtax (From_, To_, CountryCode, PeriodCode, TaxCode, SAmt, EAmt, Fix, Percentage, Revision) values (@From_, @To_, @CountryCode, @PeriodCode, @TaxCode, @SAmt, @EAmt, @Fix, @Percentage, @Revision)";
        await _sql.ExecuteCmd<dynamic>(sql, matrixwtax, conn);

        sql = $@"SELECT * FROM {schema}.Matrixwtax WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<MatrixwtaxModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<MatrixwtaxModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, From_, To_, CountryCode, PeriodCode, TaxCode, SAmt, EAmt, Fix, Percentage, Revision from {schema}.Matrixwtax where Id = @Id";
        var data = await _sql.FetchData<MatrixwtaxModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<MatrixwtaxModel?>?> _02Revisions(string schema, string conn)
    {
        string  sql     = $@"select distinct Revision from {schema}.Matrixwtax order by Revision";
        var     data    = await _sql.FetchData<MatrixwtaxModel?, dynamic>(sql, new {  }, conn);
        return  data;
    }
    

    public async Task<MatrixwtaxModel?> _03(int id, MatrixwtaxModel matrixwtax, string schema, string conn)
    {
        string sql = $@"Update {schema}.Matrixwtax set From_ = @From_, To_ = @To_, CountryCode = @CountryCode, PeriodCode = @PeriodCode, TaxCode = @TaxCode, SAmt = @SAmt, EAmt = @EAmt, Fix = @Fix, Percentage = @Percentage, Revision = @Revision where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, matrixwtax, conn);

        sql = $@" select  * from {schema}.Matrixwtax x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixwtaxModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<MatrixwtaxModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Matrixwtax where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Matrixwtax x where x.Id = @Id ;";
        var data = await _sql.FetchData<MatrixwtaxModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}










