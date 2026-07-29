using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    [Route("13T")]
    public class PayrollTransactionController : Controller
    {
        private const string ViewPath = "~/Applications/PayrollTransaction/Pages/";

        private static readonly Dictionary<int, string> ReportViews = new()
        {
            // Transaction
            [502] = "_502_PayrollEntry",
            [503] = "_503_13thMonthEntry",

            // Earnings
            [552] = "_552_EmployeeEarnings",
            [553] = "_553_GroupEarnings",
            [554] = "_554_FixedEarnings",
            [555] = "_555_GroupFixedEarnings",

            // Deductions
            [602] = "_602_EmployeeDeductions",
            [603] = "_603_GroupDeductions",
            [604] = "_604_PartialDeductions",
            [605] = "_605_MandatoryDeductions",
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
}