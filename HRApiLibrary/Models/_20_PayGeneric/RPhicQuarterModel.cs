namespace HRApiLibrary.Models._20_PayGeneric;

//--- PhilHealth Employer's Quarterly Remittance Report (MBPay frmphicquarterly -> rptphquarter): one row per employee, the PhilHealth
//--- premium (ACCT_PHIC) of the 1st, 2nd and 3rd month of the quarter; employer share = employee share ------------------------------
public class RPhicQuarterModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	Ln 				{ get; set; }
	public string?  	Fn 				{ get; set; }
	public string?  	Mi 				{ get; set; }
	public double   	P1ee 			{ get; set; }
	public double   	P1er 			{ get => P1ee; }
	public double   	P2ee 			{ get; set; }
	public double   	P2er 			{ get => P2ee; }
	public double   	P3ee 			{ get; set; }
	public double   	P3er 			{ get => P3ee; }
	public string?  	Trn 			{ get; set; }
	public double   	Amount 			{ get; set; }
}
