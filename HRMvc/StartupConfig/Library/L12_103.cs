using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.StartupConfig.Library;

public class L12_103
{
    private readonly IOempmasDataAccess _oempmas;
    private readonly IOeducateDataAccess _oeducate;
    private readonly IOcivstatDataAccess _ocivstat;
    private readonly IOgenderDataAccess _ogender;
    private readonly IOfamilyDataAccess _ofamily;
    private readonly IOemergencDataAccess _oemergenc;

    public L12_103(IOempmasDataAccess oempmas, IOcivstatDataAccess ocivstat, IOgenderDataAccess ogender, IOeducateDataAccess oeducate,  IOfamilyDataAccess ofamily, IOemergencDataAccess oemergenc)
    {
        _oempmas         = oempmas;
        _oeducate        = oeducate;
        _ofamily         = ofamily;
        _oemergenc       = oemergenc;
        _ocivstat        = ocivstat;
        _ogender         = ogender;
    }

    public async Task<List<OempmasModel?>?> _02Oempmass(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber,pisdb, conn);
        return empass; 
    }

    public async Task<List<OeducateModel?>?> _02Oeducate(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber;
        var pisdb = uc.OpisDb;
        var conn = uc.Conn;

        var educate = await _oeducate._02(empnumber, pisdb, conn);
        return educate;
    }


    public async Task<List<OfamilyModel?>?> _02OFamily(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber;
        var pisdb = uc.OpisDb;
        var conn = uc.Conn;

        var family = await _ofamily._02(empnumber, pisdb, conn);
        return family;
    }

    public async Task<List<OemergencModel?>?> _02OEmergenc(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber;
        var pisdb = uc.OpisDb;
        var conn = uc.Conn;

        var emergenc = await _oemergenc._02(empnumber, pisdb, conn);
        return emergenc;
    }




    public async Task<List<OcivstatModel?>?> _02OCivstat(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber;
        var pisdb = uc.OpisDb;
        var conn = uc.Conn;

        var civstats = await _ocivstat._02( pisdb, conn);
        return civstats;
    }

    public async Task<List<OgenderModel?>?> _02OGender(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber;
        var pisdb = uc.OpisDb;
        var conn = uc.Conn;

        var genders = await _ogender._02(pisdb, conn);
        return genders;
    }




}