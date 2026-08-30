using HRApiLibrary.Models._00_Main;

namespace HRMvc.Applications.PayrollTransaction.Vars;

public class V103Model
{
    public string OPisdb                    { get; set; } = string.Empty;
    public string OPaydb                    { get; set; } = string.Empty;
    public string Maindb                    { get; set; } = string.Empty;
    public string Mainpisdb                 { get; set; } = string.Empty;
    public string Mainpaydb                 { get; set; } = string.Empty;



    public string Conn                      { get; set; } = string.Empty;

    public string Action                    { get; set; } = string.Empty;
    public bool IsLoading                   { get; set; } = true;
    public bool UcLoaded                    { get; set; } = false;

    public string SelectedUser              { get; set; } = string.Empty;
    public string SelectedMenu              { get; set; } = string.Empty;


    public IEnumerable<MenuModel?>           SelectedSystemMenu { get; set; } = new List<MenuModel>();
    public IEnumerable<MenuModel?>           SelectedDtlsMenu   { get; set; } = new List<MenuModel>();

    public List<UsersModel>         Users            { get; set; } = [];
    public List<MenuTypeModel?>?    MenuTypes        { get; set; } = [];
    public List<MenuModel?>?        SystemMenus      { get; set; } = [];
    public List<MenuModel?>?        DtlsMenus        { get; set; } = [];
}

public class MenuTypeModel
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

