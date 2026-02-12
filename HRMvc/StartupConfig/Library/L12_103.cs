using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRMvc.Applications.Vars;

namespace HRMvc.StartupConfig.Library;

public class L12_103
{
    private readonly IOEmpmasDataAccess _oempmas;
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


    public L12_103(IOEmpmasDataAccess oempmas, IOCivstatDataAccess ocivstat, IOGenderDataAccess ogender, 
                  IOFamilyDataAccess ofamily, IOParentDataAccess oparent, IOChildrenDataAccess ochildren, IOEmergencDataAccess oemergenc,
                  IOEducateDataAccess oeducate, IOEmployDataAccess oemploy, IOReferDataAccess orefer, IOTrainDataAccess otrain,
                  IOProcodeDataAccess oprocode, IOMlacodeDataAccess omlacode
                )
    {
        _oempmas         = oempmas;
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

    }

    public async Task<List<OempmasModel?>?> _02Oempmass(UserClaimsModel uc)
    {
        var empnumber = uc.OempNumber; 
        var pisdb = uc.OpisDb; 
        var conn = uc.Conn;
        
        var empass = await _oempmas._02(empnumber,pisdb, conn);
        return empass; 
    }

 

    public async Task<V12_103?>? V12_103(UserClaimsModel uc)
    {
        V12_103? v12_103 = new V12_103();

        var empnumber   = uc.OempNumber;
        var pisdb       = uc.OpisDb;
        var conn        = uc.Conn;

        var civstats        = await _ocivstat._02(pisdb, conn);
        var genders         = await _ogender._02(pisdb, conn);
        var family          = await _ofamily._02(empnumber, pisdb, conn);
        var parents         = await _oparent._02(empnumber, pisdb, conn);
        var childrens       = await _ochildren._02(empnumber, pisdb, conn);
        var emergencs       = await _oemergenc._02(empnumber, pisdb, conn);

        var educates        = await _oeducate._02(empnumber, pisdb, conn);
        var employs         = await _oemploy._02(empnumber, pisdb, conn);
        var refers          = await _orefer._02(empnumber, pisdb, conn);
        var trains          = await _otrain._02( empnumber, pisdb, conn);

        var procode          = await _oprocode._02( pisdb, conn);
        var mlacode          = await _omlacode._02( pisdb, conn);


        v12_103.OCivstats   = civstats;
        v12_103.OGenders    = genders;

        v12_103.OFamilys    = family;
        v12_103.OParents    = parents;
        v12_103.OChildrens  = childrens;

        v12_103.OEmergencs  = emergencs;

        v12_103.OEductates  = educates;
        v12_103.OEmploys    = employs;
        v12_103.ORefers     = refers;
        v12_103.OTrains     = trains;

        v12_103.OProcode    = procode;
        v12_103.OMlaCode    = mlacode;


        return v12_103;
    }


    }
