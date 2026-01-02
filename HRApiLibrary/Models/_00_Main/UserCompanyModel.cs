namespace HRApiLibrary.Models._00_Main; 

public class UserCompanyModel
{
    public int          Id                  { get; set; } = 0;
    public int          OwnerId             { get; set; } = 0;
    public string?      CompanySName        { get; set; } = string.Empty;
    public string?      CompanyName         { get; set; } = string.Empty;
    public int          CountryId           { get; set; } = 0;
    public int          RegionId            { get; set; } = 0;
    public int          CityId              { get; set; } = 0;
    public string?      Zipcode             { get; set; } = string.Empty;
    public int          CurrencyId          { get; set; } = 2;
    public string?      StorageId           { get; set; } = string.Empty;
    public string?      AmsSchema           { get; set; } = string.Empty;
    public string?      ApplicantSchema     { get; set; } = string.Empty;
    public string?      PisSchema           { get; set; } = string.Empty;
    public string?      PaySchema           { get; set; } = string.Empty;

    //--------------------------------------------------------------------
    public string?      CountryName         { get; set; } = string.Empty;
    public string?      OwnerName           { get; set; } = string.Empty;

}
