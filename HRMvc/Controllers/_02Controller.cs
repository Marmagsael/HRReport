// using HRMvc.Applications._02HR.Views.BlazorPages._03Transaction;
// using HRMvc.Applications._02HR.Views.BlazorPages._04Accountability;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;

namespace HRMvc.Controllers;

public class _02Controller : Controller
{
    [HttpGet("02")]
    public IActionResult Index()
    {
        return View("~/Applications/_02HR/Views/Pages/Index.cshtml");
    }

    [HttpGet("02/0201")]
    public IActionResult _0201_Dashboard()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_211Dashboard.cshtml");
    }

    [HttpGet("02/0202")]
    public IActionResult _0202_UserMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_212UserManagement.cshtml");
    }

    [HttpGet("02/0203")]
    public IActionResult _0203_PasswordMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_213PasswordManagement.cshtml");
    }


    [HttpGet("02/0204")]
    public IActionResult _0204_AboutTheSystem()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_214AboutTheSystem.cshtml");
    }

    //------ Data Entry ------//
    [HttpGet("02/0211")]
    public IActionResult _0211_EmployeeEntry()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_221EmployeeEntry.cshtml");
    }

    [HttpGet("02/0212")]
    public IActionResult _0212_Division()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_222Division.cshtml");
    }


    [HttpGet("02/0213")]
    public IActionResult _0213_Department()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_223Department.cshtml");
    }

    [HttpGet("02/0214")]
    public IActionResult _0214_Section()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_224Section.cshtml");
    }

    [HttpGet("02/0215")]
    public IActionResult _0215_Deployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_225Deployment.cshtml"); // To be Created
    }


    [HttpGet("02/0216")]
    public IActionResult _0215_Position()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_226Position.cshtml");
    }

    [HttpGet("02/0217")]
    public IActionResult _0216_Designation()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_227Designation.cshtml"); // To be Created
    }

    [HttpGet("02/0218")]
    public IActionResult _0217_Status()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_228Status.cshtml");
    }

    [HttpGet("02/0219")]
    public IActionResult _0218_LeaveMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_229LeaveMgt.cshtml");
    }

    [HttpGet("02/0220")]
    public IActionResult _0219_PenaltyMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_230PenaltyMgt.cshtml");
    }

    [HttpGet("02/0221")]
    public IActionResult _0220_DeviationMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_231DeviationMgt.cshtml");
    }


    [HttpGet("02/0222")]
    public IActionResult _0220_EmployeeTypeMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_232EmploymentTypeMgt.cshtml");
    }


    //----- Transaction -----//
    [HttpGet("02/0231")]
    public IActionResult _0231_StatusMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_241StatusMgt.cshtml");
    }

    [HttpGet("02/0232")]
    public IActionResult _0232_Deployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_242EmployeeDeployment.cshtml");
    }

    [HttpGet("02/0233")]
    public IActionResult _0233_Deviation()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_243Deviation.cshtml");
    }

    [HttpGet("02/0234")]
    public IActionResult _0234_DisciplinaryAction()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_244DisciplinaryAction.cshtml");
    }

    [HttpGet("02/0235")]
    public IActionResult _0235_Reinstatement()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_245Reinstatement.cshtml");
    }

    [HttpGet("02/0236")]
    public IActionResult _0236_ChangeEmployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_246ChangeDeployment.cshtml"); // To be Created
    }

   

    [HttpGet("02/0237")]
    public IActionResult _0237_RecallEmployee()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_247RecallEmployee.cshtml");
    }

    [HttpGet("02/0238")]
    public IActionResult _0238_GrpRecall()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_248GrpRecall.cshtml");
    }

    [HttpGet("02/0241")]
    public IActionResult _0239_LeaveOfAbsence()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_249LeaveOfAbsence.cshtml");
    }




    //----- Accountabilty -----//
    [HttpGet("02/0251")]
    public IActionResult _0251_InventoryEntry()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_261Inventory.cshtml");
    }

    [HttpGet("02/0252")]
    public IActionResult _0252_DeploymentEntry()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_262InventoryDeplyment.cshtml");
    }

    [HttpGet("02/0253")]
    public IActionResult _0253_InventoryStatusEntry()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_263InventoryStatus.cshtml");
    }

    [HttpGet("02/0254")]
    public IActionResult _0254_EquipmentReport()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_264InventoryReport.cshtml");
    }


    //----- Reports ------//
    [HttpGet("02/0261")]
    public IActionResult _0261_EmployeeMaterList()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_281EmployeeMasterList.cshtml");
    }

    [HttpGet("02/0262")]
    public IActionResult _0262_EmployeeStatusReport()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_282EmployeeStatusRep.cshtml");
    }

    [HttpGet("02/0263")]
    public IActionResult _0263_ManpowerMovement()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_283ManpowerMovement.cshtml");
    }

    [HttpGet("02/0264")]
    public IActionResult _0264_OnLeaveEmployee()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_284OnLeaveEmployee.cshtml");
    }

    [HttpGet("02/0265")]
    public IActionResult _0265_NewlyHiredForTheMonth()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_285NewlyHired.cshtml");
    }

    [HttpGet("02/0266")]
    public IActionResult _0266_ForRegularization()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_286ForRegularization.cshtml");
    }

    [HttpGet("02/0267")]
    public IActionResult _0267_ResignedForTheMonth()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_287ResignedForTheMonth.cshtml");
    }
}
