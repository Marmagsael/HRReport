namespace HRApiLibrary.Models._10_Pis;

public class MymovementModel
{
    public int          Id              { get; set; } = 0;
    public DateTime     Date            { get; set; }
    public int          Companyid       { get; set; } = 0;
    public string?      Refno           { get; set; } = string.Empty;
    public string?      Mode            { get; set; } = string.Empty;
    public string?      Dtls            { get; set; } = string.Empty;
    public DateTime     Created         { get; set; }
}