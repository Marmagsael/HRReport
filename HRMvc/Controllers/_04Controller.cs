using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers; 

public class _04Controller : Controller
{
    [HttpGet("04")]
    public IActionResult Index()
    {
        return View("~/Applications/_04Accounting/Views/Pages/Index.cshtml");
    }
    
    [HttpGet("04/411")]
    public IActionResult _411TransactionDashboard()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_410/_411TransactionDashboard.cshtml");
    }

    [HttpGet("04/421")]
    public IActionResult _421ProductsAndServices()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_420/_421ProductsAndServices.cshtml");
    }

    [HttpGet("04/422")]
    public IActionResult _422Customers()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_420/_422Customers.cshtml");
    }

    [HttpGet("04/423")]
    public IActionResult _423Estimates()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_420/_423Estimates.cshtml");
    }

    [HttpGet("04/424")]
    public IActionResult _424Invoices()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_420/_424Invoices.cshtml");
    }

    [HttpGet("04/425")]
    public IActionResult _425CustomerStatements()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_420/_425CustomerStatements.cshtml");
    }

    [HttpGet("04/431")]
    public IActionResult _431ProductsAndServices()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_430/_431ProductsAndServices.cshtml");
    }
    
    [HttpGet("04/432")]
    public IActionResult _432Bills()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_430/_432Bills.cshtml");
    }
    
    [HttpGet("04/441")]
    public IActionResult _441ChartOfAccounts()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_440/_441ChartOfAccounts.cshtml");
    }

    [HttpGet("04/442")]
    public IActionResult _442TransactionReconciliation()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_440/_442TransactionReconciliation.cshtml");
    }

    [HttpGet("04/443")]
    public IActionResult _443Transaction()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_440/_443Transaction.cshtml");
    }

    [HttpGet("04/451")]
    public IActionResult _451Reports()
    {
        return View("~/Applications/_04Accounting/Views/Pages/_450/_451Reports.cshtml");
    }





}
