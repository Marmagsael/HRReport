namespace HRApiLibrary.Models._20_PayGeneric;

//--- Monthly Earnings / Monthly Deductions (MBPay frmearnmonthly / frmdedmonthly -> rptDedMonthly) ----------------------
//--- One account, one row per employee: period 01 (1-15) and period 02 (16-end) amounts ------------------------------
public class REarnDedMonthlyModel
{
	public string?  	Ln 				{ get; set; }
	public string?  	Fn 				{ get; set; }
	public string?  	Mi 				{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public double   	Amount01 		{ get; set; }
	public double   	Amount02 		{ get; set; }
}
