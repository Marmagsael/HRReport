using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2054Model
    {

        public string? OPisdb       { get; set; } = string.Empty;
        public string? Maindb       { get; set; } = string.Empty;
        public string? Mainpisdb    { get; set; } = string.Empty;
        public string? Conn         { get; set; } = string.Empty;

        
        public bool ShowUserList    { get; set; } = false;

        public OEmpmasModel?            Empmas  { get; set; } = new();
        public List<OEmpmasModel?>?     Empmass { get; set; } = new();

    }
}
