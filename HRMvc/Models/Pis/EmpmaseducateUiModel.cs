using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmaseducateUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

   
    [Display(Name = "EmpmasId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }

   
    [Display(Name = "Code")]
    [StringLength(1, ErrorMessage = "This field must not exceed 1 characters.")]
    public string? Code { get; set; }

   
    [Display(Name = "School Name")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? School { get; set; }

   
    [Display(Name = "From")]
    [DataType(DataType.DateTime)]
    public DateTime FROM_ { get; set; }

   
    [Display(Name = "To")]
    [DataType(DataType.DateTime)]
    public DateTime TO_ { get; set; }

   
    [Display(Name = "Course")]
    [StringLength(75, ErrorMessage = "This field must not exceed 75 characters.")]
    public string? COURSE { get; set; }

   
    [Display(Name = "LEVEL")]
    [StringLength(8, ErrorMessage = "This field must not exceed 8 characters.")]
    public string? LEVEL { get; set; }
}
