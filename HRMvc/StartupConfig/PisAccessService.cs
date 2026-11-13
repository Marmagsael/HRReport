using System.Security.Claims;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.Models._90_Utils;
using HRMvc.Applications.PayrollTransaction.Vars;

namespace HRMvc.StartupConfig;

public class PisAccessService
{
    protected virtual string Module => "pis";

    private readonly IOPisDomainaccessDataAccess _access;
    private readonly IOEmpmasDataAccess             _oldEmpmas;
    private readonly IMainmenuDataAccess            _menu;
    private HashSet<int>? _allowed;
    private bool _loaded;

    public PisAccessService(IOPisDomainaccessDataAccess access, IOEmpmasDataAccess oldEmpmas, IMainmenuDataAccess menu)
    {
        _access    = access;
        _oldEmpmas = oldEmpmas;
        _menu      = menu;
    }

    private static readonly string[] UnrestrictedEmails = ["marmagsael@gmail.com", "00003@email.com"];

    private static bool IsFirstEmployee(string? empNumber)
        => int.TryParse(empNumber?.Trim(), out var n) && n == 1;

    // -- null = no restriction; otherwise the MainPis.Menu ids checked for this user in 14/2002 --
    // -- No restriction : employee number 1 (00001 / 0001) and the emails in UnrestrictedEmails --
    public async Task<HashSet<int>?> AllowedMenuIdsAsync(ClaimsPrincipal user)
    {
        if (_loaded) return _allowed;

        var uc = PayClaimsHelper.UserToClaims(user);
        if (UnrestrictedEmails.Contains(uc.Email?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            || IsFirstEmployee(user.FindFirst("Empnumber")?.Value))
        {
            _loaded = true;
            return _allowed = null;
        }

        var allowed = new HashSet<int>();
        try
        {
            // -- The old PIS is the source of truth : the user's employee number is found by email, as at login --
            var empNumber = string.Empty;
            if (!string.IsNullOrWhiteSpace(uc.Email) && !string.IsNullOrWhiteSpace(uc.OpisDb))
            {
                var emp = await _oldEmpmas._02ByEmail(uc.Email, uc.OpisDb, uc.Conn ?? "");
                empNumber = emp?.FirstOrDefault()?.EmpNumber?.Trim() ?? string.Empty;
            }

            if (IsFirstEmployee(empNumber))
            {
                _loaded = true;
                return _allowed = null;
            }

            if (!string.IsNullOrEmpty(empNumber) && !string.IsNullOrWhiteSpace(uc.OpisDb))
            {
                foreach (var id in await LoadIdsAsync(empNumber, uc)) allowed.Add(id);
            }
        }
        catch
        {
            allowed.Clear();
        }

        _allowed = allowed;
        _loaded  = true;
        return _allowed;
    }

    protected virtual async Task<List<int>> LoadIdsAsync(string empNumber, HRApiLibrary.Models._00_Main.UserClaimsModel uc)
    {
        var rows = await _access._02ByUserName_Module(empNumber, Module, uc.OpisDb!, uc.Conn ?? "") ?? [];
        return rows.Where(r => r?.Idsysmenu != null).Select(r => r!.Idsysmenu!.Value).ToList();
    }

    // -- A menu tagged with a company type (Menu.CoTypeCode) is only for the companies of that type; 'ALL' is for every company --
    // -- A company without a type only gets the 'ALL' menus --
    private async Task<bool> IsForCompanyTypeAsync(ClaimsPrincipal user, int menuId)
    {
        var uc     = PayClaimsHelper.UserToClaims(user);
        var schema = Module == "pis" ? "MainPis" : "MainPay";
        var menus  = await _menu._02(schema, uc.CoTypeCode, uc.Conn ?? "") ?? [];
        var row    = (await _menu._02(schema, uc.Conn ?? "") ?? []).FirstOrDefault(m => m?.Id == menuId);
        if (row == null) return true;
        return menus.Any(m => m?.Id == menuId);
    }

    public async Task<bool> CanOpenAsync(ClaimsPrincipal user, int menuId)
    {
        if (!await IsForCompanyTypeAsync(user, menuId)) return false;
        var allowed = await AllowedMenuIdsAsync(user);
        return allowed == null || allowed.Contains(menuId);
    }

    // -- Every item stays in the menu; a link the user has no access to is marked "NoAccess" (shown grey, no link) --
    public static List<MainmenuModel?> Filter(List<MainmenuModel?> menus, HashSet<int>? allowed)
    {
        if (allowed == null) return menus;

        return menus.Select(m =>
        {
            if (m == null || m.Type == "Separator" || string.IsNullOrEmpty(m.Action)) return m;
            if (m.Id is int id && allowed.Contains(id)) return m;
            return new MainmenuModel { Id = m.Id, Type = "NoAccess", IdParent = m.IdParent, Indent = m.Indent, Icon = m.Icon, DispText = m.DispText, Action = m.Action, Odr = m.Odr, IsCommercialUse = m.IsCommercialUse };
        }).ToList();
    }
}
