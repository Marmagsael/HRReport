namespace HRApiLibrary.Models._11_AMS;

public class BioManHourHdrModel
{
    public long     Id              { get; set; }
    public int      PayrollGrpId    { get; set; }
    public DateTime CoverageStart   { get; set; } = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime CoverageEnd     { get; set; } = DateTime.Today;
    public string?  Remarks         { get; set; }

    // --- Not in table ---
    public string   PayrollGrpName  { get; set; } = string.Empty;
}
