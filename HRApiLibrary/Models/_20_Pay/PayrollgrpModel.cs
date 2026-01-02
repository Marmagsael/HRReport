namespace HRApiLibrary.Models._20_Pay;

public class PayrollgrpModel
{
    public int      Id              { get; set; }
    public string?  ClNumber        { get; set; }
    public string?  Name            { get; set; }
    public double   RatePerHr       { get; set; }
    public double   RatePerDay      { get; set; }
    public double   RatePerMonth    { get; set; }
    public double   RatePerYr       { get; set; }
    public double   MinMoRate       { get; set; }
    public string?  Status          { get; set; }
    public int      PayRateId       { get; set; }

    //---------------------------------------------------------
    public  bool     Show            { get; set;} = false;
    
    // --- Payroll Details -----------------------------------
    public  int     EmpCount        { get; set; } = 0;
    public string?  PayStatus       { get; set; } = string.Empty;
    public string?  S1              { get; set; } = string.Empty;
}
