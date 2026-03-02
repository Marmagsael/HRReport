namespace HRApiLibrary.Models._10_Pis;

public class AtttemplatereqhdrModel
{
	public  int?            Id                      {get; set; } 
	public  int?            UserId                  {get; set; } 
	public  string?         EmpNumber               {get; set; } 
	public  DateTime?       DateRequested           {get; set; } 
	public  DateTime?       Effectivity             {get; set; } 
	public  string?         Remarks                 {get; set; } 
	public  string?         Status                  {get; set; } 
	public  string?         EmpNumber_Approver      {get; set; } 
}