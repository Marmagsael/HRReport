using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HRMvc.Applications._01Register.Vars;

// -- 01.02 Register as Employee : step 1, verified before the password is enabled ---
public class V01RegisterEmployeeCheck
{
    [Display(Name = "Company Code")]
    [Required(ErrorMessage = "Please enter the company code.")]
    [StringLength(10, ErrorMessage = "Company code must not exceed 10 characters.")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Company code must contain letters and numbers only, with no spaces.")]
    public string CompanyCode       { get; set; } = string.Empty;

    [Display(Name = "Employee No.")]
    [Required(ErrorMessage = "Please enter your employee number.")]
    [StringLength(10, ErrorMessage = "Employee number must not exceed 10 characters.")]
    public string EmpNumber         { get; set; } = string.Empty;

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Please enter your last name.")]
    [StringLength(45, ErrorMessage = "Last name must not exceed 45 characters.")]
    public string LastName          { get; set; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
    public string Email             { get; set; } = string.Empty;
}

// -- 01.02 Register as Employee : step 2, password ----------------------------------
public class V01RegisterEmployee : V01RegisterEmployeeCheck
{
    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please enter a password.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be 8 to 50 characters.")]
    [DataType(DataType.Password)]
    public string Password          { get; set; } = string.Empty;

    [Display(Name = "Confirm Password")]
    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword   { get; set; } = string.Empty;

    // -- Set by the server check, never bound from the form --
    [BindNever] public int    CompanyId     { get; set; }
    [BindNever] public string PISSchema     { get; set; } = string.Empty;
}

// -- 01.03 Register as Company Owner : step 1, verified before the rest is enabled --
public class V01RegisterOwnerCheck
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(60, ErrorMessage = "Email must not exceed 60 characters.")]
    public string Email             { get; set; } = string.Empty;

    [Display(Name = "Company Code")]
    [Required(ErrorMessage = "Please enter a company code.")]
    [StringLength(10, ErrorMessage = "Company code must not exceed 10 characters.")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Company code must contain letters and numbers only, with no spaces.")]
    public string CompanyCode       { get; set; } = string.Empty;   // also the schema prefix, saved in upper case
}

// -- 01.03 Register as Company Owner : step 2, owner, password and company ----------
public class V01RegisterOwner : V01RegisterOwnerCheck
{
    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Please enter your first name.")]
    [StringLength(25, ErrorMessage = "First name must not exceed 25 characters.")]   // MainPis.Empmas sizes
    public string FirstName         { get; set; } = string.Empty;

    [Display(Name = "Middle Name")]
    [StringLength(15, ErrorMessage = "Middle name must not exceed 15 characters.")]
    public string? MiddleName       { get; set; }

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Please enter your last name.")]
    [StringLength(25, ErrorMessage = "Last name must not exceed 25 characters.")]
    public string LastName          { get; set; } = string.Empty;

    [Display(Name = "Suffix")]
    [StringLength(3, ErrorMessage = "Suffix must not exceed 3 characters.")]
    public string? Suffix           { get; set; }

    [Display(Name = "Password")]
    [Required(ErrorMessage = "Please enter a password.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be 8 to 50 characters.")]
    [DataType(DataType.Password)]
    public string Password          { get; set; } = string.Empty;

    [Display(Name = "Confirm Password")]
    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword   { get; set; } = string.Empty;

    [Display(Name = "Company Name")]
    [Required(ErrorMessage = "Please enter the company name.")]
    [StringLength(120, ErrorMessage = "Company name must not exceed 120 characters.")]
    public string CompanyName       { get; set; } = string.Empty;

    [Display(Name = "Country")]
    [Required(ErrorMessage = "Please select a country.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a country.")]
    public int? CountryId           { get; set; }

    [Display(Name = "Currency")]
    [Required(ErrorMessage = "Please select a currency.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a currency.")]
    public int? CurrencyId          { get; set; }
}

// -- 01.06 Create New Company (logged-in user, from My Engagement) ------------------
public class V01NewCompany
{
    [Display(Name = "Company Code")]
    [Required(ErrorMessage = "Please enter a company code.")]
    [StringLength(10, ErrorMessage = "Company code must not exceed 10 characters.")]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Company code must contain letters and numbers only, with no spaces.")]
    public string CompanyCode       { get; set; } = string.Empty;   // also the schema prefix, saved in upper case

    [Display(Name = "Company Name")]
    [Required(ErrorMessage = "Please enter the company name.")]
    [StringLength(120, ErrorMessage = "Company name must not exceed 120 characters.")]
    public string CompanyName       { get; set; } = string.Empty;

    [Display(Name = "Country")]
    [Required(ErrorMessage = "Please select a country.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a country.")]
    public int? CountryId           { get; set; }

    [Display(Name = "Currency")]
    [Required(ErrorMessage = "Please select a currency.")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a currency.")]
    public int? CurrencyId          { get; set; }
}

// -- 01.07 Invite User (owner of the current company, from My Engagement) -----------
public class V01InviteUser : V01RoleAccess
{
    [Display(Name = "Employee")]
    [Required(ErrorMessage = "Please select an employee.")]
    public string? EmpNumber            { get; set; }   // {OldPis}.Empmas.EMPNUMBER
}

// -- Invite User / Registration Requests : the role and access given to the new member --------
// -- Employee (3, the default) or User (2); an Employee is portal only, so it gets no module access --
public class V01RoleAccess
{
    [Display(Name = "Role")]        public int  CompanyUserTypeId       { get; set; } = 3;
    [Display(Name = "PIS")]         public bool WithPISAccess           { get; set; }
    [Display(Name = "Payroll")]     public bool WithPayAccess           { get; set; }
    [Display(Name = "Attendance")]  public bool WithAMSAccess           { get; set; }
    [Display(Name = "Accounting")]  public bool WithAccountingAccess    { get; set; }
    [Display(Name = "Applicant")]   public bool WithApplicantAccess     { get; set; }

    public void Normalize()
    {
        if (CompanyUserTypeId != 2) CompanyUserTypeId = 3;
        if (CompanyUserTypeId == 3)
            WithPISAccess = WithPayAccess = WithAMSAccess = WithAccountingAccess = WithApplicantAccess = false;
    }
}

// -- 01.08 Registration Requests : the access the owner gives on approve ----------
public class V01ApproveRequest : V01RoleAccess
{
}

// -- 01.09 Company Users : role and access of one member ------------------------------
public class V01CompanyUser
{
    [Display(Name = "Role")]        public int  CompanyUserTypeId       { get; set; } = 2;  // 2 User, 3 Employee, 5 System Admin
    [Display(Name = "PIS")]         public bool WithPISAccess           { get; set; }
    [Display(Name = "Payroll")]     public bool WithPayAccess           { get; set; }
    [Display(Name = "Attendance")]  public bool WithAMSAccess           { get; set; }
    [Display(Name = "Accounting")]  public bool WithAccountingAccess    { get; set; }
    [Display(Name = "Applicant")]   public bool WithApplicantAccess     { get; set; }
}

// -- 01.04 Email OTP --------------------------------------------------------------
public class V01RegisterOtp
{
    [Display(Name = "Verification Code")]
    [Required(ErrorMessage = "Please enter the 6-digit code.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "The code must be 6 digits.")]
    public string Code              { get; set; } = string.Empty;
}

// -- Pending registration kept in session until the OTP is verified ---------------
public class V01RegisterPending
{
    public string   Purpose         { get; set; } = string.Empty;   // REG-EMP / REG-OWNER
    public string   Email           { get; set; } = string.Empty;
    public string   OtpHash         { get; set; } = string.Empty;   // sha2-512
    public DateTime ExpiresAt       { get; set; }
    public int      Attempts        { get; set; }

    public V01RegisterEmployee? Employee { get; set; }              // Password holds the sha2-512 hash
    public V01RegisterOwner?    Owner    { get; set; }              // Password holds the sha2-512 hash
}
