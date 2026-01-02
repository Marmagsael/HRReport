using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HRMvc.Models.Pis;

public class EmpmasgovphUiModel
{
    
    [Display(Name = "Id")]
    [Range(0, int.MaxValue, ErrorMessage = "Invalid integer value")]
    public int Id { get; set; }

    
    [Display(Name = "SSS No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Sss { get; set; }

    
    [Display(Name = "TIN")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Tin { get; set; }

    
    [Display(Name = "PAG-IBIG No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? PagibigNo { get; set; }

    
    [Display(Name = "PhilHealth No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Phic { get; set; }

    
    [Display(Name = "HDMF")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? Hdmf { get; set; }

    
    [Display(Name = "Bank Account No.")]
    [StringLength(15, ErrorMessage = "This field must not exceed 15 characters.")]
    public string? BankNo { get; set; }

    
    [Display(Name = "Bank/Branch")]
    [StringLength(45, ErrorMessage = "This field must not exceed 45 characters.")]
    public string? BankName { get; set; }

    
    [Display(Name = "Driver's License")]
    [StringLength(25, ErrorMessage = "This field must not exceed 25 characters.")]
    public string? Drv_License { get; set; }

    
    [Display(Name = "Driver Lic Expiration")]
    [DataType(DataType.Date)]
    public DateTime Drv_Exp { get; set; }

    
    [Display(Name = "DPA Consent Signed")]
    [DataType(DataType.Date)]
    public DateTime dpadate { get; set; }

    
    [Display(Name = "Tax Table Code")]
    public string? TaxCode { get; set; }
}
