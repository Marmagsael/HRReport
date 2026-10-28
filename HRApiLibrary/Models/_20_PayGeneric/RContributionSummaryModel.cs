namespace HRApiLibrary.Models._20_PayGeneric;

//--- Contribution summary / certification of an employee (MBPay frmpagibigcontribution -> rptPagibigContribution,
//--- frmphilhealthcontribution -> rptPhilHealthContribution). Amt2 = Amount (the PhilHealth layout reads amt2) -----------
public class RContributionSummaryModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	Trn 			{ get; set; }
	public int      	Year 			{ get; set; }
	public string?  	MonthName 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public double   	Amt2 			{ get => Amount; }
}
