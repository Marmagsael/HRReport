using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis;

public class EmpmastrainingUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }


    [Display(Name = "EmpmasId")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int EmpmasId { get; set; }


    [Display(Name = "Program")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? ProgramName { get; set; }


    [Display(Name = "Date Taken")]
    [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
    public string? DateTaken { get; set; }


    [Display(Name = "Date Started")]
    [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
    public string? DateStart { get; set; }


    [Display(Name = "Date Ended")]
    [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
    public string? DateEnd { get; set; }


    [Display(Name = "Total Hours")]
    [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
    public string? TotalHrs { get; set; }


    [Display(Name = "Total Days")]
    [StringLength(20, ErrorMessage = "This field must not exceed 20 characters.")]
    public string? TotalDays { get; set; }


    [Display(Name = "Venue")]
    [StringLength(50, ErrorMessage = "This field must not exceed 50 characters.")]
    public string? School { get; set; }


    [Display(Name = "Trainor")]
    [StringLength(30, ErrorMessage = "This field must not exceed 30 characters.")]
    public string? Trainor { get; set; }
}