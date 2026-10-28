namespace HRApiLibrary.Models._20_PayGeneric;

//--- Loan / contribution remittance of a month: one row per employee (MBPay frmhdmfloanremit -> rptHDMFLoanRemit, ...) --
public class RLoanRemitModel
{
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	EmpMidNm 		{ get; set; }
	public string?  	Tin 			{ get; set; }
	public string?  	Sss 			{ get; set; }
	public string?  	PagibigNo 		{ get; set; }
	public string?  	Empbirth 		{ get; set; }
	public string?  	Clnumber 		{ get; set; }
	public string?  	Clname 			{ get; set; }
	public double   	Amount 			{ get; set; }
	//--- Pag-IBIG contribution: employer share (capped by PAGIBIG_ER_MAX) and employee + employer ---
	public double   	Er 				{ get; set; }
	public double   	Total 			{ get => Amount + Er; }
}
