namespace HRApiLibrary.Models._20_PayGeneric;

//--- Earnings Summary / Deductions Summary (MBPay frmearnsum -> rptEarnSum, frmdedsum -> rptDedSum) -----------------------
//--- One row per tbltran line of the period, for the chosen accounts ---------------------------------------------------
public class REarnDedSummaryModel
{
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Empmidnm 		{ get; set; }
	public string?  	Acctnumber 		{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Trn 			{ get; set; }
}
