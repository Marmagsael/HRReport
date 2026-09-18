using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;
using HRApiLibrary.Models._20_Pay.OPay;
using HRApiLibrary.Models._90_Utils;

namespace HRMvc.Applications.PayrollTransaction.Vars;

public class V103Model
{

        public string?                      OPaydb                              { get; set; } = string.Empty;
        public string?                      OPisdb                              { get; set; } = string.Empty;
        public string?                      Maindb                              { get; set; } = string.Empty;
        public string?                      Mainpaydb                           { get; set; } = string.Empty;
        public string?                      Conn                                { get; set; } = string.Empty;
        public string?                      Module                              { get; set; } = string.Empty;

        public string?                      Action                              { get; set; } = string.Empty;
        public bool                         IsLoading                           { get; set; } = true;
        public bool?                        UcLoaded                            { get; set; } = false;


        public string?                      ModalCaption                        { get; set; } = string.Empty;
        public bool?                        ShowUserEntryModal                  { get; set; } = false;
        public bool?                        ShowUserAccessModal                 { get; set; } = false;



        public IEnumerable<MenuModel?>      SelectedSystemMenu                   { get; set; } = new List<MenuModel>();
        public IEnumerable<MenuModel?>      SelectedDtlsMenu                     { get; set; } = new List<MenuModel>();

        
        public List<MenuModel?>?            SystemMenus                          { get; set; } = [];
        public List<MenuModel?>?            DtlsMenus                            { get; set; } = [];



        public List<ODomainusrModel?>?      Users                                { get; set; } = [];
        public ODomainusrModel?             User                                 { get; set; } = new();

        public List<OEmpmasModel?>?         Employees                            { get; set; } = new();



        public int              PageSize            { get; set; } = 25;
        public List<int?>       PageSizes   = new() { 25, 50, 75, 100, null };
    }

   
