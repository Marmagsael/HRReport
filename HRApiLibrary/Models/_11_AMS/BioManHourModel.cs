namespace HRApiLibrary.Models._11_AMS;

public class BioManHourModel
{
    public long     Id              { get; set; }
    public long     BioManHourHdrId { get; set; }
    public string   EmpNumber       { get; set; } = string.Empty;
    public double?  RdDays          { get; set; }
    public double?  RdRd            { get; set; }
    public double?  RdTardiness     { get; set; }
    public double?  RdOt            { get; set; }
    public double?  LhDays          { get; set; }
    public double?  LhRd            { get; set; }
    public double?  LhTardiness     { get; set; }
    public double?  LhOt            { get; set; }
    public double?  ShDays          { get; set; }
    public double?  ShRd            { get; set; }
    public double?  ShTardiness     { get; set; }
    public double?  ShOt            { get; set; }
    public double?  DhDays          { get; set; }
    public double?  DhRd            { get; set; }
    public double?  DhTardiness     { get; set; }
    public double?  DhOt            { get; set; }

    // --- Not in table ---
    public string   EmpName         { get; set; } = string.Empty;
    public bool     IsSelected      { get; set; }
}
