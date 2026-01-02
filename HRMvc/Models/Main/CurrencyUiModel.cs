using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Main; 

public class CurrencyUiModel
{
    [Required]
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Code")]
    [StringLength(5, ErrorMessage = "This field must not exceed 5 characters.")]
    public string? Code { get; set; }

    [Required]
    [Display(Name = "Name")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Symbol")]
    [StringLength(5, ErrorMessage = "This field must not exceed 5 characters.")]
    public string? Symbol { get; set; }
}