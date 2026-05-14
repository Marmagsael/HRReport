namespace HRApiLibrary.Models._10_Pis;

public class OtreqhistModel
{
	public int              Id                  {get; set; } 
	public int?             OtReqHdrId         {get; set; } 
	public DateTime?        DActionTaken        {get; set; } 
	public string?          SetStatusTo         {get; set; } 
	public string?          Empnumber_Approver  {get; set; } 
	public string?          Remarks             {get; set; } 
}