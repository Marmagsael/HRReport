namespace HRApiLibrary.Models._20_Pay;

public class EmprateshistModel
{
    public int          Id              { get; set; }

    public int          EmpmasId        { get; set; } = 0;

    public string?      EmpNumber       { get; set; } = string.Empty;

    public int          PayrollgrpId    { get; set; } = 0;

    public bool         UsePaygrpRates  { get; set; } = true;

    public double       RatePerHr       { get; set; } = 0;

    public double       RatePerDay      { get; set; } = 0;

    public double       RatePerMonth    { get; set; } = 0;

    public double       RatePerYr       { get; set; } = 0;

    public DateTime     Created         { get; set; } 

    public int          UserId          { get; set; } = 0;

    public string?      Action          { get; set; } = string.Empty;

    public double       EmpRate         { get; set; } = 0;

    public int          PayRateId       { get; set; } = 0;
}
