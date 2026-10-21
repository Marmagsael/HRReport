namespace HRApiLibrary.Models._10_Pis;

// Status : N - New, S - Save, FA - For Approval, A - Approved, C - Cancel, R - Returned
public class ObapplicationModel
{
    public int?         Id                  { get; set; } = 0;
    public int?         Yr                  { get; set; } = DateTime.Now.Year;
    public int?         EmpmasId            { get; set; }
    public DateTime     DateApplied         { get; set; } = DateTime.Now.Date;
    public DateTime     ObStart             { get; set; } = DateTime.Now.Date;
    public DateTime     ObEnd               { get; set; } = DateTime.Now.Date;
    public string?      Destination         { get; set; } = "";
    public string?      Purpose             { get; set; } = "";
    public string?      ContactPerson       { get; set; } = "";
    public string?      TelNo               { get; set; } = "";
    public string?      Remarks             { get; set; } = "";
    public double       DaysCnt             { get; set; } = 0;
    public double       DaysWithPay         { get; set; } = 0;
    public int?         Approver1Id         { get; set; } = 0;
    public int?         Approver2Id         { get; set; } = 0;
    public int?         ApproverLevel       { get; set; } = 1;
    public string?      Status              { get; set; } = "N";
    public string?      ApprRemarks         { get; set; } = "";
    public DateTime?    DateApprove1        { get; set; }
    public DateTime?    DateApprove2        { get; set; }

    //--------------------------------------------------------
    public string?      RequestorName       { get; set; } = string.Empty;
    public string?      Approver1Name       { get; set; } = string.Empty;
    public string?      Approver2Name       { get; set; } = string.Empty;
    public bool         IsSelected          { get; set; } = false;
}
