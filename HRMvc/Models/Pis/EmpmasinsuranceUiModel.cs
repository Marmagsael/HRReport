using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasinsuranceUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "Insurance")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? INSURANCE { get; set; }

    
    [Display(Name = "Policy No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? PolicyNo { get; set; }

    
    [Display(Name = "Face Value")]
    public double FaceValue { get; set; }

    
    [Display(Name = "Premium")]
    public double Premium { get; set; }

    
    [Display(Name = "Insurance Expiration")]
    [DataType(DataType.Date)]
    public DateTime InsExpire { get; set; }
}
