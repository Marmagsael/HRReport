using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;
using HRMvc.Applications.Vars;
using HRMvc.Views.BlazorPages._01MyProfile;

namespace HRMvc.StartupConfig.Library;

public class L12_102
{
    private readonly IOempmasDataAccess _oempmas;
    private readonly IAtttemplateDataAccess _attTemplate; 
    private readonly IAttpunchesDataAccess _attpunches;

    public L12_102(IOempmasDataAccess oempmas, 
                   IAtttemplateDataAccess attTemplate, 
                   IAttpunchesDataAccess attpunches)
    {
        _oempmas        = oempmas;
        _attTemplate    = attTemplate;
        _attpunches     = attpunches;
    }

    public async Task<List<OempmasModel?>?> _02Oempmass(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber,pisdb, conn);
        return empass; 
    }

    public async Task<V12_102?>? V12_102(UserClaimsModel uc)
    {
        V12_102? v12_102 = new V12_102();

        //--- _131MyAttendance Template -----------------------------// 
        var empmasid        = uc.UserId;
        var pisdb           = uc.SchemaUserPis; 
        var conn            = uc.Conn;
        var Atttemplates    = await _attTemplate._02s(empmasid, pisdb, conn);
        if(Atttemplates.Count < 1 )
        {
            var res = _attTemplate._02NoSchedule(empmasid);
            Atttemplates.Add(res); 
        }
        v12_102.Atttemplates = Atttemplates;

        // --- Previous Punches -----------------------------//
        //var Attdailys = await _attpunches._02._05_Last7Days(empmasid, pisdb, conn);



        return v12_102;
    }
    
}