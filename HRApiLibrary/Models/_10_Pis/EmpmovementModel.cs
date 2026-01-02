namespace HRApiLibrary.Models._10_Pis;

public class EmpmovementModel
{
    public int          Id              { get; set; } = 0;
    public int          EmpmasId       { get; set; } = 0;
    public DateTime     Date            { get; set; }
    public string?      Refno           { get; set; } = string.Empty;
    public string?      Mode            { get; set; } = string.Empty;
    public string?      Dtls            { get; set; } = string.Empty;
    public int          Userid          { get; set; } = 0;
    public DateTime     Created         { get; set; }
}