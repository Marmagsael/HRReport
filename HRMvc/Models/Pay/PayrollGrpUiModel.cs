using System.ComponentModel.DataAnnotations;

namespace HRMvc.Models.Pay
{
    public class PayrollGrpUiModel
    {
        [Display(Name = "ID")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Payroll group code is required.")]
        [StringLength(50)]
        [Display(Name = "Code")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "CL Number is required.")]
        [StringLength(50)]
        [Display(Name = "CL Number")]
        public string? ClNumber { get; set; }

        [Display(Name = "Deployment")]
        public string? Deployment { get; set; }

        [Required(ErrorMessage = "Payroll group name is required.")]
        [StringLength(150)]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Rate per hour is required.")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Rate Per Hour")]
        public double RatePerHr { get; set; }

        [Required(ErrorMessage = "Rate per day is required.")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Rate Per Day")]
        public double RatePerDay { get; set; }

        [Required(ErrorMessage = "Rate per month is required.")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Rate Per Month")]
        public double RatePerMonth { get; set; }

        [Required(ErrorMessage = "Rate per year is required.")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Rate Per Year")]
        public double RatePerYr { get; set; }

        [Required(ErrorMessage = "Minimum daily rate is required.")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Minimum Daily Rate")]
        public double MinDailyRate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Pay Rate")]
        public int? PayRateId { get; set; }


        public bool Show { get; set; } = false;

    }
}


