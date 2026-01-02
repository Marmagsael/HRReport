using HRApiLibrary.Models._10_Pis;
using Radzen.Blazor;

namespace HRMvc.Applications._02HR._02Library
{
    public class V243
    {
        public string?  pisdb                       { get; set; } = string.Empty;
        public string?  paydb                       { get; set; } = string.Empty;
        public string?  conn                        { get; set; } = string.Empty;
        public string?  Action                      { get; set; } = string.Empty;
        public string?  Mode                        { get; set; } = "DEV";
        public string?  Key                         { get; set; } = "Deviation";


        public bool     ShowSearchModal             { get; set; } = false;
        public bool     ShowStatusSettingModal      { get; set; } = false;
        public bool     IsLoading                   { get; set; } = false;

        public string   Empnumber                   { get; set; } = string.Empty;
        public bool     IsCheckedAll                { get; set; } = false;

        public bool     SaveDisabled                { get; set; } = false;
        public bool     PostDisabled                { get; set; } = false;
        public bool     PrintDisabled               { get; set; } = false;
        public bool     ShowErrorMessage            { get; set; } = false;

      


        public List<Rempstat_baseModel?>?        DefaultStatusList              { get; set; }

        public List<RdevdataModel?>?             AllegationList                 { get; set; }
        public List<DeviationModel?>?           EmployeeAllegedViolations      { get; set; }


        public List<DeprecModel?>?              QualifiedEmployeeList           { get; set; }   
        public List<DeprecModel?>?              QualifiedEmployeeListGrid       { get; set; }   
        public RadzenDataGrid<DeprecModel?>?    QualifiedEmployeesGrid          { get; set; }
        public DeprecModel?                     Employee                        { get; set; }



        public List<RempstatModel?>?            RempStatusList                  { get; set; }
        public List<RempstatModel?>?            RempStatusListGrid              { get; set; }
        public RadzenDataGrid<RempstatModel?>?  RempStatusGrid                  { get; set; }

        public DeviationModel?                  Deviation                        { get; set; }


        public TrandeviationapprovalModel?      TrandeviationAppr               { get; set; } = new();
        public TrandeviationModel?              Trandeviation                   { get; set; }



    }
}
