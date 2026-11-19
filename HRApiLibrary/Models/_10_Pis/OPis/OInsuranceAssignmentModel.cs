namespace HRApiLibrary.Models._10_Pis.OPis
{
    // One row per employee assigned to an insurance (used by the Insurance Assignment report)
    public class OInsuranceAssignmentModel
    {
        public string?   InsuranceType  { get; set; }
        public string?   InsuranceName  { get; set; }
        public string?   PolicyNo       { get; set; }
        public double?   FaceValue      { get; set; }
        public double?   Premiums       { get; set; }
        public DateTime? InsExpire      { get; set; }

        public string?   EmpNumber      { get; set; }
        public string?   EmpName        { get; set; }
        public string?   ClName         { get; set; }
        public string?   EmpStatus      { get; set; }
    }
}
