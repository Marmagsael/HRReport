namespace HRApiLibrary.Models._20_PayGeneric;

//--- Account Total (MBPay frmaccounttotals -> rptAcctTotals (ATM) / rptAcctTotals1 (cash)) -------------------------------
//--- One row per account of a period: amount, employer share and hours; NetPay and TotalEmp are the same on every row --
public class RAcctTotalModel
{
	public bool     	IsAtm 			{ get; set; }
	public string?  	Acctnumber 		{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public string?  	Section 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public double   	Sss_ershare 	{ get; set; }
	public double   	Hrs 			{ get; set; }
	public double   	NetPay 			{ get; set; }
	public int      	TotalEmp 		{ get; set; }
}

public class RAcctTotalTranModel
{
	public string?  	EmpNumber 		{ get; set; }
	public string?  	AcctNumber 		{ get; set; }
	public string?  	AcctName 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public bool     	IsAtm 			{ get; set; }
}
