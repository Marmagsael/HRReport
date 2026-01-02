using HRApiLibrary.Models._00_Main;
using HRMvc.Models.Authentication;
using HRMvc.Models.Main.LoginSignUp;
using HRMvc.Models.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HRApiLibrary.DataAccess._00_Main;
using HRMvc.DataAccess.Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using MySqlX.XDevAPI;

namespace HRMvc.Controllers.Main;

public class AuthenticationController : Controller
{
    private readonly IConfiguration                 _config;
    private readonly I_00UsersAccess                 _userAccess;
    private readonly I_90_001_MySqlDataAccess       _mysql;
    private readonly I_00MainDA                     _mainDA;
                                                                        
    
    private readonly I_10_EmpmasDataAccess          _empmas; 
    private readonly ClaimsAccess                   _claimsAccess;      
    private readonly I_09_02_VarsGlobal             _vars;
    private readonly I_00MainPisTblMakerAccess      _mainPisTblMaker;
    private readonly I_20_002_PayTblMaker           _payTblMaker;
    private readonly I_00UserscompanyDataAccess     _userCompany;
    private readonly I_AcctgTableMaker              _acctg;
    
            



    public AuthenticationController(IConfiguration config,
                                    I_00UsersAccess userAccess,
                                    I_90_001_MySqlDataAccess mysql,
                                    I_00MainDA mainDA,
                                    I_09_02_VarsGlobal vars,
                                    ClaimsAccess claimsAccess,
                                    I_00MainPisTblMakerAccess mainPisTblMaker,
                                    I_10_EmpmasDataAccess empmas, 
                                    I_20_002_PayTblMaker payTblMaker, 
                                    I_00UserscompanyDataAccess userCompany, 
                                    I_AcctgTableMaker acctg)
    {
        _config             = config;
        _userAccess         = userAccess;
        _mysql              = mysql;
        _mainDA             = mainDA;
        _vars               = vars;
        _claimsAccess       = claimsAccess;
        _mainPisTblMaker    = mainPisTblMaker;
        _empmas             = empmas;
        _payTblMaker        = payTblMaker;
        _userCompany        = userCompany;
        _acctg              = acctg;
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
        ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;
        return View();
    }
    
    [HttpGet("changingclaims/{link}/{userid}")]
    public async Task<IActionResult> ChangingClaims(string link, string userid)
    {
        //ViewData["CoName"] = _config.GetSection("CompanyInfo:CompanyName").Value;

        var uid     = int.Parse(userid); 
        var user    = await _mainDA._02UsersById(uid);
        var uc      = await _mainDA._02UserCompany(user?.DefaultCoId ?? 0);

        CreateClaims(user ?? new UsersModel(){Id=0}, uc); 
        return Redirect("~/13");
        
        //return Content($"user id : {uc?.CompanyName} ** link {link}");
        
    }
    

    // -- 03 ---------------------------
    [AllowAnonymous]
    [HttpGet("registerOrdinary")]
    public IActionResult RegisterOrdinary()
    {
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
        CreateClaims(user, uc); //-- 02.04 
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

        
        UsersModel? user = await _mainDA._02UsersLoginLoginName(login.EmpNumber, login.Password);
        
        if (user == null)
        {
            //return Ok("User is null");
            ViewData["ErrorMsg"] = "Invalid username / password.";
            return View("Login");
        }

        // -- Get User Company -------------------------------------------------------
        var conn     = _config.GetSection("Schema:DefConn").Value.ToString();
        var schema   = _config.GetSection("Schema:Main").Value.ToString();

        var uc = await _02UserCompany(user, schema, conn);

        // Create Schema and Tables  --------------------------------------------------------
        _01SchemaAndTables(uc?.PisSchema, conn); // Added By Judith .To create the pis table if it does not existS

        CreateClaims(user, uc);
        await CreateCompany(user, uc, conn);    
        
        
        return Redirect("13");


    }

    private async Task CreateCompany(UsersModel user, UserCompanyModel? uc, string conn)
    {

        
        var userId = user.Id;
        //var usr = await _userAccess._02ById(userId); 
        var coId   = user.DefaultCoId;
        var userDb = "U"+userId.ToString().Trim();
        await _mainPisTblMaker._01UserTable(userDb, conn);


        if (coId > 1)
        {

            var prefix  = userDb + "C" + coId.ToString();
            var pisdb   = prefix + "pis";
            var paydb   = prefix + "pay";
            var acctgdb = prefix + "acctg";
            
            
            _mainPisTblMaker._01MainPisTableInternal(pisdb, conn);
            
            await _payTblMaker._01(paydb);
            var x = await _userCompany._02(coId, "Main", conn!);
            if(x?.CountryId == 2) await _payTblMaker._01PH(paydb); 
            
            await _acctg.AccountingTableMaker(acctgdb, conn); 

            
        }

        
        //await _mainPisTblMaker._01(uc.PisSchema, conn); 


    } 
    


    //**********************************************************************************************
    // -- Left Menu ------------------------------------------------------------------------------------

    // -- 20.00 ------------------------------------------------------------------------------------
    public async Task<IActionResult> _200ChangeCompany(int userCompanyId)
    {

        var conn    = _config.GetSection("Schema:DefConn").Value.ToString();
        var schema  = _config.GetSection("Schema:Main").Value.ToString();

        if (userCompanyId == -1)
        {
            return RedirectToAction("_202RegisterNewCompany");
        }

        var empmasId = User.Claims.Where(c => c.Type == "UserId").FirstOrDefault()?.Value;
        var empId       = int.Parse(empmasId!);
        //string? defCompanyId    = User.Claims.Where(c => c.Type == "DefCompayId").FirstOrDefault()?.Value;
        //int defCoId = int.Parse(defCompanyId!);
        UsersModel? user = await _mainDA._02UsersById(empId, schema, conn);
        user!.DefaultCoId = userCompanyId;

        await _mainDA._03Users(userCompanyId, user, schema, conn);

        UserCompanyModel? uc = await _02UserCompany(user, schema, conn);
        CreateClaims(user, uc);

        if (userCompanyId <= 0) 
            return  Redirect("~/main/my-profile/my-201-records");
        return      Redirect("~/main/company-records/personnel-information");

    }

    public IActionResult _201PersonalProfile()
    {
        return View();
    }

    public IActionResult _202RegisterNewCompany()
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
        string sql = $@"SELECT c.* FROM {schema}.Country c order by Name";
        IEnumerable<CountryUiModel> country = await _mysql.FetchData<CountryUiModel, dynamic>(sql, new { }, conn);

        //-- Currency -----------------------------------------------
        sql = $@"SELECT c.* FROM {schema}.Currency c order by Name";
        IEnumerable<CurrencyUiModel> currency = await _mysql.FetchData<CurrencyUiModel, dynamic>(sql, new { }, conn);


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
            }
            else
            {
                // -- Fetch assign company to user --------------------------------
                uc = await _mainDA._02UserCompany(user.DefaultCoId, schema, conn);

            }
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

    public async void CreateClaims(UsersModel user, UserCompanyModel? uc)
    {
        var userId          = user.Id.ToString();;
        var prefix          = "U" + userId.Trim() + "C1"; 

        var pisSchema       = prefix + "Pis";
        var paySchema       = prefix + "Pay";
        var acctgSchema     = prefix + "Acctg";
        var appSchema       = prefix + "App";
        var amsSchema       = prefix + "Ams";
        var conn            = user.Domain;
        var coName          = uc?.CompanyName ?? "-";

        if (!string.IsNullOrEmpty(user.DefaultCoId.ToString()) && user.DefaultCoId != 0)
        {
            prefix      = "U" + userId + "C" + uc?.Id.ToString().Trim()??"0"; 
            pisSchema   = uc?.PisSchema;
            paySchema   = uc?.PaySchema;
            acctgSchema = prefix+"Acctg";
            appSchema   = uc?.ApplicantSchema;
            amsSchema   = uc?.AmsSchema;
        }



        //return Content($"uc id : {uc.PisSchema}");
        
        var loginName               = user.LoginName ?? "-1";
        //var defCoId                 = uc?.Id.ToString() ?? "0";
        var defCoId                 = user.DefaultCoId.ToString() ?? "0";
        var isExclusiveCompany      = _config.GetSection("CompanyInfo:Exclusive").Value;

        var claims = new List<Claim>
        {
            new("UserId",               userId),
            new("UserName",             loginName),
            new("DefCompayId",          defCoId),
            new("PisSchema",            pisSchema ?? ""),
            new("PaySchema",            paySchema ?? ""),
            new("AcctgSchema",          acctgSchema ?? ""),
            new("ApplicantSchema",      appSchema ?? ""),
            new("AmsSchema",            amsSchema ?? ""),
            new("CoName",               coName ?? ""),
            new("Conn",                 conn  ?? "MySql"),  
            new("IsExclusiveCompany",   isExclusiveCompany)
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

        var sql = $"SELECT * FROM {schema}.`Currency` order by Code";
        var currency = await _mysql.FetchData<CurrencyModel, dynamic>(sql, new { }, conn);

        return currency;

    }

    //-- 02.04 --- Claims 

    //-- 02.04 --- Claims 
    public async void CreateClaims(UsersModel user)
    {
        
        var userId = user.Id.ToString() ?? "0";;
        var prefix = "U" + userId.Trim() + "C1"; 

        var pisSchema       = prefix + "Pis";
        var paySchema       = prefix + "Pay";
        var acctgSchema     = prefix + "Acctg";
        var appSchema       = prefix + "App";
        var amsSchema       = prefix + "Ams";
        var conn           = user.Domain;
        var defCoId              = "0";
        
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
            new("DefCompayId",          defCoId ?? ""),
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
