using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis;
public class EmpmasfamilyrefUiModel
{
    [Display(Name = "Code")]
    public string? Code { get; set; }

   
    [Display(Name = "Name")]
    [StringLength(80, ErrorMessage = "This field must not exceed 80 characters.")]
    public string? Name { get; set; }
}
