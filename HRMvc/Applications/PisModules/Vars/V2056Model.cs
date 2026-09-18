using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2056Model
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


        public OLevtblModel? LeaveType                      { get; set; } = new();
        public List<OLevtblModel> LeaveTypes                { get; set; } = new();
        public IEnumerable<OLevtblModel> RepDtls            { get; set; } = new List<OLevtblModel>();
        public OLevtblModel? LeaveTypeOriginalState         { get; set; } = null;

        //Error Message ----------------------------
        public string CodeErrorMsg                          { get; set; } = string.Empty;
        public string DescriptionErrorMsg                   { get; set; } = string.Empty;



    }
}
