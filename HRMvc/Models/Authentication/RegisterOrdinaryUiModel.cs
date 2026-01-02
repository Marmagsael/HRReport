using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Authentication; 

public class RegisterOrdinaryUiModel
{
    [Display(Name = "Username")]
    [Required(ErrorMessage = "Please enter your username.")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    [DataType(DataType.EmailAddress)]
    [Required(ErrorMessage = "Please enter your email address.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;


    [Display(Name = "Confirm Password")]
    [Required(ErrorMessage = "Please enter again your password.")]
    [DataType(DataType.Password)]
    [StringLength(20, ErrorMessage = "The {0} must be at least 6 characters long.", MinimumLength = 6)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public bool withError { get; set; } = true;
    public string? errUserMsg { get; set; } = string.Empty;
    public string? errEmailMsg { get; set; } = string.Empty;

    




}
