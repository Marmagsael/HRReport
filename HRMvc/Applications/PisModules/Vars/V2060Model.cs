using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2060Model
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



        public OProcodeModel? ProvincialCode                { get; set; } = new();
        public List<OProcodeModel?>? ProvincialCodes        { get; set; } = new();


        public IEnumerable<OProcodeModel>       RepDtls                 { get; set; } = new List<OProcodeModel>();
        public OProcodeModel?                   ProvOriginalState       { get; set; } = null;

        //Error Message ----------------------------
        public string CodeErrorMsg                          { get; set; } = string.Empty;
        public string LocationErrorMsg                      { get; set; } = string.Empty;
    }
}
