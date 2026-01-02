using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasclearancephUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Nbi_Taken { get; set; }

    
    [Display(Name = "NBI Expire")]
    [DataType(DataType.Date)]
    public DateTime Nbi_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Nbi_Remarks { get; set; }

    
    [Display(Name = "Nbi_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Nbi_Link { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Police_Taken { get; set; }

    
    [Display(Name = "Police Expire")]
    [DataType(DataType.Date)]
    public DateTime Police_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Police_Remarks { get; set; }

    
    [Display(Name = "Police_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Police_Link { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Pnp_Taken { get; set; }

    
    [Display(Name = "PNP Expire")]
    [DataType(DataType.Date)]
    public DateTime Pnp_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Pnp_Remarks { get; set; }

    
    [Display(Name = "Pnp_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Pnp_Link { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Brgy_Taken { get; set; }

    
    [Display(Name = "Brangay Expire")]
    [DataType(DataType.Date)]
    public DateTime Brgy_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Brgy_Remarks { get; set; }

    
    [Display(Name = "Brgy_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Brgy_Link { get; set; }

    
    [Display(Name = "Court Taken")]
    [DataType(DataType.Date)]
    public DateTime Court_Taken { get; set; }

    
    [Display(Name = "Court Expire")]
    [DataType(DataType.Date)]
    public DateTime Court_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Court_Remarks { get; set; }

    
    [Display(Name = "Court_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Court_Link { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Neuro_Taken { get; set; }

    
    [Display(Name = "Neuro Expire")]
    [DataType(DataType.Date)]
    public DateTime Neuro_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Neuro_Remarks { get; set; }

    
    [Display(Name = "Neuro_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Neuro_Link { get; set; }

    
    [Display(Name = "Date Taken")]
    [DataType(DataType.Date)]
    public DateTime Drug_Taken { get; set; }

    
    [Display(Name = "Drug Expire")]
    [DataType(DataType.Date)]
    public DateTime Drug_Exp { get; set; }

    
    [Display(Name = "Remarks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Drug_Remarks { get; set; }

    
    [Display(Name = "Drug_Link")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Drug_Link { get; set; }
}
