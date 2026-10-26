using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.DataAccess._11_AMS;
using HRApiLibrary.Models._10_Pis;
using HRMvc.DataAccess.Main;
using HRMvc.Models.Authentication;
using HRMvc.Models.Main;
using HRMvc.Models.Main.LoginSignUp;
using HRMvc.StartupConfig;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Security.Claims;

namespace HRMvc.Controllers.Main;

public class AuthenticationController : Controller
{
    private readonly IConfiguration                 _config;
    private readonly I_00UsersAccess                 _userAccess;
    private readonly I_90_001_MySqlDataAccess       _mysql;
    private readonly I_00MainDA                     _mainDA;
                                                                        
    private readonly I_10_EmpmasDataAccess          _empmas; 
    private readonly IEmpmasInternalDataAccess      _empmasInternal; 
    
    private readonly ClaimsAccess                   _claimsAccess;      
    private readonly I_09_02_VarsGlobal             _vars;
    private readonly I_00MainPisTblMakerAccess      _mainPisTblMaker;
    private readonly I_20_002_PayTblMaker           _payTblMaker;
    private readonly IAMSTableMaker                 _amsTblMaker;
    private readonly I_00UserscompanyDataAccess     _userCompany;
    private readonly I_AcctgTableMaker              _acctg;
    private readonly IOEmpmasDataAccess             _oldEmpmas;
    private readonly IAtttemplatereqhdrDataAccess   _attTemplateReqhdr;
    private readonly I_00EngagementDataAccess       _engagement;
    
            



    public AuthenticationController(IConfiguration             config,
                                    I_00UsersAccess            userAccess,
                                    I_90_001_MySqlDataAccess   mysql,
                                    I_00MainDA                 mainDA,
                                    I_09_02_VarsGlobal         vars,
                                    ClaimsAccess               claimsAccess,
                                    I_00MainPisTblMakerAccess  mainPisTblMaker,
                                    IAMSTableMaker             amsTblMaker,
                                    I_10_EmpmasDataAccess      empmas, 
                                    IEmpmasInternalDataAccess  empmasInternal, 
                                    I_20_002_PayTblMaker       payTblMaker, 
                                    I_00UserscompanyDataAccess userCompany, 
                                    I_AcctgTableMaker          acctg, 
                                    IOEmpmasDataAccess         oldEmpmas, 
                                    IAtttemplatereqhdrDataAccess attTemplateReqhdr,
                                    I_00EngagementDataAccess   engagement )
    {
        _engagement         = engagement;
        _config             = config;
        _userAccess         = userAccess;
        _mysql              = mysql;
        _mainDA             = mainDA;
        _vars               = vars;
        _claimsAccess       = claimsAccess;
        _mainPisTblMaker    = mainPisTblMaker;
        _empmas             = empmas;
        _empmasInternal     = empmasInternal;
        _payTblMaker        = payTblMaker;
        _userCompany        = userCompany;
        _acctg              = acctg;
        _oldEmpmas          = oldEmpmas;
        _amsTblMaker        = amsTblMaker;
        _attTemplateReqhdr  = attTemplateReqhdr;
    }



    // **********************************************************************************************
    // --- Get Methods -----------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 00 ---------------------------
    public IActionResult Index()
    {
        return View();
    }


    // -- 01 ---------------------------
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login()
    {
        ViewData["Exclusive"]   = _config.GetSection("CompanyInfo:Exclusive").Value;
        ViewData["CoName"]      = _config.GetSection("CompanyInfo:CompanyName").Value;
        return View();
    }

    // -- 02 ---------------------------
    [AllowAnonymous]
    [HttpGet("register")]
    public IActionResult Register()
    {
        ViewData["Exclusive"] = _config.GetSection("CompanyInfo:Exclusive").Value;
        ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;
        return View();
    }
    
    
    

    // -- 03 ---------------------------
    [AllowAnonymous]
    [HttpGet("registerOrdinary")]
    public IActionResult RegisterOrdinary()
    {
        ViewData["Exclusive"] = _config.GetSection("CompanyInfo:Exclusive").Value;
        ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;
        return View();
    }



    // **********************************************************************************************
    // --- Post Methods -----------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 03.01 -------------------------------------------------------------------------
    [AllowAnonymous]
    [HttpPost("registerOrdinary")]
    public async Task<IActionResult> RegisterOrdinarySave(RegisterOrdinaryUiModel input)
    {

        var inputData = input;

        await CheckUser(input); // ==> 03.01.01

        if (input.withError)
        {
            return View("RegisterOrdinary");
        }
        else
        {
            await LoadCreateUserCompanyData(); // ==> 03.02 (Postback Action)

            CreateUserCompanyUiModel userCompany = new()
            {
                LoginName   = input.Username,
                Password    = input.Password,
                Email       = input.Email
            };

            ViewData["input"] = inputData;
            return View("SignupCreateUserCompany", userCompany); //== 3.01.02 

        }




    }


    // -- 03.02 -------------------------------------------------------------------------
    [AllowAnonymous]
    [HttpPost("signupCreateUserCompanySave")]
    public async Task<IActionResult> SignupCreateUserCompanySave(CreateUserCompanyUiModel model)
    {
        string conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        string schema   = _config.GetSection("Schema:Main").Value.ToString();
        ViewData["ErrorMsg"] = string.Empty;

        // 1). Create User -------------------------------------------------------------------
        UsersModel? user = await _01User(model, schema, conn); // -- 03.02.01
        if (user == null)
        {
            ViewData["ErrorMsg"] = "Error Creating user!";
            return View("SignupCreateUserCompany", model);
            // === To Create Error Signing Page ================================

        }
        if (user.DefaultCoId != 0)
        {
            ViewData["ErrorMsg"] = "User Already Exists.";
            return View("SignupCreateUserCompany", model);
            // === To Create Error Signing Page ================================
        }
        model.UserId = user.Id;

        // 2). Create User Company --------------------------------------------------------
        UserCompanyModel? userCompany = await _01userCompany(model, schema, conn); // -- 03.02.02

        // 3). Create Company User  --------------------------------------------------------
        CompanyUsersModel? companyUser = await _01companyUser(userCompany!, schema, conn);  // -- 03.02.03

        // 4). Create Schema and Tables  --------------------------------------------------------
        _01SchemaAndTables(userCompany?.PisSchema, conn);  // -- 03.02.04

        // 5). Create Cookies  --------------------------------------------------------
        UserCompanyModel? uc = await _02UserCompany(user, schema, conn);
        await CreateClaims(user, uc); //-- 02.04 
        await CreateCompany(user, uc, conn);

        // 6). Save User to Own PIS schema --------------------------------------------
        EmpmasInternalModel empmasInternal = new ()
                                {
                                    Id          = user.Id,
                                    SystemId    = 0,
                                    EmpNumber   = "", 
                                    EmpLastNm   = model.EmpLastNm,
                                    EmpFirstNm  = model.EmpFirstNm,
                                    EmpMidNm    = model.EmpMidNm,
                                    Suffix      = model.Suffix,
                                    EmpAlias    = model.EmpAlias
                                }; 
        
        _01EmpmasInternal(empmasInternal, userCompany?.PisSchema, conn);

        user.DefaultCoId = userCompany!.Id;
        await _userAccess._03(user.Id, user);

        return Redirect("main/my-profile/my-201-records");
        
        
        
    }


    
    // -- 3.02.01 ----------------------------------------------------------------
    private async Task<UsersModel?> _01User(CreateUserCompanyUiModel model, string schema, string conn)
    {
        string domain = string.Empty;
        if (_config.GetSection("Schema:Domain").Value != null) domain = _config.GetSection("Schema:Domain").Value;

        UsersModel? user = new()
        {
            LoginName   = model.LoginName,
            Password    = model.Password,
            Email       = model.Email,
            Domain      = domain,
            UserType    = 1,
            Status      = "A",
            DefaultCoId = 0
        };
        UsersModel? userCreated = await _userAccess._01(user, schema, conn);

        // --- Create MainPis.Empmas ----------------------------------------------------------
        if(userCreated != null)
        {
            EmpmasModel empmas = new()
            {
                Id          = userCreated.Id!,
                EmpLastNm   = model.EmpLastNm,
                EmpFirstNm  = model.EmpFirstNm,
                EmpMidNm    = model.EmpMidNm,
                Suffix      = model.Suffix,
                EmpAlias    = model.EmpAlias
            };
            string mainPisSchema = _config.GetSection("Schema:MainPis").Value; 
            await _empmas._01Empmas(empmas, mainPisSchema, conn); 

        }
        



        return userCreated;

    }

    // -- 3.02.02 ----------------------------------------------------------------
    private async Task<UserCompanyModel?> _01userCompany(CreateUserCompanyUiModel model, string schema, string conn)
    {
        UserCompanyModel userCompany = new UserCompanyModel
        {
            OwnerId         = model.UserId,
            CompanySName    = model.CompanySName,
            CompanyName     = model.CompanyName,
            CountryId       = model.CountryId,
            RegionId        = model.RegionId,
            CityId          = model.CityId,
            Zipcode         = model.Zipcode,
            CurrencyId      = model.CurrencyId,
            AmsSchema       = "Default",
            ApplicantSchema = "Default",
            PisSchema       = "Default",
            PaySchema       = "Default"
        };

        UserCompanyModel? uc = await _mainDA._01UserCompany(userCompany, schema, conn);
        if (uc?.PaySchema == "Default")
        {
            var prefix       = @$"U{uc.OwnerId.ToString().Trim()}C{uc.Id.ToString().Trim()}";
            uc.AmsSchema        = $"{prefix}Ams";
            uc.ApplicantSchema  = $"{prefix}App";
            uc.PisSchema        = $"{prefix}Pis";
            uc.PaySchema        = $"{prefix}Pay";

            UserCompanyModel? ucUpdated = await _mainDA._03UserCompany(uc.Id, uc, schema, conn);
            return ucUpdated;

        }
        return uc;
    }

    // -- 3.02.03 ----------------------------------------------------------------
    private async Task<CompanyUsersModel?> _01companyUser(UserCompanyModel userCompany, string schema, string conn)
    {
        CompanyUsersModel cu = new CompanyUsersModel
        {
            UserId              = userCompany.OwnerId,
            CompanyId           = userCompany.Id,
            Status              = "A",
            CompanyUserTypeId   = 4

        };

        CompanyUsersModel? cuNew = await _mainDA._01CompanyUsers(cu, schema, conn);
        return cuNew;

    }

    // -- 3.02.04 ----------------------------------------------------------------
    private void _01SchemaAndTables(string? schema, string conn)
    {
        if (schema != null || schema != "Default")
        {
            _mainPisTblMaker._01MainPisTableInternal(schema!, conn);
            _mainPisTblMaker._01MainPisTable(schema!, conn);
        }
    }

    // -- 3.02.05 ----------------------------------------------------------------
    private async void _01EmpmasInternal(EmpmasInternalModel? empmas, string? pisSchema, string conn)
    {
        if (empmas != null)
        {
            if (pisSchema != null)
            {
                await _empmas._01EmpmasInternal(empmas,pisSchema, conn);
            }
        }
    }




    //**********************************************************************************************
    // -- Login ------------------------------------------------------------------------------------

    // -- 10.00 ------------------------------------------------------------------------------------
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUiModel login)
    {
        try
        {
            string isExclusive = _config.GetSection("CompanyInfo:Exclusive").Value;
         
            UsersModel? user = await _mainDA._02UsersLoginEmail(login.Email, login.Password);

            if (user == null)
            {
                LoadLoginViewData();
                ViewData["ErrorMsg"] = "Invalid username / password.";
                return View("Login", login);
            }

            var conn = _config.GetSection("Schema:DefConn").Value.ToString();
            var schema = _config.GetSection("Schema:Main").Value.ToString();

            // -- Registered employee not approved yet ----------------------------------
            string? approvalMsg = await _02ApprovalMsg(user, isExclusive, schema, conn);
            if (approvalMsg != null)
            {
                LoadLoginViewData();
                ViewData["ErrorMsg"] = approvalMsg;
                return View("Login", login);
            }

            // -- Get User Company -------------------------------------------------------
            var uc = await _02UserCompany(user, schema, conn);
            if (uc == null)
            {
                LoadLoginViewData();
                ViewData["ErrorMsg"] = "Your account is not linked to any company yet.";
                return View("Login", login);
            }

            if (isExclusive == "true")
            {
                uc.ExclusiveCompany = login.CompanyId;
            }

            await CreateClaims(user, uc);

            // Tables of the current company : only when appsettings "Migrations" is "true" --
            await _01CurrentCompanyTables(user, uc, conn);

            HttpContext.Session.SetString("OldPis", uc.OldPis ?? "");
            HttpContext.Session.SetString("OldPay", uc.OldPay ?? "");

            if (string.IsNullOrEmpty(uc.OldPis)) return Redirect("~/13");

            var empmas = await _oldEmpmas._02ByEmail(user.Email ?? "00000", uc.OldPis, conn ?? "");
            if (empmas.Count > 0)
                HttpContext.Session.SetString("EmpNumber", empmas.First().EmpNumber ?? "00000");

            return Redirect("12/102");
        }
        catch (MySqlException ex)
        {
            LoadLoginViewData();
            ViewData["ErrorMsg"] = "Cannot reach the server right now. Please try again shortly.";
            Console.WriteLine(ex);
            return View("Login", login);
        }
        catch (Exception ex)
        {
            LoadLoginViewData();
            ViewData["ErrorMsg"] = "Something went wrong. Please try again.";
            return View("Login", login);
        }
    }


    // -- 10.01 Registered employee : CompanyInvite (UserId + CompanyId + Status) ----------------
    // -- P pending / D declined (self-registered, InvitedById 0) block the login, unless the user is already in CompanyUsers (A) --
    // -- Exclusive : the deployment company; otherwise only users with no company yet ------------
    private async Task<string?> _02ApprovalMsg(UsersModel user, string isExclusive, string schema, string conn)
    {
        int? companyId;
        if (isExclusive == "true")      companyId = int.Parse(_config.GetSection("CompanyInfo:DefaultCoId").Value);
        else if (user.DefaultCoId == 0) companyId = null;
        else                            return null;

        var statuses    = await _engagement._02ApprovalStatus(user.Id ?? 0, companyId, schema, conn);

        if (statuses.Contains("M")) return null;    // -- member already --
        if (statuses.Contains("P")) return "Your registration is still waiting for company approval.";
        if (statuses.Contains("D")) return "Your registration was not approved. Please contact your company.";
        return null;
    }

    private void LoadLoginViewData()
    {
        ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;
        ViewData["Exclusive"] = _config.GetSection("CompanyInfo:Exclusive").Value;
    }


    [HttpGet("changingclaims/{link}/{userid}")]
    public async Task<IActionResult> ChangingClaims(string link, string userid)
    {
        //ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;

        // -- Only the logged-in user's own claims are rebuilt; the userid in the URL must be that user --
        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var uid);
        if (uid <= 0 || userid?.Trim() != uid.ToString()) return StatusCode(StatusCodes.Status403Forbidden);

        var user    = await _mainDA._02UsersById(uid);
        if (user == null) return StatusCode(StatusCodes.Status403Forbidden);
        var uc      = await _mainDA._02UserCompany(user?.DefaultCoId ?? 0);

        await CreateClaims(user, uc);
        HttpContext.Session.Clear();

        var conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        await _01CurrentCompanyTables(user, uc, conn);

        HttpContext.Session.SetString("OldPis", uc.OldPis ?? "");
        HttpContext.Session.SetString("OldPay", uc.OldPay ?? "");
        HttpContext.Session.SetString("EmpNumber", "00000");

        if (uc.OldPis.Length <= 0) return Redirect("~/13");

        var empmas = await _oldEmpmas._02ByEmail(user.Email??"00000", uc.OldPis,  conn??"");
        if(empmas.Count > 0 ) HttpContext.Session.SetString("EmpNumber", empmas.First().EmpNumber ?? "00000");
        

        return Redirect("~/13");
        
        //return Content($"user id : {uc?.CompanyName} ** link {link}");
        
    }

    // -- 02.05 Tables of the current company (login / switch company) : only when appsettings "Migrations" is "true" --
    // -- a new company gets its tables + initial data in L01CompanySetup.CreateSchemas --
    private async Task _01CurrentCompanyTables(UsersModel? user, UserCompanyModel? uc, string conn)
    {
        if (!MigrationExt.On(_config) || user == null || uc == null) return;

        try
        {
            await _mainPisTblMaker._01UserTable("U" + (user.DefaultCoId)?.ToString().Trim(), conn);
        }
        catch (Exception ex) { Console.WriteLine($"Migrations : user tables failed : {ex.Message}"); }

        await MigrationExt.RunCompany(HttpContext.RequestServices, uc.Id);
    }

    private async Task CreateCompany(UsersModel user, UserCompanyModel? uc, string conn)
    {

        
        var userId = user.Id;
        var coId   = user.DefaultCoId;
        var userDb = "U"+ (user.DefaultCoId)?.ToString().Trim();

        await _mainPisTblMaker._01UserTable(userDb, conn);

        if (coId > 1)
        {

            var prefix  = userDb + "C" + coId.ToString();
            var pisdb   = prefix + "pis";
            var paydb   = prefix + "pay";
            var acctgdb = prefix + "acctg";
            
            if(uc.PisSchema.Trim().Length > 0) pisdb   = uc.PisSchema.Trim();
            if(uc.PaySchema.Trim().Length > 0) paydb   = uc.PaySchema.Trim();
            if(!string.IsNullOrWhiteSpace(uc.AcctgSchema)) acctgdb = uc.AcctgSchema.Trim();

            await _mainPisTblMaker._01MainPisTableInternal(pisdb, conn);
            await _amsTblMaker._01AMSTable(pisdb, conn);
            
            await _payTblMaker._01(paydb);
            var x = await _userCompany._02(coId, "Main", conn!);
            if(x?.CountryId == 2) await _payTblMaker._01PH(paydb); 
            
            await _acctg.AccountingTableMaker(acctgdb, conn); 

        }

        
        //await _mainPisTblMaker._01(uc.PisSchema, conn); 


    } 
    


    //**********************************************************************************************
    // -- Left Menu ------------------------------------------------------------------------------------

    // -- 20.03 Switch company : the only way to change company; it is also the default on the next login --
    // -- (replaces the old GET _200ChangeCompany, which had no membership check) --
    [HttpPost("company/switch/{companyId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> _203SwitchCompany(int companyId)
    {
        var conn    = _config.GetSection("Schema:DefConn").Value!;
        var schema  = _config.GetSection("Schema:Main").Value!;
        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId);

        // -- Only to a company where the user is approved --
        if (userId <= 0 || !await _engagement._03DefaultCompany(userId, companyId, schema, conn))
        {
            TempData["EngError"] = "You can only switch to a company where your access is approved.";
            return Redirect("~/12/003");
        }

        UsersModel? user        = await _mainDA._02UsersById(userId, schema, conn);
        UserCompanyModel? uc    = await _02UserCompany(user!, schema, conn);
        await CreateClaims(user!, uc);
        await _01CurrentCompanyTables(user, uc, conn);

        TempData["EngOk"] = $"You are now working in {uc?.CompanyName}.";
        return Redirect("~/12/003");
    }

    public IActionResult _201PersonalProfile()
    {
        return View();
    }


    // -- Left Menu ------------------------------------------------------------------------------------
    //**********************************************************************************************



    // **********************************************************************************************
    // --- Process Methods --------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 3.01.01 ----------------------------------------------------------------
    private async Task<IActionResult> CheckUser(RegisterOrdinaryUiModel input)
    {
        UsersModel? userName = await _userAccess._02ByLoginName(input.Username);
        ViewData["errUserMsg"] = "";
        ViewData["errEmailMsg"] = "";

        input.errUserMsg = string.Empty;
        input.errEmailMsg = string.Empty;
        input.withError = false;

        if (userName != null)
        {
            ViewData["errUserMsg"] = "Username already exists.";
            input.errUserMsg = "Username already exists.";
            input.withError = true;
        };

        var emailAdd = await _userAccess._02ByEmail(input.Email);
        if (emailAdd != null)
        {
            ViewData["errEmailMsg"] = "Email already exists.";
            input.errEmailMsg = "Email already exists.";
            input.withError = true;
        }

        ViewData["input"] = input;
        return View("RegisterOrdinary", input);
    }

    // -- 3.01.02 ----------------------------------------------------------------
    private async Task<IActionResult> LoadCreateUserCompanyData()
    {
        string conn         = _config.GetSection("Schema:DefConn").Value.ToString();
        string schema       = _config.GetSection("Schema:Main").Value.ToString();

        ViewData["CoName"]  = _config.GetSection("CompanyInfo:CompanyName").Value;

        //-- Country -----------------------------------------------
        var country     = await _mainDA._02CountryList(schema, conn);

        //-- Currency -----------------------------------------------
        var currency    = await _mainDA._02CurrencyList(schema, conn);


        CreateUserCompanyUiModel userCompany = new CreateUserCompanyUiModel();
        ViewData["country"]     = country;
        ViewData["currency"]    = currency;

        return View("RegisterOrdinary");

    }

    // -- 10.00.01 ---------------------------------------------------------------
    private async Task<UserCompanyModel?> _02UserCompany(UsersModel user, string schema, string conn)
    {

        UserCompanyModel? uc = new();
        string isExclusive = _config.GetSection("CompanyInfo:Exclusive").Value;

        if (isExclusive == "true")
        {
            // -- Naka exclusive ang use sa server ng company -------------------------
            string userCompanyId = _config.GetSection("CompanyInfo:DefaultCoId").Value;
            uc = await _mainDA._02UserCompany(Int32.Parse(userCompanyId), schema, conn);
        }
        else
        {
            if (user.DefaultCoId == 0)
            {
                //  -- Walang naka assign na company 
                var res = await _mainDA._02UserCompanyPerOwnerId(user.Id);
                uc = res.FirstOrDefault();

                // -- Not an owner : first company where the user is an approved member --
                if (uc == null)
                {
                    int? coId = await _engagement._02FirstMemberCompany(user.Id ?? 0, schema, conn);
                    if (coId != null) uc = await _mainDA._02UserCompany(coId, schema, conn);
                }
            }
            else
            {
                // -- Fetch assign company to user --------------------------------
                uc = await _mainDA._02UserCompany(user.DefaultCoId, schema, conn);
            }
        }

        // --- Fetch Assigned Old Pis and Old Pay ------------------------------------------------------
        if (user.DefaultCoId > 0)
        {
            var res = await _mainDA._02UserCompany(user.DefaultCoId, schema, conn);
            if (uc.OldPis != null && uc.OldPis != "") uc.OldPis = res?.OldPis ?? "";
            if (uc.OldPay != null && uc.OldPay != "") uc.OldPay = res?.OldPay ?? "";
        }

        return uc;

    }

    







    // **********************************************************************************************
    // --- API Internal Library  --------------------------------------------------------------------
    // **********************************************************************************************
    [AllowAnonymous]
    [HttpGet("_02Country")]
    public async Task<List<CountryModel?>> _02Country()
    {
        string conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        string schema   = _config.GetSection("Schema:Main").Value.ToString();
        List<CountryModel?> country = await _mainDA._02CountryList(schema, conn);

        return country;
    }


    //-- 02.02 --- Region per country   
    [AllowAnonymous]
    [HttpGet("_02ProvinceStateByCountryId/{countryId}")]
    public async Task<List<ProvinceStateModel?>> _02ProvinceStateByCountryId(int countryId)
    {
        string conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        string schema   = _config.GetSection("Schema:Main").Value.ToString();
        List<ProvinceStateModel?> region = await _mainDA._02ProvinceStatePerCountry(countryId, schema, conn);

        return region;
    }


    //-- 02.03 --- City per region  
    [AllowAnonymous]
    [HttpGet("_02CityByRegionId/{regionId}")]
    public async Task<List<CityModel?>> _02CityByRegionId(int regionId)
    {
        string conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        string schema   = _config.GetSection("Schema:Main").Value.ToString();

        List<CityModel?> city = await _mainDA._02CityPerRegion(regionId, schema, conn);
        return city;
    }

    public async Task CreateClaims(UsersModel user, UserCompanyModel? uc)
    {
        var userId          = user.Id.ToString();
        var empmasId        = string.Empty;
        var prefix          = "U" + userId.Trim() + "C1"; 

        var pisSchema       = prefix + "Pis";
        var paySchema       = prefix + "Pay";
        var acctgSchema     = prefix + "Acctg";
        var appSchema       = prefix + "App";
        var amsSchema       = prefix + "Ams";
        var coName          = uc?.CompanyName ?? "-";
        var email           = user.Email;
        
        var empnumber       = user.LoginName??"00000";
        var oldPis           = uc?.OldPis ?? "";
        var oldPay           = uc?.OldPay ?? "";
        var exclusiveCompany = uc?.ExclusiveCompany ?? "";

        var conn            = user.Domain;
        var res             = await _empmasInternal._02BySystemIds(user?.Id??00, uc?.PisSchema ?? "", conn??"");
        
        if(res.Count > 0 ) 
        {
            empnumber = res.First().EmpNumber ?? "00000";    
            empmasId = res.First().Id.ToString() ?? "0";
        }

        if (!string.IsNullOrEmpty(user.DefaultCoId.ToString()) && user.DefaultCoId != 0)
        {
            prefix      = "U" + userId + "C" + uc?.Id.ToString().Trim()??"0"; 
            pisSchema   = uc?.PisSchema;
            paySchema   = uc?.PaySchema;
            acctgSchema = string.IsNullOrWhiteSpace(uc?.AcctgSchema) ? prefix + "Acctg" : uc.AcctgSchema;   // registered companies : {CompanyCode}_Acctg
            appSchema   = uc?.ApplicantSchema;
            amsSchema   = uc?.AmsSchema;
        }



        // --- apply approved change schedule request once its Effectivity has arrived
        try
        {
            if (int.TryParse(empmasId, out int eId) && eId > 0)
                await _attTemplateReqhdr._03AttTemplateReqhdr_to_AttTemplate(user.Id, eId, pisSchema, conn);
        }
        catch (Exception ex) { Console.WriteLine($"Apply change schedule failed : {ex.Message}"); }

        var loginName               = user.LoginName ?? "-1";
        var defCoId                 = user.DefaultCoId.ToString() ?? "0";
        var isExclusiveCompany      = _config.GetSection("CompanyInfo:Exclusive").Value;

        // --- Role in the current company (main.CompanyUsers); users with no row keep the User role
        int? companyUserType        = (user.Id ?? 0) > 0 && (uc?.Id ?? 0) > 0
                                    ? await _engagement._02CompanyUserType(user.Id!.Value, uc!.Id!.Value, _config.GetSection("Schema:Main").Value, conn)
                                    : null;

        if (isExclusiveCompany == "true")
        {
            // conn = exclusiveCompany;
        }


        var claims = new List<Claim>
        {
            new("UserId",               userId),
            new("UserName",             loginName),
            new("Email",                email??""),
            new("DefCompayId",          defCoId),
            new("PisSchema",            pisSchema ?? ""),
            new("PaySchema",            paySchema ?? ""),
            new("AcctgSchema",          acctgSchema ?? ""),
            new("ApplicantSchema",      appSchema ?? ""),
            new("AmsSchema",            amsSchema ?? ""),
            new("CoName",               coName ?? ""),
            new("Conn",                 conn  ?? "MySql"),

            new("EmpmasId",             empmasId  ?? "0"),
            new("Empnumber",            empnumber  ?? "00000"),  
            new("OldPis",               oldPis  ?? "secpis"),  
            new("OldPay",               oldPay  ?? "pay"),  
            
            new("IsExclusiveCompany",   isExclusiveCompany),
            new("ExclusiveCompany",    exclusiveCompany), // For GSIA
            new(CompanyRole.ClaimType,  (companyUserType ?? CompanyRole.User).ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimPrincipal);
    }

    [AllowAnonymous]
    [HttpGet("_02Currency")]
    public async Task<List<CurrencyModel>> _02CurrencyList()
    {
        var conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        var schema   = _config.GetSection("Schema:Main").Value.ToString();

        var currency = await _mainDA._02CurrencyList(schema, conn);

        return currency.OrderBy(c => c?.Code).ToList()!;

    }

    //-- 02.04 --- Claims 

    //-- 02.04 --- Claims 
    public async Task CreateClaims(UsersModel user)
    {
        
        var userId = user.Id.ToString() ?? "0";;
        var prefix = "U" + userId.Trim() + "C1"; 

        var pisSchema       = prefix + "Pis";
        var paySchema       = prefix + "Pay";
        var acctgSchema     = prefix + "Acctg";
        var appSchema       = prefix + "App";
        var amsSchema       = prefix + "Ams";
        var conn           = user.Domain;
        var defCoId               = "0";
        var email           = user.Email ?? "-";
        
        var resUc = await _mainDA._02UserCompanyPerOwnerId(user.Id); 
        var coName                = resUc.FirstOrDefault()?.CompanyName ?? "Undefined Company";

        var currUser    = await _mainDA._02UsersById(user.Id);
        defCoId                   = currUser?.DefaultCoId.ToString().Trim() ?? "0"; 
        if (currUser?.DefaultCoId > 0)
        {
            var userCo = await _mainDA._02UserCompany(currUser.DefaultCoId);
            var ownerId = userCo?.OwnerId.ToString().Trim() ?? "0"; 
            
            prefix = "U" + ownerId + "C" + userCo?.Id ?? "0"; 
            
            pisSchema       = prefix + "Pis";
            paySchema       = prefix + "Pay";
            acctgSchema     = prefix + "Acctg";
            appSchema       = prefix + "App";
            amsSchema       = prefix + "Ams";
            coName          = userCo?.CompanyName ?? "-";
        }
        
        var loginName                   = user.LoginName ?? "-1";
        var isExclusiveCompany      = _config.GetSection("CompanyInfo:Exclusive").Value;
        
        await HttpContext.SignOutAsync(); 
        
        var claims = new List<Claim>
        {
            new("UserId",               userId),
            new("UserName",             loginName ?? ""),
            new("Email",                email     ?? "-"),
            new("DefCompayId",          defCoId   ?? ""),
            new("PisSchema",            pisSchema ?? ""),
            new("PaySchema",            paySchema ?? ""),
            new("ApplicantSchema",      appSchema ?? ""),
            new("AmsSchema",            amsSchema ?? ""),
            new("CoName",               coName ?? ""),
            new("Conn",                 conn ?? "MySql"),  
            new("IsExclusiveCompany",   isExclusiveCompany)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimPrincipal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(claimPrincipal);

    }
}
