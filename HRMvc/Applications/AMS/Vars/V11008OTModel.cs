using HRApiLibrary.Models._11_AMS;
using HRApiLibrary.Modules._11003O;

namespace HRMvc.Applications.AMS.Vars; 

public class V11008OTModel
{
    public List<M11003_Payrollgrp>?         PayrollGrps             { get; set; } = new();
    public int?                             SelectedPayrollGrpId    { get; set; }
    public List<Ams_otsettingsModel?>       Employees               { get; set; } = new();
    public List<Ams_otsettingsModel?>       SelectedEmployees       { get; set; } = new();
    public (string Type, string Message)    Notification            { get; set; } = (string.Empty, string.Empty);
}
