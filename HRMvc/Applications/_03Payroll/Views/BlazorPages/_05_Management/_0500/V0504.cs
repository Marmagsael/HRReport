using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._05_Management._0500; 

public class V0504
{
    public PayrollgrpModel?                 Payrollgrp          { get; set; } = new();
    public List<PayrollgrpModel?>?          Payrollgrps         { get; set; } = new();
    public CoaModel?                        Coa                 { get; set; } = new(); 
    public List<CoaModel?>?                 Coas                { get; set; } = new(); 
    public EmpratesModel?                   Emprate             { get; set; } = new(); 
    public List<EmpratesModel?>?            Emprates            { get; set; } = new(); 
    public EmpratesdtlModel?                Empratedtl          { get; set; } = new(); 
    public List<EmpratesdtlModel?>?         Empratesdtls        { get; set; } = new(); 
    public PayrateModel?                    Payrate             { get; set; } = new(); 
    public List<PayrateModel?>?             Payrates            { get; set; } = new(); 
    public List<AtttypeModel?>?             Atttypes            { get; set; } = new(); 
    public List<AttdutytypeModel?>?         Attdutytypes        { get; set; } = new(); 
    public List<AttinModel?>?               Attins              { get; set; } = new(); 
    public List<AttdurationModel>           Attdurations        { get; set; } = new(); 
    public List<string?>?                   Dutypes             { get; set; } = new(); 
    public AtttemplateModel?                Atttemplate         { get; set; } = new(); 
    public SettingsModel?                   Setting             { get; set; } = new(); 
}
