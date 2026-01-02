using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasaddressUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    [Display(Name = "PresAddStreet")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddStreet { get; set; }

    
    [Display(Name = "PresAddVillage")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddVillage { get; set; }

    
    [Display(Name = "PresAddBrgy")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddBrgy { get; set; }

    
    [Display(Name = "PresAddCityId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int PresAddCityId { get; set; }


    [Display(Name = "City")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddCity { get; set; }


    [Display(Name = "PresAddProvId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int PresAddProvId { get; set; }

    
    [Display(Name = "PresAddProv")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddProv { get; set; }

    
    [Display(Name = "PresAddStateId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int PresAddStateId { get; set; }

    
    [Display(Name = "PresAddState")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddState { get; set; }

    
    [Display(Name = "PresAddCountryId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int PresAddCountryId { get; set; }

    
    [Display(Name = "PresAddCountry")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddCountry { get; set; }

    
    [Display(Name = "PresAddZipCode")]
    [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
    public string? PresAddZipCode { get; set; }

    
    [Display(Name = "Address")]
    [StringLength(200, ErrorMessage = "This field must not exceed 200 characters.")]
    public string? PresAdd { get; set; }

    
    [Display(Name = "Telephone")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? PresAddTelNo { get; set; }

    
    [Display(Name = "ProvAddStreet")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddStreet { get; set; }

    
    [Display(Name = "ProvAddVillage")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddVillage { get; set; }

    
    [Display(Name = "ProvAddBrgy")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddBrgy { get; set; }

    
    [Display(Name = "ProvAddCityId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int ProvAddCityId { get; set; }

    
    [Display(Name = "City")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddCity { get; set; }

    
    [Display(Name = "ProvAddProvId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int ProvAddProvId { get; set; }

    
    [Display(Name = "ProvAddProv")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddProv { get; set; }

    
    [Display(Name = "ProvAddStateId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int ProvAddStateId { get; set; }

    
    [Display(Name = "ProvAddState")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddState { get; set; }

    
    [Display(Name = "ProvAddCountryId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int ProvAddCountryId { get; set; }

    
    [Display(Name = "ProvAddCountry")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddCountry { get; set; }

    
    [Display(Name = "ProvAddZipCode")]
    [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
    public string? ProvAddZipCode { get; set; }

    
    [Display(Name = "Address")]
    [StringLength(200, ErrorMessage = "This field must not exceed 200 characters.")]
    public string? ProvAdd { get; set; }

    
    [Display(Name = "Telephone")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? ProvAddTelNo { get; set; }

    
    [Display(Name = "Countrycode")]
    public string? Countrycode { get; set; }

    
    [Display(Name = "EmailAdd")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? EmailAdd { get; set; }

    
    [Display(Name = "EmailAdd1")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? EmailAdd1 { get; set; }

    
    [Display(Name = "CellNo")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? CellNo { get; set; }

    
    [Display(Name = "CellNo1")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? CellNo1 { get; set; }
}
