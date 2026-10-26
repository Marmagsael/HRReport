namespace HRApiLibrary.Models._00_Main;

// -- Register as Employee : values saved after the email OTP is verified --
public class RegisterEmployeeModel
{
    public string   Email           { get; set; } = string.Empty;
    public string   Password        { get; set; } = string.Empty;   // sha2-512 hash
    public string   Domain          { get; set; } = string.Empty;   // connection name, e.g. MySqlConn

    public int      CompanyId       { get; set; }
    public string   PISSchema       { get; set; } = string.Empty;   // the company's new PIS schema

    // -- From {OldPis}.Empmas --
    public string   EmpNumber       { get; set; } = string.Empty;
    public string?  LastName        { get; set; }
    public string?  FirstName       { get; set; }
    public string?  MiddleName      { get; set; }
    public string?  Suffix          { get; set; }

    // -- Set by _01Employee --
    public int      EmpmasId        { get; set; }                   // {PISSchema}.Empmas.Id (found or made by EmpNumber)
    public int      UserId          { get; set; }
}
