using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2004Model
    {

        public string   pisdb               { get; set; } = string.Empty;
        public string   paydb               { get; set; } = string.Empty;
        public string   conn                { get; set; } = string.Empty;
        public string   Action              { get; set; } = string.Empty;
        public bool     IsLoading            { get; set; } = true;


        public int SelectedPayrollGroup     { get; set; } = 0;
        public string SelectedEmployee      { get; set; } = string.Empty;



        public List<PayrollgrpModel?>?   PayrollGrps                 { get; set; } = [];
        public List<OEmpmasModel?>?      EmpmasList                  { get; set; } = [];
        public List<OEmpmasModel?>?       SelectedAssignedEmployees   { get; set; } = new();
        public List<OEmpmasModel?>?       AssignedEmployees           { get; set; } = new();




        public AtttemplateModel         CurrentAttendance           { get; set; } = new();




    }
}


