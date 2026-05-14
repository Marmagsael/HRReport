using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OChartofacctDataAccess : IOChartofacctDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OChartofacctDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01(GChartofacctModel chartofacct, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Chartofacct 
							(AcctNumber, AcctName, AcctType, isTaxable, isYTDAcct, isTaxExcl, islock, ischargeable, 
							 hasRateOverBasic, isOthers, isFixed, timedMode, shortDesc, show01, sort, special_, show02, 
							 DedSort, isTH, Deferd, isOT, isMealAcct, formula, OTrate, withSSS, withPHIC, withPagibig, 
							 isGovAcct, isLegalHoliday, isExtLoan, ExtLoanPercentage, status_, customRate, Taxable_Type, 
							 MWE_Type, TaxExptAmt, Annualize) values 
							(@AcctNumber, @AcctName, @AcctType, @isTaxable, @isYTDAcct, @isTaxExcl, @islock, @ischargeable, 
							 @hasRateOverBasic, @isOthers, @isFixed, @timedMode, @shortDesc, @show01, @sort, @special_, @show02, 
							 @DedSort, @isTH, @Deferd, @isOT, @isMealAcct, @formula, @OTrate, @withSSS, @withPHIC, @withPagibig, 
							 @isGovAcct, @isLegalHoliday, @isExtLoan, @ExtLoanPercentage, @status_, @customRate, @Taxable_Type, 
							 @MWE_Type, @TaxExptAmt, @Annualize)";
        await _sql.ExecuteCmd<dynamic>(sql, chartofacct, conn);
    }


    public async Task<List<GChartofacctModel?>?> _02s(string acctNumber, string schema, string conn)
    {
        string sql = $@"select  
		 from {schema}.Chartofacct where AcctNumber = @AcctNumber";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, new { AcctNumber = acctNumber }, conn);
        return data;
    }


    public async Task<GChartofacctModel?> _03(GChartofacctModel chartofacct, string schema, string conn)
    {
        string sql = $@"Update {schema}.Chartofacct set 
							AcctName = @AcctName, 
							AcctType = @AcctType, 
							isTaxable = @isTaxable, 
							isYTDAcct = @isYTDAcct, 
							isTaxExcl = @isTaxExcl, 
							islock = @islock, 
							ischargeable = @ischargeable, 
							hasRateOverBasic = @hasRateOverBasic, 
							isOthers = @isOthers, 
							isFixed = @isFixed, 
							timedMode = @timedMode, 
							shortDesc = @shortDesc, 
							show01 = @show01, 
							sort = @sort, 
							special_ = @special_, 
							show02 = @show02, 
							DedSort = @DedSort, 
							isTH = @isTH, 
							Deferd = @Deferd, 
							isOT = @isOT, 
							isMealAcct = @isMealAcct, 
							formula = @formula, 
							OTrate = @OTrate, 
							withSSS = @withSSS, 
							withPHIC = @withPHIC, 
							withPagibig = @withPagibig, 
							isGovAcct = @isGovAcct, 
							isLegalHoliday = @isLegalHoliday, 
							isExtLoan = @isExtLoan, 
							ExtLoanPercentage = @ExtLoanPercentage, 
							status_ = @status_, 
							customRate = @customRate, 
							Taxable_Type = @Taxable_Type, 
							MWE_Type = @MWE_Type, 
							TaxExptAmt = @TaxExptAmt, 
							Annualize = @Annualize 
						where AcctNumber = @AcctNumber;
					    select  * from {schema}.Chartofacct  where AcctNumber = @AcctNumber ;";
        var data = await _sql.FetchData<GChartofacctModel?, dynamic>(sql, chartofacct, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(string acctnumber, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Chartofacct where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { AcctNumber = acctnumber }, conn);

    }
}

public interface IOChartofacctDataAccess
{
    Task _01(GChartofacctModel chartofacct, string schema, string conn);
    Task<List<GChartofacctModel?>?> _02s(string acctNumber, string schema, string conn);
    Task<GChartofacctModel?> _03(GChartofacctModel chartofacct, string schema, string conn);
    Task _04(string acctnumber, string schema, string conn);
}
