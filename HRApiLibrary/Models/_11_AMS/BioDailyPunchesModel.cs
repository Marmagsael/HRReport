namespace HRApiLibrary.Models._11_AMS;

public class BioDailyPunchesModel
{
    public string    EmpNumber    { get; set; } = string.Empty;
    public DateOnly  Date         { get; set; }
    public int?      PayrollGrpId { get; set; }
    public string?   DutyType     { get; set; }
    public string?   DayType      { get; set; }
    public int?      InSchedule   { get; set; }
    public int?      OutSchedule  { get; set; }
    public DateTime? PunchIn      { get; set; }
    public DateTime? PunchOut     { get; set; }
    public double?   DayWork      { get; set; }
    public double?   Overtime     { get; set; }
    public string?   DutyStatus   { get; set; }
    public double?   ND           { get; set; }
    public double?   Late         { get; set; }
    public double?   Undertime    { get; set; }
    public double?   Absent       { get; set; }

    // --- Not in table ---
    public string    EmpName      { get; set; } = string.Empty;
    public bool      IsSelected   { get; set; }
}
