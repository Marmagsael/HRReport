namespace HRApiLibrary.Models._00_Main; 

public class CompanyUsersModel
{
    public int          UserId              { get; set; }
    public int          CompanyId           { get; set; }
    public string?      Status              { get; set; } = "A";
    public DateTime     DateInvited         { get; set; }
    public DateTime     DateAccepted        { get; set; } =  new DateTime(0001, 01, 01, 01, 01, 01);
    public int          CompanyUserTypeId   { get; set; } = 3; 
    public int          InvitedById         { get; set; } = 0; 

    //-------------------------------------------------
    public string?      CompanySName        { get; set; }   = string.Empty; 
    public string?      CountryName         { get; set; }   = string.Empty; 
    public string?      CompanyName         { get; set; }   = string.Empty; 
}