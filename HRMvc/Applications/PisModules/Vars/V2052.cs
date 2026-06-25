using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using Radzen.Blazor;
using Radzen;
using HRApiLibrary.Models._20_Pay;

using System.Reflection;
using OfficeOpenXml.FormulaParsing.Utilities;
using System;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars;




public class V2052
{


    public string?  maindb            { get; set; } = string.Empty;
    public string?  pisdb            { get; set; } = string.Empty;
    public string?  newpisdb         { get; set; } = string.Empty;
    public string?  paydb            { get; set; } = string.Empty;
    public string?  conn             { get; set; } = string.Empty;
    public int?     defcoid          { get; set; } = 0;
    public int?     userid           { get; set; } = 0; 

    public bool                          ShowDataEntry          { get; set; } = false;
    public string?                       Action                 { get; set; } = string.Empty;
    public bool?                         IsNewEntry             { get; set; } = false;
    public string?                       ActionRef              { get; set; } = string.Empty;
    public int?                          TabNo                  { get; set; } = 1;
    public int?                          SelectedId             { get; set; } = 0;
    public string?                       SelectedEmpnumber      { get; set; } = string.Empty;
    public string?                       LastEmpNumber          { get; set; } = string.Empty;
    public bool?                         IsEmailValid           { get; set; } = true;
    public bool?                         IsEmpNoValid           { get; set; } = true;

    public OEmpmasModel?                   Empmas               { get; set; } = new();  
    public EmpmasAddressModel              Empmasaddress        { get; set; } = new();
    public DeprecModel?                    Deprec               { get; set; } = new();
    public OEducateModel?                  Education            { get; set; } = new();

    public List<OEmployModel?>?             Employments         { get; set; } = new();
    public List<OEducateModel?>?            Educations          { get; set; } = new();
    public List<EmpmasEducateRefModel?>?    EducationRefs       { get; set; } = new();
    public List<OFamilyModel?>?             Familys             { get; set; } = new();
    public List<OParentModel?>?             Parents             { get; set; } = new();
    public List<OChildrenModel?>?           Children            { get; set; } = new();
    public List<OEmergencModel?>?           EmergencyContacts   { get; set; } = new();
    public List<EmpmasRelativesRefModel?>?  RelativesRef        { get; set; } = new();
    public List<OTrainModel?>?              Trainings           { get; set; } = new();
    public List<OReferModel?>?              CharRefs            { get; set; } = new();
    public List<EmpmasInsuranceModel?>?     Insurances          { get; set; } = new();

    public List<OCivstatModel?>?            OCivStats           { get; set; } = new();
    public List<OGenderModel?>?             OGenders            { get; set; } = new();
    public List<CountryModel?>?             Countrys            { get; set; } = new();
    public List<OMlacodeModel?>?             MlaList             { get; set; } = new();
    public List<OProcodeModel?>?             ProList             { get; set; } = new();
    
    public OEmpmasModel?                    EmpmasToEdit        { get; set; } = new();

    public List<PisEmpmasModel?>?           Empmass             { get; set; } = new();
    public List<OEmpstatModel?>?            OEmpstats           { get; set; } = new();

    public List<OEmpmasModel?>?             EmpmasList          { get; set; } = new();
    public List<RdivisionModel?>?           Rdivisions          { get; set; } = new();
    public List<RdepartmentModel?>?         Rdepartments        { get; set; } = new();
    public List<RsectionModel?>?            Rsections           { get; set; } = new();
    public List<LeavegrpModel?>?            Leavegrps           { get; set; } = new();
    public List<PayrollgrpModel?>?          Payrollgprs         { get; set; } = new();
    public List<OPositionModel?>?           OPositions           { get; set; } = new();
    public List<EmploymenttypeModel?>?      Employmenttypes     { get; set; } = new();

    public List<EmpmovementModel?>?         EmpmovementList     { get; set; } = new();


    public RadzenDataGrid<OEmployModel?>?                   EmploymentGrid          = new();
    public RadzenDataGrid<EmpmasEducateModel?>?             EducationGrid           = new();
    public RadzenDataGrid<OFamilyModel?>?                   FamilyGrid              = new();
    public RadzenDataGrid<OParentModel?>?                   ParentGrid              = new();
    public RadzenDataGrid<OChildrenModel?>?                 ChildrenGrid            = new();
    public RadzenDataGrid<OEmergencModel?>?                 EmergencyContactGrid    = new();
    public RadzenDataGrid<EmpmasRelativesModel?>?           RelativesGrid           = new();
    public RadzenDataGrid<OTrainModel?>?                    TrainingGrid            = new();
    public RadzenDataGrid<OReferModel?>?                    ReferenceGrid           = new();
    public RadzenDataGrid<EmpmasInsuranceModel?>?           InsuranceGrid           = new();


    public OEmpmasModel EmpmasToEditMapper(OEmpmasModel source, OEmpmasModel destination)
    {
        var d = destination;
        var e = source; 
        if (source != null)
        {
            d = new()
                {
                    UserId          = e.UserId,
                    EmpmasId        = e.EmpmasId,
                    EmpNumber       = e.EmpNumber,
                    EmpLastNm       = e.EmpLastNm,
                    EmpFirstNm      = e.EmpFirstNm,
                    EmpMidNm        = e.EmpMidNm,
                    Suffix          = e.Suffix,
                    EmpAlias        = e.EmpAlias,
                    Fullname        = e.Fullname,
                    Email           = e.Email,
                };
        }

        return d; 

    }

    public EmpmasModel OldEmpmasToNewEmpmasMapper(OEmpmasModel source, EmpmasModel destination)
    {
        var d = destination;
        var e = source;
        if (source != null)
        {
            d = new()
            {
                SystemId    = e.UserId,
                EmpNumber   = e.EmpNumber,
                EmpLastNm   = e.EmpLastNm,
                EmpFirstNm  = e.EmpFirstNm,
                EmpMidNm    = e.EmpMidNm,
                Suffix      = e.Suffix,
                EmpAlias    = e.EmpAlias,
            };
        }

        return d;

    }

    public DeprecModel      DeprecToEditMapper(DeprecModel source, DeprecModel dest)
    {
        var deprec = dest; 
        if(source != null )
        {
            deprec = new()
            {
                EmpmasId                = source.EmpmasId, 
                Divid                   = source.Divid, 
                Depid                   = source.Depid, 
                Secid                   = source.Secid, 
                Leavegrpid              = source.Leavegrpid, 
                Payrollgrpid            = source.Payrollgrpid, 
                Positionid              = source.Positionid, 
                Employmenttypeid        = source.Employmenttypeid, 
                Empstatusid             = source.Empstatusid, 
                Dhired                  = source.Dhired, 
                Dregularization         = source.Dregularization, 
                Dtraineestart           = source.Dtraineestart, 
                Dtraineeend             = source.Dtraineeend, 
                Dcontractualstart       = source.Dcontractualstart, 
                Dcontractualend         = source.Dcontractualend, 
                Dprobationarystart      = source.Dprobationarystart, 
                Dprobationaryend        = source.Dprobationaryend, 
                Dregularizationstart    = source.Dregularizationstart, 
                Dregularizationend      = source.Dregularizationend, 
                Dpermanentstart         = source.Dpermanentstart, 
                Dresigned               = source.Dresigned, 
                Dterminated             = source.Dterminated, 
                Dseparated              = source.Dseparated, 
                Remarks                 = source.Remarks, 

                Divname                 = source.Divname, 
                Depname                 = source.Depname, 
                Secname                 = source.Secname, 
                Leavegrpname            = source.Leavegrpname, 
                Payrollgrpname          = source.Payrollgrpname, 
                Positionname            = source.Positionname, 
                Employmenttypename      = source.Employmenttypename, 
                Empstatusname           = source.Empstatusname 

            }; 
        }
        return dest; 
    }

    public Object ObjectMapper(Object src, Object des)
    {
        
        Type typeScr                    = src.GetType();
        PropertyInfo[] propertiesScr    = typeScr.GetProperties();
        
        Type typeDes                    = des.GetType();
        PropertyInfo[] propertiesDes    = typeDes.GetProperties();


        for (int i = 0; i < propertiesScr.Count(); i++)
        {                
            for (int j = 0; j < propertiesDes.Count(); j++)
            {
                if(propertiesScr[i].Name.Equals(propertiesDes[j].Name) ) 
                {
                    var valScr = propertiesScr[i].GetValue(src,null);
                    propertiesDes[j].SetValue(des, valScr, null);
                }
            }
        }


        return des;
    }

    public void PrintProperties(Object obj)
    {
        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();
        
        foreach (PropertyInfo property in properties)
        {
            var value = property.GetValue(obj, null);
        }
    }

}


