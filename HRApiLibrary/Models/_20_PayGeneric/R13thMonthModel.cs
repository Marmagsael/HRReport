namespace HRApiLibrary.Models._20_PayGeneric;

//--- 13th Month reports (MBPay frm13thmonthdetailed / summary / register / per client / per employee / bank advice) ------
//--- One row per employee: the basic pay (E001) of the six months in slots M1..M12 (period 01 and 02 of each month; MBPay
//--- added periods 03-05 to period 02), Total = their sum, Amount = Total (bank advice), 13th month = Total / 12 ---------
public class R13thMonthModel
{
	public string?  	Clnumber 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Empnumber 		{ get; set; }
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Acctcode 		{ get; set; }
	public double   	M1 				{ get; set; }
	public double   	M2 				{ get; set; }
	public double   	M3 				{ get; set; }
	public double   	M4 				{ get; set; }
	public double   	M5 				{ get; set; }
	public double   	M6 				{ get; set; }
	public double   	M7 				{ get; set; }
	public double   	M8 				{ get; set; }
	public double   	M9 				{ get; set; }
	public double   	M10 			{ get; set; }
	public double   	M11 			{ get; set; }
	public double   	M12 			{ get; set; }
	public double   	Total 			{ get => M1 + M2 + M3 + M4 + M5 + M6 + M7 + M8 + M9 + M10 + M11 + M12; }
	public double   	Amount 			{ get => Total; }
	public string?  	Code 			{ get; set; }
	public string?  	Name 			{ get; set; }
}

//--- A tbltran E001 row with its employee, before it is put in a slot ---
public class R13thTranModel
{
	public string?  	Period 			{ get; set; }
	public string?  	Empnumber 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Empstat 		{ get; set; }
	public string?  	Clnumber 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Acctcode 		{ get; set; }
}
