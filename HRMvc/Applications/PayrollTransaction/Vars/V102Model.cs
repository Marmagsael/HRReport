using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications.PayrollTransaction.Vars;

public class V102Model
{
    public string           pisdb           { get; set; } = string.Empty;
    public string           paydb           { get; set; } = string.Empty;
    public string           conn            { get; set; } = string.Empty;
    public string           Action          { get; set; } = string.Empty;
    public bool             ShowDE          { get; set; } = false;
    public string           ErrorMsg        { get; set; } = string.Empty;
    public bool             ShowAddMember   { get; set; } = false;
    public bool             IsAllSelected   { get; set; } = false;




    public List<PayrollgrpModel?>?      PayrollGrps             { get; set; } = [];
    public List<PayrateModel?>?         PayRates                { get; set; } = [];
    public PayrollgrpModel?             PayrollGrp              { get; set; } = new PayrollgrpModel();
    public List<EmpmasInternalModel?>?  EmpmasList              { get; set; } = [];
    public List<DeprecModel?>?          Deprecs                 { get; set; } = [];

    public int                          SelectedPayrollGroup    { get; set; } = 0;
    public List<EmpmasInternalModel?>?  SelectedEmployees       { get; set; } = [];
    public string                       SelectedEmployee        { get; set; } = string.Empty;


    public AtttemplateModel             CurrentAttendance       { get; set; } = new();




    public List<PayrollGroupStatusModel>           PayrollGroupStatus { get; set; } = new List<PayrollGroupStatusModel>
    {
        new PayrollGroupStatusModel { Name = "Active", Code = "A" },
        new PayrollGroupStatusModel { Name = "Inactive", Code = "I" }
    };
}

public class PayrollGroupStatusModel
{
    public string Name { get; set; }
    public string Code { get; set; }
}