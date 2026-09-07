using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
     public class V2062Model
    {

        public string? OPisdb                        { get; set; } = string.Empty;
        public string? Maindb                        { get; set; } = string.Empty;
        public string? Mainpisdb                     { get; set; } = string.Empty;
        public string? Conn                          { get; set; } = string.Empty;
         

        public string? ModalCaption                  { get; set; } = string.Empty;
        public bool ShowEntryModal                   { get; set; } = false;
          

        public OEmpstatModel? EmpStat               { get; set; } = new();
        public List<OEmpstatModel?>? EmpStats       { get; set; } = new();



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
