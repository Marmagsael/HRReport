namespace HRApiLibrary.Models._10_Pis;

public class EmpmasInternalModel
{
    public int          Id              { get; set; }
    public int          SystemId        { get; set; }   = 0;
    public string?      EmpNumber       { get; set; }   = string.Empty; 
    public string?      EmpLastNm       { get; set; }   = string.Empty;
    public string?      EmpFirstNm      { get; set; }   = string.Empty;
    public string?      EmpMidNm        { get; set; }   = string.Empty;
    public string?      Suffix          { get; set; }   = string.Empty;
    public string?      EmpAlias        { get; set; }   = string.Empty;

    //-----------------------------------------------------------
    public bool         Sel             { get; set; } = false;
    public string?      Fullname        { get; set; }   = string.Empty;

    public string?      Email           { get; set; }   = string.Empty;
    public int          UsersId         { get; set; }   = 0;
    public int          PayrollgrpId    { get; set; }   = 0;
    public string?      PayrollGrp      { get; set; }   = string.Empty;
    public string?      EmpStatus       { get; set; }   = string.Empty;

    

}
