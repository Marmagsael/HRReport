using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }


    [Display(Name = "Last Name")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? EmpLastNm { get; set; }


    [Display(Name = "First Name")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? EmpFirstNm { get; set; }

    [Display(Name = "Middle Name")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpMidNm { get; set; }

    [Display(Name = "Suffix")]
    public string? Suffix { get; set; }

    [Display(Name = "Alias")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? EmpAlias { get; set; }
}
