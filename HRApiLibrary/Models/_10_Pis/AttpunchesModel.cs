namespace HRApiLibrary.Models._10_Pis;

public class AttpunchesModel
{
    public int          Dayno       { get; set; } = 0;
    public int          Empmasid    { get; set; } = 0;
    public DateTime     Punchdate   { get; set; }
    public string?      Action      { get; set; } = string.Empty;    //In or Out 
    public int          Puncht      { get; set; } = 0;
    public int          Dutytypeid  { get; set; } = 0;
    public int          Timezoneid  { get; set; } = 0;
    public string?      Ipaddress   { get; set; } = string.Empty;
    public string?      Macaddress  { get; set; } = string.Empty;
    public string?      Userid      { get; set; } = string.Empty ;


}
