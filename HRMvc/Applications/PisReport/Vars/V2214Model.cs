namespace HRMvc.Applications.PisReport.Vars;

public class V2214Model
{
    public string?              Msg       { get; set; } = "";
    public bool                 IsLoading { get; set; } = true;
    public List<R2214Model>     RepDtls   { get; set; } = []; 
}


public class R2214Model
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