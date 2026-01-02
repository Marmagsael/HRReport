using HRApiLibrary.Models._90_Utils;

namespace HRApiLibrary.Models._00_Main;

public class UserClaimsModel
{
    public int                      UserId                  { get; set; } = -1;
    public string?                  UserName                { get; set; } = string.Empty;
    public string?                  DefCompanyId            { get; set; } = string.Empty;
    public string?                  SchemaMain              { get; set; } = string.Empty;
    public string?                  SchemaMainPis           { get; set; } = string.Empty;
    public string?                  SchemaUserPis           { get; set; } = string.Empty;
    public string?                  SchemaUserPay           { get; set; } = string.Empty;
    public string?                  SchemaUserAms           { get; set; } = string.Empty;
    public string?                  SchemaUserAcctg         { get; set; } = string.Empty;  
    public string?                  SchemaUserApp           { get; set; } = string.Empty;
    public string?                  CoName                  { get; set; } = string.Empty;

    //------------------------------------------------------------------------------------
    public string?                  Conn                    { get; set; } = string.Empty;
    public string?                  ConnNoDb                { get; set; } = string.Empty;
    public string?                  ConnPay                { get; set; } = string.Empty;
    public string?                  ConnPis                { get; set; } = string.Empty;
    public string?                  ConnAcctg              { get; set; } = string.Empty;
    public string?                  IsExclusiveCompany      { get; set; }
    public List<MenusModel?>?       Menus                   { get; set; } = new(); 

}
