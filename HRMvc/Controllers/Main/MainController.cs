using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._90_Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Xml.Schema;
using static Dapper.SqlMapper;

namespace HRMvc.Controllers.Main;

public class MainController : Controller
{
    private readonly I_90_001_MySqlDataAccess _mysql;
    private readonly IConfiguration _config;

    private readonly I_00MainDA _mainDA;
    private readonly I_00MainPisAccess _mainPisDA;



    public MainController(I_90_001_MySqlDataAccess mysql, IConfiguration config, I_00MainDA mainDA, I_00MainPisAccess mainPisDA)
    {
        _mysql = mysql;
        _config = config;
        _mainDA = mainDA;
        _mainPisDA = mainPisDA;
    }
    public IActionResult Index()
    {
        return View();
    }



    [HttpGet("00home")]
    public IActionResult _00(int action)
    {
        return View("~/Views/Main/_00Home/Index.cshtml");
    }


    //*****************************************
    // -- 01 Profiles--------------------------
    //*****************************************
    [HttpGet("main/my-profile/my-201-records")]
    [HttpGet("main/my-profile/my-201-records/{action}")]
    public IActionResult _111My201Records(int action)
    {
        return View("~/Views/Main/_01MyProfile/_111My201Records.cshtml");
    }

    [HttpGet("main/my-profile/201")]
    public IActionResult _111My201()
    {
        return View("~/Views/Main/_01MyProfile/_111My201Records.cshtml");
    }

    [HttpGet("main/my-profile/employment")]
    public IActionResult _112Employment()
    {
        return View("~/Views/Main/_01MyProfile/_112Employment.cshtml");
    }

    [HttpGet("main/my-profile/trainings")]
    public IActionResult _113Trainings()
    {
        return View("~/Views/Main/_01MyProfile/_113Trainings.cshtml");
    }

    [HttpGet("main/my-profile/uploadables")]
    public IActionResult _114Uploadables()
    {
        return View("~/Views/Main/_01MyProfile/_114Uploadables.cshtml");
    }

    
    [HttpGet("main/my-profile/engagement")]
    public IActionResult _121Engagement()
    {
        return View("~/Views/Main/_01MyProfile/_121Engagement.cshtml");
    }
    
    [HttpGet("main/my-profile/myattendance")]
    public IActionResult _131MyAttendance()
    {
        return View("~/Views/Main/_01MyProfile/_131MyAttendance.cshtml");
    }

    [HttpGet("main/my-profile/leaveapplication")]
    public IActionResult _132LeaveApplication()
    {
        return View("~/Views/Main/_01MyProfile/_132LeaveApplication.cshtml");
    }

    //[HttpGet("main/my-profile/appointment")]
    //public IActionResult _133Appointment()
    //{
    //    return View("~/Views/Main/_01MyProfile/_133Appointment.cshtml");
    //}

    [HttpGet("main/my-profile/payslip")]
    public IActionResult _151Payslip()
    {
        return View("~/Views/Main/_01MyProfile/_151Payslip.cshtml");
    }

    [HttpGet("main/my-profile/scheduledeductions")]
    public IActionResult _152ScheduleDeductions()
    {
        return View("~/Views/Main/_01MyProfile/_152ScheduleDeductions.cshtml");
    }










    //*****************************************
    // -- 02 Employment------------------------
    //*****************************************
    [HttpGet("main/employee-mgt/dashboard")]
    public IActionResult _211Dashboard()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_211Dashboard.cshtml");
    }

    [HttpGet("main/employee-mgt/user-mgt")]
    public IActionResult _212UserManagement()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_212UserManagement.cshtml");
    }

    [HttpGet("main/employee-mgt/password-mgt")]
    public IActionResult _213PasswordManagement()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_213PasswordManagement.cshtml");
    } 
    
    
    [HttpGet("main/employee-mgt/about-the-system")]
    public IActionResult _214AboutTheSystem()
    {
        return View("~/Applications/_02HR/Views/Pages/_01System/_214AboutTheSystem.cshtml");
    }

    //--- Data Entry ------------------------------------------------
    

    
    [HttpGet("main/employee-mgt/employee-entry")]
    public IActionResult _221EmployeeEntry()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_221EmployeeEntry.cshtml");
    }
    
    [HttpGet("main/employee-mgt/division")]
    public IActionResult _222Division()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_222Division.cshtml");
    }
    
    
    [HttpGet("main/employee-mgt/department")]
    public IActionResult _223Department()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_223Department.cshtml");
    }
    
    [HttpGet("main/employee-mgt/section")]
    public IActionResult _224Section()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_224Section.cshtml");
    }

    [HttpGet("main/employee-mgt/deployment")]
    public IActionResult _225Deployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_225Deployment.cshtml");
    }

    [HttpGet("main/employee-mgt/position")]
    public IActionResult _226Position()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_226Position.cshtml");
    }

    [HttpGet("main/employee-mgt/designation")]
    public IActionResult _227Designation()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_228Status.cshtml");
    }

    [HttpGet("main/employee-mgt/status")]
    public IActionResult _228Status()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_228Status.cshtml");
    } 
    
    [HttpGet("main/employee-mgt/leave-mgt")]
    public IActionResult _229LeaveMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_229LeaveMgt.cshtml");
    }
    
    [HttpGet("main/employee-mgt/penalty-mgt")]
    public IActionResult _230PenaltyMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_230PenaltyMgt.cshtml");
    }
    
    
    [HttpGet("main/employee-mgt/diviation-mgt")]
    public IActionResult _231DeviationMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_231DeviationMgt.cshtml");
    }


    [HttpGet("main/employee-mgt/employment-type-mgt")]
    public IActionResult _232EmploymentTypeMgt()
    {
        return View("~/Applications/_02HR/Views/Pages/_02DataEntry/_232EmploymentTypeMgt.cshtml");
    }


    //--- Transaction ------------------------------------------------
    [HttpGet("main/transaction/status-mgt")]
    public IActionResult _241StatusMgt()
    {
       return View("~/Applications/_02HR/Views/Pages/_03Transaction/_241StatusMgt.cshtml");       
    }
    
    [HttpGet("main/transaction/deployment")]
    public IActionResult _242EmployeeDeployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_242EmployeeDeployment.cshtml");
    }

    [HttpGet("main/transaction/deviation")]
    public IActionResult _243Deviation()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_243Deviation.cshtml");
    }


    [HttpGet("main/transaction/disciplinary-action")]
    public IActionResult _244DisciplinaryAction()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_244DisciplinaryAction.cshtml");
    }

    [HttpGet("main/transaction/reinstatement")]
    public IActionResult _245Reinstatement()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_245Reinstatement.cshtml");
    }

    [HttpGet("main/transaction/change-employment")]
    public IActionResult _246ChangeEmployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_246ChangeEmployment.cshtml");
    }

    [HttpGet("main/transaction/recall-employee")]
    public IActionResult _247RecallEmployee()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_247RecallEmployee.cshtml");
    }


    [HttpGet("main/transaction/grp-recall")]
    public IActionResult _248GrpRecall()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_248GrpEmployeeRecall.cshtml");
    }
    

    [HttpGet("main/transaction/leave-transaction")]
    public IActionResult _249LeaveOfAbsence()
    {
        return View("~/Applications/_02HR/Views/Pages/_03Transaction/_249LeaveOfAbsence.cshtml");
    }
    

    
    
    //--- Accountability ---------------------------------------------
    [HttpGet("main/transaction/inventory")]
    public IActionResult _261Inventory()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_261Inventory.cshtml");
    }
    
    [HttpGet("main/transaction/inventory-deployment")]
    public IActionResult _262InventoryDeployment()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_262InventoryDeplyment.cshtml");
    }
    
    [HttpGet("main/employee-mgt/inventory-status")]
    public IActionResult _263InventoryStatus()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_263InventoryStatus.cshtml");
    }
    
    [HttpGet("main/transaction/inventory-report")]
    public IActionResult _264InventoryReport()
    {
        return View("~/Applications/_02HR/Views/Pages/_04Accountability/_264InventoryReport.cshtml");
    }

    
    //--- Reports ----------------------------------------------------
    [HttpGet("main/report/employee-master-list")]
    public IActionResult _281EmployeeMasterList()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_281EmployeeMasterList.cshtml");
    }

    [HttpGet("main/report/employee-status-report")]
    public IActionResult _282EmployeeStatusRep()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_282EmployeeStatusRep.cshtml");
    }

    [HttpGet("main/report/manpower-movement")]
    public IActionResult _283ManpowerMovement()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_283ManpowerMovement.cshtml");
    }

    [HttpGet("main/report/onLeave-employee")]
    public IActionResult _284OnLeaveEmployee()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_284OnLeaveEmployee.cshtml");
    }

    [HttpGet("main/report/newly-hired")]
    public IActionResult _285NewlyHired()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_285NewlyHired.cshtml");
    }

    [HttpGet("main/report/for-regularization")]
    public IActionResult _286ForRegularization()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_286ForRegularization.cshtml");
    }

    [HttpGet("main/report/resigned-for-the-month")]
    public IActionResult _287ResignedForTheMonth()
    {
        return View("~/Applications/_02HR/Views/Pages/_05Report/_287ResignedForTheMonth.cshtml");
    }

    


 





    //*****************************************
    // -- 03 Payroll System -------------------
    //*****************************************

    // -- 03.01 Payroll System --
    [HttpGet("main/payroll-system/dashboard")]
    public IActionResult _311Dashboard()
    {
        return View("~/Views/Main/_03Payroll/_311Dashboard.cshtml");
    }







    //*****************************************
    // -- 04 Accounting System ----------------
    //*****************************************

    [HttpGet("main/accounting-system/dashboard")]
    public IActionResult _411Index()
    {
        return View("~/Views/Main/_04Accounting/index.cshtml");
    }

   

    //**********************************************************
    // -- 05 Profiles : User right menus -----------------------


    [HttpGet("main/profile/password")]
    public IActionResult _401Password()
    {
        return View();
    }


    [HttpGet("main/profile/my-engagement")]
    public IActionResult _402MyEngagement()
    {
        return View();
    }


    [HttpGet("main/profile/businesses-and-subscription")]
    //public async Task<IActionResult> _403BusinessesAndSubscription()
    public IActionResult _403BusinessesAndSubscription()
    {
        //IEnumerable<Claim?> claims = User.Claims;
        //UserClaimsModel ucm = _mainDA._02UserClaimsContent(claims);

        //UserClaimsModel uClaim = _mainDA._02UserClaimsContent(User.Claims);
        //IEnumerable<CompaniesAssignedToUserModel?> ownCompanies     = await _mainDA._02CompanyUsers_Owned_WTypeWCoDtls_List(uClaim.UserId!, uClaim.SchemaMain!, uClaim?.Conn!);
        //IEnumerable<CompaniesAssignedToUserModel?> OtherCompanies   = await _mainDA._02CompanyUsers_Employment_WTypeWCoDtls_List(uClaim!.UserId!, uClaim.SchemaMain!, uClaim.Conn!);

        //return Ok(OtherCompanies);
        return View(); 
    }

    public async Task<IActionResult> _404Logout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/login");
    }


    // -- 04 Profiles--------------------------
    //*****************************************


    //*****************************************
    // -- 05 Data Call ------------------------

    [HttpGet("mainpis/empmas/{id}")]
    public async Task<EmpmasModel?> _02Empmas(int id)
    {
        IEnumerable<Claim?> claims = User.Claims;
        UserClaimsModel userClaim = _mainDA._02UserClaimsContent(claims); 

        EmpmasModel? empmas = await _mainPisDA._02Empmas(id, userClaim.SchemaMainPis!, userClaim.Conn!);
        return empmas;
    }

    [HttpGet("mainpis/my201records/{id}")]
    public async Task<EmpmasRecordModel?> _02my201records(int id)
    {
        EmpmasRecordModel empmasRecord = new(); 
        IEnumerable<Claim?> claims = User.Claims;
        UserClaimsModel userClaim = _mainDA._02UserClaimsContent(claims);

        EmpmasModel? empmas = await _mainPisDA._02Empmas(id, userClaim.SchemaMainPis!, userClaim.Conn!);
        empmasRecord.Empmas = empmas;

        EmpmasPIModel? empmasPi = await _mainPisDA._02EmpmasPI(id, userClaim.SchemaMainPis!, userClaim.Conn!);
        empmasRecord.EmpmasPI = empmasPi;


        return empmasRecord;
    }

    // -- 05 Data Call ------------------------
    //*****************************************


}

