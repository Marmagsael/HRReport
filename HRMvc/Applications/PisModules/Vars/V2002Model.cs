using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisModules.Vars
{
    public class V2002Model
    {
        public string? OPisdb       { get; set; } = string.Empty;
        public string? Maindb       { get; set; } = string.Empty;
        public string? Mainpisdb    { get; set; } = string.Empty;
        public string? Conn         { get; set; } = string.Empty;
        public string? Module       { get; set; } = string.Empty;


        public string? Action       { get; set; } = string.Empty;
        public bool? IsLoading      { get; set; } = true;
        public bool? UcLoaded       { get; set; } = false;
        public bool? ShowUserList   { get; set; } = false;
        public bool? ShowUserForm   { get; set; } = false;
        public bool? ShowUserAccess { get; set; } = false;


        public OPisUsrModel?    SelectedUser                { get; set; }
        public string?          SelectedUserFullName        { get; set; }
        public string?          SelectedMenu                { get; set; } = string.Empty;


        public IEnumerable<MenuModel?> SelectedSystemMenu   { get; set; } = new List<MenuModel>();
        public IEnumerable<MenuModel?> SelectedDtlsMenu     { get; set; } = new List<MenuModel>();

        public List<OPisUsrModel?>?     Users               { get; set; } = [];
        public OPisUsrModel?            User                { get; set; } = new();
        public List<MenuModel?>?        SystemMenus         { get; set; } = [];
        public List<MenuModel?>?        DtlsMenus           { get; set; } = [];


        //Toggle Control
        public bool ShowOldPassword                 { get; set; } = false;
        public bool ShowNewPassword                 { get; set; } = false;
        public bool ShowVerifyPassword              { get; set; } = false;

        //Error Message 
        public string OldPasswordErrorMsg           { get; set; } = string.Empty;
        public string NewPasswordErrorMsg           { get; set; } = string.Empty;
        public string VerificationPasswordErrorMsg  { get; set; } = string.Empty;
    }

   
}
