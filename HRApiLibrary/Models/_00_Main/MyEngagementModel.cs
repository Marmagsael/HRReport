namespace HRApiLibrary.Models._00_Main;

// -- 12.003 My Engagement : one row per company the user belongs to (main.companyusers) --
public class MyEngagementModel
{
    public int          CompanyId           { get; set; }
    public string?      CompanyName         { get; set; }
    public string?      CompanyCode         { get; set; }
    public int?         CompanyUserTypeId   { get; set; }   // 1 Owner, 2 User
    public string?      UserType            { get; set; }
    public string?      Status              { get; set; }   // P pending, A approved, R rejected
    public DateTime?    DateInvited         { get; set; }
    public DateTime?    DateAccepted        { get; set; }
    public int          IsDefault           { get; set; }   // 1 = users.DefaultCoId
}
