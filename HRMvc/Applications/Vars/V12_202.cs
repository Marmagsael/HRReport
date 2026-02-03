using System;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_PayGeneric;
using HRApiLibrary.Models._90_Utils;

namespace HRMvc.Applications.Vars;

public class V12_202
{
    public string                   TRN             { get; set; } = string.Empty;
    public string                   SSS             { get; set; } = string.Empty;
    public string                   TIN             { get; set; } = string.Empty;  
    public string                   Payrollgrp      { get; set; } = string.Empty;  
    public string                   AttCoverage     { get; set; } = string.Empty;  
    public double                   Rate            { get; set; } = 0.0;  
    public UserClaimsModel          UserClaims      { get; set; } = new(); 
    public List<GTbltranModel?>?    Tbltrans        { get; set; } = [];
    public List<GTbltrandtlModel?>? Tbltrandtls     { get; set; } = [];
    public GPaymainhdrModel?        Paymainhdr      { get; set; } = new();
    public List<GPaymainhdrModel?>? Paymainhdrs     { get; set; } = [];
    public List<OempmasModel?>?     Oempmas         { get; set; } = [];   
    public int                      Yr              { get; set; } = DateTime.Now.Year;
    public int                      Mo              { get; set; } = DateTime.Now.Month;   
    public int                      Prd             { get; set; } = 1;       
}