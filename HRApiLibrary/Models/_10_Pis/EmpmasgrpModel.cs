namespace HRApiLibrary.Models._10_Pis;

public class EmpmasgrpModel
{
    public int?          EmpmasId            { get; set; } = 0;
    public int?          SecId               { get; set; } = 0;
    public int?          DepId               { get; set; } = 0;
    public int?          DivId               { get; set; } = 0;
    public int?          LeavegrpId          { get; set; } = 0;
    public int?          PayrollgrpId        { get; set; } = 0;

    //---------------------------------------------------------------------------
    public string?      EmpmasName              { get; set; } = string.Empty;
    public string?      SecName                 { get; set; } = string.Empty;
    public string?      DepName                 { get; set; } = string.Empty;
    public string?      DivName                 { get; set; } = string.Empty;
    public string?      LeavegrpName            { get; set; } = string.Empty;
    public string?      PayrollgrpName          { get; set; } = string.Empty;

    //--- 
    public double       ValLvCredit             { get; set; } = 0; 
    public double       ValAssignedLvCredit     { get; set; } = 0; 
}
