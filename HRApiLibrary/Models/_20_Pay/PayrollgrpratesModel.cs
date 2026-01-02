namespace HRApiLibrary.Models._20_Pay;

public class PayrollgrpratesModel
{
    public int      PayrollgrpId    { get; set; } = 0;

    public string?  coaAcctnumber   { get; set; } = string.Empty;
    
    public double   RateHr          { get; set; } = 0;

    public double   RateDay         { get; set; } = 0;

    public double   RateMonth       { get; set; } = 0;

    public double   RateYr          { get; set; } = 0;

    //-----------------------------------------------------
    public bool     Show            { get; set; } = false;
    public string?  CoaName         { get; set; } = string.Empty;


}
