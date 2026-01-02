using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmascharrefUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "EmpmasId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }

    
    [Display(Name = "Name")]
    [StringLength(50, ErrorMessage = "This field must not exceed 50 characters.")]
    public string? Name { get; set; }

    
    [Display(Name = "Address")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Addr { get; set; }

    
    [Display(Name = "Telephone")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? Tel { get; set; }

    
    [Display(Name = "Occupation")]
    [StringLength(75, ErrorMessage = "This field must not exceed 75 characters.")]
    public string? Position { get; set; }
}
