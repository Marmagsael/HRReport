using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class CoaDataAccess : ICoaDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public CoaDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<CoaModel?> _01(CoaModel coa, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Coa 
                        (AcctNumber,  AcctName,  AcctType,  ShortDesc,  HasRateOverBasic,  RateOverBasic,  SortEarn,  SortDed,  IsLock) values 
                        (@AcctNumber, @AcctName, @AcctType, @ShortDesc, @HasRateOverBasic, @RateOverBasic, @SortEarn, @SortDed, @IsLock) 
                        on duplicate key update AcctName = @AcctName, AcctType = @AcctType, ShortDesc = @ShortDesc; 
                        SELECT * FROM {schema}.Coa WHERE AcctNumber = @AcctNumber; ";
        var res = await _sql.FetchData<CoaModel?, dynamic>(sql, coa, conn);
        return res.FirstOrDefault();
    }

    public async Task<CoaModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  AcctNumber, AcctName, AcctType, ShortDesc, HasRateOverBasic, RateOverBasic, SortEarn, SortDed, IsLock 
                            from {schema}.Coa where Id = @Id";
        var data = await _sql.FetchData<CoaModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
        
    public async Task<List<CoaModel?>?> _02ByType(string acctType, string schema, string conn)
    {
        string  sql     = $@"select * from {schema}.Coa where AcctType = @AcctType order by isLock desc, AcctNumber;";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { AcctType = acctType }, conn);
        return data;
    }
    public async Task<List<CoaModel?>?> _02ByTypeNotLocked(string acctType, string schema, string conn)
    {
        string  sql     = $@"select * from {schema}.Coa 
                             where AcctType = @AcctType and IsLock != 1 
                             order by isLock desc, AcctNumber;";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { AcctType = acctType }, conn);
        return data;
    }
        
    public async Task<List<CoaModel?>?> _02(string schema, string conn)
    {
        string sql = $@"(select * from {schema}.Coa where acctType = 'E' order by isLock desc, SortEarn) 
                        union 
                        (select * from {schema}.Coa where acctType = 'D' order by isLock desc, SortDed); ";
        var data = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<List<CoaModel?>?> _02_Earnings_HasROB(string schema, string conn)
    { 
        string  sql     = $@"select  * from {schema}.Coa 
                             where Upper(left(Acctnumber,1)) = 'E' and HasRateOverBasic = 1 
                             order by AcctName; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<CoaModel?>?> _02_Earnings_HasNoROB(string schema, string conn)
    { 
        string  sql     = $@"select  * from {schema}.Coa 
                             where Upper(left(Acctnumber,1)) = 'E' and HasRateOverBasic != 1 
                             order by AcctName; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    
    
    public async Task<List<CoaModel?>?> _02_Earnings(string schema, string conn)
    { 
        string  sql     = $@"select  * from {schema}.Coa 
                                    where Upper(left(Acctnumber,1)) = 'E' order by AcctNumber; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<CoaModel?>?> _02_Earnings_TaxMap(string schema, string conn)
    { 
        var  sql     = $@"select  c.AcctNumber, c.AcctName, if(ct.Acctnumber is null, 0, 1) IsSelected from {schema}.Coa c
                                 left join {schema}.CoaTax ct on ct.AcctNumber = c.AcctNumber
                             where Upper(left(c.Acctnumber,1)) = 'E' 
                             order by AcctNumber; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<CoaModel?>?> _02_Earnings_SSSMap(string schema, string conn)
    { 
        var  sql     = $@"select  c.AcctNumber, c.AcctName, if(ct.Acctnumber is null, 0, 1) IsSelected from {schema}.Coa c
                                 left join {schema}.CoaSSS ct on ct.AcctNumber = c.AcctNumber
                             where Upper(left(c.Acctnumber,1)) = 'E' 
                             order by AcctNumber; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<CoaModel?>?> _02_Earnings_PHICMap(string schema, string conn)
    { 
        var  sql     = $@"select  c.AcctNumber, c.AcctName, if(ct.Acctnumber is null, 0, 1) IsSelected from {schema}.Coa c
                                 left join {schema}.CoaPHIC ct on ct.AcctNumber = c.AcctNumber
                             where Upper(left(c.Acctnumber,1)) = 'E' 
                             order by AcctNumber; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    
    public async Task  _0104_Earnings_TaxMap(CoaModel coa, string paydb, string conn)
    {
        var sql = @$"Delete from {paydb}.CoaTax where AcctNumber = @AcctNumber";
        if (coa.IsSelectedB) sql = $"Insert into {paydb}.CoaTax (AcctNumber) values (@AcctNumber);";
        await _sql.ExecuteCmd<dynamic>(sql, new { AcctNumber= coa.AcctNumber }, conn);
    }
    public async Task  _0104_Earnings_SSSMap(CoaModel coa, string paydb, string conn)
    {
        var sql = @$"Delete from {paydb}.CoaSSS where AcctNumber = @AcctNumber";
        if (coa.IsSelectedB) sql = $"Insert into {paydb}.CoaSSS (AcctNumber) values (@AcctNumber);";
        await _sql.ExecuteCmd<dynamic>(sql, new { AcctNumber= coa.AcctNumber }, conn);
    }
    public async Task  _0104_Earnings_PHICMap(CoaModel coa, string paydb, string conn)
    {
        var sql = @$"Delete from {paydb}.CoaPHIC where AcctNumber = @AcctNumber";
        if (coa.IsSelectedB) sql = $"Insert into {paydb}.CoaPHIC (AcctNumber) values (@AcctNumber);";
        await _sql.ExecuteCmd<dynamic>(sql, new { AcctNumber= coa.AcctNumber }, conn);
    }
    
    public async Task<List<CoaModel?>?> _02_Deductions(string schema, string conn)
    { 
        string  sql     = $@" select  * from {schema}.Coa 
                              where Upper(left(Acctnumber,1)) = 'D' order by AcctNumber; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    
    public async Task<List<CoaModel?>?> _02_CheckInTbltran(string acctNumber, string schema, string conn)
    { 
        string  sql     = $@"select  acctNumber from {schema}.Tbltran where Acctnumber = @Acctnumber limit 1; ";
        var     data    = await _sql.FetchData<CoaModel?, dynamic>(sql, new {Acctnumber = acctNumber }, conn);
        return data;
    }


    public async Task<CoaModel?> _03(int id, CoaModel coa, string schema, string conn)
    {
        string sql = $@"Update {schema}.Coa set 
                                AcctNumber          = @AcctNumber, 
                                AcctName            = @AcctName,  
                                AcctType            = @AcctType,  
                                ShortDesc           = @ShortDesc,  
                                HasRateOverBasic    = @HasRateOverBasic,  
                                RateOverBasic       = @RateOverBasic,  
                                SortEarn            = @SortEarn,  
                                SortDed             = @SortDed,  
                                IsLock              = @IsLock where Id = @Id;
                        select  * from {schema}.Coa x where x.Id = @Id ;";
        var data = await _sql.FetchData<CoaModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<CoaModel?> _03Basic(CoaModel coa, string schema, string conn)
    {
        string sql = $@"Update {schema}.Coa set 
                            AcctName            = @AcctName,  
                            ShortDesc           = @ShortDesc 
                        where AcctNumber = @AcctNumber;
                        select  * from {schema}.Coa x where  x.AcctNumber = @AcctNumber ;";

        var data = await _sql.FetchData<CoaModel?, dynamic>(sql, coa, conn);
        return data?.FirstOrDefault();

    }

    public async Task<CoaModel?> _04(string acctNumber, string schema, string conn)
    {
        var trans           = await _02_CheckInTbltran(acctNumber, schema, conn);
        List<CoaModel?> data = new();

        string sql = "";
        if (trans == null || trans.Count < 1 ) 
        {   sql  = $@"Delete from {schema}.Coa where acctNumber = @acctNumber;
                      Select  * from {schema}.Coa x where x.AcctNumber = @AcctNumber ;"; } 
        else
        {   sql = $@"Select  * from {schema}.Coa x where x.AcctNumber = @AcctNumber ;"; }

        data = await _sql.FetchData<CoaModel?, dynamic>(sql, new { AcctNumber = acctNumber }, conn);

        return data?.FirstOrDefault();

    }
}


public class RdepartmentDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdepartmentDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdepartmentModel?> _01(RdepartmentModel rdepartment, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdepartment (SName, Name, SupervisorId) values (@SName, @Name, @SupervisorId)";
        await _sql.ExecuteCmd<dynamic>(sql, rdepartment, conn);

        sql = $@"SELECT * FROM {schema}.Rdepartment WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<RdepartmentModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, SName, Name, SupervisorId from {schema}.Rdepartment where Id = @Id";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<RdepartmentModel?> _03(int id, RdepartmentModel rdepartment, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdepartment set SName = @SName, Name = @Name, SupervisorId = @SupervisorId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rdepartment, conn);

        sql = $@" select  * from {schema}.Rdepartment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdepartmentModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdepartment where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdepartment x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepartmentModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}
