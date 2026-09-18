using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2057Model
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



        public OPenaltyModel?           Penalty            { get; set; } = new();
        public List<OPenaltyModel?>?    Penalties          { get; set; } = new();



        public IEnumerable<OPenaltyModel> RepDtls           { get; set; } = new List<OPenaltyModel>();
        public OPenaltyModel? PenaltyvOriginalState         { get; set; } = null;

        //Error Message ----------------------------
        public string PenaltyNoErrorMsg                     { get; set; } = string.Empty;
        public string DescriptionErrorMsg                   { get; set; } = string.Empty;




        public List<YesNoOption> YesNoOptions = new()
        {
            new() { Text = "Yes",  Value = "1" },
            new() { Text = "No",   Value = "0" }
        };

        public class YesNoOption
        {
            public string Text { get; set; } = "";
            public string Value { get; set; } = "";
        }



    }
}
