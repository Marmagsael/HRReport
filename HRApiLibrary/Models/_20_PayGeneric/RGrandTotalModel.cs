namespace HRApiLibrary.Models._20_PayGeneric;

//--- Grand Total (MBPay frmtotalded -> rptGTCL (by client) / rptGTTT (grand total)) -------------------------------------
//--- One row per account (per client when grouped by client); deductions are negative ---------------------------------
public class RGrandTotalModel
{
	public string?  	Clnumber 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Acctnumber 		{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public string?  	Section 		{ get; set; }
	public double   	Amount 			{ get; set; }
}
