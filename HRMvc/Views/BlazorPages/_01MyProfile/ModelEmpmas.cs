using System.ComponentModel.DataAnnotations;

namespace HRMvc.Views.BlazorPages._00Main;

public class ModelEmpmas
{
    [Required]
    [StringLength(60, ErrorMessage ="Entry too long.")]
    public string? EmpLastName { get; set; }
}
