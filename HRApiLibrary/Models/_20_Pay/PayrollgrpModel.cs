namespace HRApiLibrary.Models._20_Pay;

public class PayrollgrpModel
{
    public int?      Id                  { get; set; } = 0;
    public string?   Code                { get; set; }
    public string?   ClNumber            { get; set; }
    public string?   Name                { get; set; }
    public double    RatePerHr           { get; set; }
    public double    RatePerDay          { get; set; }
    public double    RatePerMonth        { get; set; }
    public double    RatePerYr           { get; set; }
    public double    MinDailyRate        { get; set; }
    public string?   Status              { get; set; } = "A";
    public int?      PayRateId           { get; set; }

    //---------------------------------------------------------
    public bool      Show                { get; set; } = false;
    public string?   Deployment          { get; set; }

}