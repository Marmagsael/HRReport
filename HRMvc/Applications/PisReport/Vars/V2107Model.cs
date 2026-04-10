namespace HRMvc.Applications.PisReport.Vars
{
    public class V2107Model
    {
        public string Action { get; set; } = "";
        public bool IsGroupByClient { get; set; } = false;
        public List<R2107Model> RepDtls { get; set; } = [];
    }


    public class R2107Model
    {
        public string?      EmpNumber       { get; set; } = "";
        public string?      EmpName         { get; set; } = "";
        public DateTime?    DateHired       { get; set; } 
        public DateTime?    regref          { get; set; }
        public string?      StatusName     { get; set; } = "";
        public string?      ClName          { get; set; } = "";

        //------------------------------------
        public string? EmpFirstNm { get; set; }
        public string? EmpLastNm { get; set; }

    }
}
