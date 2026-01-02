using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._06._0602;

public class V0602
{
    public string?                          ViewMode                    { get; set; } = "FixedEarnings"; // EmployeeEarnings, GroupEarnings 
    public int                              EmpmasId                    { get; set; } = 0;
    
    public EmpmasInternalModel              EmpmasInternal              { get; set; } = new(); 
    public CoaModel?                        Coa                         { get; set; } = new(); 
    public List<CoaModel?>?                 Coas                        { get; set; } = new(); 
    public FixedEarningsGrpModel?           FixedEarningsGrp            { get; set; } = new(); 
    public List<FixedEarningsGrpModel?>?    FixedEarningsGrps           { get; set; } = [];
    public FixedearningsModel?              Fixedearning                { get; set; } = new();
    public List<FixedearningsModel?>?       Fixedearnings               { get; set; } = [];
    
    public PeriodTmpModel?                  PeriodTmp                   { get; set; } = new();

    public string                           erorrMsgFe                  { get; set; } = string.Empty;

    public List<PayrollgrpModel?>?          Payrollgrps                 { get; set; }
    public Fixedearnings_grpModel?          FEGrp                       { get; set; } = new();
    public List<Fixedearnings_grpModel?>?   FEGrps                      { get; set; } = new();
    public List<EmpmasInternalModel?>?      Empmas_FEGrps               { get; set; } = new();
    public List<string?>                    errorFEGrp                  { get; set; } = [];

}