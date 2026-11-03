namespace HRApiLibrary.Models._20_Pay;

public class AcctMapRowModel
{
    public string   Field       { get; set; } = string.Empty;
    public string   Label       { get; set; } = string.Empty;
    public string   Group       { get; set; } = string.Empty;
    public string   AcctCode    { get; set; } = string.Empty;
    public double   OtRate      { get; set; }
    public bool     Changed     { get; set; }
}

public class AcctMapAccountModel
{
    public string?  AcctNumber  { get; set; }
    public string?  AcctName    { get; set; }
    public double   OtRate      { get; set; }
    public string   Display     => string.IsNullOrWhiteSpace(AcctNumber) ? "-- Unassigned --" : $"{AcctNumber} - {AcctName}";
}
