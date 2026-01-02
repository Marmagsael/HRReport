using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Authentication;

public class LoginUiModel
{
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Please enter your username.")]
    public string EmpNumber { get; set; } = string.Empty;


    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please enter your password.")]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
