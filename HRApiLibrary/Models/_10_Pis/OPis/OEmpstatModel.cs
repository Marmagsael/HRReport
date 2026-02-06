namespace HRApiLibrary.Models._10_Pis.OPis;

public class OEmpstatModel
{
	public string?  Code            { get; set; } 
	public string?  Name            { get; set; } 
	public string?  IsResigned      { get; set; } 
	public string?  IsOnLeaved      { get; set; } 
	public string?  IsFloating      { get; set; } 
	public string?  IsSuspended     { get; set; } 
	public int?     IsInPayroll     { get; set; } 
	public int?     InLicVer        { get; set; } 
	public int?     InOe            { get; set; } 
	public int?     IsDeviation     { get; set; } 
}