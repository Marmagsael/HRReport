using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._06._0600;

public class V0603
{
    public EmpmasInternalModel?             EmpmasInternal      { get; set; } = new();
    public List<EmpmasInternalModel?>?      EmpamsInternals     { get; set; } = new();
    public List<string?>?                   ErorrMsgs           { get; set; } = new();
    public LoansModel                       Loan                { get; set; } = new(); 
    public List<LoansModel?>?               Loans               { get; set; } = new(); 
    public List<CoaModel?>?                 Coas                { get; set; } = new(); 


}
