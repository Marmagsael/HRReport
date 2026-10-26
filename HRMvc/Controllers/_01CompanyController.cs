using System.Text.RegularExpressions;
using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;
using HRMvc.Applications._01Register.Library;
using HRMvc.Applications._01Register.Vars;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HRMvc.Controllers;

// -- 01.06 Create New Company : logged-in user becomes the owner of another company -----
// -- Same company setup as Register as Company Owner, but no new users row and no OTP ---
[Route("company")]
public class _01CompanyController : Controller
{
    private const string Pages = "~/Applications/_01Register/Pages/";

    private readonly IConfiguration             _config;
    private readonly ILogger<_01CompanyController> _log;
    private readonly I_00RegisterDataAccess     _register;
    private readonly L01CompanySetup            _setup;
    private readonly I_00EngagementDataAccess   _engagement;

    public _01CompanyController(IConfiguration config, ILogger<_01CompanyController> log,
                                I_00RegisterDataAccess register, L01CompanySetup setup, I_00EngagementDataAccess engagement)
    {
        _config     = config;
        _log        = log;
        _register   = register;
        _setup      = setup;
        _engagement = engagement;
    }

    // -- Exclusive server (CompanyInfo:Exclusive = true) : no Create New Company and no Invite User --
    // -- Accept / Decline of an invite, the Registration Requests and the Company Users stay open --
    private static readonly string[] AjaxActions    = [nameof(Check), nameof(InviteSearch), nameof(InviteCheck)];
    private static readonly string[] AlwaysAllowed  = [nameof(InviteAccept), nameof(InviteDecline),
                                                       nameof(Requests), nameof(RequestApprove), nameof(RequestDecline),
                                                       nameof(Users), nameof(UserSave)];

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string action = (context.ActionDescriptor as ControllerActionDescriptor)?.ActionName ?? "";

        if (_config.GetSection("CompanyInfo:Exclusive").Value == "true" && !AlwaysAllowed.Contains(action))
        {
            const string msg = "Creating companies and inviting users are not available on this server.";
            if (AjaxActions.Contains(action))
            {
                context.Result = Json(new { ok = false, error = msg });
                return;
            }

            TempData["EngError"] = msg;
            context.Result = Redirect("~/12/003");
            return;
        }

        base.OnActionExecuting(context);
    }


    // **********************************************************************************************
    // --- Get Methods ------------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 01.06 Create New Company --------------------------------------------------
    [HttpGet("new")]
    public async Task<IActionResult> New()
    {
        await LoadLookups();
        return View(Pages + "_01_06_NewCompany.cshtml", new V01NewCompany());
    }


    // **********************************************************************************************
    // --- Post Methods -----------------------------------------------------------------------------
    // **********************************************************************************************

    // -- 01.06.01 Company code available (AJAX) -------------------------------------
    [HttpPost("new/check")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Check(string companyCode)
    {
        companyCode = (companyCode ?? string.Empty).Trim().ToUpperInvariant();
        if (await _setup.CodeTaken(companyCode))
            return Json(new { ok = false, error = "This company code is already taken." });

        return Json(new { ok = true });
    }

    // -- 01.06.02 Save : userscompany + companyusers, then the company schemas ----------
    [HttpPost("new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(V01NewCompany input)
    {
        input.CompanyCode = (input.CompanyCode ?? string.Empty).Trim().ToUpperInvariant();
        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId);

        RegisterOwnerModel? owner = userId > 0 ? await _setup.OwnerByUserId(userId) : null;
        if (owner == null)
            ModelState.AddModelError(string.Empty, "Your account profile was not found. Please log in again.");

        if (ModelState.IsValid && await _setup.CodeTaken(input.CompanyCode))
            ModelState.AddModelError(nameof(input.CompanyCode), "This company code is already taken.");

        // -- Only the Philippines has payroll tables for now --
        if (ModelState.IsValid)
        {
            var country = await _setup.Country(input.CountryId);
            if (country?.Code != L01CompanySetup.SupportedCountry)
                ModelState.AddModelError(nameof(input.CountryId), $"{country?.Name ?? "This country"} is not available yet. Only the Philippines is supported for now.");
        }

        if (!ModelState.IsValid)
        {
            await LoadLookups();
            return View(Pages + "_01_06_NewCompany.cshtml", input);
        }

        owner!.UserId       = userId;
        owner.CompanyName   = input.CompanyName.Trim();
        owner.CompanyCode   = input.CompanyCode;
        owner.CountryId     = input.CountryId;
        owner.CurrencyId    = input.CurrencyId;

        // -- 1). userscompany + companyusers (one transaction) --
        try
        {
            await _register._01Company(owner, _config.GetSection("Schema:Main").Value, _config.GetSection("Schema:DefConn").Value);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "New company {Code} for user {UserId}: main records failed", owner.CompanyCode, userId);
            ModelState.AddModelError(string.Empty, "We could not create the company. Please try again.");
            await LoadLookups();
            return View(Pages + "_01_06_NewCompany.cshtml", input);
        }

        // -- 2). Company schemas + owner in the company's Empmas --
        try
        {
            await _setup.CreateSchemas(owner);
        }
        catch (Exception ex)
        {
            // -- The company row already exists, so the form is not shown again --
            _log.LogError(ex, "New company {Code} for user {UserId}: company schemas failed", owner.CompanyCode, userId);
            TempData["EngError"] = $"{owner.CompanyName} was created, but setting up its tables did not finish. Please contact support.";
            return Redirect("~/12/003");
        }

        TempData["EngOk"] = $"{owner.CompanyName} ({owner.CompanyCode}) was created.";
        return Redirect("~/12/003");
    }


    // **********************************************************************************************
    // --- 01.07 Invite User : owner / system admin invites an employee of the old PIS who has an account --
    // **********************************************************************************************

    [HttpGet("invite")]
    public async Task<IActionResult> Invite()
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null) return OnlyAdminCanInvite();

        SetInviteView(company);
        return View(Pages + "_01_07_InviteUser.cshtml", new V01InviteUser());
    }

    // -- 01.07.01 Search (AJAX) : old PIS employee no. or name, 30 rows at most, nothing loaded up front --
    [HttpGet("invite/search")]
    public async Task<IActionResult> InviteSearch(string? term)
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null || !ValidSchemas(company) || (term ?? "").Trim().Length < 2) return Json(Array.Empty<object>());

        var rows = await _engagement._02InviteSearch(company.OldPis!, term!, Main, Conn);
        return Json(rows.Select(r => new { id = r.EmpNumber,
                                           text = $"{r.EmpNumber} - {r.LastName}, {r.FirstName}" + (string.IsNullOrEmpty(r.EmpStatus) ? "" : $" ({r.EmpStatus})") }));
    }

    // -- 01.07.02 Check (AJAX) : the picked employee's email and whether it can be invited --
    [HttpGet("invite/check")]
    public async Task<IActionResult> InviteCheck(string? empNumber)
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null || !ValidSchemas(company)) return Json(new { ok = false, error = "Only the owner or a system admin of your current company can invite users." });

        var candidate   = string.IsNullOrWhiteSpace(empNumber) ? null
                        : await _engagement._02InviteCandidate(company.Id, empNumber.Trim(), company.PISSchema!, company.OldPis!, Main, Conn);
        string? error   = InviteError(candidate);
        return Json(new { ok = error == null, email = candidate?.Email, error });
    }

    [HttpPost("invite")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InviteSave(V01InviteUser input)
    {
        int userId  = UserId();
        var company = await _setup.CurrentAdminCompany(userId);
        if (company == null) return OnlyAdminCanInvite();

        // -- Checked again here : the picked employee may have changed since the check --
        InviteCandidateModel? candidate = null;
        if (ModelState.IsValid && ValidSchemas(company))
        {
            candidate       = await _engagement._02InviteCandidate(company.Id, input.EmpNumber!.Trim(), company.PISSchema!, company.OldPis!, Main, Conn);
            string? error   = InviteError(candidate);
            if (error != null) ModelState.AddModelError(nameof(input.EmpNumber), error);
        }

        if (!ModelState.IsValid || candidate == null)
        {
            SetInviteView(company);
            ViewData["PickedText"] = candidate == null ? null : $"{candidate.EmpNumber} - {candidate.LastName}, {candidate.FirstName}";
            return View(Pages + "_01_07_InviteUser.cshtml", input);
        }

        input.Normalize();
        await _engagement._01Invite(new CompanyInviteModel
        {
            UserId                  = candidate.UserId,
            CompanyId               = company.Id,
            InvitedById             = userId,
            CompanyUserTypeId       = input.CompanyUserTypeId,
            WithPISAccess           = input.WithPISAccess,
            WithPayAccess           = input.WithPayAccess,
            WithAMSAccess           = input.WithAMSAccess,
            WithAccountingAccess    = input.WithAccountingAccess,
            WithApplicantAccess     = input.WithApplicantAccess
        }, candidate.OldPis(), company.PISSchema!, Main, Conn);

        TempData["EngOk"] = $"An invitation was sent to {candidate.FirstName} {candidate.LastName} ({candidate.Email}).";
        return Redirect("~/12/003");
    }

    // -- 01.07.03 My Invitations : accept / decline --------------------------------------------
    [HttpPost("invite/accept/{companyId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InviteAccept(int companyId)
    {
        string? error = await _engagement._03AcceptInvite(UserId(), companyId, Main, Conn);

        if (error != null)  TempData["EngError"] = error;
        else                TempData["EngOk"]    = "Invitation accepted. You can now switch to this company.";
        return Redirect("~/12/003");
    }

    [HttpPost("invite/decline/{companyId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InviteDecline(int companyId)
    {
        await _engagement._03DeclineInvite(UserId(), companyId, Main, Conn);
        TempData["EngOk"] = "Invitation declined.";
        return Redirect("~/12/003");
    }


    // **********************************************************************************************
    // --- 01.08 Registration Requests : owner approves / declines employees who registered ---------
    // **********************************************************************************************

    [HttpGet("requests")]
    public async Task<IActionResult> Requests()
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null) return OnlyAdminCanApprove();

        ViewData["CompanyName"] = company.CompanyName;
        List<RegistrationRequestModel> rows = ValidSchemas(company)
                                            ? await _engagement._02Requests(company.Id, company.PISSchema!, Main, Conn)
                                            : [];
        if (!ValidSchemas(company))
            ViewData["SetupError"] = "This company has no PIS or old PIS set up, so its registrations cannot be reviewed yet.";

        return View(Pages + "_01_08_Requests.cshtml", rows);
    }

    // -- 01.08.01 Approve : the employee status is checked again, it may have changed since the register --
    [HttpPost("requests/approve/{userId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestApprove(int userId, V01ApproveRequest input)
    {
        int ownerId = UserId();
        var company = await _setup.CurrentAdminCompany(ownerId);
        if (company == null)        return OnlyAdminCanApprove();
        if (!ValidSchemas(company)) return Redirect("~/company/requests");

        var request = await _engagement._02Request(userId, company.Id, company.PISSchema!, Main, Conn);
        if (request == null)
        {
            TempData["ReqError"] = "This request is no longer pending.";
            return Redirect("~/company/requests");
        }

        var stat = string.IsNullOrEmpty(request.EmpNumber) ? null : await _register._02OldEmpStatus(request.EmpNumber, company.OldPis!, Conn);
        if (stat?.IsResigned == 1)
        {
            TempData["ReqError"] = $"{request.FirstName} {request.LastName} cannot join the company because the employee status is {stat.EmpStatus}. Decline the request instead.";
            return Redirect("~/company/requests");
        }

        input.Normalize();
        string? error = await _engagement._03ApproveRequest(new CompanyInviteModel
        {
            UserId                  = userId,
            CompanyId               = company.Id,
            InvitedById             = ownerId,
            CompanyUserTypeId       = input.CompanyUserTypeId,
            WithPISAccess           = input.WithPISAccess,
            WithPayAccess           = input.WithPayAccess,
            WithAMSAccess           = input.WithAMSAccess,
            WithAccountingAccess    = input.WithAccountingAccess,
            WithApplicantAccess     = input.WithApplicantAccess
        }, company.PISSchema!, Main, Conn);

        if (error != null)  TempData["ReqError"] = error;
        else                TempData["ReqOk"]    = $"{request.FirstName} {request.LastName} was approved and can now log in.";
        return Redirect("~/company/requests");
    }

    [HttpPost("requests/decline/{userId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestDecline(int userId)
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null) return OnlyAdminCanApprove();

        await _engagement._03DeclineRequest(userId, company.Id, Main, Conn);
        TempData["ReqOk"] = "The request was declined.";
        return Redirect("~/company/requests");
    }


    // **********************************************************************************************
    // --- 01.09 Company Users : role (Owner only) and access (Owner or System Admin) of each member ---
    // **********************************************************************************************

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null) return OnlyAdminCanManage();

        ViewData["CompanyName"] = company.CompanyName;
        ViewData["IsOwner"]     = company.CompanyUserTypeId == 1;
        ViewData["Me"]          = UserId();
        List<CompanyMemberModel> rows = ValidSchemas(company)
                                      ? await _engagement._02Members(company.Id, company.PISSchema!, _config.GetSection("Schema:MainPis").Value, Main, Conn)
                                      : [];
        return View(Pages + "_01_09_Users.cshtml", rows);
    }

    [HttpPost("users/{userId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserSave(int userId, V01CompanyUser input)
    {
        var company = await _setup.CurrentAdminCompany(UserId());
        if (company == null) return OnlyAdminCanManage();

        // -- Only the Owner changes roles; nobody changes their own row here --
        bool isOwner = company.CompanyUserTypeId == 1;
        if (userId == UserId())
        {
            TempData["UsrError"] = "You cannot change your own role or access here.";
            return Redirect("~/company/users");
        }

        bool saved = await _engagement._03Member(new CompanyInviteModel
        {
            UserId                  = userId,
            CompanyId               = company.Id,
            CompanyUserTypeId       = input.CompanyUserTypeId,
            WithPISAccess           = input.WithPISAccess,
            WithPayAccess           = input.WithPayAccess,
            WithAMSAccess           = input.WithAMSAccess,
            WithAccountingAccess    = input.WithAccountingAccess,
            WithApplicantAccess     = input.WithApplicantAccess
        }, isOwner, Main, Conn);

        if (saved)  TempData["UsrOk"]    = "The user was updated.";
        else        TempData["UsrError"] = "This user cannot be changed.";
        return Redirect("~/company/users");
    }


    // **********************************************************************************************
    // --- Process Methods --------------------------------------------------------------------------
    // **********************************************************************************************

    private string Main => _config.GetSection("Schema:Main").Value!;
    private string Conn => _config.GetSection("Schema:DefConn").Value!;

    private int UserId()
    {
        int.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId);
        return userId;
    }

    private IActionResult OnlyAdminCanInvite()
    {
        TempData["EngError"] = "Only the owner or a system admin of your current company can invite users. Switch to that company first.";
        return Redirect("~/12/003");
    }

    private IActionResult OnlyAdminCanManage()
    {
        TempData["EngError"] = "Only the owner or a system admin of your current company can manage its users. Switch to that company first.";
        return Redirect("~/12/003");
    }

    private IActionResult OnlyAdminCanApprove()
    {
        TempData["EngError"] = "Only the owner or a system admin of your current company can review registration requests. Switch to that company first.";
        return Redirect("~/12/003");
    }

    // -- Schema names go into the SQL, so they must be plain names --
    private static bool ValidSchemas(V01CompanyLookup company)
    {
        return new[] { company.PISSchema, company.OldPis }.All(s => !string.IsNullOrEmpty(s) && Regex.IsMatch(s, @"^\w+$"));
    }

    private void SetInviteView(V01CompanyLookup company)
    {
        ViewData["CompanyName"] = company.CompanyName;
        if (!ValidSchemas(company))
            ViewData["SetupError"] = "This company has no PIS or old PIS set up, so its employees cannot be invited yet.";
    }

    // -- Why an employee cannot be invited, or null when the invite can be sent --
    // -- Already a user / invited comes from CompanyUsers / CompanyInvite of this company, not from Empmas.SystemId --
    // -- Same rules as Register as Employee : the old PIS is the source of truth, the new PIS row must match it --
    public static string? InviteError(InviteCandidateModel? c)
    {
        if (c == null)                                      return "Please select an employee.";
        if (c.IsResigned == 1)                              return $"This employee cannot join the company because the employee status is {c.EmpStatus ?? "Resigned"}.";
        if (!string.IsNullOrEmpty(c.MemberStatus))          return "This employee is already a user of the company.";
        if (c.InviteStatus == "I")                          return "This employee was already invited and has not answered yet.";
        if (c.InviteStatus == "P")                          return "This employee registered and is waiting for your approval. Open Registration Requests.";
        if (string.IsNullOrEmpty(c.Email))                  return "This employee has no email on record. Add the email to the employee's record first.";
        if (c.UserId <= 0)                                  return "No account uses this email yet. Ask the employee to register first, then send the invite.";
        if (c.LinkedUserId > 0 && c.LinkedUserId != c.UserId) return "This employee record is linked to another account.";

        List<string> different = c.EmpmasId > 0 ? PisEmpmasLink.Discrepancies(c.OldPis(), c.NewPis()) : [];
        if (different.Count > 0)                            return PisEmpmasLink.DiscrepancyMessage(different, "this");
        return null;
    }

    private async Task LoadLookups()
    {
        ViewData["Country"]  = await _setup.Countries();
        ViewData["Currency"] = await _setup.Currencies();
    }
}
