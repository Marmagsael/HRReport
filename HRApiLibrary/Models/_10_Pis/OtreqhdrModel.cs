namespace HRApiLibrary.Models._10_Pis;

public class OtreqhdrModel
{
	public int?         Id                  { get; set; } = 0;
	public int?         UserId              { get; set; } = 0; 
	public string?      EmpNumber           { get; set; } = ""; 
	public DateTime?    DateRequested       { get; set; } = DateTime.Now;
	public DateTime?    CovStart            { get; set; } 
	public DateTime?    CovEnd              { get; set; } 
	public int?         AttReqTypeId        { get; set; } 
	public string?      Remarks             { get; set; } 
	public string?      Status              { get; set; } = "N"; 
	public string?      EmpNumber_Approver  { get; set; } 
	public double?      TotHrs              { get; set; } 
	public int?         PayYear             { get; set; } 
	public string?      PayMo               { get; set; } 
	public string?      PayPP               { get; set; } 

	//-------------------------------------------------------------------
	public string? 		ApproverName 		{ get; set; }
}