namespace HRApiLibrary.Models._10_Pis;

public class AtttemplatereqhistModel
{
	public int?         Id                      { get; set; } 
	public int?			AtttemplateReqHdrId 	{ get; set; } 
	public int?			UserId 					{ get; set; } 
	public int?         AttReqHdrId             { get; set; } 
	public DateTime?    DActionTaken            { get; set; } 
	public string?      SetStatusTo             { get; set; } 
	public string?      Empnumber_Approver      { get; set; } 
	public string?      Remarks                 { get; set; } 
}