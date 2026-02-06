namespace HRMvc.Models.Report_PIS;

public class R2110Model
{
    public string?     EmpNumber            { get; set; }   = string.Empty; 
    public string?     EmpLastNm            { get; set; }   = string.Empty; 
    public string?     EmpFirstNm           { get; set; }   = string.Empty; 
    public string?     EmpMidNm             { get; set; }   = string.Empty; 
    public string?     InsName              { get; set; }   = string.Empty; 
    public string?     PolicyNo             { get; set; }   = string.Empty; 
    public double?     FaceValue            { get; set; }   = 0; 
    public double?     Premium              { get; set; }   = 0; 
    public DateTime?   ExpirationDate       { get; set; }   = null; 
    public DateTime?   DateOfBirth          { get; set; }   = null; 
    public DateTime?   DateHired            { get; set; }   = null; 
    public string?     EmpStatus            { get; set; }   = string.Empty; 
    
}


