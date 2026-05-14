namespace HRApiLibrary.Models._10_Pis;

public class AttreqhistModel
{
	public int                  Id                      { get; set; } 
	public int                  AttReqHdrId             { get; set; } 
	public DateTime             DActionTaken            { get; set; } 
	public string?              SetStatusTo             { get; set; } 
	public string?              Empnumber_Approver      { get; set; } 
	public string?              Remarks                 { get; set; } 

	//--- Added Fields ------------------------------------------------
	public string?  			ApproverName 			{ get; set; }
   
}
