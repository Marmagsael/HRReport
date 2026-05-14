namespace HRApiLibrary.Models._00_MainPis; 

public class EmpmasSecLicModel
{
    public int Id { get; set; }

    public string? SecLicense { get; set; }

    public DateTime LicExpire { get; set; }

    public string? BadgeNo { get; set; }

    public string? SbrNo { get; set; }

    public string? OpNo { get; set; }

    public DateTime Validated { get; set; }

    public string? VFee { get; set; }

    public DateTime Revalidated { get; set; }

    public string? ValStatus { get; set; }
}
