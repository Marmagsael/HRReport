namespace HRApiLibrary.Models._20_Pay.Report;

public class PayslipModel
{
    public string?                     CoCode                 { get; set; } = string.Empty; 
    public string?                     CoName                 { get; set; } = string.Empty;    
    public string?                     CoAddress              { get; set; } = string.Empty;    
    public DateTime?                   AttStart               { get; set; }
    public DateTime?                   AttEnd                 { get; set; }
    public string?                     DateCovered            { get; set; } = string.Empty;
    public string?                     PaymainhdrStatus       { get; set; } = ""; 
    
    public List<PayslipdtlModel?>     PayslipDtls            { get; set; } = [];
    
}