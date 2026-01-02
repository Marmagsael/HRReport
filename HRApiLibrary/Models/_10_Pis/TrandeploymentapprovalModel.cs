
namespace HRApiLibrary.Models._10_Pis;

public class TrandeploymentapprovalModel
{
    public int          Id                  { get; set; }
    public int          IdEmpmas            { get; set; }
    public string       TranNumber          { get; set; } = string.Empty;
    public DateTime      PrepDate           { get; set; } = DateTime.Now;
    public DateTime?     DepStart            { get; set; }
    public DateTime?     DepEnd              { get; set; }
    public DateTime?     DateApproved        { get; set; }
    public string       Mode                { get; set; } = string.Empty;
    public int          IdEmploymentType    { get; set; }
    public int          IdDivision          { get; set; }
    public int          IdSection           { get; set; }
    public int          IdDepartment        { get; set; }
    public int          IdPosition          { get; set; }
    public int          IdDesignation       { get; set; }
    public int          IdPayrollGrp        { get; set; }
    public int          IdDeployment        { get; set; }
    public int          IdApprover          { get; set; }
    public int          MarkApprove         { get; set; }

    //For Deviation
    public DateTime? ReportDate              { get; set; } 
    public DateTime? LetterSent              { get; set; } 
    public DateTime? LetterReceived          { get; set; } 
    public string   Remarks                 { get; set; } = string.Empty;


}
