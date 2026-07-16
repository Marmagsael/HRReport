using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using System;

namespace HRMvc.Applications.Vars;

public class V12_103
{

    //Exclusive ==========================================================
    public List<OEmpmasModel?>?      OEmpmass            { get; set; } = [];
    public List<OCivstatModel?>?     OCivstats            { get; set; } = [];
    public List<OGenderModel?>?      OGenders             { get; set; } = [];
    public List<OFamilyModel?>?      OFamilys             { get; set; } = [];
    public List<OParentModel?>?      OParents             { get; set; } = [];
    public List<OChildrenModel?>?    OChildrens           { get; set; } = [];
    public List<OEmergencModel?>?    OEmergencs           { get; set; } = [];

   
    public List<OEducateModel?>?     OEductates           { get; set; } = [];
    public List<OEmployModel?>?      OEmploys             { get; set; } = [];
    public List<OReferModel?>?       ORefers              { get; set; } = [];
    public List<OTrainModel?>?       OTrains              { get; set; } = [];
    public List<OInsuranceaccidentModel?>?  OInsurances   { get; set; } = [];

    public List<OProcodeModel?>?     OProcode             { get; set; } = [];
    public List<OMlacodeModel?>?     OMlaCode             { get; set; } = [];


    //Non Exclusive ==========================================================
    public List<EmpmasInternalModel?>?              Empmass             { get; set; } = [];
    public List<EmpmasAddressModel?>?               Addresses           { get; set; } = [];
    public List<EmpmasCharRefModel?>?               CharRefs            { get; set; } = [];
    public List<EmpmasEducateModel?>?               Educates            { get; set; } = [];
    public List<EmpmasEducateRefModel?>?            EducateRefs         { get; set; } = [];
    public List<EmpmasEmergencyContactModel?>?      EmergencyContacts   { get; set; } = [];
    public List<EmpmasEmploymentModel?>?            Employments         { get; set; } = [];
    public List<EmpmasFamilyModel?>?                Familys             { get; set; } = [];
    public List<EmpmasFamilyRefModel?>?             FamilyRefs          { get; set; } = [];
    public List<EmpmasGovPhModel?>?                 GovsPh              { get; set; } = [];
    public List<EmpmasInsuranceModel?>?             Insurances          { get; set; } = [];
    public List<EmpmasPIModel?>?                    PIs                 { get; set; } = [];
    public List<EmpmasRelativesModel?>?             Relatives           { get; set; } = [];
    public List<EmpmasRelativesRefModel?>?          RelativesRefs       { get; set; } = [];
    public List<EmpmasSecLicModel?>?                SecLics             { get; set; } = [];
    public List<EmpmasTrainingModel?>?              Trainings           { get; set; } = [];
    public List<EmpmasClearancePhModel?>?           Clearances          { get; set; } = [];

}
