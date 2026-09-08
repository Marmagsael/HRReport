namespace HRApiLibrary.Models._10_Pis.OPis
{
    public class OPisDomainusrModel
    {
        public string? Empnumber          { get; set; }

        public string? Status             { get; set; }
         
        public int? CreatedBy             { get; set; } //System Id

        public DateTime? DateCreated      { get; set; }


        // Full Name ---------------------------------------
        public string? FullName           { get; set; }
        public string StatusName => Status == "A" ? "Active" : "Disabled";


    }
}
