using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis
{
    public class EmpmasemergencycontactUiModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "EmpmasId")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
        public int EmpmasId { get; set; }

        [Required]
        [Display(Name = "Contact Person")]
        [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Address")]
        [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
        public string? Addr { get; set; }

        [Required]
        [Display(Name = "Relationship")]
        [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
        public string? Relationship { get; set; }

        [Required]
        [Display(Name = "TelNo")]
        [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
        public string? TelNo { get; set; }
    }
}
