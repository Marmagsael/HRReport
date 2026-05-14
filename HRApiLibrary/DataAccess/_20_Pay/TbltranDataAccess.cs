using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class TbltranDataAccess : ITbltranDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public TbltranDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<TbltranModel?> _01(string tbl, TbltranModel tbltran, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.{tbl} 
                            (TRN, EmpmasId, empNumber, acctNumber, Qty, Rate, RateTypeId, amount, dTimeStamp, postedby) values 
                            (@TRN, @EmpmasId, @empNumber, @acctNumber, @Qty, @Rate, @RateTypeId, @amount, @dTimeStamp, @postedby); 
                        SELECT * FROM {schema}.Tbltran WHERE Trn = @Trn and Empnumber = @Empnumber and EmpmasId = @EmpmasId; ";
        var res = await _sql.FetchData<TbltranModel?, dynamic>(sql, tbltran, conn);

        
        return res.FirstOrDefault();
    }
    
    public async Task<TbltranModel?> _02(string tbl, string trn, string acctNumber, int empmasId, string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.{tbl} WHERE Trn = @Trn and AcctNumber = @AcctNumber and EmpmasId = @EmpmasId";
        var data    = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber, EmpmasId = empmasId }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<TbltranModel?>?> _02ByTrn(string tbl, string trn, string schema, string conn)
    {
        string sql  = $@"select  * from {schema}.{tbl} WHERE Trn = @Trn";
        var data    = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { Trn = trn}, conn);
        return data;
    }

    public async Task<TbltranModel?> _03(string tbl, TbltranModel tbltran, string schema, string conn)
    {
        string sql = $@"Update {schema}.{tbl} set 
                            empNumber   = @empNumber,  
                            Qty         = @Qty,  
                            Rate        = @Rate,  
                            RateTypeId  = @RateTypeId,  
                            amount      = @amount,  
                            dTimeStamp  = @dTimeStamp,  
                            postedby    = @postedby  
                        WHERE Trn = @Trn and AcctNumber = @AcctNumber and EmpmasId = @EmpmasId;
                        SELECT * FROM {schema}.Tbltran WHERE Trn = @Trn and AcctNumber = @AcctNumber and EmpmasId = @EmpmasId; ";
        var data = await _sql.FetchData<TbltranModel?, dynamic>(sql, tbltran, conn);
        return data?.FirstOrDefault();
    }

    public async Task<TbltranModel?> _04(string tbl, string trn, string acctNumber, int empmasId, string schema, string conn)
    {
        string sql  = $@"Delete from {schema}.{tbl} WHERE Trn = @Trn and AcctNumber = @AcctNumber and EmpmasId = @EmpmasId;
                         Select  * from {schema}.Tbltran WHERE Trn = @Trn and AcctNumber = @AcctNumber and EmpmasId = @EmpmasId;";
        var data    = await _sql.FetchData<TbltranModel?, dynamic>(sql, new { Trn = trn, AcctNumber = acctNumber, EmpmasId = empmasId}, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04TmpByTrn(string trn, string schema, string conn)
    {
        string sql  = $@"Delete from {schema}.TmpTbltran WHERE Trn = @Trn ;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Trn = trn}, conn);
    }



}