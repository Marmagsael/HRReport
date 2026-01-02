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

namespace HRMvc.Applications._02HR._02Library;



public class V222
{


    public string?  pisdb            { get; set; } = string.Empty;
    public string?  paydb            { get; set; } = string.Empty;
    public string?  conn             { get; set; } = string.Empty;
    public int      defcoid          { get; set; } = 0;
    public int      userid           { get; set; } = 0; 

    public bool                             ShowDataEntry       { get; set; } = false;
    public string                           Action              { get; set; } = string.Empty;
    public string                           ActionRef           { get; set; } = string.Empty;
    public int                              TabNo               { get; set; } = 1;
    public int                              SelectedId          { get; set; } = 0;

    public List<Uc_accessreqModel?>?       UcAccessreq          { get; set; } = new();
    public PisEmpmasModel?                  Empmas              { get; set; } = new();  
    public EmpmasAddressModel               Empmasaddress       { get; set; } = new();
    public DeprecModel?                     Deprec              { get; set; } = new();
    public EmpmasEmploymentModel?           Employment          { get; set; } = new();
    public EmpmasEducateModel?              Education           { get; set; } = new();
    public EmpmasFamilyModel?               Family              { get; set; } = new();
    public List<EmpmasFamilyRefModel?>?     FamilyRef           { get; set; } = new();
    public EmpmasTrainingModel?             Training            { get; set; } = new();
    public EmpmasCharRefModel?              CharRef             { get; set; } = new();
    public EmpmasInsuranceModel?            Insurance           { get; set; } = new();
    public EmpmasGovPhModel?                EmpmasGovPh         { get; set; } = new();
    public EmpmasPIModel?                   EmpmasPI            { get; set; } = new(); 

    public List<EmpmasEmploymentModel?>?    Employments         { get; set; } = new();
    public List<EmpmasEducateModel?>?       Educations          { get; set; } = new();
    public List<EmpmasEducateRefModel?>?    EducationRefs          { get; set; } = new();
    public List<EmpmasFamilyModel?>?        Familys             { get; set; } = new();
    public List<EmpmasTrainingModel?>?      Trainings           { get; set; } = new();
    public List<EmpmasCharRefModel?>?       CharRefs            { get; set; } = new();
    public List<RCivStatModel?>?            RCivStats           { get; set; } = new();
    public List<CountryModel?>?             Countrys            { get; set; } = new();
    
    public EmpmasgrpModel                   Empmasgrp           { get; set; } = new();
    
    public PisEmpmasModel?                  EmpmasToEdit        { get; set; } = new();
    public DeprecModel?                     DeprecToEdit        { get; set; } = new();
    public EmpmasAddressModel               EmpmasaddressToEdit { get; set; } = new();
    public EmpmasPIModel?                   EmpmasPIToEdit      { get; set; } = new(); 

    public List<PisEmpmasModel?>?           Empmass             { get; set; } = new();
    public List<RempstatModel?>?            Rempstats           { get; set; } = new();

    public List<EmpmasModel?>?              EmpmasList          { get; set; } = new();
    public List<RdivisionModel?>?           Rdivisions          { get; set; } = new();
    public List<RdepartmentModel?>?         Rdepartments        { get; set; } = new();
    public List<RsectionModel?>?            Rsections           { get; set; } = new();
    public List<LeavegrpModel?>?            Leavegrps           { get; set; } = new();
    public List<PayrollgrpModel?>?          Payrollgprs         { get; set; } = new();
    public List<PositionModel?>?            Positions           { get; set; } = new();
    public List<EmploymenttypeModel?>?      Employmenttypes     { get; set; } = new();

    public RadzenDataGrid<PisEmpmasModel?>? EmpmassGrid         { get; set; } = new();
    public DataGridEditMode                 EditMode1           { get; set; } = DataGridEditMode.Single;

    public List<PisEmpmasModel?>?           EmpmassToInsert     { get; set; }= new();
    public List<PisEmpmasModel?>?           EmpmassToUpdate     { get; set; } = new();

    public List<EmpmovementModel?>?         EmpmovementList    { get; set; } = new();


    public RadzenDataGrid<EmpmasEmploymentModel?>? EmploymentGrid = new();
    public RadzenDataGrid<EmpmasEducateModel?>? EducationGrid = new();
    public RadzenDataGrid<EmpmasFamilyModel?>? FamilyGrid = new();


    public PisEmpmasModel   EmpmasToEditMapper(PisEmpmasModel source, PisEmpmasModel destination)
    {
        var d = destination;
        var e = source; 
        if (source != null)
        {
            d = new()
                {
                    Id              = e.Id,
                    SystemId        = e.SystemId,
                    EmpNumber       = e.EmpNumber,
                    EmpLastNm       = e.EmpLastNm,
                    EmpFirstNm      = e.EmpFirstNm,
                    EmpMidNm        = e.EmpMidNm,
                    Suffix          = e.Suffix,
                    EmpAlias        = e.EmpAlias,

                    Fullname        = e.Fullname,
                    Empmasname      = e.Empmasname,
                    Email           = e.Email,
                    UsersId         = e.UsersId,

                    Divid           = e.Divid,
                    Depid           = e.Depid,
                    Secid           = e.Secid,
                    Leavegrpid      = e.Leavegrpid,
                    Payrollgrpid    = e.Payrollgrpid,

                    Dept_           = e.Dept_,
                    Position_       = e.Position_,
                    Deployment_     = e.Deployment_,
                    PayrollGrp_     = e.PayrollGrp_,
                    EmpStat_        = e.EmpStat_,
                    SalaryGrade_    = e.SalaryGrade_
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
                Empmasid                = source.Empmasid, 
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


