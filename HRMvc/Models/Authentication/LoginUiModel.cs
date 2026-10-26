using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Authentication;
public class LoginUiModel
{
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Please enter your username.")]
    public string EmpNumber { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please enter your password.")]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Company")]
    [Required(ErrorMessage = "Please select a company.")]
    public string CompanyId { get; set; } = string.Empty;
}