namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OInsuranceModel
    {
        public int? Id                   { get; set; } = 0;

        public string? Name             { get; set; }

        public string? PolicyNo         { get; set; }

        public string? InsuranceType    { get; set; }

        public double? FaceValue        { get; set; }

        public double? Premiums         { get; set; }

        public DateTime? InsExpire      { get; set; }
    }
}
