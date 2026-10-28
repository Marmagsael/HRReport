namespace HRApiLibrary.Models._20_PayGeneric;

//--- Payslip (MBPay frmps "Export to PDF" -> payslip2pdf.frx) ------------------------------------------------------------
//--- One row per payslip line: earning on the left (Acctname/Amount), deduction on the right (Acctname2/Amount2).
//--- The employee header (MBPay curPSHeader, curPSRate, curPSTH) is repeated on every line of the payslip.
public class RPayslipModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	Empnumber 		{ get; set; }
	public string?  	Emplastnm 		{ get; set; }
	public string?  	Empfirstnm 		{ get; set; }
	public string?  	Empmidnm 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Taxcode 		{ get; set; }
	public string?  	Tin 			{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	Period 			{ get; set; }
	public double   	Rate 			{ get; set; }
	public double   	Th 				{ get; set; }

	public string?  	Acctnumber 		{ get; set; } = "";
	public string?  	Acctname 		{ get; set; } = "";
	public double   	Amount 			{ get; set; }
	public string?  	DurationText 	{ get; set; } = "";
	public string?  	Acctnumber2 	{ get; set; } = "";
	public string?  	Acctname2 		{ get; set; } = "";
	public double   	Amount2 		{ get; set; }
}

public class RPayslipTranModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public string?  	AcctNumber 		{ get; set; }
	public string?  	AcctName 		{ get; set; }
	public double   	Amount 			{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	EmpMidNm 		{ get; set; }
	public string?  	TaxCode 		{ get; set; }
	public string?  	Tin 			{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	ClName 			{ get; set; }
	public string?  	AttStart 		{ get; set; }
	public string?  	AttEnd 			{ get; set; }
}

public class RPayslipDtlModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public string?  	DtlCd 			{ get; set; }
	public double?  	NVal 			{ get; set; }
}
