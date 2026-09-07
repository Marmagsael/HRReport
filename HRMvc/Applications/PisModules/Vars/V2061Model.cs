using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2061Model
    {

        public string? OPisdb                        { get; set; } = string.Empty;
        public string? Maindb                        { get; set; } = string.Empty;
        public string? Mainpisdb                     { get; set; } = string.Empty;
        public string? Conn                          { get; set; } = string.Empty;
         

        public string? ModalCaption                  { get; set; } = string.Empty;
        public bool ShowEntryModal                   { get; set; } = false;
          

        public OMlacodeModel? MlaCode               { get; set; } = new();
        public List<OMlacodeModel?>? MlaCodes       { get; set; } = new();
    }
}
