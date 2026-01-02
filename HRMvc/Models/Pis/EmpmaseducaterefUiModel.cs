using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis;

public class EmpmaseducaterefUiModel
{
    
    [Display(Name = "Code")]
    [StringLength(3, ErrorMessage = "This field must not exceed 3 characters.")]
    public string? Code { get; set; }

    
    [Display(Name = "Name")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? Name { get; set; }
}
