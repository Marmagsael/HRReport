namespace HRApiLibrary.Models._20_Pay; 

public class SssMatrixModel
{
    public int          Id                 { get; set; }
    public DateTime     DateStart          { get; set; }
    public DateTime     DateEnd            { get; set; }
    public string?      FStart             { get; set; }
    public string?      FEnd               { get; set; }
    public string?      Ee                 { get; set; }
    public string?      Er                 { get; set; }
    public string?      Ecc                { get; set; }
    public string?      Compensation       { get; set; }
}
