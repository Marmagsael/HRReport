using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2054Model
    {

        public string? OPisdb                       { get; set; } = string.Empty;
        public string? Maindb                       { get; set; } = string.Empty;
        public string? Mainpisdb                    { get; set; } = string.Empty;
        public string? Conn                         { get; set; } = string.Empty;
        
        
        public string? ModalCaption                 { get; set; } = string.Empty;
        public bool ShowInsuranceEntryModal                  { get; set; } = false;
        public bool ShowInsuranceAssignmentModal    { get; set; } = false;



        public InsuranceModel?           Insurance  { get; set; } = new();
        public List<InsuranceModel?>?    Insurances { get; set; } = new();

    }
}
