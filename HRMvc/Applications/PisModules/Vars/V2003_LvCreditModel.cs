using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class V2003_LvCreditModel
{
    public LeavecreditModel           LvCredit                  { get; set; } = new() ;
    public List<LeavecreditModel>?    LvApproversLvCredit       { get; set; } = []; 
    
}
