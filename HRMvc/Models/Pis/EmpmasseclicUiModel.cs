using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasseclicUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "Sec. License")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? SecLicense { get; set; }

    
    [Display(Name = "Expiration")]
    [DataType(DataType.Date)]
    public DateTime LicExpire { get; set; }

    
    [Display(Name = "Badge No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? BadgeNo { get; set; }

    
    [Display(Name = "Sbr No.")]   
    public string? SbrNo { get; set; }

    
    [Display(Name = "OP No.")]
    public string? OpNo { get; set; }

    
    [Display(Name = "Date Validated")]
    [DataType(DataType.Date)]
    public DateTime Validated { get; set; }

    
    [Display(Name = "VFee")]
    public double VFee { get; set; }

    
    [Display(Name = "Date Re-validated")]
    [DataType(DataType.Date)]
    public DateTime Revalidated { get; set; }

    
    [Display(Name = "Validation Status")]
    public string? ValStatus { get; set; }
}
