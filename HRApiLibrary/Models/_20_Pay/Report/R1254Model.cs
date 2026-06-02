namespace HRApiLibrary.Models._20_Pay.Report;

public class R1254Model
{
    public string?      EmpNumber       { get; set; } = string.Empty; 
    public int?          No              { get; set; }
    public string?      Pin             { get; set; } = string.Empty; 
    public string?      EmpLastNm       { get; set; } = string.Empty; 
    public string?      EmpSuffixNm     { get; set; } = string.Empty; 
    public string?      EmpFirstNm      { get; set; } = string.Empty; 
    public string?      EmpMidNm        { get; set; } = string.Empty; 
    public double?      Salary          { get; set; } = 0; 
    public string?      Status          { get; set; } = string.Empty; 
    public DateTime?    EffectDate      { get; set; } 
    public DateTime?    BithDate        { get; set; } 
    public double?      Ps              { get; set; } = 0; 
    public double?      Es              { get; set; } = 0; 
    public string?      Gender          { get; set; } = string.Empty; 
    
}