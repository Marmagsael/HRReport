namespace HRApiLibrary.Models._10_Pis;

public class LeaveapplicationdtlModel
{
	public int?				Id 				        { get; set; } = 0;
    public int?             LeaveApplicationId      { get; set; } = 0;
    public int?				EmpmasId 		        { get; set; } = 0;
    public string?			EmpNumber 		        { get; set; } = string.Empty;  
	public DateTime?		Start 			        { get; set; } = DateTime.Now.Date; 
	public string? 			DutyType 		        { get; set; } = "R"; 
	public int?				TimeStart 		        { get; set; } = 0;
    public int?				TimeDuration 	        { get; set; } = 0;
    public DateTime?		End 			        { get; set; } = DateTime.Now.Date;
    public decimal?			CreditedHrs 	        { get; set; } = 0;
	public int?				IsPayable 		        { get; set; } = 0; 
	public int?				LeavedayTypeId 		    { get; set; } = 1; 
	
	//----------------------------------------------------
	public bool     		IsPayableB              { get => IsPayable == 1; set => IsPayable = value ? 1 : 0; }
}