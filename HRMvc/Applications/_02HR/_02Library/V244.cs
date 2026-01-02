using HRApiLibrary.Models._10_Pis;
using Radzen.Blazor;

namespace HRMvc.Applications._02HR._02Library
{
    public class V244
    {
        public string?                              pisdb                          { get; set; } = string.Empty;
        public string?                              paydb                          { get; set; } = string.Empty;
        public string?                              conn                           { get; set; } = string.Empty;
        public string?                              Action                         { get; set; }  = string.Empty;
        public string?                              Mode                           { get; set; } = "DiA";
        public string?                              Key                            { get; set; } = "Disciplinary";
        public int                                  ActiveTab                      { get; set; } = 0;
        public string?                              Module                         { get; set; } = "Disciplinary";



        public PisEmpmasModel?                      Empmas                          { get; set; } = new();
        public List<PisEmpmasModel?>?               EmpmasListGrid                  { get; set; } = new();
        public RadzenDataGrid<PisEmpmasModel?>?     EmpmasGrid                      { get; set; }



        public DeprecModel                          DeviatedEmployee                { get; set; } = new();
        public List<DeprecModel?>?                  DeviationLineUp                 { get; set; } = new();
        public RadzenDataGrid<DeprecModel?>?        DeviationLineUpGrid             { get; set; }        
        
        public DeprecModel                          InvestigatedEmployee            { get; set; } = new();
        public List<DeprecModel?>?                  InvestigationLineUp             { get; set; } = new();
        public RadzenDataGrid<DeprecModel?>?        InvestigationLineUpGrid         { get; set; }


        public DeprecModel?                         Deprec                          { get; set; } = new();
        public List<RdevdataModel?>?                DevDataList                     { get; set; } = new();


        public TrandeviationModel?                  Trandeviation                   { get; set; } = new();
        public TrandeviationotherModel?             TrandeviationOther              { get; set; } = new();
        public TrandeviationapprovalhistoryModel?   TrandeviationHistory            { get; set; } = new();

        public List<DeviationModel?>?               AllegationsHistoryList          { get; set; } = new();
        public RadzenDataGrid<DeviationModel?>?     AllegationGrid                  { get; set; }

        public DeviationModel?                      Deviation                       { get; set; }
        public EmpblockpostModel?                   EmployeeBlockPost               { get; set; }

        public List<RempstatModel?>?                StatusListGrid                  { get; set; } = new();
        public RadzenDataGrid<RempstatModel?>?      StatusGrid                      { get; set; }


        // ***********************************************************
        // Diciplinary 
        // ***********************************************************
        public bool                             ShowAllegationHistoryModal  { get; set; } = false;
        public bool?                            IsCheckedAll                { get; set; } = false;

        public bool                             ShowErrorMessage            { get; set; } = false;
        public bool                             ShowStatusSettingModal      { get; set; } = false;
        public bool                             SaveDisabled                { get; set; } = true;
        public bool                             PostDisabled                { get; set; } = true;
        public bool                             IsResigned                  { get; set; } = false;

        public TrandisciplinaryapprovalModel?   TrandisciplinaryAppr        { get; set; } = new();
        public TrandisciplinaryModel?           Trandisciplinary            { get; set; } = new();

        public List<RempstatModel?>?            DiscStatusList              { get; set; } = new();


        public List<RpenaltyModel?>?            PenaltyList                 { get; set; } = new();
        public List<RpenaltyModel?>?            PenaltiesForAllegation      { get; set; } = new();

        // ***********************************************************
        // Exonerate
        // ***********************************************************
        public bool                             ExoSaveDisabled             { get; set; } = true;
        public bool                             ExoPostDisabled             { get; set; } = true;
        public bool                             ExoIsCheckedAll             { get; set; } = true;
        public bool                             ShowStatusSettingModalForExonerate { get; set; } = false;

        public bool                             ExoShowErrorMessage         { get; set; } = false;
        public TranexonerateModel?              Tranexonerate               { get; set; } = new();
        public TranexonerateapprovalModel?      TranexonerateAppr           { get; set; } = new();
        public TranexonerateapprovalhistoryModel? TranexonerateApprHist     { get; set; } = new();

        public List<RempstatModel?>?            ExoRempStatusList           { get; set; } = new();
        public List<RempstatModel?>?            ExoRempStatusListGrid       { get; set; } = new();
        public RadzenDataGrid<RempstatModel?>?  ExoRempStatusGrid           { get; set; }


        // ***********************************************************
        // Investigate
        // ***********************************************************
        public bool                             InvSaveDisabled             { get; set; } = true;
        public bool                             InvPostDisabled             { get; set; } = true;
        public bool                             InvUpdateDisabled           { get; set; } = true;
        public bool                             EditInvestigate             { get; set; } = false;
        
        public bool                             ShowStatusSettingModalForInvestigate { get; set; } = false;

        public bool                             InvShowErrorMessage         { get; set; } = false;
        public TraninvestigateModel?            TraninvestigatedDtls        { get; set; } = new();
        public TraninvestigateModel?            Traninvestigate             { get; set; } = new();
        public TraninvestigateapprovalModel?    TranInvestigateAppr         { get; set; } = new();
        public List<RempstatModel?>?            InvRempStatusList           { get; set; } = new();
        public List<RempstatModel?>?            InvRempStatusListGrid       { get; set; } = new();
        public RadzenDataGrid<RempstatModel?>?  InvRempStatusGrid           { get; set; }

    }
}
