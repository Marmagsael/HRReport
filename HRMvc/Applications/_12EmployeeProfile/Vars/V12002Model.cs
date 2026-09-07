using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications._12EmployeeProfile.Vars
{
    public class V12002Model
    {
        public bool             IsLoading   { get; set; }

        public OPisUsrModel?    User        { get; set; } = new();


        //Toggle Control
        public bool ShowOldPassword                 { get; set; } = false;
        public bool ShowNewPassword                 { get; set; } = false;
        public bool ShowVerifyPassword              { get; set; } = false;

        //Error Message 
        public string OldPasswordErrorMsg           { get; set; } = string.Empty;
        public string NewPasswordErrorMsg           { get; set; } = string.Empty;
        public string VerificationPasswordErrorMsg  { get; set; } = string.Empty;
    }
}