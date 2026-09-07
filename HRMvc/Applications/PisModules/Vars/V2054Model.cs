using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2054Model
    {

        public string?      OPisdb                          { get; set; } = string.Empty;
        public string?      Maindb                          { get; set; } = string.Empty;
        public string?      Mainpisdb                       { get; set; } = string.Empty;
        public string?      Conn                            { get; set; } = string.Empty;
        
        
        public string?      ModalCaption                    { get; set; } = string.Empty;
        public bool         ShowInsuranceEntryModal         { get; set; } = false;
        public bool         ShowInsuranceAssignmentModal    { get; set; } = false;



        public OInsuranceModel?           Insurance         { get; set; } = new();
        public List<OInsuranceModel?>?    Insurances        { get; set; } = new();
        public List<OEmpmasModel?>?       OEmpmass          { get; set; } = new();

        public string? SelectedPolicyNo         { get; set; } = string.Empty;

        public List<string> InsuranceTypes      { get; set; } = new()
        {
            "Life",
            "Accident"
        };


        public List<string> Policies            { get; set; } = new()
        {
            "Policy 1",
            "Policy 2"
        };



    }
}
