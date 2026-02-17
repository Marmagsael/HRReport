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
	public 	string?			Status 				{get; set; } 
	public 	double			TotHrs 				{get; set; } = 0; 
	public 	int 			UserId_FApprover 	{get; set; } 
}