namespace HRApiLibrary.Models._10_Pis;

public class LvcreditModel
{
    public int?         Year            { get; set; } = DateTime.Now.Year; 
    public int?         EmpmasId        { get; set; }
    public int?         LeaveTypeId     { get; set; }
    public DateTime?    CreditStart     { get; set; } = DateTime.Now; 
    public DateTime?    CreditEnd       { get; set; } = DateTime.Now.AddYears(1); 
    public double?      Credit          { get; set; } = 0; 
}