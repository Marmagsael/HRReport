using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models;
public class EmpmasUiModel
{
    [Display(Name = "Id")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? EmpLastNm {get; set; }

    [Required]
    [Display(Name = "First Name")]
    [StringLength(17, ErrorMessage = "This field must not exceed 17 characters.")]
    public string? EmpFirstNm {get; set; }

    [Display(Name = "Middle Name")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpMidNm { get; set; }

    [Display(Name = "Suffix")]
    public string? Suffix { get; set; }

    [Display(Name = "Alias")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpAlias { get; set; } 
}
