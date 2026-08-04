using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;
namespace HRMvc.Applications.PayrollTransaction.Vars;

public class V102Model
{

    public string   pisdb               { get; set; } = string.Empty; 
    public string   paydb               { get; set; } = string.Empty; 
    public string   conn                { get; set; } = string.Empty;
    public string   Action              { get; set; } = string.Empty;
    public string   SelectedEmpnumber   { get; set; } = string.Empty;


    public List<PayrollgrpModel?>?          Payrollgrps            { get; set; } = []; 
    public List<EmpmasInternalModel?>?      EmpmasList             { get; set; } = [];


    public int                              SelectedPayrollGroup    { get; set; } = 0; 
    public List<EmpmasInternalModel>        SelectedEmployees       { get; set; } = []; 
    public string                           SelectedEmployee        { get; set; } = string.Empty; 
    public AtttemplateModel                 CurrentAttendance       { get; set; } = new(); 

}

