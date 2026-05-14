namespace HRApiLibrary.Models._10_Pis;
public class OtreqdtlModel
{
	public int? 			OtReqHdrId		{get; set; } 
	public int? 			EmpmasId 		{get; set; } 
	public DateTime		PunchIn 		{get; set; } 
	public double 		TotHrs 			{get; set; } 
	public int? 			DutyTypeId		{get; set; } 
	public int? 			DayTypeId 		{get; set; } 

	//  Others  ----------------------------------------
	public string? 		DutyTypeName 	{ get; set; }
	public string? 		DayTypeName 	{ get; set; }

}