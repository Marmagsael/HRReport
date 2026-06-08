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

public class L12_103NonExlusive
{
    private readonly I_10_EmpmasDataAccess _empmas;
    private readonly IOGenderDataAccess _ogender;
    private readonly IOCivstatDataAccess _ocivstat;


    public L12_103NonExlusive(I_10_EmpmasDataAccess empmas, IOCivstatDataAccess ocivstat, IOGenderDataAccess ogender )
    {
        _empmas         = empmas;
        _ogender        = ogender;
        _ocivstat       = ocivstat;

    }


    public async Task<V12_103> V12_103(UserClaimsModel uc)
    {
        V12_103 v12_103             = new();
        var oldschema               = uc.OpisDb;
        var schema                  = uc.SchemaUserPis;
        var conn                    = uc.Conn;
        var empno                   = uc.OempNumber;
        var userId                  = uc.UserId;

        v12_103.OCivstats           = await _ocivstat._02(oldschema, conn);
        v12_103.OGenders            = await _ogender._02(oldschema, conn);


        v12_103.Empmass             = await _empmas._02BySystemId(userId, schema, conn);
        var empmasFirst             = v12_103.Empmass.FirstOrDefault() ?? new EmpmasInternalModel();

        v12_103.Addresses           = await _empmas._02EmpmasAddresss(empmasFirst.Id, schema, conn);
        var empmasAddFirst          = v12_103.Addresses.FirstOrDefault() ?? new EmpmasAddressModel();

        v12_103.PIs                 = await _empmas._02EmpmasPIs(empmasFirst.Id, schema, conn); 
        var empmasPiFirst           = v12_103.PIs.FirstOrDefault() ?? new EmpmasPIModel();

        if (v12_103.PIs.Any())  v12_103.PIs.ForEach(p => { if (p != null) p.Age = ComputeAgeByDOB(p.EmpBirth); });

        v12_103.Familys             = await _empmas._02EmpmasFamilyList(empmasFirst.Id, schema, conn);
        v12_103.Relatives           = await _empmas._02EmpmasRelativesList(empmasFirst.Id, schema, conn);
        v12_103.EmergencyContacts   = await _empmas._02EmpmasEmergencyContactList(empmasFirst.Id, schema, conn);
        v12_103.Educates            = await _empmas._02EmpmasEducateList(empmasFirst.Id, schema, conn);
        v12_103.Employments         = await _empmas._02EmpmasEmploymentList(empmasFirst.Id, schema, conn);
        v12_103.Trainings           = await _empmas._02EmpmasTrainingList(empmasFirst.Id, schema, conn);
        v12_103.GovsPh              = await _empmas._02EmpmasGovPhList(empmasFirst.Id, schema, conn);
        v12_103.Insurances          = await _empmas._02EmpmasInsuranceListByEmpmasId(empmasFirst.Id, schema, conn);
        v12_103.CharRefs            = await _empmas._02EmpmasCharRefList(empmasFirst.Id, schema, conn);
        var secLics                 = await _empmas._02EmpmasSecLicList(empmasFirst.Id, schema, conn);
        var secLicFirst             = secLics.FirstOrDefault() ?? new EmpmasSecLicModel();


        v12_103.OEmpmass = new List<OEmpmasModel?>()
        {
            new OEmpmasModel()
            {
                EmpNumber       = empmasFirst.EmpNumber,
                EmpLastNm       = empmasFirst.EmpLastNm,
                EmpFirstNm      = empmasFirst.EmpFirstNm,
                EmpMidNm        = empmasFirst.EmpMidNm,
                Sex_            = empmasPiFirst?.Sex_,
                EmpStatus       = empmasFirst.EmpStatus,
                Email           = empmasAddFirst.EmailAdd,
                DateHired       = empmasFirst.DateHired,
                PositionName    = empmasFirst.PositionName,
            }
        };




        v12_103.SecLics = new List<EmpmasSecLicModel?>()
        {
            new EmpmasSecLicModel()
            {
                SecLicense  = secLicFirst.SecLicense,
                LicExpire   = secLicFirst.LicExpire,
                BadgeNo     = secLicFirst.BadgeNo,
                SbrNo       = secLicFirst.SbrNo,
                OpNo        = secLicFirst.OpNo,
                Validated   = secLicFirst.Validated,
                VFee        = secLicFirst.VFee,
                Revalidated = secLicFirst.Revalidated,
                ValStatus   = secLicFirst.ValStatus,
                EmpStatus   = empmasFirst.EmpStatus,
                PositionName= empmasFirst.PositionName,
                DateHired   = empmasFirst.DateHired,
                RegRef      = empmasFirst.Regref,
                Separate    = empmasFirst.Separate,


            }
        };



       





        return v12_103;
    }

    private int? ComputeAgeByDOB(DateTime? dob)
    {
        if (dob == null) return 0;

        var age = DateTime.Today.Year - dob.Value.Year;
        if (dob.Value.Date > DateTime.Today.AddYears(-age)) age--;

        return age;
    }
}
