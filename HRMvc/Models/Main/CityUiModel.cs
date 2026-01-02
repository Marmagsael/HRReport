using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Main; 

public class CityUiModel
{
    [Required]
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "City")]
    [StringLength(100, ErrorMessage = "This field must not exceed 100 characters.")]
    public string? CityName { get; set; }

    [Required]
    [Display(Name = "Country Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int CountryId { get; set; }

    
    [Required]
    [Display(Name = "Country Code")]
    public string? CountryCode { get; set; }

    [Display(Name = "Country")]
    public string? Country { get; set; }


    [Required]
    [Display(Name = "Region Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int RegionId { get; set; }


    [Display(Name = "Region")]
    public string? Region { get; set; }


    
}
