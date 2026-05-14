namespace HRApiLibrary.Models._20_Pay;

public class LoandtlModel
{
	public int Id {get; set; } 

	public string? orno {get; set; } 

	public string? empnumber {get; set; } 

	public string? trn {get; set; } 

	public double amount {get; set; } 

	public string? acctnumber {get; set; } 

	public DateTime loandate {get; set; } 

	public double amort {get; set; } 

	public double balance {get; set; } 
}