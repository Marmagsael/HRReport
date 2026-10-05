namespace HRApiLibrary.Models._10_Pis.OPis;

public class OEmpstatModel
{
	public string?  Code            { get; set; } = string.Empty;
    public string?  Name            { get; set; } = string.Empty;
    public string?  IsResigned      { get; set; } = "0";
    public string?  IsOnLeaved      { get; set; } = "0";
    public string?  IsFloating      { get; set; } = "0";
    public string?  IsSuspended     { get; set; } = "0";
	public int?     IsInPayroll     { get; set; } = 0;
	public int?     InLicVer        { get; set; } = 0;
	public int?     InOe            { get; set; } = 0;
	public int?     IsDeviation     { get; set; } = 0;

	// --- Others -------------------------------------
	public int? 		IsSelected 			{ get; set; } = 0; 
	public bool     	IsSelectedB         { get => IsSelected == 1; set => IsSelected = value ? 1 : 0; }

}