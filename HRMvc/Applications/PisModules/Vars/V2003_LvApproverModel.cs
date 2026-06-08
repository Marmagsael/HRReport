using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class  V2003_LvApproverModel
{
    public LeaveapproverModel           LvApprover      { get; set; }
    public List<LeaveapproverModel>?    LvApprovers     { get; set; }
}
