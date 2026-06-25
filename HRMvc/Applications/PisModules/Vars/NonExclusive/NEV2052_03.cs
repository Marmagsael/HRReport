using HRApiLibrary.Models._00_Main;

namespace HRMvc.Applications.PisModules.Vars.NonExclusive;
public class NEV2052_03
{
    public List<ProvinceStateModel?>?   Provinces       { get; set; } = new(); 
    public List<ProvinceStateModel?>?   ProvincesProv   { get; set; } = new(); 
    public List<CityModel?>?            Citys           { get; set; } = new(); 
    public List<CityModel?>?            CitysProv       { get; set; } = new(); 
    
}
