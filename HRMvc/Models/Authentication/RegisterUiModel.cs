using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Authentication;

public class RegisterUiModel
{
    [Display(Name = "Employee Number")]
    [Required(ErrorMessage = "Please enter your employee number")]
    public string EmpNumber { get; set; } = string.Empty;

    [Display(Name = "Position")]
    [Required]
    public string Position { get; set; } = string.Empty;

    [Display(Name = "Deployment Code")]
    [Required]
    public string Position_ { get; set; } = string.Empty;

    [Display(Name = "Latest Movement No.")]
    [Required]
    public string MovNumber { get; set; } = string.Empty;

    [Display(Name = "Sec License No.")]
    [Required]
    public string SecLicense { get; set; } = string.Empty;

    [Display(Name = "Date Hired")]
    [DataType(DataType.Date)]
    [Required]
    public string DateHired { get; set; } = string.Empty;

    [Display(Name = "Password")]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;


    [Display(Name = "Confirm Password")]
    [Required]
    [DataType(DataType.Password)]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

}
