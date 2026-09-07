using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2053Model
    {
        public string? OPisdb       { get; set; } = string.Empty;
        public string? Maindb       { get; set; } = string.Empty;
        public string? Mainpisdb    { get; set; } = string.Empty;
        public string? Conn         { get; set; } = string.Empty;

        public bool ShowClientForm  {  get; set; } = false;
        public List<OClientModel?>? Clients { get; set; } = [];
        public OClientModel?        Client  { get; set; } = new();
    }
}
