using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class DedmandatoryDataAccess : IDedmandatoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DedmandatoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DedmandatoryModel?> _01(DedmandatoryModel dedmandatory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Dedmandatory 
                            (AcctNumber,  ContAmt,  MaxAmt,  Remarks,  P1,  P2,  P3,  P4,  P5) values 
                            (@AcctNumber, @ContAmt, @MaxAmt, @Remarks, @P1, @P2, @P3, @P4, @P5) 
                        on duplicate key update ContAmt = @ContAmt, 
                                                MaxAmt  = @MaxAmt, 
                                                Remarks = @Remarks,  
                                                Status  = @Status,  
                                                P1      = @P1,  
                                                P2      = @P2,  
                                                P3      = @P3,  
                                                P4      = @P4,  
                                                P5      = @P5 ;
                        SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber 
                        WHERE d.ID = (SELECT @@IDENTITY)";

        if (dedmandatory.Id > 0)
        {
            sql = $@"Insert into {schema}.Dedmandatory 
                            (Id, AcctNumber, ContAmt, MaxAmt, Remarks, Status, P1, P2, P3, P4, P5) values 
                            (@Id, @AcctNumber, @ContAmt, @MaxAmt, @Remarks, @Status, @P1, @P2, @P3, @P4, @P5) 
                        on duplicate key update ContAmt = @ContAmt, 
                                                MaxAmt  = @MaxAmt, 
                                                Remarks = @Remarks,  
                                                Status  = @Status,  
                                                P1      = @P1,  
                                                P2      = @P2,  
                                                P3      = @P3,  
                                                P4      = @P4,  
                                                P5      = @P5 ;
                        SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber 
                        WHERE d.ID = @Id";
        }
            
        var res = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, dedmandatory, conn);

        return res.FirstOrDefault();
    }


    public async Task<DedmandatoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber  
                        where d.Id = @Id";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<DedmandatoryModel?>?> _02(string schema, string conn)
    {
        string sql = $@"SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber ";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    public async Task<List<DedmandatoryModel?>?> _02ByStatus(string status, string schema, string conn)
    {
        string sql = $@"SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber 
                            where Status = @Status ";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new { Status = status }, conn);
        return data;
    }
    
    public async Task<DedmandatoryModel?> _03(int id, DedmandatoryModel dedmandatory, string schema, string conn)
    {
        string sql = $@"Update {schema}.Dedmandatory set 
                            AcctNumber  = @AcctNumber, 
                            ContAmt     = @ContAmt,  
                            MaxAmt      = @MaxAmt,  
                            Remarks     = @Remarks,  
                            Status      = @Status,  
                            P1          = @P1,  
                            P2          = @P2,  
                            P3          = @P3,  
                            P4          = @P4,  
                            P5          = @P5 
                        where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, dedmandatory, conn);

        sql = $@" SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber 
                   where d.Id = @Id ;";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<DedmandatoryModel?> _03Stop(int id, string schema, string conn)
    {
        string sql = $@"Update {schema}.Dedmandatory set Status      = 'S' where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new {  Id = id }, conn);

        sql = $@" SELECT c.AcctName, d.* FROM {schema}.Dedmandatory d 
                            left join {schema}.coa c on c.acctNumber = d.acctNumber 
                   where d.Id = @Id ;";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DedmandatoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Dedmandatory where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Dedmandatory x where x.Id = @Id ;";
        var data = await _sql.FetchData<DedmandatoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}