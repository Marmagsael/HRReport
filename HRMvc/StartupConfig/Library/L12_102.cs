using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;
using HRMvc.Applications.Vars;
using HRMvc.Views.BlazorPages._01MyProfile;

namespace HRMvc.StartupConfig.Library;

public class L12_102
{
    private readonly IOEmpmasDataAccess _oempmas;
    private readonly IAtttemplateDataAccess _attTemplate; 
    private readonly IAttpunches1DataAccess _attpunches1;

    public L12_102(IOEmpmasDataAccess oempmas, 
                   IAtttemplateDataAccess attTemplate, 
                   IAttpunches1DataAccess attpunches1)
    {
        _oempmas        = oempmas;
        _attTemplate    = attTemplate;
        _attpunches1     = attpunches1;
    }

    public async Task<List<OempmasModel?>?> _02Oempmass(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber??"",pisdb??"", conn??"");
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

        // --- Punches  -----------------------------//
            //--- Get Last 7 Days Punches -----------------//
            var Attpunches1_7days = await _attpunches1._02LastPunches(empmasid, 7, pisdb, conn); 

            //--- Get Puches without Out -----------------//
            var Attpunches1_Wo_Out = await _attpunches1._02NoPunchOut(empmasid, pisdb, conn); 

        v12_102.Attpunches1_7days   = Attpunches1_7days;
        v12_102.Attpunches1_Wo_Out  = Attpunches1_Wo_Out;
        
        //--- Set Current Punch as the Latest Punch in the Last 7 Days ---//
        if(v12_102.Attpunches1_7days.Count < 1) 
        {
            v12_102.Attpunches1_Current = new();
            v12_102.Attpunches1_Current.Status = "-";
        }

        else    
        {
            v12_102.Attpunches1_Current = v12_102.Attpunches1_7days.FirstOrDefault();
            
            if(v12_102.Attpunches1_Current.Status == "L")   
            {
                v12_102.Attpunches1_Current = new();
                v12_102.Attpunches1_Current.EmpmasId    = empmasid;
                v12_102.Attpunches1_Current.PunchInDate = DateTime.Now.Date;
                v12_102.Attpunches1_Current.Status = "-";
            }
        }
        //var ac = v12_102.Attpunches1_Current;    
        //Console.WriteLine($"PunchInDate : {ac.PunchInDate}"); 




        return v12_102;
    }
    
}
