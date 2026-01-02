namespace HRApiLibrary.Models._10_Pis;

public class RdepModel
{

    public int          Id                  { get; set; }
    public DateTime     Trndate             { get; set; } = DateTime.Now;
    public DateTime     MovStart            { get; set; } = DateTime.Now;
    public DateTime     MovEnd              { get; set; } = new DateTime(1900, 01, 01);
    public int          Divid               { get; set; } = 0;
    public int          Depid               { get; set; } = 0;
    public int          Secid               { get; set; } = 0;
    public int          Posid               { get; set; } = 0;
    public int          Deploymodeid        { get; set; } = 1;
    public int          Employttypeid       { get; set; } = 1;
    public int          Payrollgrpid        { get; set; } = 0;
    public int          Apprsystemid        { get; set; } = 1;
    public int          Userid              { get; set; } = 0;
    public string?      Rstat               { get; set; } = string.Empty;
    public DateTime     Datecreated         { get; set; } = DateTime.Now;
    public DateTime     Dateapproved        { get; set; } = new DateTime(1900, 01, 01);

    //-----------------------------------------------------------------------------
    
    public string       EmpName             { get; set; } = string.Empty; 
    public string       RStatName           { get; set; } = string.Empty; 
    public string       DivName             { get; set; } = string.Empty; 
    public string       DepName             { get; set; } = string.Empty; 
    public string       SecName             { get; set; } = string.Empty;
    public string       PosName             { get; set; } = string.Empty;
    public string       DeploymodeName      { get; set; } = string.Empty;
    public string       PayrollgrpName      { get; set; } = string.Empty;
    public string       ApprsystemName      { get; set; } = string.Empty;

}