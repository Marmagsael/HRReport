namespace HRApiLibrary.Models._00_Main;

// -- Register as Company Owner : values saved after the email OTP is verified --
public class RegisterOwnerModel
{
    public string   Email           { get; set; } = string.Empty;
    public string   Password        { get; set; } = string.Empty;   // sha2-512 hash
    public string   Domain          { get; set; } = string.Empty;   // connection name, e.g. MySqlConn

    public string   FirstName       { get; set; } = string.Empty;
    public string?  MiddleName      { get; set; }
    public string   LastName        { get; set; } = string.Empty;
    public string?  Suffix          { get; set; }

    public string   CompanyName     { get; set; } = string.Empty;
    public string   CompanyCode     { get; set; } = string.Empty;   // letters/numbers, upper case, schema prefix
    public int?     CountryId       { get; set; }
    public int?     CurrencyId      { get; set; }

    // -- Schema names : company code + "_" + suffix, e.g. MBI_Pis --
    public string   PISSchema       => $"{CompanyCode}_Pis";
    public string   AMSSchema       => PISSchema;                   // AMS stays in the PIS schema for now
    public string   PaySchema       => $"{CompanyCode}_Pay";
    public string   ApplicantSchema => $"{CompanyCode}_App";           // name saved only, no table maker yet
    public string   AcctgSchema     => $"{CompanyCode}_Acctg";         // tables are made by the login (CreateCompany)
    public string   OldPis          => $"{CompanyCode}_OPis";
    public string   OldPay          => $"{CompanyCode}_OPay";

    // -- Set by _01Owner --
    public int      UserId          { get; set; }
    public int      CompanyId       { get; set; }
}
