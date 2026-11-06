using System.Security.Claims;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._90_Utils;
using HRMvc.Applications.PayrollTransaction.Vars;

namespace HRMvc.StartupConfig;

public class PisAccessService
{
    private const string Module = "pis";

    private readonly IOPisDomainaccessDataAccess _access;
    private readonly IOEmpmasDataAccess             _oldEmpmas;
    private HashSet<int>? _allowed;
    private bool _loaded;

    public PisAccessService(IOPisDomainaccessDataAccess access, IOEmpmasDataAccess oldEmpmas)
    {
        _access    = access;
        _oldEmpmas = oldEmpmas;
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
                var rows = await _access._02ByUserName_Module(empNumber, Module, uc.OpisDb, uc.Conn ?? "") ?? [];
                foreach (var r in rows)
                    if (r?.Idsysmenu is int id) allowed.Add(id);
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

    public async Task<bool> CanOpenAsync(ClaimsPrincipal user, int menuId)
    {
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
            return new MainmenuModel { Id = m.Id, Type = "NoAccess", IdParent = m.IdParent, Indent = m.Indent, Icon = m.Icon, DispText = m.DispText, Action = m.Action, Odr = m.Odr };
        }).ToList();
    }
}
