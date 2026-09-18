namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OPenaltyModel
    {
        public string? Dev_No       { get; set; }

        public string? Freq         { get; set; }

        public string? Penalty_No   { get; set; }

        public string? Desc_        { get; set; }

        public string? ResetRegRef  { get; set; }  = "0";

        public string? IsTerminated { get; set; } = "0";

        public Double? Days         { get; set; } = 0;
    }
}
