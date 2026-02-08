using System.Security.Claims;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRMvc.StartupConfig;

public class UserClaimsContextService
{
    private readonly I_00MainDA _mainDA;
    private readonly SessionService _session;

    public UserClaimsContextService(I_00MainDA mainDA, SessionService session)
    {
        _mainDA = mainDA;
        _session = session;
    }

    public UserClaimsModel Build(ClaimsPrincipal user)
    {
        var model = _mainDA._02UserClaimsContent(user.Claims);

        model.OempNumber = _session.EmpNumber;
        model.OpayDb     = _session.OldPay;
        model.OpisDb     = _session.OldPis;

        return model;
    }
}