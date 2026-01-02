namespace HRApiLibrary.Models._20_Pay;

public class EmpratesdtlModel
{
    public int      EmpmasId        { get; set; } = 0;

    public int      PayrollGrpId    { get; set; } = 0;

    public string?  AcctNumber      { get; set; } = string.Empty;

    public double   Rate            { get; set; } = 0;

    public int      PayrateId       { get; set; } = 0;

    //--------------------------------------------------------
    public string?  AcctName        { get; set; } = string.Empty;
    public double   RateOverBasic   { get; set; } = 0; 
    public string?  EmpName         { get; set; } = string.Empty;

    public bool     Show            { get; set; } = false;
}