using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

public class _03Controller : Controller
{
    [HttpGet("03")]
    public IActionResult Index()
    {
        return View("~/Applications/_03Payroll/Views/Pages/Index.cshtml");
    }

    [HttpGet("03/0000")]
    public IActionResult _0000_Register()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0000_Register.cshtml");
    }

    [HttpGet("03/0001")]
    public IActionResult _0001_NewCompany()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0001_NewCompany.cshtml");
    }

    [HttpGet("03/0501")]
    public IActionResult _0501_CompanySettings()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0501_CompanySettings.cshtml");
    }

    [HttpGet("03/0502")]
    public IActionResult _0502_ChartofAccounts()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0502_ChartOfAccounts.cshtml");
    }

    [HttpGet("03/0503")]
    public IActionResult _0503_PayrollGroup()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0503_PayrollGroup.cshtml");
    }

    [HttpGet("03/0504")]
    public IActionResult _0504_EmployeeManagement()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0504_EmployeeManagement.cshtml");
    }

    [HttpGet("03/0505")]
    public IActionResult _0504_EmployeeRates()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0505_EmployeeRates.cshtml");
    }

    [HttpGet("03/0506")]
    public IActionResult _0504_UserManagement()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_05/_0506_UserManagement.cshtml");
    }

    // ***** Transaction ************************************************
    [HttpGet("03/0601")]
    public IActionResult _0601_AttenanceEntry()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0601_AttendanceEntry.cshtml");
    }

    [HttpGet("03/0602")]
    public IActionResult _0602_EarningsEntry()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0602_EarningsEntry.cshtml");
    }

    [HttpGet("03/0603")]
    public IActionResult _0603_LoansManagement()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0603_LoansManagement.cshtml");
    }

    [HttpGet("03/0604")]
    public IActionResult _0604_DeductionEntry()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0604_DeductionEntry.cshtml");
    }

    [HttpGet("03/0605")]
    public IActionResult _0605_PayrollProcess()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0605_PayrollProcess.cshtml");
    }

    [HttpGet("03/0606")]
    public IActionResult _0605_PayrollCalculator()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_06/_0606_PayrollProcess_New.cshtml");
    }

    // ***** Report ************************************************
    [HttpGet("03/0701")]
    public IActionResult _0701_PayrollRegister()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_07/_0701_PayrollRegister.cshtml");
    }

    [HttpGet("03/0702")]
    public IActionResult _0702_PaySlip()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_07/_0702_PaySlip.cshtml");
    }

    [HttpGet("03/0703")]
    public IActionResult _0703_BankAdvise()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_07/_0703_BankAdvise.cshtml");
    }

    [HttpGet("03/0704")]
    public IActionResult _0704_BankCharge()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_07/_0704_BankCharge.cshtml");
    }

    [HttpGet("03/0705")]
    public IActionResult _0705ComplianceReport()
    {
        return View("~/Applications/_03Payroll/Views/Pages/_07/_0705_ComplianceReport.cshtml");
    }



}
