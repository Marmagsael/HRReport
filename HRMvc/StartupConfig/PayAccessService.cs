using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.Models._00_Main;

namespace HRMvc.StartupConfig;

public class PayAccessService : PisAccessService
{
    private readonly IODomainaccessDataAccess _payAccess;

    public PayAccessService(IOPisDomainaccessDataAccess access, IOEmpmasDataAccess oldEmpmas, IODomainaccessDataAccess payAccess) : base(access, oldEmpmas)
    {
        _payAccess = payAccess;
    }

    protected override string Module => "pay";

    protected override async Task<List<int>> LoadIdsAsync(string empNumber, UserClaimsModel uc)
    {
        if (string.IsNullOrWhiteSpace(uc.OpayDb)) return [];
        var rows = await _payAccess._02ByUserName_Module(empNumber, Module, uc.OpayDb, uc.Conn ?? "") ?? [];
        return rows.Where(r => r?.Idsysmenu != null).Select(r => r!.Idsysmenu!.Value).ToList();
    }
}
