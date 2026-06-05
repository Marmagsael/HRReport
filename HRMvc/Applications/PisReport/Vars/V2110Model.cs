namespace HRMvc.Applications.PisReport.Vars
{
    public class V2110Model
    {
        public string? Action { get; set; } = "";
        public bool IsGroupByClient { get; set; } = false;
        public List<R2110Model> RepDtls { get; set; } = [];
    }


    public class R2110Model
    {
        public string? EmpNumber    { get; set; } = "";
        public string? EmpLastNm    { get; set; } = "";
        public string? EmpFirstNm   { get; set; } = "";
        public string? EmpMidNm     { get; set; } = "";
        public string? EmpName      { get; set; } = "";
        public string? Insurance    { get; set; } = "";
        public string? PolicyNo     { get; set; } = "";
        public string? EmpStatus    { get; set; } = "";
        public Double? FaceValue    { get; set; } = 0.00;
        public Double? Premium      { get; set; } = 0.00;
        public DateTime? InsExpire  { get; set; } 
        public DateTime? EmpBirth   { get; set; } 
        public DateTime? DateHired  { get; set; }

        //-------------------------
        public string? ClName       { get; set; } = "";

    }


}
