namespace HRApiLibrary.Models._10_Pis;

public class LeaveapplicationdtlModel
{
	public int?				Id 				        { get; set; } = 0;
    public int?             LeaveApplicationId      { get; set; } = 0;
    public int?				EmpmasId 		        { get; set; } = 0;
    public string?			EmpNumber 		        { get; set; } = string.Empty;  
	public DateTime?		Date 			        { get; set; } = DateTime.Now.Date; 
	public string? 			DutyType 		        { get; set; } = "R"; 
	public int?				IsPayable 		        { get; set; } = 0; 
	public int?				LeaveDayTypeId 		    { get; set; } = 0; 

	//---------------------------------------------------------------------------------------------------------------
	public bool     		IsPayableB              { get => IsPayable == 1; set => IsPayable = value ? 1 : 0; }
}