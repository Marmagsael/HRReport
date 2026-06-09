namespace HRApiLibrary.Models._10_Pis;

public class LeaveapplicationModel
{
    public int?         Id                  { get; set; } = 0; 
    public int?         Yr                  { get; set; } = DateTime.Now.Year; 
    public int?         EmpmasId            { get; set; }
    public DateTime     DateApplied         { get; set; } = DateTime.Now.Date;
    public int?         LeaveTypeId         { get; set; } = 3; 
    public double       LvBalance           { get; set; } = 0; 
    public double       DaysCnt             { get; set; } = 0; 
    public string?      LvTime              { get; set; } = "";
    public double       DaysWithPay         { get; set; } = 0; 
    public string?      Urgency             { get; set; } = "";
    public DateTime     LvStart             { get; set; } = DateTime.Now.Date;
    public DateTime     LvEnd               { get; set; } = DateTime.Now.Date;
    public string?      Reason              { get; set; } = "";
    public string?      ApprRemarks         { get; set; } = "";
    public string?      Address             { get; set; } = "";
    public string?      TelNo               { get; set; } = "";
    public int?         Approver1Id         { get; set; } = 0;
    public int?         Approver2Id         { get; set; } = 0;
    public int?         Approver3Id         { get; set; } = 0;
    public string?      Status              { get; set; } = "N"; 
    public DateTime?    DateApprove1        { get; set; }   
    public DateTime?    DateApprove2        { get; set; }
    public DateTime?    DateApprove3        { get; set; }
    public int?         ApproverLevel       { get; set; } = 1; 

    //--------------------------------------------------------
    public string?      Empmasname           { get; set; } = string.Empty; 
    public string?      Approver1Name        { get; set; } = string.Empty; 
    public string?      Approver2Name        { get; set; } = string.Empty; 
    public string?      Leavetypename        { get; set; } = string.Empty;
    public string?      RequestorName        { get; set; } = string.Empty;
    public bool         IsSelected           { get; set; } = false;

}