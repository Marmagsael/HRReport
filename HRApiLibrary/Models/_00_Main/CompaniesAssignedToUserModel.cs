namespace HRApiLibrary.Models._00_Main; 

public class CompaniesAssignedToUserModel
{
    //---- User Company ------------------------------
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string? CompanySName { get; set; } = "Company Short Name";
    public string? CompanyName { get; set; } = "Copmany Name";
    public int CountryId { get; set; }
    public int RegionId { get; set; }
    public int CityId { get; set; }
    public string? Zipcode { get; set; }
    public int CurrencyId { get; set; } = 2;
    public string? StorageId { get; set; }
    public string? AmsSchema { get; set; }
    public string? ApplicantSchema { get; set; }
    public string? PisSchema { get; set; }
    public string? PaySchema { get; set; }

    //---- Company User ------------------------------
    public int UserId { get; set; }
    public int CompanyId { get; set; }
    public string? Status { get; set; }
    public DateTime DateInvited { get; set; }
    public DateTime DateAccepted { get; set; }
    public int CompanyUserTypeId { get; set; } = 0;

    //--- Type --------------------------------------
    public int UserTypeId { get; set; }
    public string? UserType { get; set; }
    public int UTypeVisible { get; set; }
}