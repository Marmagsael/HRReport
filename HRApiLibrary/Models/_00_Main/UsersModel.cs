namespace HRApiLibrary.Models._00_Main;

public class UsersModel
{
    public int      Id              { get; set; }
    public string?  LoginName       { get; set; }
    public string?  Password        { get; set; }
    public string?  Email           { get; set; } = null;
    public string?  Domain          { get; set; }
    public int      UserType        { get; set; } = 0;
    public string?  Status          { get; set; } = "A";
    public int      DefaultCoId     { get; set; } = 0;

    //************************************************************
    public string?  AFullName       { get; set; } = "";
    public string?  AAlias          { get; set; } = "";
        
}
