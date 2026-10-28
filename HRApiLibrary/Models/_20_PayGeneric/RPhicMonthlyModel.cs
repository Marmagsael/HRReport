namespace HRApiLibrary.Models._20_PayGeneric;

//--- Monthly PHIC Report per payroll group (MBPay frmphmoperclient -> rptPhMo): one row per employee, Amount = the month's PhilHealth
//--- premium (employee share); the employer share is the same amount, the total twice it -----------------------------------------
public class RPhicMonthlyModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Phic 			{ get; set; }
	public double   	Amount 			{ get; set; }
}

//--- A PhilHealth tbltran line with its employee, before the month is added up ---
public class RPhicLineModel
{
	public string?  	Empnumber 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Clnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Phic 			{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
}
