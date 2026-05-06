using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    [Route("13")]
    public class PayrollReportController : Controller
    {
        private static readonly Dictionary<string, string> ReportViews = new()
        {
            // Earnings
            ["1102"] = "_1102_EarningsSummary",
            ["1103"] = "_1103_EarningsHistory",
            ["1104"] = "_1104_MonthlyEarningsReport",

            // Deductions
            ["1152"] = "_1152_DeductionSummary",
            ["1153"] = "_1153_ConsolidatedDeductionSummary",
            ["1154"] = "_1154_DeductionHistory",
            ["1155"] = "_1155_MonthlyDeductionSummary",

            // Standard
            ["1202"] = "_1202_Payslip",
            ["1203"] = "_1203_PayrollRegister",
            ["1204"] = "_1204_BankCharge",
            ["1205"] = "_1205_BankAdvise",
            ["1206"] = "_1206_PayrollSummaryPerGroup",
            ["1207"] = "_1207_13thMonthSummary",
            ["1208"] = "_1208_13thMonthBankAdvise",
            ["1209"] = "_1209_13thMonthDetailed",
            ["1212"] = "_1212_13thMonthRegister",
            ["1213"] = "_1213_13thMonthPayslip",

            // Compliance
            ["1252"] = "_1252_SSSContribution",
            ["1253"] = "_1253_SSSLoanRemittance",
            ["1254"] = "_1254_PagibigContribution",
            ["1255"] = "_1255_PagibigLoanRemittance",
            ["1256"] = "_1256_CalamityLoanRemittance",
            ["1257"] = "_1257_PHICContribution",

            // Others
            ["1302"] = "_1302_TaxAnnualization",
            ["1303"] = "_1303_DutyRendered",
            ["1304"] = "_1304_QuitClaims",
            ["1305"] = "_1305_Compliance",
            ["1306"] = "_1306_PISValidation",
            ["1307"] = "_1307_Retirement",
            ["1308"] = "_1308_GroupRetirement",
            ["1309"] = "_1309_AccountTotal",
            ["1312"] = "_1312_GrandTotal",
            ["1313"] = "_1313_AccountTotal"
        };

        [HttpGet("")]
        public IActionResult Index()
        {
            return View("~/Applications/PayrollReport/Views/Pages/Index.cshtml");
        }

        [HttpGet("Coa")]
        public IActionResult Coa()
        {
            return View("~/Applications/PayrollReport/Views/Pages/Coa.cshtml");
        }

        // 🔥 Clean numeric route
        [HttpGet("{reportCode:int}")]
        public IActionResult Report(int? reportCode)
        {
            var key = reportCode.ToString();

            if (!ReportViews.TryGetValue(key, out var viewName))
                return NotFound();

            return View($"~/Applications/PayrollReport/Views/Pages/{viewName}.cshtml");
        }
    }
}
