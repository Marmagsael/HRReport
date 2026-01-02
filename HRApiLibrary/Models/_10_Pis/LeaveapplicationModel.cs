namespace HRApiLibrary.Models._10_Pis;

public class LeaveapplicationModel
{
    public int          Id              { get; set; }
    public int          Yr              { get; set; }
    public int          EmpmasId        { get; set; }
    public DateTime     DateApplied     { get; set; }
    public int          LeaveTypeId     { get; set; }
    public double       LvBalance       { get; set; }
    public double       DaysCnt         { get; set; }
    public string?      LvTime          { get; set; }
    public double       DaysWithPay     { get; set; }
    public string?      Urgency         { get; set; }
    public DateTime     LvStart         { get; set; }
    public DateTime     LvEnd           { get; set; }
    public string?      Reason          { get; set; }
    public string?      Address         { get; set; }
    public string?      TelNo           { get; set; }
    public int          Approver1Id     { get; set; }
    public int          Approver2Id     { get; set; }
    public int          Approver3Id     { get; set; }
    public string?      Status          { get; set; }

    //--------------------------------------------------------
    public string? Empmasname           { get; set; } = string.Empty; 
    public string? Leavetypename        { get; set; } = string.Empty; 

}