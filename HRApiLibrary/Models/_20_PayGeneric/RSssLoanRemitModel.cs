namespace HRApiLibrary.Models._20_PayGeneric;

//--- SSS Loan Remittance (MBPay frmsssloanremittance -> rptSSSREmit): one row per employee, Amort = the month's loan amortization.
//--- Date / Amount are those of the employee's first loansref row (MBPay grouped by employee only); Clnumber / Clname list the clients ---
public class RSssLoanRemitModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Empmidnm 		{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	Date 			{ get; set; }
	public double   	Amount 			{ get; set; }
	public double   	Amort 			{ get; set; }
	public string?  	Clnumber 		{ get; set; }
	public string?  	Clname 			{ get; set; }
}
