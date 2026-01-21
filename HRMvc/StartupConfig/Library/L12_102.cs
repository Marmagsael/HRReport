using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.StartupConfig.Library;

public class L12_102
{
    private readonly IOempmasDataAccess _oempmas;

    public L12_102(IOempmasDataAccess oempmas)
    {
        _oempmas = oempmas;
    }

    public async Task<List<OempmasModel?>?> _02Oempmass(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber,pisdb, conn);
        return empass; 
    }
    
}