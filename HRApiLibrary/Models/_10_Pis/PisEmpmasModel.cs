namespace HRApiLibrary.Models._10_Pis; 
                
public class PisEmpmasModel
{
    public int      Id                  { get; set; } = 0;
    public int      SystemId            { get; set; } = 0;
    public string?  EmpNumber           { get; set; } = string.Empty;
    public string?  EmpLastNm           { get; set; } = string.Empty;
    public string?  EmpFirstNm          { get; set; } = string.Empty;
    public string?  EmpMidNm            { get; set; } = string.Empty;
    public string?  Suffix              { get; set; } = string.Empty;
    public string?  EmpAlias            { get; set; } = string.Empty;

    //----------------------------------------------------------------
    public string?  Fullname            { get; set; } = string.Empty;
    public string?  Empmasname          { get; set; } = string.Empty;
    public string?  Email               { get; set; } = string.Empty;
    public int      UsersId             { get; set; } = 0;

    public int      Divid               { get; set; } = 0;
    public int      Depid               { get; set; } = 0;
    public int      Secid               { get; set; } = 0;
    public int      DeploymentId               { get; set; } = 0;
    public int      Leavegrpid          { get; set; } = 0;
    public int      Payrollgrpid        { get; set; } = 0;

    public string?  Dept_               { get; set; } = string.Empty;
    public string?  Division_           { get; set; } = string.Empty;
    public string?  Section_            { get; set; } = string.Empty;
    public string?  Position_           { get; set; } = string.Empty;
    public string?  Deployment_         { get; set; } = string.Empty;
    public string?  PayrollGrp_         { get; set; } = string.Empty;
    public string?  EmpStat_            { get; set; } = string.Empty;
    public string?  SalaryGrade_        { get; set; } = string.Empty;




    //--------------------------------------------------------------------------
    public int          IsOnDeviation               { get; set; } = 0;
    public int          IdDeviation                 { get; set; } = 0;
    public int          IsOnDiciplinary             { get; set; } = 0;
    public int          IsOnInvestigation           { get; set; } = 0;
    public int          EmpStatId                   { get; set; } = 0;

    // -----------------------------------------------------------------------------
    public string       DeploymentName              { get; set; } = string.Empty;
    public DateTime?    DeploymentDate              { get; set; }


}
