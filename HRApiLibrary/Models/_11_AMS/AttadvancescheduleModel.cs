namespace HRApiLibrary.Models._11_AMS;


public class AttadvancescheduleModel
{
    public int          Id                  { get; set; }
    public string?      Empnumber           { get; set; }
    public int          Attscheddailyid     { get; set; }
    public DateTime     Date                { get; set; }
    public string?      Dutytype            { get; set; }
    public int          Pin                 { get; set; }
    public int          Duration            { get; set; }
    public int          Pout                { get; set; }
    public double       Nd                  { get; set; }
}
