using HRApiLibrary.Models._00_Main;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models;
    public class EmpmasaddressUiModel
    {
        [Required]
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Street")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddStreet {get; set; }

        [Required]
        [Display(Name = "Village")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddVillage {get; set; }

        [Required]
        [Display(Name = "Barangay")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddBrgy { get; set; }

        [Required]
        [Display(Name = "City Code")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? PresAddCityCode { get; set; }

        [Required]
        [Display(Name = "City")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddCity { get; set; }

        [Required]
        [Display(Name = "Province Code")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? PresAddProvCode { get; set; }

        [Required]
        [Display(Name = "Province")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddProv { get; set; }

        [Required]
        [Display(Name = "State Code")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? PresAddStateCode { get; set; }

        [Required]
        [Display(Name = "State")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddState { get; set; }

        [Required]
        [Display(Name = "Country Code")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? PresAddCountryCode { get; set; }

        [Required]
        [Display(Name = "Country")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddCountry { get; set; }

        [Display(Name = "Zip Code")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? PresAddZipCode { get; set; }

        [Required]
        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "This field must not exceed 200 characters.")]
        public string? PresAdd { get; set; }

        [Required]
        [Display(Name = "Telephone No.")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? PresAddTelNo { get; set; }

        [Required]
        [Display(Name = "ProvAddStreet")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddStreet { get; set; }

        [Required]
        [Display(Name = "ProvAddVillage")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddVillage { get; set; }

        [Required]
        [Display(Name = "ProvAddBrgy")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddBrgy { get; set; }

        [Required]
        [Display(Name = "ProvAddCityCode")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? ProvAddCityCode { get; set; }

        [Required]
        [Display(Name = "ProvAddCity")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddCity { get; set; }

        [Required]
        [Display(Name = "ProvAddProvCode")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? ProvAddProvCode { get; set; }

        [Required]
        [Display(Name = "ProvAddProv")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddProv { get; set; }

        [Required]
        [Display(Name = "ProvAddStateCode")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? ProvAddStateCode { get; set; }

        [Required]
        [Display(Name = "ProvAddState")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddState { get; set; }

        [Required]
        [Display(Name = "ProvAddCountryCode")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? ProvAddCountryCode { get; set; }

        [Required]
        [Display(Name = "ProvAddCountry")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddCountry { get; set; }

        [Required]
        [Display(Name = "ProvAddZipCode")]
        [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
        public string? ProvAddZipCode { get; set; }

        [Required]
        [Display(Name = "ProvAdd")]
        [StringLength(200, ErrorMessage = "This field must not exceed 200 characters.")]
        public string? ProvAdd { get; set; }

        [Required]
        [Display(Name = "ProvAddTelNo")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? ProvAddTelNo { get; set; }

        [Required]
        [Display(Name = "Country Code")]
        public string? Countrycode { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? EmailAdd { get; set; }

        [Required]
        [Display(Name = "Email Address 1")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? EmailAdd1 { get; set; }

        [Required]
        [Display(Name = "Mobile No.")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? CellNo { get; set; }

        [Required]
        [Display(Name = "Mobile No.1")]
        [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
        public string? CellNo1 { get; set; } 
}

