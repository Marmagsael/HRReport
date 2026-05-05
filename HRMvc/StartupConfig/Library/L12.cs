using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_PayGeneric;
using HRMvc.Applications.Vars;

namespace HRMvc.StartupConfig.Library;

public class L12
{
    private readonly IOtbltranDataAccess _otbltran;
    private readonly IOEmpmasDataAccess _OEmpmas;

    public L12(IOtbltranDataAccess otbltran, IOEmpmasDataAccess OEmpmas)
    {
        _otbltran   = otbltran;
        _OEmpmas    = OEmpmas;
    }
    public async Task<List<OEmpmasModel?>?> _02OEmpmass(UserClaimsModel uc)
    {
        var empnumber   = uc.OempNumber; 
        var pisdb       = uc.OpisDb; 
        var conn        = uc.Conn;
        
        var empass = await _OEmpmas._02(empnumber,pisdb, conn);
        return empass; 
    }

    public async Task<List<GTbltranModel?>?>  _02Trans_ByEmpnumber(UserClaimsModel uc)
    {
        var tbltrans = await _otbltran._02Trns_ByEmpnumber(uc.OempNumber, uc.OpayDb, uc.Conn);
        return tbltrans;
    }
    
    public async Task<List<GTbltranModel?>?>  _02Trans_ByEmpnumber(string? trn, string? empnumber, UserClaimsModel uc)
    {
        var tbltrans = await _otbltran._02ByTrnAndEmpnumber(trn, empnumber, uc.OpayDb, uc.Conn);
        return tbltrans;
    }

    public async Task<V12_202?>? _12_202(UserClaimsModel uc)
    {
        var empnumber   = uc.OempNumber; 
        var pisdb       = uc.OpisDb; 
        var conn        = uc.Conn;


        V12_202? v      = new V12_202();
        v.UserClaims    = uc;
        v.OEmpmas       = await _OEmpmas._02(empnumber,pisdb, conn);        

        return v;
    }

}



