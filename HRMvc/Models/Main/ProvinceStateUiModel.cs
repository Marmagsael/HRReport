using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Main; 

public class ProvinceStateUiModel
{
    [Required]
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Code")]
    public string? Code { get; set; }

    [Required]
    [Display(Name = "Name")]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Country Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int CountryId { get; set; }

    [Required]
    [Display(Name = "Country")]
    public string? Country { get; set; }
}