namespace HRMvc.Models.Report_Pay;

public class R1104Model
{
    public string?      Trn              { get; set; }   = string.Empty; 
    public string?      EmpFirstNm       { get; set; }   = string.Empty; 
    public string?      EmpLastNm        { get; set; }   = string.Empty; 
    public string?      EmpMidNm         { get; set; }   = string.Empty; 
    public double?      PrdAmount1       { get; set; }   = 0.00; 
    public double?      PrdAmount2       { get; set; }   = 0.00; 
}
