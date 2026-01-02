using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmasemploymentUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

   
    [Display(Name = "EmpmasId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }

   
    [Display(Name = "Company")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? CompName { get; set; }

   
    [Display(Name = "Address")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? Address { get; set; }

   
    [Display(Name = "Telephone")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? Tel { get; set; }

   
    [Display(Name = "Position")]
    [StringLength(75, ErrorMessage = "This field must not exceed 75 characters.")]
    public string? Pos { get; set; }

   
    [Display(Name = "From")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? From_ { get; set; }

   
    [Display(Name = "To")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? To_ { get; set; }

   
    [Display(Name = "Salary")]
    public double Sal { get; set; }

   
    [Display(Name = "Remarks")]
    [StringLength(120, ErrorMessage = "This field must not exceed 120 characters.")]
    public string? Rem { get; set; }
}
