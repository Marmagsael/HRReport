namespace HRApiLibrary.Models._20_PayGeneric;

//--- SSS Contribution Remittance (MBPay frmssscontremit / frmssscontremit2 -> rptSSSContRemit, the SSS R3 form) -----------
//--- One row per employee: Amount = employee share of the month, Fssser / Fecc from ssstbl (employer share, EC) ----------
public class RSssRemitModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Empmidnm 		{ get; set; }
	public string?  	Sss 			{ get; set; }
	public double   	Amount 			{ get; set; }
	public double   	Fssser 			{ get; set; }
	public double   	Fecc 			{ get; set; }
}
