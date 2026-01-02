using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pis;

public class EmpmaspiUiModel
{
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime EmpBirth { get; set; }

    
    [Display(Name = "Birth Place")]
    [StringLength(75, ErrorMessage = "This field must not exceed 75 characters.")]
    public string? BirthPlace { get; set; }

    
    [Display(Name = "Gender")]
    [StringLength(1, ErrorMessage = "This field must not exceed 1 characters.")]
    public string? Sex_ { get; set; }

    
    [Display(Name = "Civil Status")]
    [StringLength(1, ErrorMessage = "This field must not exceed 1 characters.")]
    public string? CivStat_ { get; set; }

    
    [Display(Name = "Citizenship")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Citizen { get; set; }

    
    [Display(Name = "Religion")]
    [StringLength(35, ErrorMessage = "This field must not exceed 35 characters.")]
    public string? Religion { get; set; }

    
    [Display(Name = "Height")]
    public int Height { get; set; }


    [Display(Name = "Height Inch")]
    public int HeightInch { get; set; }


    [Display(Name = "Weight")]
    public double Weight { get; set; }

    
    [Display(Name = "Hair")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Hair { get; set; }

    
    [Display(Name = "Eyes")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Eyes { get; set; }

    
    [Display(Name = "Complexion")]
    [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
    public string? Complexion { get; set; }

    
    [Display(Name = "Marks")]
    [StringLength(60, ErrorMessage = "This field must not exceed 60 characters.")]
    public string? Marks { get; set; }

    
    [Display(Name = "BloodType")]
    [StringLength(10, ErrorMessage = "This field must not exceed 10 characters.")]
    public string? BloodType { get; set; }

    
    [Display(Name = "Spouse")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? Spouse { get; set; }

    
    [Display(Name = "Occupation")]
    [StringLength(75, ErrorMessage = "This field must not exceed 75 characters.")]
    public string? Occupation { get; set; }

    
    [Display(Name = "No. of Children")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int NoChildren { get; set; }
}
