using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications._12EmployeeProfile.Blazor.Vars
{
    public class V12104Model
    {
        public string Action    = string.Empty;

        public List<int>        Years   = new();
        public List<MonthItem>  Months  = new();
        public List<string>     Weeks   = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

        public int SelectedMonth   = DateTime.Today.Month;
        public int SelectedYear    = DateTime.Today.Year;

        public List<DailyAttendance>    Calendar        = new();
        public List<AttdutytypeModel?>? AttyDutyTypes   = new();

       

    }

    public class DailyAttendance
    {
        public DateTime?    Date        { get; set; }
        public TimeSpan?    ExpTimeIn   { get; set; }
        public TimeSpan?    ExpTimeOut  { get; set; }
        public string?      DutyType    { get; set; } = "";

        public TimeSpan?    TimeIn      { get; set; }
        public TimeSpan?    TimeOut     { get; set; }

        public string       Status      { get; set; } = "";
        public bool         IsToday     { get; set; }

        public string?     Late          { get; set; } = "";
        public string?     UnderTime        { get; set; } = "";
    }

    public class MonthItem
    {
        public int      Value { get; set; }
        public string   Name { get; set; } = "";
    }

    public class AttTempRes
    {
        public TimeSpan ExpTimeIn   { get; set; }
        public TimeSpan ExpTimeOut  { get; set; }
        public string   DutyType    { get; set; } = "";
        public double   DutyHrs     { get; set; } = 0.0;
    }
}
