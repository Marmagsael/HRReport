namespace HRMvc.Applications.PisReport.Vars
{
    public class V2110Model
    {
        public string? EmpNumber    { get; set; } = "";
        public string? EmpName      { get; set; } = "";
        public string? Insurance    { get; set; } = "";
        public string? PolicyNo     { get; set; } = "";
        public string? EmpStatus    { get; set; } = "";
        public string? FaceValue    { get; set; } = "0.00";
        public string? Premium      { get; set; } = "0.00";
        public DateTime? InsExpire  { get; set; } 
        public DateTime? EmpBirth   { get; set; } 
        public DateTime? DateHired  { get; set; } 
    }

    
}
