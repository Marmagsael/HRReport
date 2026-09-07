using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2059Model
    {

        public string? OPisdb { get; set; } = string.Empty;
        public string? Maindb { get; set; } = string.Empty;
        public string? Mainpisdb { get; set; } = string.Empty;
        public string? Conn { get; set; } = string.Empty;


        public string? ModalCaption { get; set; } = string.Empty;
        public bool ShowEntryModal { get; set; } = false;


        public OAreaModel? Area { get; set; } = new();
        public List<OAreaModel?>? Areas { get; set; } = new();
    }
}
