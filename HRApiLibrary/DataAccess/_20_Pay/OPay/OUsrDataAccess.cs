using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.OPay;
using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OUsrDataAccess : IOUsrDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OUsrDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OUsrModel?> _01(OUsrModel usr, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Usr (UserName, FullName, Pwrd, Stat_, DednAccess, ArchievedAccess, PartDedSetupAccess, DedSumAccess, DedSumEmpAccess, MoDedRepAccess, ConDedSumAccess, ESumAccess, EHisAccess, MoERepAccess, Email) values (@UserName, @FullName, @Pwrd, @Stat_, @DednAccess, @ArchievedAccess, @PartDedSetupAccess, @DedSumAccess, @DedSumEmpAccess, @MoDedRepAccess, @ConDedSumAccess, @ESumAccess, @EHisAccess, @MoERepAccess, @Email)";
        await _sql.ExecuteCmd<dynamic>(sql, usr, conn);

        sql = $@"SELECT * FROM {schema}.Usr WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<OUsrModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<OUsrModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  UserName, FullName, Pwrd, Stat_, DednAccess, ArchievedAccess, PartDedSetupAccess, DedSumAccess, DedSumEmpAccess, MoDedRepAccess, ConDedSumAccess, ESumAccess, EHisAccess, MoERepAccess, Email from {schema}.Usr where Id = @Id";
        var data = await _sql.FetchData<OUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<OUsrModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  UserName, FullName, Pwrd, Stat_, DednAccess, ArchievedAccess, PartDedSetupAccess, DedSumAccess, DedSumEmpAccess, MoDedRepAccess, ConDedSumAccess, ESumAccess, EHisAccess, MoERepAccess, Email from {schema}.Usr ORDER BY FullName";
        var data = await _sql.FetchData<OUsrModel?, dynamic>(sql, new {  }, conn);
        return data;
    }



    public async Task<GridResultModel<OUsrModel>> _02Grid(GridRequestModel request, string schemapay, string conn)
    {
        var columns = new Dictionary<string, string>
        {
            ["UserName"] = "u.UserName",
            ["FullName"] = "u.FullName",
            ["Stat_"] = "u.Stat_"
        };

        // SORTING
        var sortColumn = columns.GetValueOrDefault(
            request.SortField, "u.FullName");

        var sortOrder =
            request.SortDirection == "DESC"  ? "DESC" : "ASC";

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

        var data = await _sql.FetchData<OUsrModel, dynamic>(sql, parameters, conn);

        return new GridResultModel<OUsrModel>
        {
            Data    = data ?? new List<OUsrModel>(),
            Total   = total
        };
    }

    public async Task<OUsrModel?> _03(int id, OUsrModel usr, string schema, string conn)
    {
        string sql = $@"Update {schema}.Usr set UserName = @UserName, FullName = @FullName, Pwrd = @Pwrd, Stat_ = @Stat_, DednAccess = @DednAccess, ArchievedAccess = @ArchievedAccess, PartDedSetupAccess = @PartDedSetupAccess, DedSumAccess = @DedSumAccess, DedSumEmpAccess = @DedSumEmpAccess, MoDedRepAccess = @MoDedRepAccess, ConDedSumAccess = @ConDedSumAccess, ESumAccess = @ESumAccess, EHisAccess = @EHisAccess, MoERepAccess = @MoERepAccess, Email = @Email where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, usr, conn);

        sql = $@" select  * from {schema}.Usr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<OUsrModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Usr where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Usr x where x.Id = @Id ;";
        var data = await _sql.FetchData<OUsrModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


}


public interface IOUsrDataAccess
{
    Task<OUsrModel?> _01(OUsrModel usr, string schema, string conn);
    Task<OUsrModel?> _02(int id, string schema, string conn);
    Task<List<OUsrModel?>?> _02(string schema, string conn);
    Task<GridResultModel<OUsrModel>> _02Grid(GridRequestModel request, string schemapay, string conn);
    Task<OUsrModel?> _03(int id, OUsrModel usr, string schema, string conn);
    Task<OUsrModel?> _04(int id, string schema, string conn);
}