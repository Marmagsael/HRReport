using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications._12EmployeeProfile.Blazor.Vars
{
    public class V12104Model
    {
        public string Action    = string.Empty;

        public List<int>        Years   = new();
        public List<MonthItem>  Months  = new();
        public List<string>     Weeks   = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

        public int PresentCount     = 0;
        public int LateCount        = 0;
        public int UnderTimeCount   = 0;
        public int AbsentCount      = 0;
        public int RestDayCount     = 0;
        public int DaysCount        = 0;

        public int SelectedMonth   = DateTime.Today.Month;
        public int SelectedYear    = DateTime.Today.Year;

        public List<DailyAttendance>    Calendar        = new();
        public DailyAttendance          DayDetail       = new();
        public List<AttdutytypeModel> AttyDutyTypes   = new();

       

    }

    public class MonthItem
    {
        public int Value    { get; set; }
        public string Name  { get; set; } = "";
    }


    public class DailyAttendance
    {
        public DateTime?    Date                { get; set; }

        public TimeSpan    ExpectedTimeIn      { get; set; }
        public TimeSpan    ExpectedTimeOut     { get; set; }

        public DateTime?    TimeIn              { get; set; }
        public DateTime?    TimeOut             { get; set; }

        public string       DutyType            { get; set; } = "";
        public string       DutyTypeName        { get; set; } = "";
        public string       AttendanceType      { get; set; } = "";

        public string       Status              { get; set; } = "";
        public bool         IsToday             { get; set; }

        public string?      LateDuration        { get; set; }
        public string?      UnderTimeDuration   { get; set; }

        public string       TotalHoursWorked    { get; set; }
        public string       ExpectedTotalHours { get; set; }

        public List<Attpunches1Model> Logs { get; set; } = new();
    }


    public class AttTempRes
    {
        public TimeSpan ExpTimeIn           { get; set; }
        public TimeSpan ExpTimeOut          { get; set; }
        public string   DutyType            { get; set; } = "";
        public double   DutyHrs             { get; set; } = 0.0;
        public string   AttendanceType      { get; set; } = "";
    }
}
