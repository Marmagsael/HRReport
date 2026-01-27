namespace HRApiLibrary.Models._10_Pis;

public class Attpunches1Model
{
	public int? 	EmpmasId         {get; set; } 
	public int? 	DayNo            {get; set; } 
	public DateTime PunchInDate      {get; set; } 
	public int 		PunchT           	{get; set; } 
	public int 		SchedDuration    {get; set; } 
	public int 		DutyTypeId       {get; set; } 
	public int  	TimeZoneIdIn     {get; set; } 
	public string? 	IpAddressIn      {get; set; } 
	public string? 	MacAddressIn     {get; set; } 
	public int 		UserIdIn         {get; set; } 
	public DateTime PunchOutDate     {get; set; } 
	public int 		TimeZoneIdOut    {get; set; } 
	public string? 	IpAddressOut     {get; set; } 
	public string? 	MacAddressOut    {get; set; } 
	public int 		UserIdOut        {get; set; } 
	public string? 	Status    		 {get; set; } = "N"; 

	//--- Extra Fields -----------------//
	public int 	TotalHrs      		{get; set; } = 0;
}