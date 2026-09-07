using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._90_Utils;


namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OPisUsrDataAccess : IOPisUsrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OPisUsrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OPisUsrModel?> _01(OPisUsrModel usr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Usr (LogName, UserName, Password, status, withInsuranceAccess, Email) values (@LogName, @UserName, @Password, @status, @withInsuranceAccess, @Email)";
        await _sql.ExecuteCmd<dynamic>(sql, usr, conn);

        sql = $@"SELECT * FROM {schema}.Usr WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<OPisUsrModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OPisUsrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  LogName, UserName, Password, status, withInsuranceAccess, Email from {schema}.Usr where Id = @Id";
        var data = await _sql.FetchData<OPisUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<GridResultModel<OPisUsrModel>> _02Grid(GridRequestModel request, string schemapay, string conn)
    {
        var columns = new Dictionary<string, string>
        {
            ["LogName"] = "u.LogName",
            ["UserName"] = "u.UserName",
            ["Status"] = "u.Status"
        };

        // SORTING
        var sortColumn = columns.GetValueOrDefault(
            request.SortField, "u.UserName");

        var sortOrder =
            request.SortDirection == "DESC" ? "DESC" : "ASC";

        // PARAMETERS
        var parameters = new Dictionary<string, object>
        {
            ["PageSize"] = request.PageSize,
            ["Offset"] = request.Offset
        };

        // FILTERING
        var where = GridHelperDataAccess.BuildWhere(request.Filters, columns, parameters);

        // RECORDS COUNT
        string countSql = $@" SELECT COUNT(*)  FROM {schemapay}.usr u  {where}";
        var totalResult = await _sql.FetchData<int, dynamic>(countSql, parameters, conn);
        var total = totalResult?.FirstOrDefault() ?? 0;

        // DATA
        string sql = $@"  SELECT  * FROM {schemapay}.usr u  {where}  ORDER BY {sortColumn} {sortOrder}  LIMIT @Offset, @PageSize";

        var data = await _sql.FetchData<OPisUsrModel, dynamic>(sql, parameters, conn);

        return new GridResultModel<OPisUsrModel>
        {
            Data = data ?? new List<OPisUsrModel>(),
            Total = total
        };
    }


    public async Task<OPisUsrModel?> _03(int id, OPisUsrModel usr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Usr set LogName = @LogName, UserName = @UserName, Password = @Password, status = @status, withInsuranceAccess = @withInsuranceAccess, Email = @Email where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, usr, conn);

        sql = $@" select  * from {schema}.Usr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OPisUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OPisUsrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Usr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Usr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OPisUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOPisUsrDataAccess
{
    Task<OPisUsrModel?> _01(OPisUsrModel usr, string schema, string conn);
    Task<OPisUsrModel?> _02(int id, string schema, string conn);
    Task<GridResultModel<OPisUsrModel>> _02Grid(GridRequestModel request, string schemapay, string conn);
    Task<OPisUsrModel?> _03(int id, OPisUsrModel usr, string schema, string conn);
    Task<OPisUsrModel?> _04(int id, string schema, string conn);
}
