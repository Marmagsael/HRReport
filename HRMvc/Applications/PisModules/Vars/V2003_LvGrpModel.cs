using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class V2003_LvGrpModel
{
    public LeavegrpModel                    Leavegrp                { get; set; } = new(); 
    public List<LeavegrpModel>?             Leavegrps               { get; set; } = [];
    public List<LeavegrpapproverModel>?     Leavegrpapprovers       { get; set; } = [];
    public List<LeavegrpapproverModel>?     FirstApprovers          { get; set; } = [];
    public List<LeavegrpapproverModel>?     FinalApprovers          { get; set; } = [];
    
    

    public string?                  ErrorMsg            { get; set; } = string.Empty;
    public bool                     ShowDE              { get; set; } = false;
    public bool                     ShowApproverEntry   { get; set; } = false;



}
