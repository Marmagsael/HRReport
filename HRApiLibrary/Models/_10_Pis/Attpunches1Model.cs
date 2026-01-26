namespace HRApiLibrary.Models._10_Pis;

public class Attpunches1Model
{
	public string? EmpmasId         {get; set; } 
	public string? DayNo            {get; set; } 
	public DateTime PunchInDate      {get; set; } 
	public string? PunchT           {get; set; } 
	public string? SchedDuration    {get; set; } 
	public string? DutyTypeId       {get; set; } 
	public string? TimeZoneIdIn     {get; set; } 
	public string? IpAddressIn      {get; set; } 
	public string? MacAddressIn     {get; set; } 
	public string? UserIdIn         {get; set; } 
	public DateTime PunchOutDate     {get; set; } 
	public string? TimeZoneIdOut    {get; set; } 
	public string? IpAddressOut     {get; set; } 
	public string? MacAddressOut    {get; set; } 
	public string? UserIdOut        {get; set; } 

	//--- Extra Fields -----------------//
	public int 	TotalHrs      		{get; set; } = 0;
}