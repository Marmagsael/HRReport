namespace HRMvc.Applications.PisReport.Vars
{
    public class V2108Model
    {
        public string? Action { get; set; } = "";
        public bool IsGroupByClient { get; set; } = false;
        public List<R2108Model> RepDtls { get; set; } = [];
    }


    public class R2108Model
    {
        public string?      EmpNumber { get; set; } = "";
        public string?      EmpName         { get; set; } = "";
        public string?      PositionName    { get; set; } = "";
        public DateTime?    DateHired       { get; set; } 
        public DateTime?    Separate        { get; set; }
        public string?      Remarks         { get; set; } = "";

        //------------------------------------
        public string? EmpFirstNm { get; set; }
        public string? EmpLastNm { get; set; }

    }
}
