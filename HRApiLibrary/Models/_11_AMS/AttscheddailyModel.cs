namespace HRApiLibrary.Models._11_AMS;

public class AttscheddailyModel
{
	public int      Id          { get; set; } = 0 ;
	public string?  Name        { get; set; } = string.Empty;
	public string?  Dutytype    { get; set; } = "R";
    public int      Pin         { get; set; } = 800;
	public int      Duration    { get; set; } = 900;
	public int      Pout        { get; set; } = 0;
    public double   Nd          { get; set; } = 0;

}