using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRMvc.StartupConfig.Library;

public class L12
{
    private readonly IOtbltranDataAccess _otbltran;

    public L12(IOtbltranDataAccess otbltran)
    {
        _otbltran = otbltran;
    }

    public async Task<List<GTbltranModel?>?>  _02Trans_ByEmpnumber(UserClaimsModel uc)
    {
        var tbltrans = await _otbltran._02Trns_ByEmpnumber(uc.OempNumber, uc.OpayDb, uc.Conn);
        return tbltrans;
    }
    
    public async Task<List<GTbltranModel?>?>  _02Trans_ByEmpnumber(string trn, string empnumber, UserClaimsModel uc)
    {
        var tbltrans = await _otbltran._02ByTrnAndEmpnumber(trn, empnumber, uc.OpayDb, uc.Conn);
        return tbltrans;
    }

}



