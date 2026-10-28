namespace HRApiLibrary.Models._20_PayGeneric;

//--- Earnings History / Deductions History (MBPay frmearnsumemp -> rptEarnHist, frmdedsumemp -> rptDedHist) --------------
//--- One employee; one row per tbltran line. Pdate = 1st of the month for period 01, 16th otherwise (MBPay) -------------
public class REarnDedHistoryModel
{
	public string?  	Period 			{ get; set; }
	public DateTime 	Pdate 			{ get; set; }
	public int      	Year 			{ get; set; }
	public string?  	MonthName 		{ get; set; }
	public string?  	Pp 				{ get; set; }
	public string?  	Acctnumber 		{ get; set; }
	public string?  	Acctname 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Trn 			{ get; set; }
}
