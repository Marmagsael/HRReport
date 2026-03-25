namespace HRApiLibrary.Models._10_Pis;

public class LeaveapplicationdtlModel
{
	public int?				Id 				{ get; set; } 
	public int?				EmpmasId 		{ get; set; } 
	public string?			EmpNumber 		{ get; set; } 
	public DateTime?		Start 			{ get; set; } 
	public string 			DutyType 		{ get; set; } = "R"; 
	public int?				TimeStart 		{ get; set; } 
	public int?				TimeDuration 	{ get; set; } 
	public DateTime?		End 			{ get; set; } 
	public decimal?			CreditedHrs 	{ get; set; } 
	public int?				IsPayable 		{ get; set; } 
	
	//----------------------------------------------------
	public bool     		IsPayableB      { get => IsPayable == 1; set => IsPayable = value ? 1 : 0; }
}