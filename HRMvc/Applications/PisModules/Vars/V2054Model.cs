using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2054Model
    {

        public string?      OPisdb                          { get; set; } = string.Empty;
        public string?      Maindb                          { get; set; } = string.Empty;
        public string?      Mainpisdb                       { get; set; } = string.Empty;
        public string?      Conn                            { get; set; } = string.Empty;



        public string?      Action                          { get; set; } = string.Empty;
        public bool         IsLoading                       { get; set; } = true;
        public bool?        UcLoaded                        { get; set; } = false;


        public string?      ModalCaption                    { get; set; } = string.Empty;
        public bool         ShowInsuranceEntryModal         { get; set; } = false;
        public bool         ShowInsuranceAssignmentModal    { get; set; } = false;



        public OInsuranceModel?           Insurance                 { get; set; } = new();
        public OInsuranceModel?           InsuranceOriginalState     { get; set; } = new();
        public List<OInsuranceModel?>?    Insurances                { get; set; } = new();
        public List<OEmpmasModel?>?       OEmpmass                  { get; set; } = new();

        public string? SelectedPolicyNo                             { get; set; } = string.Empty;

        public List<string> InsuranceTypes                          { get; set; } = new()
        {
            "Life",
            "Accident"
        };

        //Error Message ----------------------------
        public string TypeErrorMsg { get; set; } = string.Empty;
        public string InsuranceDescErrorMsg { get; set; } = string.Empty;
        public string PolicyNoErrorMsg { get; set; } = string.Empty;
        public string FaceValueErrorMsg { get; set; } = string.Empty;
        public string PremiumErrorMsg { get; set; } = string.Empty;
        public string DateExNoErrorMsg { get; set; } = string.Empty;

    }


    // Row of the Insurance Listing report (Reports/_01Pis/J_2054_IL.trdp)
    public class R2054ILModel
    {
        public string?   InsuranceType  { get; set; } = "";
        public string?   InsuranceName  { get; set; } = "";
        public string?   PolicyNo       { get; set; } = "";
        public double?   FaceValue      { get; set; } = 0;
        public double?   Premiums       { get; set; } = 0;
        public DateTime? InsExpire      { get; set; }
        public byte[]?   Logo           { get; set; } = null;
    }


    // Row of the Insurance Assignment report (Reports/_01Pis/J_2054_IA.trdp)
    public class R2054IAModel
    {
        public string?   InsuranceType  { get; set; } = "";
        public string?   InsuranceName  { get; set; } = "";
        public string?   PolicyNo       { get; set; } = "";
        public double?   FaceValue      { get; set; } = 0;
        public double?   Premiums       { get; set; } = 0;
        public DateTime? InsExpire      { get; set; }
        public string?   EmpNumber      { get; set; } = "";
        public string?   EmpName        { get; set; } = "";
        public string?   ClName         { get; set; } = "";
        public string?   EmpStatus      { get; set; } = "";
        public byte[]?   Logo           { get; set; } = null;
    }
}
