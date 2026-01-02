namespace HRApiLibrary.Models._20_Pay;

public class LoansModel
{
	public int Id {get; set; } 
	public string?  	EmpNumber 		{get; set; } 
	public DateTime 	Date 			{get; set; } 
	public string?  	DedNCode 		{get; set; } 
	public string?  	DedNDesc 		{get; set; } 
	public double   	Principal 			{get; set; } 
	public double   	Amount 			{get; set; } 
	public double   	Amort 			{get; set; } 
	public double   	Balance 		{get; set; } 
	public string?  	Status 			{get; set; } 
	public string?  	EncodedBy 		{get; set; } 
	public string?  	EncodedT 		{get; set; } 
	public string?  	ChangeBy 		{get; set; } 
	public string?  	ChangedT 		{get; set; } 
	public string?  	Posted 			{get; set; } 
	public string?  	PostFlag 		{get; set; } 
	public string?  	Remarks 		{get; set; } 
	public string?  	PayMode 		{get; set; } 
	public DateTime 	PayStart 		{get; set; } 
	public DateTime 	PayRes 			{get; set; } 
	public string?  	Cvno 			{get; set; } 
	public int 			P1 				{get; set; } 
	public int 			P2 				{get; set; } 
	public int 			P3 				{get; set; } 
	public int 			P4 				{get; set; } 
	public int 			P5 				{get; set; } 
	public string?      TrnLastPosted   {get; set; } 
	
	//-----------------------------------------------------
    public string LoanStatus
	{
		get => Status == "I" ? "Inactive" : "Active";
		set => Status = value?.Equals("Inactive", StringComparison.OrdinalIgnoreCase) == true ? "I" : "A";
	}
	public bool             P1B              { get => P1 == 1; set => P1 = value ? 1 : 0; }
	public bool             P2B              { get => P2 == 1; set => P2 = value ? 1 : 0; }
	public bool             P3B              { get => P3 == 1; set => P3 = value ? 1 : 0; }
	public bool             P4B              { get => P4 == 1; set => P4 = value ? 1 : 0; }
	public bool             P5B              { get => P5 == 1; set => P5 = value ? 1 : 0; }
    
	//-----------------------------------------------------------------
	//public string?          EmpName          { get; set; } = string.Empty;
	//public string?          EmpStatus        { get; set; } = string.Empty;
	//public string?          PayrollgrpName   { get; set; } = string.Empty;

}