namespace HRMvc.Applications.PisReport.Vars;

public class V2212Model
{
    public string Action { get; set; } = "";
    public List<R2102Model> RepDtls { get; set; } = [];
}


public class R2102Model
{
    public string? EmpNumber    { get; set; }="";
    public string? EmpName      { get; set; }="";
    public DateTime? MovDate    { get; set; } 
    public string? ClName       { get; set; }="";
    public string? Remarks      { get; set; }="";

    // Added -----------------------------------
    public string? EmpLastNm { get; set; } = "";
    public string? EmpFirstNm { get; set; } = "";

}