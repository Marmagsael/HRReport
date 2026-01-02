namespace HRApiLibrary.Models._20_Pay;

public class PaytranitemModel
{
    public string?      Trn             { get; set; } = string.Empty;
    public int          EmpmasId        { get; set; } = 0;
    public int          PayrollgrpId    { get; set; } = 0;
    public DateTime     AttStart        { get; set; }
    public DateTime     AttEnd          { get; set; }
    public string?      Acctnumber      { get; set; } = string.Empty;
    public double       U               { get; set; } = 0;
    public double       R               { get; set; } = 0;
    public double       M               { get; set; } = 0;
    public double       A               { get; set; } = 0;
}