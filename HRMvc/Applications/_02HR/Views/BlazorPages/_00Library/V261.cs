using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications._02HR.Views.BlazorPages._00Library;

public class V261
{
    public InvModel?                  Inv           { get; set; } = new(); 
    public List<InvModel?>?           Invs          { get; set; } = [];

    public List<Inv_typeModel?>?          InvTypes          { get; set; } = []; 
    public List<Inv_categoryModel?>?      InvCategorys      { get; set; } = []; 
    public List<Inv_brandModel?>?         InvBrands         { get; set; } = []; 
    public List<Inv_makeModel?>?          InvMakes          { get; set; } = []; 
    
    
}