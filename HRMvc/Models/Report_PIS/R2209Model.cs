namespace HRMvc.Models.Report_PIS;

public class R2209Model
{
    public string?     EmpNumber    { get; set; }   = string.Empty; 
    public string?     ClNumber     { get; set; }   = string.Empty; 
    public string?     ClName       { get; set; }   = string.Empty; 
    public string?     DepNumber    { get; set; }   = string.Empty; 
    public DateTime?   DepDate      { get; set; }   = null; 
    public string?     RecNumber    { get; set; }   = string.Empty; 
    public DateTime?   RecDate      { get; set; }   = null; 
    public DateTime?   ReportDate   { get; set; }   = null; 
    public string?     Reason       { get; set; }   = string.Empty; 
    
}

