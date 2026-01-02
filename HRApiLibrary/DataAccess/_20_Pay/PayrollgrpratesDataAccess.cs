using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PayrollgrpratesDataAccess : IPayrollgrpratesDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PayrollgrpratesDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PayrollgrpratesModel?> _01(PayrollgrpratesModel payrollgrprates, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Payrollgrprates 
                            (PayrollgrpId,  coaAcctnumber,  RateHr,  RateDay,  RateMonth,  RateYr) values 
                            (@PayrollgrpId, @coaAcctnumber, @RateHr, @RateDay, @RateMonth, @RateYr); 
                        SELECT * FROM {schema}.Payrollgrprates WHERE ID = (SELECT @@IDENTITY);";
        var res = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, payrollgrprates, conn);

        return res.FirstOrDefault();
    }


    public async Task<PayrollgrpratesModel?> _02(int payrollgrpId, string coaAcctnumber, string schema, string conn)
    {
        string sql = $@"select  gr.PayrollgrpId, gr.coaAcctnumber, gr.RateHr, gr.RateDay, gr.RateMonth, gr.RateYr, c.AcctName CoaName
                            from {schema}.Payrollgrprates gr 
                            left join {schema}.coa c on c.acctunmber = gr.acctNumber  
                        where PayrollgrpId=@PayrollgrpId and CoaAcctnumber=@CoaAcctnumber";
        var data = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId, CoaAcctnumber = coaAcctnumber }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PayrollgrpratesModel?>?> _02Earnings(int payrollgrpId, string schema, string conn)
    {
        string sql = $@"select  gr.PayrollgrpId, gr.coaAcctnumber, gr.RateHr, gr.RateDay, gr.RateMonth, gr.RateYr 
                            ,c.AcctName CoaName
                            from {schema}.Payrollgrprates gr 
                        left join {schema}.coa c on c.acctunmber = gr.acctNumber  
                        where left(coaAcctNumber,1) = 'E' 
                        order by gr.coaAcctNumber ";
        var data = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId}, conn);
        return data;
    }


    public async Task<List<PayrollgrpratesModel?>?> _02ByPayrollgrpId(int payrollgrpId, string schema, string conn)
    {
        string sql = $@"select  PayrollgrpId, coaAcctnumber, RateHr, RateDay, RateMonth, RateYr from {schema}.Payrollgrprates where Id = @Id";
        var data = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data;
    }


    public async Task<PayrollgrpratesModel?> _03(PayrollgrpratesModel payrollgrprates, string schema, string conn)
    {
        string sql = $@"Update {schema}.Payrollgrprates set 
                            RateHr      = @RateHr, 
                            RateDay     = @RateDay, 
                            RateMonth   = @RateMonth, 
                            RateYr      = @RateYr 
                        where PayrollgrpId = @PayrollgrpId and coaAcctnumber = @coaAcctnumber;
                        select  * from {schema}.Payrollgrprates where PayrollgrpId = @PayrollgrpId and coaAcctnumber = @coaAcctnumber;";
        var data = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, payrollgrprates, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PayrollgrpratesModel?> _04(int payrollgrpId, string coaAcctnumber, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Payrollgrprates 
                            where PayrollgrpId = @PayrollgrpId and CoaAcctnumber = @coaAcctnumber;
                        select  * from {schema}.Payrollgrprates where PayrollgrpId = @PayrollgrpId and CoaAcctnumber = @coaAcctnumber;";
        var data = await _sql.FetchData<PayrollgrpratesModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId, CoaAcctnumber = coaAcctnumber }, conn);
        return data?.FirstOrDefault();
    }
}