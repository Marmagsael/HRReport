namespace HRMvc.Applications.PisReport.Vars
{

    public class V2105Model
    {
        public string? Action { get; set; } = "";
        public List<R2105Model> RepDtls { get; set; } = [];
    }

    public class R2105Model
    {
        public string?      EmpNumber       { get; set; } = "";
        public string?      EmpName         { get; set; } = "";
        public DateTime?    DateHired       { get; set; } 
        public string?      PositionName    { get; set; } = "";
        public string?      SecLicense      { get; set; } = "";
        public DateTime?    LicExpire       { get; set; } 
        public string?      ClName          { get; set; } = "";
        public string?      EmpStatus       { get; set; } = "";


        // Added --------------------------------------------------
        public string?      EmpLastNm       { get; set; } = "";
        public string?      EmpFirstNm      { get; set; } = "";

    }
}
