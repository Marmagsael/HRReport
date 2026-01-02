using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers
{
    [Route("13")]
    public class PayrollReportController : Controller
    {
        [HttpGet("")] public IActionResult Index()
        { return View("~/Applications/PayrollReport/Views/Pages/Index.cshtml"); }
        
        //--- Earnings Report ----------------------------------------------------
        [HttpGet("Coa")] public IActionResult Coa()
        { return View("~/Applications/PayrollReport/Views/Pages/Coa.cshtml"); }

        
        //--- Earnings Report ----------------------------------------------------
        [HttpGet("PayRep_1102")] public IActionResult PayRep_1102()
        { return View("~/Applications/PayrollReport/Views/Pages/_1102_EarningsSummary.cshtml"); }
        
        [HttpGet("PayRep_1103")] public IActionResult PayRep_1103()
        { return View("~/Applications/PayrollReport/Views/Pages/_1103_EarningsHistory.cshtml"); }
        
        [HttpGet("PayRep_1104")] public IActionResult PayRep_1104()
        { return View("~/Applications/PayrollReport/Views/Pages/_1104_MonthlyEarningsReport.cshtml"); }
        
        //--- Deductions Report ----------------------------------------------------
        [HttpGet("PayRep_1152")] public IActionResult PayRep_1152()
        { return View("~/Applications/PayrollReport/Views/Pages/_1152_DeductionSummary.cshtml"); }
        
        [HttpGet("PayRep_1153")] public IActionResult PayRep_1153()
        { return View("~/Applications/PayrollReport/Views/Pages/_1153_ConsolidatedDeductionSummary.cshtml"); }
        
        [HttpGet("PayRep_1154")] public IActionResult PayRep_1154()
        { return View("~/Applications/PayrollReport/Views/Pages/_1154_DeductionHistory.cshtml"); }
        
        [HttpGet("PayRep_1155")] public IActionResult PayRep_1155()
        { return View("~/Applications/PayrollReport/Views/Pages/_1155_MonthlyDeductionSummary.cshtml"); }
        
        
        //--- Standard Report ----------------------------------------------------
        [HttpGet("PayRep_1202")] public IActionResult PayRep_1202()
        { return View("~/Applications/PayrollReport/Views/Pages/_1202_Payslip.cshtml"); }
        
        [HttpGet("PayRep_1203")] public IActionResult PayRep_1203()
        { return View("~/Applications/PayrollReport/Views/Pages/_1203_PayrollRegister.cshtml"); }
        
        [HttpGet("PayRep_1204")] public IActionResult PayRep_1204()
        { return View("~/Applications/PayrollReport/Views/Pages/_1204_BankCharge.cshtml"); }
        
        [HttpGet("PayRep_1205")] public IActionResult PayRep_1205()
        { return View("~/Applications/PayrollReport/Views/Pages/_1205_BankAdvise.cshtml"); }
        
        [HttpGet("PayRep_1206")] public IActionResult PayRep_1206()
        { return View("~/Applications/PayrollReport/Views/Pages/_1206_PayrollSummaryPerGroup.cshtml"); }
        
        [HttpGet("PayRep_1207")] public IActionResult PayRep_1207()
        { return View("~/Applications/PayrollReport/Views/Pages/_1207_13thMonthSummary.cshtml"); }
        
        [HttpGet("PayRep_1208")] public IActionResult PayRep_1208()
        { return View("~/Applications/PayrollReport/Views/Pages/_1208_13thMonthBankAdvise.cshtml"); }
        
        [HttpGet("PayRep_1209")] public IActionResult PayRep_1209()
        { return View("~/Applications/PayrollReport/Views/Pages/_1209_13thMonthDetailed.cshtml"); }
        
        [HttpGet("PayRep_1212")] public IActionResult PayRep_1212()
        { return View("~/Applications/PayrollReport/Views/Pages/_1212_13thMonthRegister.cshtml"); }
        
        [HttpGet("PayRep_1213")] public IActionResult PayRep_1213()
        { return View("~/Applications/PayrollReport/Views/Pages/_1213_13thMonthPayslip.cshtml"); }
        
        
        //--- Compliance Report ----------------------------------------------------
        [HttpGet("PayRep_1252")] public IActionResult PayRep_1252()
        { return View("~/Applications/PayrollReport/Views/Pages/_1252_SSSContribution.cshtml"); }
        
        [HttpGet("PayRep_1253")] public IActionResult PayRep_1253()
        { return View("~/Applications/PayrollReport/Views/Pages/_1253_SSSLoanRemittance.cshtml"); }
        
        [HttpGet("PayRep_1254")] public IActionResult PayRep_1254()
        { return View("~/Applications/PayrollReport/Views/Pages/_1254_PagibigContribution.cshtml"); }
        
        [HttpGet("PayRep_1255")] public IActionResult PayRep_1255()
        { return View("~/Applications/PayrollReport/Views/Pages/_1255_PagibigLoanRemittance.cshtml"); }

        [HttpGet("PayRep_1256")] public IActionResult PayRep_1256()
        { return View("~/Applications/PayrollReport/Views/Pages/_1256_CalamityLoanRemittance.cshtml"); }
        
        [HttpGet("PayRep_1257")] public IActionResult PayRep_1257()
        { return View("~/Applications/PayrollReport/Views/Pages/_1256_CalamityLoanRemittance.cshtml"); }
        
        //--- Others Report ----------------------------------------------------
        [HttpGet("PayRep_1302")] public IActionResult PayRep_1302()
        { return View("~/Applications/PayrollReport/Views/Pages/_1302_TaxAnnualization.cshtml"); }
        
        [HttpGet("PayRep_1303")] public IActionResult PayRep_1303()
        { return View("~/Applications/PayrollReport/Views/Pages/_1303_DutyRendered.cshtml"); }
        
        [HttpGet("PayRep_1304")] public IActionResult PayRep_1304()
        { return View("~/Applications/PayrollReport/Views/Pages/_1304_QuitClaims.cshtml"); }
        
        [HttpGet("PayRep_1305")] public IActionResult PayRep_1305()
        { return View("~/Applications/PayrollReport/Views/Pages/_1305_Compliance.cshtml"); }
        
        [HttpGet("PayRep_1306")] public IActionResult PayRep_1306()
        { return View("~/Applications/PayrollReport/Views/Pages/_1306_PISValidation.cshtml"); }
        
        [HttpGet("PayRep_1307")] public IActionResult PayRep_1307()
        { return View("~/Applications/PayrollReport/Views/Pages/_1307_Retirement.cshtml"); }
        
        [HttpGet("PayRep_1308")] public IActionResult PayRep_1308()
        { return View("~/Applications/PayrollReport/Views/Pages/_1308_GroupRetirement.cshtml"); }
        
        [HttpGet("PayRep_1309")] public IActionResult PayRep_1309()
        { return View("~/Applications/PayrollReport/Views/Pages/_1309_AccountTotal.cshtml"); }
        
        [HttpGet("PayRep_1312")] public IActionResult PayRep_1312()
        { return View("~/Applications/PayrollReport/Views/Pages/_1312_GrandTotal.cshtml"); }
        
        [HttpGet("PayRep_1313")] public IActionResult PayRep_1313()
        { return View("~/Applications/PayrollReport/Views/Pages/_1313_AccountTotal.cshtml"); }


    }
}
