using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

public class _00Controller : Controller
{
    [HttpGet("00")]
    public IActionResult Index()
    {
        return View("~/Applications/_00Home/Views/Index.cshtml");
    }

    // --- dun ka maghanap sa => Controller-> Main->MainController 


    [HttpGet("_001MyProfile")]
    public IActionResult _001MyProfile()
    {
        return Redirect("main/my-profile/my-201-records");
    }


    public IActionResult _002EmployeeMgt()
    {
        return Redirect("main/my-profile/my-201-records");
    } 
    
    public IActionResult _003Payroll()
    {
        return Redirect("main/my-profile/my-201-records");
    } 
    
    public IActionResult _004Accounting()
    {
        return Redirect("main/my-profile/my-201-records");
    }
    
    [HttpGet("00/CT")] //CT - ClassTool 
    public IActionResult _005ClassTool()
    {
        return View("~/Applications/_00Home/Tools/Views/_00_ClassMaker.cshtml");
    }
    
   
}
