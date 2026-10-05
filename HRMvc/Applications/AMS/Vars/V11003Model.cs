using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications.AMS.Vars;

public class V11003Model
{
    public int SelectedPayrollGroup { get; set; } = 0;
    public string SelectedEmployee { get; set; } = string.Empty;

    public List<PayrollgrpModel?>? PayrollGrps { get; set; } = [];
    public List<OEmpmasModel?>? EmpmasList { get; set; } = [];
    public List<OEmpmasModel?> SelectedAssignedEmployees { get; set; } = new();
    public List<OEmpmasModel?>? AssignedEmployees { get; set; } = new();

    public AtttemplateModel CurrentAttendance { get; set; } = new();

}
