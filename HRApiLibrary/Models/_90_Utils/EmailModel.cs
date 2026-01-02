using Org.BouncyCastle.Asn1.X509.SigI;

namespace HRApiLibrary.Models._90_Utils;

public class EmailModel
{
    public string? Type { get; set; }
    public string? Subject { get; set; }
    public string? CompanyName { get; set; }
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; }

    public ModulesAccessModel? Modules { get; set; }  
}

public class ModulesAccessModel
{
    public int Info         { get; set; } = 0;
    public int PersonalData { get; set; } = 0;
    public int Address      { get; set; } = 0;
    public int Education    { get; set; } = 0;
    public int Family       { get; set; } = 0;
    public int References   { get; set; } = 0;
    public int Employment   { get; set; } = 0;
    public int Trainings    { get; set; } = 0;
}
