namespace HRApiLibrary.Models._20_Pay.OPay
{
    public class OUsrModel
    {
        public string? UserName { get; set; }

        public string? FullName { get; set; }

        public string? Pwrd { get; set; }

        public string? Stat_ { get; set; }

        public string? DednAccess { get; set; }

        public string? ArchievedAccess { get; set; }

        public string? PartDedSetupAccess { get; set; }

        public string? DedSumAccess { get; set; }

        public string? DedSumEmpAccess { get; set; }

        public string? MoDedRepAccess { get; set; }

        public string? ConDedSumAccess { get; set; }

        public string? ESumAccess { get; set; }

        public string? EHisAccess { get; set; }

        public string? MoERepAccess { get; set; }

        public string? Email { get; set; }

         //---------------------------------------------
        public string? NewPassword          { get; set; }
        public string? VerifyPassword       { get; set; }

        public string StatusName => Stat_ == "1" ? "Active" : "Disabled";

    }
}
