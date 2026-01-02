using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._05_Management._0500;

public class V0505
{
    public string?                      Action          { get; set; } = string.Empty; 
    
    public List<PayrollgrpModel?>?      Payrollgrps     { get; set; } = [];
    public PayrollgrpModel?             Payrollgrp      { get; set; } = new();
    public List<EmpratesModel?>?        Emprates        { get; set; } = []; 
    public EmpratesModel?               Emprate         { get; set; } = new(); 
    
    public List<PayrateModel?>?        Payrates        { get; set; } = []; 

    
    
    
    
    
    public List<RdeploymentModel>      Deployments    { get; set; } = []; 
    public RdeploymentModel?           Deployment     { get; set; } = new(); 
    
}