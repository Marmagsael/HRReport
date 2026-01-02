using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Main.LoginSignUp; 

public class CreateUserCompanyUiModel
{
    // --- Login Information --------------------------------------------------------------
    public int UserId { get; set; }
    public string? LoginName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }


    //---- User Information ( Empmas ) ----------------------------------------------------
    [Required]
    [Display(Name = "Empmas Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }

    [Required(ErrorMessage = "Please enter your last name.")]
    [Display(Name = "Last Name")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? EmpLastNm { get; set; }

    [Required(ErrorMessage = "Please enter your first name.")]
    [Display(Name = "First Name")]
    [StringLength(17, ErrorMessage = "This field must not exceed 17 characters.")]
    public string? EmpFirstNm { get; set; }

    [Display(Name = "Middle Name")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpMidNm { get; set; }

    [Display(Name = "Suffix")]
    public string? Suffix { get; set; }

    [Display(Name = "Alias")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpAlias { get; set; }


    //**************************************************************************
    //---- Company Profile ----------------------------------------------------
    //**************************************************************************
    [Required]
    [Display(Name = "User Company Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int UsersCompanyId { get; set; }


    [Required(ErrorMessage = "Please enter your company name.")]
    [Display(Name = "Company Name")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? CompanyName { get; set; }

    [Required(ErrorMessage = "Please enter the short name of your company.")]
    [Display(Name = "Short Name")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? CompanySName { get; set; }

    [Required(ErrorMessage = "Please select a country.")]
    [Display(Name = "Country")]
    [Range(0, int.MaxValue, ErrorMessage = "Please select a country.")]
    public int CountryId { get; set; }

    [Required]
    [Display(Name = "Country")]
    public string? CountryCode { get; set; }

    [Required]
    [Display(Name = "Country Name")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? Country { get; set; }



    [Required(ErrorMessage = "Please select a region.")]
    [Display(Name = "Region")]
    [Range(0, int.MaxValue, ErrorMessage = "Please select a region.")]
    public int RegionId { get; set; }


    [Required]
    [Display(Name = "Region Name")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? Region { get; set; }


    [Required(ErrorMessage = "Please select a city.")]
    [Display(Name = "City")]
    [Range(0, int.MaxValue, ErrorMessage = "Please select a city.")]
    public int CityId { get; set; }

    [Required]
    [Display(Name = "City Name")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? City { get; set; }


    [Display(Name = "Postal")]
    public string? Zipcode { get; set; }

    [Required(ErrorMessage = "Please select a currency.")]
    [Display(Name = "Currency")]
    [Range(0, int.MaxValue, ErrorMessage = "Please select a currency.")]
    public int CurrencyId { get; set; }

    [Required]
    [Display(Name = "Currency Name")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? Currency { get; set; }



}
