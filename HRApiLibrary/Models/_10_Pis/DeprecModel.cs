namespace HRApiLibrary.Models._10_Pis;

public class DeprecModel
{
    public int          Empmasid                { get; set; } = 0;
    public string       TranNumber               { get; set; } = string.Empty;
    public int          Divid                   { get; set; } = 0;
    public int          Depid                   { get; set; } = 0;
    public int          Secid                   { get; set; } = 0;
    public int          Leavegrpid              { get; set; } = 0;
    public int          Payrollgrpid            { get; set; } = 0;
    public int          Positionid              { get; set; } = 0;
    public int          IdDeployment            { get; set; } = 0;
    public int          Employmenttypeid        { get; set; } = 0;
    public int          Empstatusid             { get; set; } = 0;
    public DateTime     DepDate                 { get; set; }
    public DateTime     Dhired                  { get; set; }
    public DateTime     Dregularization         { get; set; }
    public DateTime     Dtraineestart           { get; set; }
    public DateTime     Dtraineeend             { get; set; }
    public DateTime     Dcontractualstart       { get; set; }
    public DateTime     Dcontractualend         { get; set; }
    public DateTime     Dprobationarystart      { get; set; }
    public DateTime     Dprobationaryend        { get; set; }
    public DateTime     Dregularizationstart    { get; set; }
    public DateTime     Dregularizationend      { get; set; }
    public DateTime     Dpermanentstart         { get; set; }
    public DateTime     Dresigned               { get; set; }
    public DateTime     Dterminated             { get; set; }
    public DateTime     Dseparated              { get; set; }
    public string?      Remarks                 { get; set; } = string.Empty;

    //==================================================
    public string?      Empname                 { get; set; } = string.Empty;
    public string?      Divname                 { get; set; } = string.Empty;
    public string?      Depname                 { get; set; } = string.Empty;
    public string?      Secname                 { get; set; } = string.Empty;
    public string?      Leavegrpname            { get; set; } = string.Empty;
    public string?      Payrollgrpname          { get; set; } = string.Empty;
    public string?      Positionname            { get; set; } = string.Empty;
    public string?      Employmenttypename      { get; set; } = string.Empty;
    public string?      Empstatusname           { get; set; } = string.Empty;
    public string?      Deploymentname          { get; set; } = string.Empty;


    //=====================================================
    public int IsOnDeviation        { get; set; } = 0;
    public int IdDeviation          { get; set; } = 0;
    public int IsOnDiciplinary      { get; set; } = 0;
    public int IsOnInvestigation    { get; set; } = 0;
    public int IdInvestigate        { get; set; } = 0;
    public string? Empnumber         { get; set; } 




}
