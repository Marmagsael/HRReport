using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis;

public class EmpmasrelativesUiModel
{
    
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "EmpmasId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }

    
    [Display(Name = "Name")]
    [StringLength(80, ErrorMessage = "This field must not exceed 80 characters.")]
    public string? Name { get; set; }

    
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime Birth { get; set; }

    
    [Display(Name = "Gender")]
    public string? Sex { get; set; }

    
    [Display(Name = "RelativesRefCode")]
    [StringLength(5, ErrorMessage = "This field must not exceed 5 characters.")]
    public string? RelativesRefCode { get; set; }
}
