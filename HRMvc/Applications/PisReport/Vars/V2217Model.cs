namespace HRMvc.Applications.PisReport.Vars;


public class V2217Model
{
    public string? Action                { get; set; } = "";
    public bool IsGroupByClient         { get; set; } = false;
    public List<R2217Model> RepDtls     { get; set; } = [];
}



public class R2217Model
{
    public string?      EmpNumber       { get; set; } ="";
    public string?      EmpName         { get; set; } ="";
    public string?      EmpStatus       { get; set; } ="";
    public DateTime?    LicExpire       { get; set; } 
    public string?      SecLicense      { get; set; } ="";
    public string?      ClName          { get; set; } ="";

    //------------------------------------------------------
    public string?      EmpLastNm       { get; set; } = "";
    public string?      EmpFirstNm      { get; set; } = "";
    public string?      Client_         { get; set; } = "";



}