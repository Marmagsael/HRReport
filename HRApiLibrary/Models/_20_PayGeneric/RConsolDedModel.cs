namespace HRApiLibrary.Models._20_PayGeneric;

//--- Deduction Summary (MBPay rptdedsummary, a screen grid with no report): total of the chosen deduction accounts per
//--- employee of the chosen employee statuses. Code / Name hold the employee status list ---------------------------------
public class RConsolDedModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Code 			{ get; set; }
	public string?  	Name 			{ get; set; }
}
