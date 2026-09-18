using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2063Model
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



          

        public OCivstatModel? CivilStatus                   { get; set; } = new();
        public List<OCivstatModel?>? CivilStatuses          { get; set; } = new();

        public IEnumerable<OCivstatModel> RepDtls           { get; set; } = new List<OCivstatModel>();
        public OCivstatModel? CivsvOriginalState            { get; set; } = null;

        //Error Message ----------------------------
        public string CodeErrorMsg { get; set; } = string.Empty;
        public string DescriptionErrorMsg { get; set; } = string.Empty;
    }
}
