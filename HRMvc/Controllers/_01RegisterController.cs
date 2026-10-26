using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;
using HRMvc.Applications._01Register.Library;
using HRMvc.Applications._01Register.Vars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMvc.Controllers;

// -- 01 Register : Employee / Company Owner, both verified by email OTP --------------
[AllowAnonymous]
[Route("signup")]
public class _01RegisterController : Controller
{
    private const string Pages          = "~/Applications/_01Register/Pages/";
    private const string SessionKey     = "_01RegisterPending";
    private const int    OtpMinutes     = 10;
    private const int    OtpMaxAttempts = 5;
    private const string ExistingUserMsg = "Account already exists. Please log in instead.";

    private readonly IConfiguration             _config;
    private readonly ILogger<_01RegisterController> _log;
    private readonly I_00RegisterDataAccess     _register;
    private readonly L01CompanySetup            _setup;

    public _01RegisterController(IConfiguration config, ILogger<_01RegisterController> log,
                                 I_00RegisterDataAccess register, L01CompanySetup setup)
    {
        _config         = config;
        _log            = log;
        _register       = register;
        _setup          = setup;
    }


    // **********************************************************************************************
    // --- Get Methods ------------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 01.01 Choose register type ------------------------------------------------
    [HttpGet("")]
    public IActionResult Choose()
    {
        if (IsExclusive()) return Redirect("~/signup/employee");    // -- exclusive : employees only --

        SetCoName();
        return View(Pages + "_01_01_Choose.cshtml");
    }

    // -- 01.02 Register as Employee ------------------------------------------------
    [HttpGet("employee")]
    public IActionResult Employee()
    {
        SetCoName();
        return View(Pages + "_01_02_Employee.cshtml", new V01RegisterEmployee());
    }

    // -- 01.03 Register as Company Owner -------------------------------------------
    [HttpGet("owner")]
    public async Task<IActionResult> Owner()
    {
        if (IsExclusive()) return Redirect("~/signup/employee");

        SetCoName();
        await LoadOwnerLookups();
        return View(Pages + "_01_03_Owner.cshtml", new V01RegisterOwner());
    }

    // -- 01.04 Email OTP ---------------------------------------------------------
    [HttpGet("otp")]
    public IActionResult Otp()
    {
        var pending = GetPending();
        if (pending == null) return Redirect("~/signup");

        SetCoName();
        ViewData["Email"] = pending.Email;
        return View(Pages + "_01_04_Otp.cshtml", new V01RegisterOtp());
    }

    // -- 01.05 Done ----------------------------------------------------------------
    [HttpGet("done")]
    public IActionResult Done()
    {
        SetCoName();
        ViewData["Purpose"] = TempData["Purpose"] as string;
        return View(Pages + "_01_05_Done.cshtml");
    }


    // **********************************************************************************************
    // --- Post Methods -----------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 01.02.01 Step 1 : verify company code, employee no. and email (AJAX) ---------
    [HttpPost("employee/check")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployeeCheck(V01RegisterEmployeeCheck input)
    {
        input.CompanyCode = input.CompanyCode.ToUpperInvariant();
        if (!ModelState.IsValid)               return Json(new { ok = false, errors = ModelErrors() });
        if (await EmailExists(input.Email))     return Json(new { ok = false, existing = true });

        var (company, _, checkErrors) = await CheckEmployee(input);
        if (checkErrors.Count > 0) return Json(new { ok = false, errors = checkErrors });

        return Json(new { ok = true, companyName = company!.CompanyName });
    }

    // -- 01.02.02 Step 2 : save (the step 1 check runs again here) -------------------
    [HttpPost("employee")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EmployeeSave(V01RegisterEmployee input)
    {
        input.CompanyCode = input.CompanyCode.ToUpperInvariant();
        if (ModelState.IsValid && await EmailExists(input.Email))
            ModelState.AddModelError(nameof(input.Email), ExistingUserMsg);

        if (ModelState.IsValid)
        {
            var (company, empmas, checkErrors) = await CheckEmployee(input);
            foreach (var e in checkErrors) ModelState.AddModelError(e.Key, e.Value);

            if (checkErrors.Count == 0)
            {
                input.CompanyId = company!.Id;
                input.PISSchema = company.PISSchema!;
            }
        }

        if (!ModelState.IsValid)
        {
            SetCoName();
            return View(Pages + "_01_02_Employee.cshtml", input);
        }

        input.Password          = Sha512(input.Password);
        input.ConfirmPassword   = string.Empty;
        StartOtp(new V01RegisterPending { Purpose = "REG-EMP", Email = input.Email, Employee = input });
        return Redirect("~/signup/otp");
    }

    // -- 01.03.01 Step 1 : verify email and company code (AJAX) ----------------------
    [HttpPost("owner/check")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OwnerCheck(V01RegisterOwnerCheck input)
    {
        if (IsExclusive()) return Json(new { ok = false });

        input.CompanyCode = input.CompanyCode.ToUpperInvariant();
        if (!ModelState.IsValid)               return Json(new { ok = false, errors = ModelErrors() });
        if (await EmailExists(input.Email))     return Json(new { ok = false, existing = true });

        var checkErrors = await CheckOwner(input);
        if (checkErrors.Count > 0) return Json(new { ok = false, errors = checkErrors });

        return Json(new { ok = true });
    }

    // -- 01.03.02 Step 2 : save (the step 1 check runs again here) -------------------
    [HttpPost("owner")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OwnerSave(V01RegisterOwner input)
    {
        if (IsExclusive()) return Redirect("~/signup/employee");

        input.CompanyCode = input.CompanyCode.ToUpperInvariant();
        if (ModelState.IsValid && await EmailExists(input.Email))
            ModelState.AddModelError(nameof(input.Email), ExistingUserMsg);

        if (ModelState.IsValid)
            foreach (var e in await CheckOwner(input)) ModelState.AddModelError(e.Key, e.Value);

        // -- Only the Philippines has payroll tables for now --
        if (ModelState.IsValid)
        {
            var country = await _setup.Country(input.CountryId);
            if (country?.Code != L01CompanySetup.SupportedCountry)
                ModelState.AddModelError(nameof(input.CountryId), $"{country?.Name ?? "This country"} is not available yet. Only the Philippines is supported for now.");
        }

        if (!ModelState.IsValid)
        {
            SetCoName();
            await LoadOwnerLookups();
            return View(Pages + "_01_03_Owner.cshtml", input);
        }

        input.Password          = Sha512(input.Password);
        input.ConfirmPassword   = string.Empty;
        StartOtp(new V01RegisterPending { Purpose = "REG-OWNER", Email = input.Email, Owner = input });
        return Redirect("~/signup/otp");
    }

    // -- 01.04.01 ------------------------------------------------------------------
    [HttpPost("otp")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OtpVerify(V01RegisterOtp input)
    {
        var pending = GetPending();
        if (pending == null) return Redirect("~/signup");

        if (ModelState.IsValid)
        {
            if (DateTime.Now > pending.ExpiresAt)
                ModelState.AddModelError(nameof(input.Code), "The code has expired. Please request a new one.");
            else if (pending.Attempts >= OtpMaxAttempts)
                ModelState.AddModelError(nameof(input.Code), "Too many attempts. Please request a new code.");
            else if (Sha512(input.Code) != pending.OtpHash)
            {
                pending.Attempts++;
                SetPending(pending);
                ModelState.AddModelError(nameof(input.Code), "The code is incorrect.");
            }
        }

        if (!ModelState.IsValid)
        {
            SetCoName();
            ViewData["Email"] = pending.Email;
            return View(Pages + "_01_04_Otp.cshtml", input);
        }

        if (pending.Purpose == "REG-EMP" || pending.Purpose == "REG-OWNER")
        {
            var error = pending.Purpose == "REG-EMP"
                      ? await SaveEmployee(pending.Employee!)  // -- 01.04.02
                      : await SaveOwner(pending.Owner!);       // -- 01.04.03
            if (error != null)
            {
                SetCoName();
                ViewData["Email"] = pending.Email;
                ModelState.AddModelError(string.Empty, error);
                return View(Pages + "_01_04_Otp.cshtml", input);
            }
        }

        HttpContext.Session.Remove(SessionKey);
        TempData["Purpose"] = pending.Purpose;
        return Redirect("~/signup/done");
    }

    // -- 01.04.04 ------------------------------------------------------------------
    [HttpPost("otp/resend")]
    [ValidateAntiForgeryToken]
    public IActionResult OtpResend()
    {
        var pending = GetPending();
        if (pending == null) return Redirect("~/signup");

        StartOtp(pending);
        TempData["OtpResent"] = "A new code has been sent to your email.";
        return Redirect("~/signup/otp");
    }


    // **********************************************************************************************
    // --- Process Methods --------------------------------------------------------------------------
    // **********************************************************************************************

    private void SetCoName()
    {
        ViewData["CoName"]      = _config.GetSection("CompanyInfo:CompanyName").Value;
        ViewData["Exclusive"]   = IsExclusive() ? "true" : "false";
    }

    // -- CompanyInfo:Exclusive = true : one company per deployment, so no owner register --
    private bool IsExclusive()
    {
        return _config.GetSection("CompanyInfo:Exclusive").Value == "true";
    }

    private async Task LoadOwnerLookups()
    {
        ViewData["Country"]  = await _setup.Countries();
        ViewData["Currency"] = await _setup.Currencies();
    }

    // -- 01.04.02 Employee : users + companyusers (pending) + MainPis.Empmas -----------------------
    // -- Returns an error message, or null when everything was saved ------------------------------
    private async Task<string?> SaveEmployee(V01RegisterEmployee input)
    {
        string conn         = _config.GetSection("Schema:DefConn").Value!;
        string schema       = _config.GetSection("Schema:Main").Value!;
        string mainPis      = _config.GetSection("Schema:MainPis").Value!;

        // -- Checked again : the email or the employee may have been taken while the OTP was pending --
        if (await EmailExists(input.Email))                         return ExistingUserMsg;
        var (company, empmas, checkErrors) = await CheckEmployee(input);
        if (checkErrors.Count > 0)                                  return checkErrors.Values.First();

        var emp = new RegisterEmployeeModel
        {
            Email       = input.Email.Trim(),
            Password    = input.Password,                           // already sha2-512
            Domain      = conn,                                     // users.Domain is the connection name the login uses
            CompanyId   = company!.Id,
            PISSchema   = company.PISSchema!,
            EmpNumber   = empmas!.EmpNumber!,                       // name from the old PIS, the source of truth
            LastName    = empmas.LastName,
            FirstName   = empmas.FirstName,
            MiddleName  = empmas.MiddleName,
            Suffix      = empmas.Suffix
        };

        try
        {
            await _register._01Employee(emp, schema, mainPis, conn);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Register employee {Email}: save failed", emp.Email);
            return "We could not create your account. Please try again.";
        }

        return null;
    }

    // -- 01.04.03 Owner : main records, then the company schemas (prefix = company code) -----------
    // -- Returns an error message, or null when everything was created ----------------------------
    private async Task<string?> SaveOwner(V01RegisterOwner input)
    {
        string conn         = _config.GetSection("Schema:DefConn").Value!;
        string schema       = _config.GetSection("Schema:Main").Value!;
        string mainPis      = _config.GetSection("Schema:MainPis").Value!;

        // -- Checked again : the email or code may have been taken while the OTP was pending --
        if (await EmailExists(input.Email))                         return ExistingUserMsg;
        if ((await CheckOwner(input)).Count > 0)                    return "This company code was taken. Please register again with another code.";

        var owner = new RegisterOwnerModel
        {
            Email       = input.Email.Trim(),
            Password    = input.Password,                           // already sha2-512
            Domain      = conn,                                     // users.Domain is the connection name the login uses
            FirstName   = input.FirstName.Trim(),
            MiddleName  = input.MiddleName?.Trim(),
            LastName    = input.LastName.Trim(),
            Suffix      = input.Suffix?.Trim(),
            CompanyName = input.CompanyName.Trim(),
            CompanyCode = input.CompanyCode.ToUpperInvariant(),
            CountryId   = input.CountryId,
            CurrencyId  = input.CurrencyId
        };

        // -- 1). users + userscompany + companyusers + MainPis.Empmas (one transaction) --
        try
        {
            await _register._01Owner(owner, schema, mainPis, conn);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Register owner {Email}: main records failed", owner.Email);
            return "We could not create your account. Please try again.";
        }

        // -- 2). Company schemas : default PIS (AMS in it for now), default Pay + PH payroll, old PIS, old Pay --
        // -- 3). Owner in the company's own Empmas --
        try
        {
            await _setup.CreateSchemas(owner);
        }
        catch (Exception ex)
        {
            // -- The account and company already exist, so the pending registration is not retried --
            _log.LogError(ex, "Register owner {Email}: company schemas for {Code} failed", owner.Email, owner.CompanyCode);
            HttpContext.Session.Remove(SessionKey);
            return "Your account was created, but setting up the company tables did not finish. Please contact support before logging in.";
        }

        return null;
    }

    // -- Existing users log in and add the company there, so register is for new emails only --
    private async Task<bool> EmailExists(string email)
    {
        string conn     = _config.GetSection("Schema:DefConn").Value!;
        string schema   = _config.GetSection("Schema:Main").Value!;

        return await _register._02EmailExists(email, schema, conn);
    }

    private Dictionary<string, string> ModelErrors()
    {
        return ModelState.Where(m => m.Value!.Errors.Count > 0)
                         .ToDictionary(m => m.Key, m => m.Value!.Errors[0].ErrorMessage);
    }

    // -- 01.03.03 Company code not used yet ---------------------------------------------
    private async Task<Dictionary<string, string>> CheckOwner(V01RegisterOwnerCheck input)
    {
        var errors      = new Dictionary<string, string>();

        if (await _setup.CodeTaken(input.CompanyCode)) errors[nameof(input.CompanyCode)] = "This company code is already taken.";

        return errors;
    }

    // -- 01.02.03 Company code -> userscompany, EmpNumber + LastName -> {OldPis}.Empmas (the source of truth) --
    // -- The new {PISSchema}.Empmas row (by EmpNumber) is only used for the account / pending checks --
    private async Task<(V01CompanyLookup?, V01EmpmasLookup?, Dictionary<string, string>)> CheckEmployee(V01RegisterEmployeeCheck input)
    {
        string conn     = _config.GetSection("Schema:DefConn").Value!;
        string schema   = _config.GetSection("Schema:Main").Value!;
        var errors      = new Dictionary<string, string>();

        // -- Company code --
        var company = await _register._02CompanyByCode(input.CompanyCode, schema, conn);

        if (company == null)
        {
            errors[nameof(input.CompanyCode)] = "Company code not found.";
            return (null, null, errors);
        }
        if (new[] { company.PISSchema, company.OldPis }.Any(s => string.IsNullOrEmpty(s) || !Regex.IsMatch(s, @"^\w+$")))
        {
            errors[nameof(input.CompanyCode)] = "This company is not set up for employee registration yet.";
            return (company, null, errors);
        }

        // -- Employee no. + last name in the old PIS, with its email and status --
        var empmas = await _register._02OldEmpmasForRegister(input.EmpNumber, input.LastName, company.OldPis!, conn);

        if (empmas == null)
        {
            errors[nameof(input.EmpNumber)] = "No employee matches this employee number and last name.";
            return (company, null, errors);
        }
        if (empmas.IsResigned == 1)
        {
            errors[nameof(input.EmpNumber)] = $"You cannot join this company because your employee status is {empmas.EmpStatus ?? "Resigned"}.";
            return (company, empmas, errors);
        }

        // -- The login finds the employee's old PIS record by email, so the two must be the same --
        if (string.IsNullOrEmpty(empmas.Email))
        {
            errors[nameof(input.Email)] = "Your employee record has no email. Please ask your company to add it first.";
            return (company, empmas, errors);
        }
        if (!string.Equals(empmas.Email.Trim(), input.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            errors[nameof(input.Email)] = "This email is not the one on your employee record. Please use the email your company has on file.";
            return (company, empmas, errors);
        }

        // -- Already linked, different record or already waiting : the new PIS row of this EmpNumber, when there is one --
        var pis = await _register._02PisEmpmasByEmpNumber(empmas.EmpNumber!, company.PISSchema!, conn);
        if (pis != null)
        {
            empmas.Id       = pis.Id;
            empmas.SystemId = pis.SystemId;

            List<string> different = PisEmpmasLink.Discrepancies(empmas, pis);

            if (pis.SystemId > 0)
                errors[nameof(input.EmpNumber)] = "This employee already has an account.";
            else if (different.Count > 0)
                errors[nameof(input.EmpNumber)] = PisEmpmasLink.DiscrepancyMessage(different);
            else if (await _register._02PendingInvite(company.Id, pis.Id, schema, conn))
                errors[nameof(input.EmpNumber)] = "This employee already has a pending registration.";
        }

        return (company, empmas, errors);
    }


    private void StartOtp(V01RegisterPending pending)
    {
        string code         = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        pending.OtpHash     = Sha512(code);
        pending.ExpiresAt   = DateTime.Now.AddMinutes(OtpMinutes);
        pending.Attempts    = 0;
        SetPending(pending);

        // TODO 01.04.05 : insert main.emailotp and send the code by email (MailKit), then set Register:ShowOtpOnScreen to false
        if (_config.GetSection("Register:ShowOtpOnScreen").Value != "false") TempData["DevOtp"] = code;
    }

    private V01RegisterPending? GetPending()
    {
        var json = HttpContext.Session.GetString(SessionKey);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<V01RegisterPending>(json);
    }

    private void SetPending(V01RegisterPending pending)
    {
        HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(pending));
    }

    // -- Same as MySQL sha2(value, 512), used by Main.Users.Password -------------------
    private static string Sha512(string value)
    {
        return Convert.ToHexString(SHA512.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
    }
}
