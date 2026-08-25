using Blazorise;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications._12EmployeeProfile.Vars
{
    public class V12_002Model
    {

        public string   OPisdb      { get; set; }       = string.Empty;
        public string   OPaydb      { get; set; }       = string.Empty;
        public string   Maindb      { get; set; }       = string.Empty;
        public string   Conn        { get; set; }       = string.Empty;
        public string   Action      { get; set; }       = string.Empty;
        public bool     IsLoading   { get; set; }       = true;
        public bool     UcLoaded    { get; set; }       = false;





        public string? SelectedUser { get; set; } = string.Empty;
        public string? SelectedMenu { get; set; } = string.Empty;



        //List ---------------------
        public List<UsersModel?>?        Users      { get; set; } = [];
        public List<MenuTypeModel?>?     MenuTypes  { get; set; } = []; 


       
    }

    public class MenuTypeModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
