namespace HRApiLibrary.Modules._12006O;

public class M12006_Site
{
    public int      Id              { get; set; }
    public string   Code            { get; set; } = string.Empty;
    public string   Name            { get; set; } = string.Empty;
    public string?  Address         { get; set; }
    public decimal  Latitude        { get; set; }
    public decimal  Longitude       { get; set; }
    public int      RadiusMeters    { get; set; } = 100;
    public string   Status          { get; set; } = "A";
}

public class M12006_EmpSite
{
    public int Id        { get; set; }
    public int EmpmasId  { get; set; }
    public int SiteId    { get; set; }

    // For display
    public string? SiteName { get; set; }
    public string? SiteCode { get; set; }
}
