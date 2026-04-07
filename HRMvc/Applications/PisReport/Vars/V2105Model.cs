namespace HRMvc.Applications.PisReport.Vars
{

    public class V2106Model
    {
        public string           Action              { get; set; } = "";
        public bool             IsGroupByClient     { get; set; } = false;
        public List<R2106Model> RepDtls             { get; set; } = [];
        public int              SelectedMonthsAgo   { get; set; } = 0;
        public int              LnMonths            { get; set; } = 0;

    }

    public class R2106Model
    {
        public string?      EmpNumber       { get; set; } = "";
        public string?      EmpName         { get; set; } = "";
        public DateTime?    DateHired       { get; set; } 
        public string?      EmpStatus       { get; set; } = "";
        public string?      ClName          { get; set; } = "";


        // Added --------------------------------------------------
        public string?      EmpLastNm       { get; set; } = "";
        public string?      EmpFirstNm      { get; set; } = "";
        public string?      Client_         { get; set; } = "";


    }
}
