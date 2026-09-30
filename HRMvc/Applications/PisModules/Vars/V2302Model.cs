using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2302Model
    {
        public string?      OPisdb                          { get; set; } = string.Empty;
        public string?      Maindb                          { get; set; } = string.Empty;
        public string?      Mainpisdb                       { get; set; } = string.Empty;
        public string?      Conn                            { get; set; } = string.Empty;




        public string?      Action                          { get; set; } = string.Empty;
        public bool         IsLoading                       { get; set; } = true;
        public bool?        UcLoaded                        { get; set; } = false;

        public string?      ModalCaption                    { get; set; } = string.Empty;
        public bool         ShowSearchModal                 { get; set; } = false;




        public OEmpmasModel? OEmpmas                        { get; set; } = new();
        public List<OEmpmasModel?>? OEmployees              { get; set; } = new();

    }
}
