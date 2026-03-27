using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class V2003_LvGrpModel
{
    public LeavegrpModel            Leavegrp    { get; set; } = new(); 
    public List<LeavegrpModel>?    Leavegrps { get; set; } = []; 

}
