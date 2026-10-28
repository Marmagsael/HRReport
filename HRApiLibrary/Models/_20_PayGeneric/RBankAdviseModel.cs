namespace HRApiLibrary.Models._20_PayGeneric;

//--- Bank Advise / Cash Basis / Bank Charge / Bank Text File (MBPay frmbankadvise, frmbankcharge, frmbanktextfile) -------
//--- Net pay (earnings - deductions) of an employee: per TRN, or for all the chosen TRNs together (Trn empty) ----------
public class RBankAdviseModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	Clname 			{ get; set; }
	public string?  	Empnumber 		{ get; set; }
	public string?  	Empname 		{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	Acctcode 		{ get; set; }
	public string?  	Period 			{ get; set; }
	public double   	Amt 			{ get; set; }
	//--- Bank Charge: the TRN of the period with the employee's earliest dTimeStamp (MBPay flags_ = 0) ---
	public bool     	IsFirstTrn 		{ get; set; }
}

public class RBankTranModel
{
	public string?  	Trn 			{ get; set; }
	public string?  	EmpNumber 		{ get; set; }
	public double   	E 				{ get; set; }
	public double   	D 				{ get; set; }
	public string?  	Batch 			{ get; set; }
	public string?  	ClName 			{ get; set; }
	public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	AcctCode 		{ get; set; }
	public string?  	AttStart 		{ get; set; }
	public string?  	AttEnd 			{ get; set; }
	public string?  	FirstStamp 		{ get; set; }
}
