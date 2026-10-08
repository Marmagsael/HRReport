using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._90_Utils;
using MySqlX.XDevAPI;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OClientDataAccess : IOClientDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OClientDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<OClientModel?> _01(OClientModel client, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Client 
							(CLNUMBER, CLNAME, ADDR1, ADDR2, AREACODE, TEL1, FAXNO, PARENT, RATE, 
							 BILLRATE, ASSIST, STATUS, COLARATE, ND_RATE, RETIRATE, UNIFRATE, FDIRATE, 
							 OTRATE, TIN, CONT, USED, CONTACT, POSTPERIOD, BATCHX, FSSSEE, FSSSER, FECC, FMEDEE, 
							 FMEDER, Remarks, contStart, contEnd, parentcd, maxsss, maxphic, ContExp, HavTax, MinRate, 
							 MealAllow, withUniform, withRetirement, region, ecolaRevised, ctpaRate, withCTPA, seaRate, 
							 withSEA, payprd, sgcode, isTrucking, isLumpsum) values 
							(@CLNUMBER, @CLNAME, @ADDR1, @ADDR2, @AREACODE, @TEL1, @FAXNO, @PARENT, @RATE, 
							 @BILLRATE, @ASSIST, @STATUS, @COLARATE, @ND_RATE, @RETIRATE, @UNIFRATE, @FDIRATE, 
							 @OTRATE, @TIN, @CONT, @USED, @CONTACT, @POSTPERIOD, @BATCHX, @FSSSEE, @FSSSER, @FECC, @FMEDEE, 
							 @FMEDER, @Remarks, @contStart, @contEnd, @parentcd, @maxsss, @maxphic, @ContExp, @HavTax, @MinRate, 
							 @MealAllow, @withUniform, @withRetirement, @region, @ecolaRevised, @ctpaRate, @withCTPA, @seaRate, 
							 @withSEA, @payprd, @sgcode, @isTrucking, @isLumpsum)";


        await _sql.ExecuteCmd<dynamic>(sql, client, conn);
        sql = $@"Select * FROM {schema}.client WHERE TRIM(UPPER(CLNUMBER)) = TRIM(UPPER(@ClNumber)) ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { client.ClNumber }, conn);
        return data.FirstOrDefault();


    }


    public async Task<int?> _02ByMaxClNumber( string? schema, string? conn)
    {

        var sql = $@"select  LPAD(MAX(CAST(clnumber AS UNSIGNED)) + 1, 5, 0) from {schema}.Client ";
        var clnumber = await _sql.FetchData<int?, dynamic>(sql, new {  }, conn);
        return clnumber.FirstOrDefault();
    }

    public async Task<List<OClientModel?>?> _02ByClNumbersCheckIfRecordExistOnTransactionTbl(string? clnumber, string? schema, string? conn)
    {
        string sql = $@"select * from {schema}.empmas where TRIM(UPPER(client_)) = TRIM(UPPER(@ClNumber)) ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { clnumber }, conn);
        return data;
    }


    public async Task<List<OClientModel?>?> _02ByClNumbers(string? clnumber, string? schema, string? conn)
    {
        string? sql  = $@"update {schema}.Client set 
                            ContStart   = if(ContStart  < '1800-01-01', '1900-01-01', ContStart), 
                            ContEnd     = if(ContEnd    < '1800-01-01', '1900-01-01', ContEnd), 
                            ContExp     = if(ContExp    < '1800-01-01', '1900-01-01', ContExp) 
                        where  ClNumber = @Clnumber ";
        _sql.ExecuteCmd<dynamic>(sql, new { ClNumber = clnumber }, conn);
        sql = $@"select  * from {schema}.Client where ClNumber = @ClNumber order by ClName ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { ClNumber = clnumber }, conn);
        return data;
    }

    public async Task<List<OClientModel?>?> _02ByStatuss(string? status, string? schema, string? conn)
    {
        string? sql  = $@"update {schema}.Client set 
                            ContStart   = if(ContStart  < '1800-01-01', '1900-01-01', ContStart), 
                            ContEnd     = if(ContEnd    < '1800-01-01', '1900-01-01', ContEnd), 
                            ContExp     = if(ContExp    < '1800-01-01', '1900-01-01', ContExp) 
                        where  Status = @Status ";
        _sql.ExecuteCmd<dynamic>(sql, new { Status = status }, conn);

        sql         = $@"select  * from {schema}.Client where Status = @Status order by ClName ";
        var data    = await _sql.FetchData<OClientModel?, dynamic>(sql, new { Status = status }, conn);
        return data;
    }


    public async Task<List<OClientModel?>?> _02( string? schema, string? conn)
    {
        string? sql = $@"update {schema}.Client set 
                            ContStart   = if(ContStart  < '1800-01-01', '1900-01-01', ContStart), 
                            ContEnd     = if(ContEnd    < '1800-01-01', '1900-01-01', ContEnd), 
                            ContExp     = if(ContExp    < '1800-01-01', '1900-01-01', ContExp) ";
        _sql.ExecuteCmd<dynamic>(sql, new {  }, conn);

        sql = $@"select  * from {schema}.Client  order by ClName ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new {  }, conn);
        return data;
    }




    public async Task<List<OClientModel?>?> _02ByStatuses(List<string> statuses, string? schema, string? conn)
    {
        string? sql = $@"update {schema}.Client set 
                            ContStart   = if(ContStart  < '1800-01-01', '1900-01-01', ContStart), 
                            ContEnd     = if(ContEnd    < '1800-01-01', '1900-01-01', ContEnd), 
                            ContExp     = if(ContExp    < '1800-01-01', '1900-01-01', ContExp) 
                        where  Status in @Status ";
        _sql.ExecuteCmd<dynamic>(sql, new { Status = statuses }, conn);

        sql = $@"select  * from {schema}.Client where Status in @Status order by ClName ";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { Status = statuses }, conn);
        return data;
    }

    public async Task<List<OClientModel?>?> _02ByClientName(string? clname, string?  schema, string? conn)
    {
        string? sql = @$"select * from {schema}.Client where TRIM(UPPER(clname)) = TRIM(UPPER(@ClName))";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { ClName = clname }, conn);
        return data;
    }

    public async Task<GridResultModel<OClientModel>> _02Grid(  GridRequestModel request, string schema,string conn)
    {

        string? sql = $@"update {schema}.Client set 
                            ContStart   = if(ContStart  < '1800-01-01', '1900-01-01', ContStart), 
                            ContEnd     = if(ContEnd    < '1800-01-01', '1900-01-01', ContEnd), 
                            ContExp     = if(ContExp    < '1800-01-01', '1900-01-01', ContExp) ";
        _sql.ExecuteCmd<dynamic>(sql, new { }, conn);



        var columns = new Dictionary<string, string>
        {
            ["ClNumber"]    = "c.ClNumber",
            ["ClName"]      = "c.ClName",
            ["AreaName"]    = "a.AreaName",
            ["ParentName"]  = "p.ClName",
            ["Status"]      = "c.Status",
            ["Addr1"]       = "c.Addr1",
            ["ContStart"]   = "c.ContStart",
            ["ContEnd"]     = "c.ContEnd",
        };

        // SORTING
        var sortColumn = columns.GetValueOrDefault(
            request.SortField,
            "c.ClName"
        );

        var sortOrder = request.SortDirection == "DESC"
            ? "DESC"
            : "ASC";

        // PARAMETERS
        var parameters = new Dictionary<string, object>
        {
            ["PageSize"] = request.PageSize,
            ["Offset"] = request.Offset
        };

        // FILTERING
        var where = GridHelperDataAccess.BuildWhere(
            request.Filters,
            columns,
            parameters
        );

       

        // COUNT
        string countSql = $@" SELECT COUNT(*) FROM {schema}.Client c
                        LEFT JOIN {schema}.Area a ON a.AreaCode = c.AreaCode
                        LEFT JOIN {schema}.Client p ON p.ClNumber = c.ParentCd
                        {where}";

        var totalResult = await _sql.FetchData<int, dynamic>( countSql, parameters, conn );
        var total = totalResult?.FirstOrDefault() ?? 0;

        // DATA
        string sql1 = $@" SELECT c.*, COALESCE(a.AreaName, '-') AS AreaName,p.ClName AS ParentName
                        FROM {schema}.Client c
                        LEFT JOIN {schema}.Area a ON a.AreaCode = c.AreaCode
                        LEFT JOIN {schema}.Client p ON p.ClNumber = c.ParentCd
                        {where}
                        ORDER BY {sortColumn} {sortOrder}
                        LIMIT @Offset, @PageSize";

        var data = await _sql.FetchData<OClientModel, dynamic>( sql1,parameters,conn);

        return new GridResultModel<OClientModel>
        {
            Data = data ?? new List<OClientModel>(),
            Total = total
        };
    }


    public async Task<GridResultModel<OClientModel>> _02GridWithEmpmas(GridRequestModel request, string schema, string conn)
    {


        var columns = new Dictionary<string, string>
        {
            ["ClNumber"]    = "c.ClNumber",
            ["ClName"]      = "c.ClName",
            ["AreaName"]    = "a.AreaName",
            ["ParentName"]  = "p.ClName",
            ["Status"]      = "c.Status",
            ["Addr1"]       = "c.Addr1",
            ["ContStart"]   = "c.ContStart",
            ["ContEnd"]     = "c.ContEnd",
        };

        // SORTING
        var sortColumn = columns.GetValueOrDefault(
            request.SortField,
            "c.ClName"
        );

        var sortOrder = request.SortDirection == "DESC"
            ? "DESC"
            : "ASC";

        // PARAMETERS
        var parameters = new Dictionary<string, object>
        {
            ["PageSize"] = request.PageSize,
            ["Offset"] = request.Offset
        };

        // FILTERING
        var where = GridHelperDataAccess.BuildWhere(
            request.Filters,
            columns,
            parameters
        );



        // COUNT
        string countSql = $@" SELECT COUNT(DISTINCT c.ClNumber) FROM {schema}.Client c
                                LEFT JOIN {schema}.Empmas e ON e.Client_ = c.ClNumber
                                LEFT JOIN {schema}.Position p ON p.Code = e.Position_
                                {where}";


        var totalResult = await _sql.FetchData<int, dynamic>(countSql, parameters, conn);
        var total = totalResult?.FirstOrDefault() ?? 0;

        // DATA


        string sql1 = $@" SELECT   c.clnumber,
                        c.clname, c.Addr1, c.Status, 'Security Services' Job,
                        GROUP_CONCAT(DISTINCT p.name SEPARATOR ' / ') AS PersonnelPositions,
                        SUM(IF(e.sex_ = 'M', 1, 0)) AS MaleCnt,
                        SUM(IF(e.sex_ = 'F', 1, 0)) AS FemaleCnt,
                        SUM(IF(e.sex_ = '' OR e.sex_ IS NULL, 1, 0)) AS OthersCnt, c.ContStart, c.ContEnd
                        FROM {schema}.Client c
                            LEFT JOIN {schema}.Empmas e  ON e.Client_ = c.ClNumber
                            LEFT JOIN {schema}.Position p  ON p.Code = e.Position_
                            {where}
                            GROUP BY c.ClNumber, c.ClName
                            ORDER BY {sortColumn} {sortOrder}
                            LIMIT @Offset, @PageSize";


        var data = await _sql.FetchData<OClientModel, dynamic>(sql1, parameters, conn);

        return new GridResultModel<OClientModel>
        {
            Data = data ?? new List<OClientModel>(),
            Total = total
        };
    }


    public async Task<OClientModel?> _03(OClientModel client, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Client set CLNUMBER = @CLNUMBER, CLNAME = @CLNAME, ADDR1 = @ADDR1, ADDR2 = @ADDR2, 
							AREACODE = @AREACODE, TEL1 = @TEL1, FAXNO = @FAXNO, PARENT = @PARENT, RATE = @RATE, 
							BILLRATE = @BILLRATE, ASSIST = @ASSIST, STATUS = @STATUS, COLARATE = @COLARATE, 
							ND_RATE = @ND_RATE, RETIRATE = @RETIRATE, UNIFRATE = @UNIFRATE, FDIRATE = @FDIRATE, 
							OTRATE = @OTRATE, TIN = @TIN, CONT = @CONT, USED = @USED, CONTACT = @CONTACT, POSTPERIOD = @POSTPERIOD, 
							BATCHX = @BATCHX, FSSSEE = @FSSSEE, FSSSER = @FSSSER, FECC = @FECC, FMEDEE = @FMEDEE, 
							FMEDER = @FMEDER, Remarks = @Remarks, contStart = @contStart, 
							contEnd = @contEnd, parentcd = @parentcd, maxsss = @maxsss, maxphic = @maxphic, 
							ContExp = @ContExp, HavTax = @HavTax, MinRate = @MinRate, MealAllow = @MealAllow, 
							withUniform = @withUniform, withRetirement = @withRetirement, region = @region, 
							ecolaRevised = @ecolaRevised, ctpaRate = @ctpaRate, withCTPA = @withCTPA, seaRate = @seaRate, 
							withSEA = @withSEA, payprd = @payprd, sgcode = @sgcode, isTrucking = @isTrucking, 
							isLumpsum = @isLumpsum  where TRIM(UPPER(ClNumber)) = TRIM(UPPER(@ClNumber));
						select  * from {schema}.Client  where TRIM(UPPER(ClNumber)) = TRIM(UPPER(@ClNumber)) ;";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, client, conn);
        return data?.FirstOrDefault();
    }


   
    public async Task<OClientModel?> _04(string? clNumber, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Client where TRIM(UPPER(CLNUMBER)) = TRIM(UPPER(@ClNumber)) ;";
        await _sql.ExecuteCmd<dynamic>(sql, new { ClNumber = clNumber }, conn);

        sql = $@" select  * from {schema}.Client x where TRIM(UPPER(CLNUMBER)) = TRIM(UPPER(@ClNumber)) ;";
        var data = await _sql.FetchData<OClientModel?, dynamic>(sql, new { ClNumber = clNumber }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOClientDataAccess
{
    Task<OClientModel?> _01(OClientModel client, string? schema, string? conn);
    Task<List<OClientModel?>?> _02(string? schema, string? conn);
    Task<int?>                  _02ByMaxClNumber(string? schema, string? conn);
    Task<List<OClientModel?>?> _02ByClNumbersCheckIfRecordExistOnTransactionTbl(string? clnumber, string? schema, string? conn);
    Task<List<OClientModel?>?> _02ByClNumbers(string? clnumber, string? schema, string? conn);
    Task<List<OClientModel?>?> _02ByStatuss(string? status, string? schema, string? conn);
    Task<List<OClientModel?>?> _02ByClientName(string? clname, string? schema, string? conn);
    Task<List<OClientModel?>?> _02ByStatuses(List<string> statuses, string? schema, string? conn);
    Task<GridResultModel<OClientModel>> _02Grid(GridRequestModel request, string schema, string conn);
    Task<GridResultModel<OClientModel>> _02GridWithEmpmas(GridRequestModel request, string schema, string conn);
    Task<OClientModel?> _03(OClientModel client, string? schema, string? conn);
    Task<OClientModel?> _04(string? clNumber, string? schema, string? conn);
}
