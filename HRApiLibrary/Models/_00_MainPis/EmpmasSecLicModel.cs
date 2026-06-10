namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasSecLicModel
{
    public int? Id { get; set; }

    public string? SecLicense { get; set; }

    public DateTime LicExpire { get; set; }

    public string? BadgeNo { get; set; }

    public string? SbrNo { get; set; }

    public string? OpNo { get; set; }

    public DateTime Validated { get; set; }

    public string? VFee { get; set; }

    public DateTime Revalidated { get; set; }

    public string? ValStatus { get; set; }


    ///=========================================
    public string? PositionName     { get; set; }
    public string? GuardNoYrs       { get; set; }
    public string? EmpStatus        { get; set; }
    public DateTime? DateHired        { get; set; }
    public DateTime? RegRef           { get; set; }
    public DateTime? Separate         { get; set; }
    public string? MilitaryNoYr     { get; set; }

}
