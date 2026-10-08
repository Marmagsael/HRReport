using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2058Model
    {

        public string?      OPisdb                          { get; set; } = string.Empty;
        public string?      Maindb                          { get; set; } = string.Empty;
        public string?      Mainpisdb                       { get; set; } = string.Empty;
        public string?      Conn                            { get; set; } = string.Empty;




        public string?      Action                          { get; set; } = string.Empty;
        public bool         IsLoading                       { get; set; } = true;
        public bool?        UcLoaded                        { get; set; } = false;

        public string?      ModalCaption                    { get; set; } = string.Empty;
        public bool         ShowEntryModal                  { get; set; } = false;
        public bool         IsEditEntry                     { get; set; } = false;


        public ODevdataModel?               Deviation                  { get; set; } = new();
        public List<ODevdataModel?>?        Deviations                 { get; set; } = new();

        public IEnumerable<ODevdataModel>   RepDtls                     { get; set; } = new List<ODevdataModel>();
        public ODevdataModel?               DeviationOriginalState      { get; set; } = null;


        //Error Message ----------------------------
        public string DevNoErrorMsg          { get; set; } = string.Empty;
        public string DevNameErrorMsg        { get; set; } = string.Empty;
    }
}
