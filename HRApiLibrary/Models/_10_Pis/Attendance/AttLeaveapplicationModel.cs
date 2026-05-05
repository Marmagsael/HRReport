namespace HRApiLibrary.Models._10_Pis;

public class AttLeaveapplicationModel
{


    public DateTime     LvStart         { get; set; }
    public DateTime     LvEnd           { get; set; }
    public string?      Reason          { get; set; }


    public DateTime     Start           { get; set; }
    public DateTime     End             { get; set; }

    public string?      DutyType        { get; set; }
    public string?      LeaveName       { get; set; }
    public string?      LeaveCode       { get; set; }
    public int?          TimeStart       { get; set; }
    public int?          TimeDuration    { get; set; }
    public int?          CreditedHrs     { get; set; }
    
    public TimeSpan?    ExpLeaveStart    { get; set; }
    public TimeSpan?    ExpLeaveEnd      { get; set; }



}