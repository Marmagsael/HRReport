namespace HRApiLibrary.Models._20_PayGeneric;

//--- Loan Summary (MBPay loans_management): the latest loan of each employee for one loan account ---------------------
//--- Pymt = payments in loandtl for that loan date; Code / Name hold the loan list ---------------------------------------
public class RLoanSummaryModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public DateTime?	Date 			{ get; set; }
	public double   	Amount 			{ get; set; }
	public double   	Balance 		{ get; set; }
	public double   	Amort 			{ get; set; }
	public string?  	Status 			{ get; set; }
	public double   	Pymt 			{ get; set; }
	public double   	PayrollDeduction { get => Amount - Balance; }
	public double   	Unpaid 			{ get => Amount - Pymt; }
	public string?  	Code 			{ get; set; }
	public string?  	Name 			{ get; set; }
}
