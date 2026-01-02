namespace HRApiLibrary.Models._10_Pis;

public class AttModel
{
    public string?      Punchtype       { get; set; } = "In";  //In or Out 
    public int          Empmasid        { get; set; } = 0;
    public string?      Empnumber       { get; set; } = string.Empty;
    public DateTime     Punchdate       { get; set; }
    public string?      Action          { get; set; } = string.Empty;
    public int          Puncht          { get; set; } = 0;
    public int          Dutytypeid      { get; set; } = 0;
    public int          Timezoneid      { get; set; } = 0;
    public string?      Ipaddress       { get; set; } = string.Empty;
    public string?      Macaddress      { get; set; } = string.Empty;
    public int          Userid          { get; set; } = 0;
    public int          Dayno           { get; set; } = 0;
}
