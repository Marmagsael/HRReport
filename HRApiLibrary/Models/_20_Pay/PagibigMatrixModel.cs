namespace HRApiLibrary.Models._20_Pay;

public class PagibigMatrixModel
{
    public int      Id             { get; set; }
    public DateTime DateStart      { get; set; }
    public DateTime DateEnd        { get; set; }
    public double   FStart         { get; set; }
    public double   FEnd           { get; set; }
    public double   Ee             { get; set; }
    public double   Er             { get; set; }
    public double   Percent        { get; set; }
}