using System;
using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications.PisModules.Vars;

public class V2003_LvTypesModel
{
    public LeavetypeModel           LvType      { get; set; } = new(); 
    public List<LeavetypeModel>?    LvTypes     { get; set; } = []; 
    public string?                  ErrorMsg    { get; set; } = string.Empty; 


}
