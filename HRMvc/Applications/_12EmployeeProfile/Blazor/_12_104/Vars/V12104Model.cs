using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._12EmployeeProfile.Blazor._12_104
{
    public class V12104Model
    {
        public string? Action    = string.Empty;
       

        public List<int>        Years   = new();
        public List<MonthItem>  Months  = new();
        public List<string>     Weeks   = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

        public int? SelectedMonth   = DateTime.Today.Month;
        public int? SelectedYear    = DateTime.Today.Year;

        public List<AttDayDetail?>           Calendar        = new();
        public AttDayDetail?                 DayDetail       = new();
        public List<AttdutytypeModel?>       AttyDutyTypes   = new();
        public AttLeave                      AttLeave        = new();
        public SettingsModel                 Settings        = new();


        public decimal?     PresentCount    = 0;
        public int?         LateCount       = 0;
        public int?         UnderTimeCount  = 0;
        public decimal?     AbsentCount     = 0;
        public int?         RestDayCount    = 0;
        public decimal?     DaysCount       = 0;
    }

    public class MonthItem
    {
        public int? Value    { get; set; }
        public string? Name  { get; set; } = "";
    }


    public class AttDayDetail
    {
        public DateTime?        Date                { get; set; }
        public TimeSpan?        ExpTimeIn           { get; set; } = TimeSpan.Zero;
        public TimeSpan?        ExpTimeOut          { get; set; } = TimeSpan.Zero;
        public DateTime?        ActualTimeIn        { get; set; }
        public DateTime?        ActualTimeOut       { get; set; }
        public string?          Status              { get; set; } = "";
        public string?          DutyTypeCode        { get; set; } = string.Empty;
        public string?          DutyTypeName        { get; set; } = string.Empty;
        public string?          AttendanceType      { get; set; } = string.Empty;
        public bool?            IsToday             { get; set; } = false;
        public string?          ExpWorkingHrs       { get; set; } = "";
        public string?          TtlWorkedHrs        { get; set; } = "";
        public string?          LateDuration        { get; set; } = "";
        public string?          UnderTimeDuration   { get; set; } = "";
        public string?          LeaveType           { get; set; } = "";
        public string?          LeaveDayType        { get; set; } = ""; // Whole Day / 1st Half / 2nd Half
        public DateTime?        LeaveStart          { get; set; } 
        public DateTime?        LeaveEnd            { get; set; } 
        public string?          Reason              { get; set; } = "";
        public bool?            IsSelected           { get; set; } = false;
        public List<Attpunches1Model?> Logs         { get; set; } = new();
        public bool IsOtherMonth                    { get; set; } = false;

    }

    public class AttTemplate
    {

        public TimeSpan?    ExpTimeIn           { get; set; } = TimeSpan.Zero;
        public TimeSpan?    ExpTimeOut          { get; set; } = TimeSpan.Zero;
        public string?      DutyTypeCode        { get; set; } = string.Empty;
        public string?      AttendanceType      { get; set; } = string.Empty;
        public int?         AttendanceTypeId    { get; set; } 
        public int?         Duration            { get; set; } = 0;
    }


    public class AttPunches
    {
        public TimeSpan?        ExpTimeIn                { get; set; } 
        public TimeSpan?        ExpTimeOut               { get; set; } 

        public DateTime?        ActualTimeIn             { get; set; } 
        public DateTime?        ActualTimeOut            { get; set; }
        public string?          DutyTypeCode             { get; set; } 
        public int?             Duration                 { get; set; } 
        public TimeSpan?        TotalWorkedHrs           { get; set; }
        public List<Attpunches1Model?>   Logs            { get; set; } = new();
    }


    public class AttLeave
    {
        public string? LeaveType        { get; set; } 
        public string? LeaveDayType     { get; set; } // Whole Day/ 1st Half / 2nd Half
        public DateTime? LeaveStart     { get; set; } 
        public DateTime? LeaveEnd       { get; set; } 
        public string? Reason           { get; set; } 
    }

    




}
