using HRApiLibrary.DataAccess._00_CT;
using HRApiLibrary.DataAccess._00_CT.Interfaces;
using HRApiLibrary.DataAccess._00_Login;
using HRApiLibrary.DataAccess._00_Login.Interface;
using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._00_MainTrans;
using HRApiLibrary.DataAccess._00_MainTrans.Interfaces;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Attendance;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._10_Pis.OPis;
using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.DataAccess._20_Pay.DA0605;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._20_Pay.OPay;
using HRApiLibrary.DataAccess._20_Pay.Report;
using HRApiLibrary.DataAccess._20_Pay_Report;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRMvc.Applications._02HR._02Library;

namespace HRMvc.StartupConfig;

public static class ApiExt
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
    }

    public static void AddApiInjectionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<I_90_001_MySqlDataAccess, _90_001_MySqlDataAccess>();
        builder.Services.AddScoped<I_00_001_LoginAccess, _00_001_LoginAccess>();

        builder.Services.AddScoped<I_00MainTblMakerAccess, _00MainTblMakerAccess>();
        builder.Services.AddScoped<I_00MainDataMakerAccess, _00MainDataMakerAccess>();

        builder.Services.AddScoped<I_00MainPisTblMakerAccess, _00MainPisTblMakerAccess>();
        builder.Services.AddScoped<I_00_CTDataAccess, _00_CTDataAccess>();
        builder.Services.AddScoped<IMsdsDataAccess, MsdsDataAccess>();

        //-- Main ---------------------------------------------------------------------
        builder.Services.AddScoped<I_00UsersAccess, _00UsersAccess>();
        builder.Services.AddScoped<I_00MainDA, _00MainDA>();
        builder.Services.AddScoped<ISystemuserDataAccess, SystemuserDataAccess>();


        //-- MainPis ---------------------------------------------------------------------
        builder.Services.AddScoped<I_10_EmpmasDataAccess, _10_EmpmasDataAccess>();
        builder.Services.AddScoped<IEngagementDataAccess, EngagementDataAccess>();
        builder.Services.AddScoped<IDevdataDataAccess, RdevdataDataAccess>();


        //-- Pis ------------------------------------------------------------------------
        builder.Services.AddScoped<IAtttemplateDataAccess, AtttemplateDataAccess>();
        builder.Services.AddScoped<IAttdailyDataAccess, AttdailyDataAccess>();
        builder.Services.AddScoped<IAttpunchesDataAccess, AttpunchesDataAccess>();
        builder.Services.AddScoped<IAttpunches1DataAccess, Attpunches1DataAccess>();
        builder.Services.AddScoped<IEmpmasInternalDataAccess, EmpmasInternalDataAccess>();
        builder.Services.AddScoped<IPissettingsDataAccess, PissettingsDataAccess>();
        builder.Services.AddScoped<ILeavetypeDataAccess, LeavetypeDataAccess>();
        builder.Services.AddScoped<ILeavegrpDataAccess, LeavegrpDataAccess>();
        builder.Services.AddScoped<ILeavedefaultapproverDataAccess, LeavedefaultapproverDataAccess>();
        builder.Services.AddScoped<ILeaveapproverDataAccess, LeaveapproverDataAccess>();
        builder.Services.AddScoped<ILeavegrpapproverDataAccess, LeavegrpapproverDataAccess>();
        builder.Services.AddScoped<ILvcreditDataAccess, LvcreditDataAccess>();
        builder.Services.AddScoped<IDeprecDataAccess, DeprecDataAccess>();
        builder.Services.AddScoped<IEmpblockpostDataAccess, EmpblockpostDataAccess>();
        builder.Services.AddScoped<IEmploymenttypeDataAccess, EmploymenttypeDataAccess>();
        builder.Services.AddScoped<IRdivisionDataAccess, HRApiLibrary.DataAccess._10_Pis.RdivisionDataAccess>();
        builder.Services.AddScoped<IRdepartmentDataAccess, HRApiLibrary.DataAccess._10_Pis.RdepartmentDataAccess>();
        builder.Services.AddScoped<IRsectionDataAccess, HRApiLibrary.DataAccess._10_Pis.RsectionDataAccess>();
        builder.Services.AddScoped<IPositionDataAccess, PositionDataAccess>();
        builder.Services.AddScoped<IRempstatDataAccess, RempstatDataAccess>();
        builder.Services.AddScoped<IRcivstatDataAccess, RcivstatDataAccess>();
        builder.Services.AddScoped<IDeviationDataAccess, DeviationDataAccess>();
        builder.Services.AddScoped<IPisEmpmasDataAccess, PisEmpmasDataAccess>();
        builder.Services.AddScoped<IDeploymodeDataAccess, DeploymodeDataAccess>();
        builder.Services.AddScoped<IEmploytypeDataAccess, EmploytypeDataAccess>();
        builder.Services.AddScoped<IRdepDataAccess, RdepDataAccess>();
        builder.Services.AddScoped<IRdepapproverDataAccess, RdepapproverDataAccess>();
        builder.Services.AddScoped<IRdeploymentDataAccess, RdeploymentDataAccess>();
        builder.Services.AddScoped<ITrandeploymentDataAccess, TrandeploymentDataAccess>();
        builder.Services.AddScoped<ITrandeploymentapprovalDataAccess, TrandeploymentapprovalDataAccess>();
        builder.Services.AddScoped<ITrandeploymentapprovalhistoryDataAccess, TrandeploymentapprovalhistoryDataAccess>();
        builder.Services.AddScoped<ITrandeviationDataAccess, TrandeviationDataAccess>();
        builder.Services.AddScoped<ITrandeviationapprovalDataAccess, TrandeviationapprovalDataAccess>();
        builder.Services.AddScoped<ITrandeviationotherDataAccess, TrandeviationotherDataAccess>();
        builder.Services.AddScoped<ITrandeviationapprovalhistoryDataAccess, TrandeviationapprovalhistoryDataAccess>();
        builder.Services.AddScoped<ITrandisciplinaryDataAccess, TrandisciplinaryDataAccess>();
        builder.Services.AddScoped<ITraninvestigateDataAccess, TraninvestigateDataAccess>();
        builder.Services.AddScoped<ITranexonerateDataAccess, TranexonerateDataAccess>();
        builder.Services.AddScoped<ITrandisciplinaryapprovalDataAccess, TrandisciplinaryapprovalDataAccess>();
        builder.Services.AddScoped<ITraninvestigateapprovalDataAccess, TraninvestigateapprovalDataAccess>();
        builder.Services.AddScoped<ITranexonerateapprovalDataAccess, TranexonerateapprovalDataAccess>();
        builder.Services.AddScoped<ITrandisciplinaryapprovalhistoryDataAccess, TrandisciplinaryapprovalhistoryDataAccess>();
        builder.Services.AddScoped<ITraninvestigateapprovalhistoryDataAccess, TraninvestigateapprovalhistoryDataAccess>();
        builder.Services.AddScoped<ITranexonerateapprovalhistoryDataAccess, TranexonerateapprovalhistoryDataAccess>();
        builder.Services.AddScoped<ITranreinstatementDataAccess, TranreinstatementDataAccess>();
        builder.Services.AddScoped<ITranreinstatementapprovalDataAccess, TranreinstatementapprovalDataAccess>();
        builder.Services.AddScoped<ITranreinstatementapprovalhistoryDataAccess, TranreinstatementapprovalhistoryDataAccess>();

        builder.Services.AddScoped<IEmptranmovementDataAccess, EmptranmovementDataAccess>();
        builder.Services.AddScoped<IParaDataAccess, ParaDataAccess>();
        builder.Services.AddScoped<IDesignationDataAccess, DesignationDataAccess>();
        builder.Services.AddScoped<IPenaltyDataAccess, RpenaltyDataAccess>();
        builder.Services.AddScoped<IAttreqtypeDataAccess, AttreqtypeDataAccess>();
        builder.Services.AddScoped<IAttreqhistDataAccess, AttreqhistDataAccess>();
        builder.Services.AddScoped<IAttreqhdrDataAccess, AttreqhdrDataAccess>();
        builder.Services.AddScoped<IAttreqdtlDataAccess, AttreqdtlDataAccess>();
        builder.Services.AddScoped<IAttdutytypeDataAccess, AttdutytypeDataAccess>();
        builder.Services.AddScoped<ILeaveapplicationDataAccess, LeaveapplicationDataAccess>();
        builder.Services.AddScoped<ILeaveapplicationdtlDataAccess, LeaveapplicationdtlDataAccess>();
        builder.Services.AddScoped<IEmpmasgrpDataAccess, EmpmasgrpDataAccess>();

        builder.Services.AddScoped<IAtttemplatereqdtlDataAccess, AtttemplatereqdtlDataAccess>();
        builder.Services.AddScoped<IAtttemplatereqhdrDataAccess, AtttemplatereqhdrDataAccess>();
        builder.Services.AddScoped<IAtttemplatereqhistDataAccess, AtttemplatereqhistDataAccess>();
        
        builder.Services.AddScoped<IOtdaytypeDataAccess, OtdaytypeDataAccess>();
        builder.Services.AddScoped<IOtdutytypeDataAccess, OtdutytypeDataAccess>();
        builder.Services.AddScoped<IOtreqdtlDataAccess, OtreqdtlDataAccess>();
        builder.Services.AddScoped<IOtreqhdrDataAccess, OtreqhdrDataAccess>();
        builder.Services.AddScoped<IOtreqhistDataAccess, OtreqhistDataAccess>();

        builder.Services.AddScoped<IRempstat_baseDataAccess, Rempstat_baseDataAccess>();
        builder.Services.AddScoped<DA222>();
        /*builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<SessionService>();*/
        
        //-- Accountability ------------------------------------------------------------
        builder.Services.AddScoped<IInvDataAccess, InvDataAccess>();
        builder.Services.AddScoped<IInvdtlDataAccess, InvdtlDataAccess>();
        builder.Services.AddScoped<IInv_typeDataAccess, Inv_typeDataAccess>();
        builder.Services.AddScoped<IInv_brandDataAccess, Inv_brandDataAccess>();
        builder.Services.AddScoped<IInv_categoryDataAccess, Inv_categoryDataAccess>();
        builder.Services.AddScoped<IInv_makeDataAccess, Inv_makeDataAccess>();
        builder.Services.AddScoped<IInv_statusDataAccess, Inv_statusDataAccess>();
        


        //-- PIS Personal ------------------------------------------------------------
        builder.Services.AddScoped<IEmpmovementDataAccess, EmpmovementDataAccess>();
        builder.Services.AddScoped<IMymovementDataAccess, MymovementDataAccess>();
        builder.Services.AddScoped<IOEmpmasDataAccess, OEmpmasDataAccess>();
        builder.Services.AddScoped<IOGenderDataAccess, OGenderDataAccess>();
        builder.Services.AddScoped<IOCivstatDataAccess, OCivstatDataAccess>();
        builder.Services.AddScoped<IOEducateDataAccess, OEducateDataAccess>();
        builder.Services.AddScoped<IOFamilyDataAccess, OFamilyDataAccess>();
        builder.Services.AddScoped<IOParentDataAccess, OParentDataAccess>();
        builder.Services.AddScoped<IOChildrenDataAccess, OChildrenDataAccess>();
        builder.Services.AddScoped<IOEmergencDataAccess, OEmergencDataAccess>();

        builder.Services.AddScoped<IOEmployDataAccess, OEmployDataAccess>();
        builder.Services.AddScoped<IOReferDataAccess, OReferDataAccess>();
        builder.Services.AddScoped<IOTrainDataAccess, OTrainDataAccess>();

        builder.Services.AddScoped<IOProcodeDataAccess, OProcodeDataAccess>();
        builder.Services.AddScoped<IOMlacodeDataAccess, OMlacodeDataAccess>();

        //-- Pay Transaction ------------------------------------------------------------
        builder.Services.AddScoped<I_20_001_PayDataAccess, _20_001_PayDataAccess>();
        
        builder.Services.AddScoped<ITbltranDataAccess, TbltranDataAccess>();
        builder.Services.AddScoped<IEmprateshistDataAccess, EmprateshistDataAccess>();
        builder.Services.AddScoped<ILoanhdrDataAccess, LoanhdrDataAccess>();
        builder.Services.AddScoped<ILoansDataAccess, LoansDataAccess>();
        builder.Services.AddScoped<IUserpayinprocessDataAccess, UserpayinprocessDataAccess>();
        builder.Services.AddScoped<IPaymainhistoryDataAccess, PaymainhistoryDataAccess>();
        builder.Services.AddScoped<IFixedearnings_grpDataAccess, Fixedearnings_grpDataAccess>();
        builder.Services.AddScoped<IDedmandatoryDataAccess, DedmandatoryDataAccess>();
        builder.Services.AddScoped<ISettingsDataAccess, SettingsDataAccess>();
        builder.Services.AddScoped<IPayrollprdDataAccess, PayrollprdDataAccess>();

        
        //-- Pay ------------------------------------------------------------------------
        builder.Services.AddScoped<I_20_002_PayTblMaker, _20_002_PayTblMaker>();
        builder.Services.AddScoped<IEmpratesDataAccess, EmpratesDataAccess>();
        builder.Services.AddScoped<IEmprateshistDataAccess, EmprateshistDataAccess>();
        builder.Services.AddScoped<ICoaDataAccess, CoaDataAccess>();
        builder.Services.AddScoped<IPaymaindtlDataAccess, PaymaindtlDataAccess>();
        builder.Services.AddScoped<IPaymainhdrDataAccess, PaymainhdrDataAccess>();
        builder.Services.AddScoped<IPayrollgrpDataAccess, PayrollgrpDataAccess>();
        builder.Services.AddScoped<IPayrollgrpratesDataAccess, PayrollgrpratesDataAccess>();
        builder.Services.AddScoped<IPayrateDataAccess, PayrateDataAccess>();
        builder.Services.AddScoped<IEmpratesdtlDataAccess, EmpratesdtlDataAccess>();
        builder.Services.AddScoped<IFixedearningsDataAccess, FixedearningsDataAccess>();
        builder.Services.AddScoped<IFixedearnings_grp_empDataAccess, Fixedearnings_grp_empDataAccess>();
        builder.Services.AddScoped<IPaymainvisacctDataAccess, PaymainvisacctDataAccess>();
        builder.Services.AddScoped<IPaytranDataAccess, PaytranDataAccess>();
        builder.Services.AddScoped<IDa605DataAccess, Da605DataAccess>();
        builder.Services.AddScoped<IDutyrenderedDataAccess, DutyrenderedDataAccess>();
        builder.Services.AddScoped<IMatrixpagibigDataAccess, MatrixpagibigDataAccess>();
        builder.Services.AddScoped<IMatrixphicDataAccess, MatrixphicDataAccess>();
        builder.Services.AddScoped<IMatrixsssDataAccess, MatrixsssDataAccess>();
        builder.Services.AddScoped<IMatrixwtaxDataAccess, MatrixwtaxDataAccess>();
        builder.Services.AddScoped<IAttendanceDataAccess, AttendanceDataAccess>();

        //-- Old Pis -----------------------------------------------------------------------
        builder.Services.AddScoped<IOCoinfoDataAccess, OCoinfoDataAccess>();
        builder.Services.AddScoped<IOClientDataAccess, OClientDataAccess>();
        builder.Services.AddScoped<IOEmpstatDataAccess, OEmpstatDataAccess>();
        
        builder.Services.AddScoped<IOPisReportDataAccess, OPisReportDataAccess>();


        //-- Old Pay -----------------------------------------------------------------------
        builder.Services.AddScoped<IOtbltranDataAccess, OtbltranDataAccess>();
        builder.Services.AddScoped<IOTbltrandtlDataAccess, OTbltrandtlDataAccess>();
        builder.Services.AddScoped<IOPaymainhdrDataAccess, OPaymainhdrDataAccess>();
        builder.Services.AddScoped<IOLoansDataAccess, OLoansDataAccess>();
        builder.Services.AddScoped<IOChartofacctDataAccess, OChartofacctDataAccess>();
        builder.Services.AddScoped<IOEmpportalDataAccess, OEmpportalDataAccess>();
        

        //-- Pay Report --------------------------------------------------------------------
        builder.Services.AddScoped<IReportDataAccess, ReportDataAccess>();
        builder.Services.AddScoped<IMainmenuDataAccess, MainmenuDataAccess>();
        builder.Services.AddScoped<IGPayrollReportDataAccess, GPayrollReportDataAccess>();


        //--- Accounting -------------------------------------------------------------------
        builder.Services.AddScoped<I_AcctgTableMaker, _AcctgTableMaker>();
        // builder.Services.AddScoped<IMainmenuDataAccess, IMainmenuDataAccess>();
        
        
    }
}
