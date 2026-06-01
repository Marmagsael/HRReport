using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using FastReport;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRMvc.Applications.Vars;
using HRMvc.Views.BlazorPages._01MyProfile;
using HRMvc.Views.BlazorPages._01MyProfile.EmpmasEmploymentDtl;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace HRMvc.StartupConfig.Library;

public class L12_103
{
    private readonly IOEmpmasDataAccess _OEmpmas;
    private readonly IOGenderDataAccess _ogender;
    private readonly IOCivstatDataAccess _ocivstat;

    private readonly IOFamilyDataAccess _ofamily;
    private readonly IOParentDataAccess _oparent;
    private readonly IOChildrenDataAccess _ochildren;
    private readonly IOEmergencDataAccess _oemergenc;

    private readonly IOEducateDataAccess _oeducate;
    private readonly IOEmployDataAccess _oemploy;
    private readonly IOReferDataAccess _orefer;
    private readonly IOTrainDataAccess _otrain;

    private readonly IOProcodeDataAccess _oprocode;
    private readonly IOMlacodeDataAccess _omlacode;


    private readonly IEmpmasInternalDataAccess _iempmas;

    


    public L12_103(IOEmpmasDataAccess OEmpmas, IOCivstatDataAccess ocivstat, IOGenderDataAccess ogender, 
                  IOFamilyDataAccess ofamily, IOParentDataAccess oparent, IOChildrenDataAccess ochildren, IOEmergencDataAccess oemergenc,
                  IOEducateDataAccess oeducate, IOEmployDataAccess oemploy, IOReferDataAccess orefer, IOTrainDataAccess otrain,
                  IOProcodeDataAccess oprocode, IOMlacodeDataAccess omlacode, IEmpmasInternalDataAccess iempmas
                )
    {
        _OEmpmas         = OEmpmas;
        _ogender         = ogender;
        _ocivstat        = ocivstat;

        _oeducate        = oeducate;
        _ofamily         = ofamily;
        _oparent         = oparent;
        _ochildren       = ochildren;
        _oemergenc       = oemergenc;

        _oemploy         = oemploy;
        _orefer          = orefer;
        _otrain          = otrain;

        _oprocode        = oprocode;
        _omlacode        = omlacode;
        _iempmas         = iempmas;

    }


    public async Task<V12_103> V12_103(UserClaimsModel uc)
    {
        V12_103 v12_103 = new();

        var schema = uc.OpisDb;
        var conn = uc.Conn;
        var empno = uc.OempNumber;

        v12_103.OCivstats = await _ocivstat._02(schema, conn);
        v12_103.OGenders = await _ogender._02(schema, conn);
        v12_103.OProcode = await _oprocode._02(schema, conn);
        v12_103.OMlaCode = await _omlacode._02(schema, conn);

        
        var empmass = await _OEmpmas._02(empno, schema, conn);
        var parents = await _oparent._02(empno, schema, conn);

        if (empmass.Any()) empmass?.ForEach(p => { if (p != null) p.Age = ComputeAgeByDOB(p.EmpBirth).ToString(); });
        if (parents.Any()) parents?.ForEach(p => { if (p != null) p.Age = ComputeAgeByDOB(p.DoB); });

        v12_103.OEmpmass    = empmass;
        v12_103.OFamilys    = await _ofamily._02(empno, schema, conn);
        v12_103.OParents    = parents;
        v12_103.OChildrens  = await _ochildren._02(empno, schema, conn);
        v12_103.OEmergencs  = await _oemergenc._02(empno, schema, conn);
        v12_103.OEductates  = await _oeducate._02(empno, schema, conn);
        v12_103.OEmploys    = await _oemploy._02(empno, schema, conn);
        v12_103.ORefers     = await _orefer._02(empno, schema, conn);
        v12_103.OTrains     = await _otrain._02(empno, schema, conn);

        
        return v12_103;
    }

   
    private int ComputeAgeByDOB(DateTime? dob)
    {
        if (dob == null) return 0;

        var age = DateTime.Today.Year - dob.Value.Year;
        if (dob.Value.Date > DateTime.Today.AddYears(-age)) age--;

        return age;
    }
}
