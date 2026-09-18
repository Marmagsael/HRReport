using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2053Model
    {
        public string? OPisdb               { get; set; } = string.Empty;
        public string? Maindb               { get; set; } = string.Empty;
        public string? Mainpisdb            { get; set; } = string.Empty;
        public string? Conn                 { get; set; } = string.Empty;

        public string?  Action              { get; set; } = string.Empty;
        public bool     IsLoading           { get; set; } = true;
        public bool?    UcLoaded            { get; set; } = false;


        public string?  ModalCaption        { get; set; } = string.Empty;
        public bool?    ShowEntryModal      { get; set; } = false;
        public bool     ShowDOLEReportModal { get; set; } = false;



        public List<OClientModel?>?         Clients             { get; set; } = [];
        public OClientModel?                Client              { get; set; } = new();

        public List<OClientstatusModel?>?   ClientStatus        { get; set; } = [];
        public List<OAreaModel?>?           Areas               { get; set; } = [];


        public IEnumerable<OClientModel>    RepDtls             { get; set; } = new List<OClientModel>();
        public IEnumerable<OClientModel>    SelectedDeployments { get; set; } = new List<OClientModel>();



        //Error Message ----------------------------
        public string DeploymentNameErrorMsg { get; set; } = string.Empty;
    }
}
