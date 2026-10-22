using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2405Model
    {
        public string?  Action              { get; set; } = string.Empty;
        public bool     IsLoading           { get; set; } = false;
        public string?  ModalCaption        { get; set; } = string.Empty;
        public bool?    IsNewEntry          { get; set; } = false;
    }
}
