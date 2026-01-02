namespace HRApiLibrary.Models._20_Pay;

public class SalgradeModel
{
	public string? SgCode {get; set; } 

	public string? SGrade {get; set; } 

	public double MonthlyRate {get; set; } 

	public double DailyRate {get; set; } 

	public double HourlyRate {get; set; } 

	public int WTax {get; set; } 

	public int WSss {get; set; } 

	public int WGsis {get; set; } 

	public int WPhic {get; set; } 

	public int WPagibig {get; set; } 

	public int IsLock {get; set; } 
}
