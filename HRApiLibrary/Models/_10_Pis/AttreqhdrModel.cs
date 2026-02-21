namespace HRApiLibrary.Models._10_Pis;

public class AttreqhdrModel
{
	public 	int 			Id 					{get; set; } 
	public 	int 			UserId 				{get; set; } 
	public 	string?			EmpNumber 			{get; set; } 
	public 	DateTime		DateRequested 		{get; set; } 
	public 	DateTime		CovStart 			{get; set; } 
	public 	DateTime		CovEnd 				{get; set; } 
	public 	int 			AttReqTypeId 		{get; set; } 
	public 	string?			Remarks 			{get; set; } 
	public 	string?			Status 				{get; set; } = "N"; 
	public 	double			TotHrs 				{get; set; } = 0; 
	public 	string? 		EmpNumber_Approver 	{get; set; } = "";

	// --- Others -----------------------------------------------------
	public string? 			ApproverName 		{ get; set; } = string.Empty; 
}