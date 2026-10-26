using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2403Model
    {
        public string?  Action              { get; set; } = string.Empty;
        public bool     IsLoading           { get; set; } = false;
        public string?  ModalCaption        { get; set; } = string.Empty;
        public bool?    IsNewEntry          { get; set; } = false;



       public bool?    ShowEntryModal      { get; set; } = false;



        public OEmpmasModel?            OEmpmas                { get; set; } = new();
        public List<OEmpmasModel?>?     OEmployees             { get; set; } = new();
        public List<OEmpstatModel?>?    OEmpStatuses           { get; set; } = new();
        public OEmpstatModel?           OEmpStatus             { get; set; } = new();
    }
}
