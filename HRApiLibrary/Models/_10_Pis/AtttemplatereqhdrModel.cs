namespace HRApiLibrary.Models._10_Pis;

public class AtttemplatereqhdrModel
{
	public  int?            Id                      { get; set; } = 0;
	public  int?            UserId                  { get; set; } = 0; 
	public  string?         EmpNumber               { get; set; } = string.Empty; 
	public  DateTime?       DateRequested           { get; set; } = DateTime.Now; 
	public  DateTime?       Effectivity             { get; set; } 
	public  string?         Remarks                 { get; set; } = string.Empty; 
	public  string?         Status                  { get; set; } = "N"; 
	public  string?         EmpNumber_Approver      { get; set; } = string.Empty; 

	// Other fields ---------------------------------------------------
	public  string?         ApproverName            { get; set; } = string.Empty;
    public string? 		    RequestorName 		    { get; set; }
	public bool             IsSelected              { get; set; } = false; 
}