namespace HRApiLibrary.Models._20_PayGeneric;

public class GTbltranModel
{
	public string?  		Trn          	{ get; set; } 
	public string?  		AcctNumber   	{ get; set; } 
	public string?  		EmpNumber    	{ get; set; } 
	public double   		Amount        	{ get; set; } 
	public DateTime?		DTimeStamp   	{ get; set; } 
	public string?  		Source       	{ get; set; } 
	public string?  		PostedBy     	{ get; set; } 

	//--- Additional Fields ----------------------------//
	public string? 			AcctName 		{ get; set; }
	public double? 			DayHrs 			{ get; set; }
	public string? 			Uom 			{ get; set; }
	
}
