using HRApiLibrary.Models._20_Pay;


namespace HRMvc.Applications._03Payroll.Views.BlazorPages._05_Management._0503.Items; 

public class V503
{
    public      PayrollgrpModel?            PayrollGroup    { get; set; } = new();
    public      List<PayrollgrpModel?>?     PayrollGroups   { get; set; } = new();

    public      CoaModel                    Coa             { get; set; } = new();  
    public      List<CoaModel?>?            Coas            { get; set; } = new();  

}
