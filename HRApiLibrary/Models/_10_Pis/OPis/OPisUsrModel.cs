namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OPisUsrModel
    {

        public string? LogName              { get; set; }

        public string? UserName             { get; set; }

        public string? Password             { get; set; }

        public string? Status               { get; set; } = "A";

        public string? withInsuranceAccess  { get; set; }

        public string? Email                { get; set; }


        //---------------------------------------------
        public string? NewPassword          { get; set; }
        public string? VerifyPassword       { get; set; }

        public string StatusName =>  Status == "A" ? "Active" : "Disabled";

    }
}
