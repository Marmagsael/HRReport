using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;
using Radzen.Blazor;

namespace HRMvc.Applications._02HR._02Library;

public class V245
{
    public string?                      pisdb                   { get; set; } = string.Empty;
    public string?                      paydb                   { get; set; } = string.Empty;
    public string?                      conn                    { get; set; } = string.Empty;
    public string?                      Action                  { get; set; } = string.Empty;
    public string?                      Key                     { get; set; } = "Reinstatement";

    public bool                         ShowStatusSettingModal  { get; set; } = false;
    public bool                         IsCheckedAll            { get; set; } = false;


    public PisEmpmasModel               Empmas                  { get; set; } = new();
    public DeprecModel?                 Deprec                  { get; set; } = new();
    public RdepModel?                   Rdep                    { get; set; } = new();
    public List<RdepModel?>?            Rdeps                   { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApprovers           { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApproverDeps        { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApproverDivs        { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApproverDicips      { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApproverReinss      { get; set; } = new();
    public List<RdepapproverModel?>?    RDepApproverCEs         { get; set; } = new();
    public List<RdeploymentModel?>?     Rdeployment             { get; set; } = new();

    public List<DeploymodeModel?>?      Deploymodes             { get; set; } = new();
    public List<EmploymenttypeModel?>?  Employtypes             { get; set; } = new();
    public List<RdivisionModel?>?       Divs                    { get; set; } = new();
    public List<RdepartmentModel?>?     Depts                   { get; set; } = new();
    public List<RdepartmentModel?>?     DeptList                { get; set; } = new();
    public List<RsectionModel?>?        Secs                    { get; set; } = new();
    public List<RsectionModel?>?        SecList                 { get; set; } = new();
    public List<PositionModel?>?        Positions               { get; set; } = new();
    public List<DesignationModel?>?     Designations            { get; set; } = new();
    public List<RdeploymentModel?>?     Deployments             { get; set; } = new();
    public List<Rempstat_baseModel?>?   DepStatus               { get; set; } = new();
    public List<PayrollgrpModel?>?      Payrollgrp              { get; set; } = new();
    public UserCompanyModel?            Uc                      { get; set; } = new();


    public List<PisEmpmasModel?>?           EmpmasList                  { get; set; } = new();
    public List<PisEmpmasModel?>?           EmpmasListGrid              { get; set; } = new();
    public RadzenDataGrid<PisEmpmasModel?>? EmpmasGrid                  { get; set; }

    public bool                             InitDeployment              { get; set; } = false;
    public bool                             InitDeviation               { get; set; } = false;
    public bool                             InitDiciplinaryAction       { get; set; } = false;
    public bool                             InitReinstatement           { get; set; } = false;
    public bool                             InitChangeEmploymentType    { get; set; } = false;
    public bool                             ShowSearchModal             { get; set; } = false;



    public TranreinstatementModel           Tranreinstatement               { get; set; } = new() { };
    public TranreinstatementapprovalModel   Tranreinstatementapproval       { get; set; } = new() { };
    public List<EmpblockpostModel?>?        EmpblockpostList                { get; set; } = new() { };


    public bool                             SaveDisabled                { get; set; } = false;
    public bool                             PostDisabled                { get; set; } = false;
    public bool                             PrintDisabled               { get; set; } = false;
    public bool                             DateEndDisabled             { get; set; } = false;
    public bool                             ShowErrorMessage            { get; set; } = false;
    public string?                          Empnumber                   { get; set; } = string.Empty;


    public List<RempstatModel?>?            RempStatusList              { get; set; }
    public List<RempstatModel?>?            RempStatusListGrid          { get; set; }
    public RadzenDataGrid<RempstatModel?>?  RempStatusGrid              { get; set; }
    public List<Rempstat_baseModel?>?       DefaultStatusList           { get; set; }
    public EmptranmovementModel?            EmpTranmovement            { get; set; }
}
