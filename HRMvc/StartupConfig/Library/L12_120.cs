using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.StartupConfig.Library;

public class L12_120
{
    private readonly IOEmpmasDataAccess _oempmas;
    private readonly IHttpContextAccessor _http;

    public L12_120(IOEmpmasDataAccess oempmas, IHttpContextAccessor http)
    {
        _oempmas = oempmas;
        _http = http;
    }


    public async Task<List<OempmasModel?>> Oempmass(UserClaimsModel uc)
    {
        List<OempmasModel?>? oEmpmass = [];
        // var pisdb = _http.HttpContext?.Session.GetString("OldPis");
        // var paydb = _http.HttpContext?.Session.GetString("OldPay");
        // var conn = uc.Conn;
        // var empnumber = _http.HttpContext?.Session.GetString("EmpNumber");

        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber,pisdb, conn);
        
        return oEmpmass;
    }
}