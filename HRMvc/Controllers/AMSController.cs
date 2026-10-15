using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

[Route("1100")]
public class AMSController : Controller
{
    private const string ViewPath = "~/Applications/AMS/Pages/";

    // GET
    private static readonly Dictionary<int, string> ReportViews = new()
    {
        [11000] = "_11000_AMS_Home_Page",
        // Settings 
        [11002]  = "_11002_StdWorkingHoursSchedule",
        [11003]  = "_11003_EmployeeSchedule",
        [11004]  = "_11004_DailyScheduleTemplate",
        [11005]  = "_11005_WeeklyScheduleTemplate",
        [11006]  = "_11006_OtherScheduleAssignment",
        [11007]  = "_11007_AdvanceScheduleAssignment",
        
        
        [11008]  = "_11008_OvertimeSettings",


        // Transaction
        [12002] = "_12002_AttendanceProcessing",
        [12003] = "_12003_OTEntry",
        [12004] = "_12004_OBDirectAssignment",
        [12005] = "_12005_PunchManagement",
        [12006] = "_12006_Sites",

    };

    [HttpGet("{reportCode:int}")]
    public IActionResult Report(int reportCode)
    {
        if (!ReportViews.TryGetValue(reportCode, out var viewName))
        {
            return NotFound($"Report code '{reportCode}' was not found.");
        }

        return View($"{ViewPath}{viewName}.cshtml");
    }

}