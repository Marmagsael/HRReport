namespace HRApiLibrary.Models._20_PayGeneric;

//--- Payroll Summary / Register Excel sheets (MBPay payroll_summary4, payroll_register4V2, payroll_summary_per_client4,
//--- payroll_summary_perclient_permo4): one row per employee (or payroll group) and account, the amount added up -------------
public class RSheetLineModel
{
	public string?  	Key 			{ get; set; }
	public string?  	Name 			{ get; set; }
	public string?  	Acctnumber 		{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public string?  	Shortdesc 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Clname 			{ get; set; }
}
