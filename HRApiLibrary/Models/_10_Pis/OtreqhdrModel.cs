namespace HRApiLibrary.Models._10_Pis;

public class OtreqhdrModel
{
	public int?         Id                  {get; set; } 
	public int?         UserId              {get; set; } 
	public string?      EmpNumber           {get; set; } 
	public DateTime?    DateRequested       {get; set; } 
	public DateTime?    CovStart            {get; set; } 
	public DateTime?    CovEnd              {get; set; } 
	public int?         AttReqTypeId        {get; set; } 
	public string?      Remarks             {get; set; } 
	public string?      Status              {get; set; } 
	public string?      EmpNumber_Approver  {get; set; } 
	public double?      TotHrs              {get; set; } 
	public int?         PayYear             {get; set; } 
	public string?      PayMo               {get; set; } 
	public string?      PayPP               {get; set; } 
}