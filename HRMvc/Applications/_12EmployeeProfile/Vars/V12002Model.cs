using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications._12EmployeeProfile.Vars
{
    public class V12002Model
    {
        public bool             IsLoading   { get; set; }

        public OPisUsrModel     User        { get; set; } = new();
    }
}