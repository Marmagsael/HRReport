using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2055Model
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


        //Error Message ----------------------------
        public string CodeErrorMsg              { get; set; } = string.Empty;
        public string DescriptionErrorMsg       { get; set; } = string.Empty;






        public OPositionModel? Position                     { get; set; } = new();
        public List<OPositionModel>? Positions              { get; set; } = new();
        public IEnumerable<OPositionModel> RepDtls          { get; set; } = new List<OPositionModel>();

        public OPositionModel? PositionOriginalState        { get; set; } = null;



        public List<GuardOption> GuardOptions = new()
        {
            new GuardOption { Text = "No", Value = "0" },
            new GuardOption { Text = "Yes", Value = "1" }
        };

        public class GuardOption
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }


    }
}
