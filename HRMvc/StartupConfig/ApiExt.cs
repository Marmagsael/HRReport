using HRApiLibrary.DataAccess._00_CT;
using HRApiLibrary.DataAccess._00_CT.Interfaces;
using HRApiLibrary.DataAccess._00_Login;
using HRApiLibrary.DataAccess._00_Login.Interface;
using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._00_MainTrans;
using HRApiLibrary.DataAccess._00_MainTrans.Interfaces;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.DataAccess._20_Pay.DA0605;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._20_Pay.Report;
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
        builder.Services.AddSingleton<I_90_001_MySqlDataAccess, _90_001_MySqlDataAccess>();
        builder.Services.AddScoped<I_00_001_LoginAccess, _00_001_LoginAccess>();

        builder.Services.AddSingleton<I_00MainTblMakerAccess, _00MainTblMakerAccess>();
        builder.Services.AddSingleton<I_00MainDataMakerAccess, _00MainDataMakerAccess>();

        builder.Services.AddSingleton<I_00MainPisTblMakerAccess, _00MainPisTblMakerAccess>();
        builder.Services.AddSingleton<I_00_CTDataAccess, _00_CTDataAccess>();
        builder.Services.AddSingleton<IMsdsDataAccess, MsdsDataAccess>();

        //-- Main ---------------------------------------------------------------------
        builder.Services.AddSingleton<I_00UsersAccess, _00UsersAccess>();
        builder.Services.AddSingleton<I_00MainDA, _00MainDA>();
        builder.Services.AddSingleton<ISystemuserDataAccess, SystemuserDataAccess>();


        //-- MainPis ---------------------------------------------------------------------
        builder.Services.AddSingleton<I_10_EmpmasDataAccess, _10_EmpmasDataAccess>();
        builder.Services.AddSingleton<IEngagementDataAccess, EngagementDataAccess>();
        builder.Services.AddSingleton<IDevdataDataAccess, RdevdataDataAccess>();


        //-- Pis ------------------------------------------------------------------------
        builder.Services.AddSingleton<IAtttemplateDataAccess, AtttemplateDataAccess>();
        builder.Services.AddSingleton<IAttdailyDataAccess, AttdailyDataAccess>();
        builder.Services.AddSingleton<IAttpunchesDataAccess, AttpunchesDataAccess>();
        builder.Services.AddSingleton<IEmpmasInternalDataAccess, EmpmasInternalDataAccess>();
        builder.Services.AddSingleton<IPissettingsDataAccess, PissettingsDataAccess>();
        builder.Services.AddSingleton<ILeavetypeDataAccess, LeavetypeDataAccess>();
        builder.Services.AddSingleton<ILeavegrpDataAccess, LeavegrpDataAccess>();
        builder.Services.AddSingleton<ILeavedefaultapproverDataAccess, LeavedefaultapproverDataAccess>();
        builder.Services.AddSingleton<ILeaveapproverDataAccess, LeaveapproverDataAccess>();
        builder.Services.AddSingleton<ILeavegrpapproverDataAccess, LeavegrpapproverDataAccess>();
        builder.Services.AddSingleton<IDeprecDataAccess, DeprecDataAccess>();
        builder.Services.AddSingleton<IEmpblockpostDataAccess, EmpblockpostDataAccess>();
        builder.Services.AddSingleton<IEmploymenttypeDataAccess, EmploymenttypeDataAccess>();
        builder.Services.AddSingleton<IRdivisionDataAccess, HRApiLibrary.DataAccess._10_Pis.RdivisionDataAccess>();
        builder.Services.AddSingleton<IRdepartmentDataAccess, HRApiLibrary.DataAccess._10_Pis.RdepartmentDataAccess>();
        builder.Services.AddSingleton<IRsectionDataAccess, HRApiLibrary.DataAccess._10_Pis.RsectionDataAccess>();
        builder.Services.AddSingleton<IPositionDataAccess, PositionDataAccess>();
        builder.Services.AddSingleton<IRempstatDataAccess, RempstatDataAccess>();
        builder.Services.AddSingleton<IRcivstatDataAccess, RcivstatDataAccess>();
        builder.Services.AddSingleton<IDeviationDataAccess, DeviationDataAccess>();
        builder.Services.AddSingleton<IPisEmpmasDataAccess, PisEmpmasDataAccess>();
        builder.Services.AddSingleton<IDeploymodeDataAccess, DeploymodeDataAccess>();
        builder.Services.AddSingleton<IEmploytypeDataAccess, EmploytypeDataAccess>();
        builder.Services.AddSingleton<IRdepDataAccess, RdepDataAccess>();
        builder.Services.AddSingleton<IRdepapproverDataAccess, RdepapproverDataAccess>();
        builder.Services.AddSingleton<IRdeploymentDataAccess, RdeploymentDataAccess>();
        builder.Services.AddSingleton<ITrandeploymentDataAccess, TrandeploymentDataAccess>();
        builder.Services.AddSingleton<ITrandeploymentapprovalDataAccess, TrandeploymentapprovalDataAccess>();
        builder.Services.AddSingleton<ITrandeploymentapprovalhistoryDataAccess, TrandeploymentapprovalhistoryDataAccess>();
        builder.Services.AddSingleton<ITrandeviationDataAccess, TrandeviationDataAccess>();
        builder.Services.AddSingleton<ITrandeviationapprovalDataAccess, TrandeviationapprovalDataAccess>();
        builder.Services.AddSingleton<ITrandeviationotherDataAccess, TrandeviationotherDataAccess>();
        builder.Services.AddSingleton<ITrandeviationapprovalhistoryDataAccess, TrandeviationapprovalhistoryDataAccess>();
        builder.Services.AddSingleton<ITrandisciplinaryDataAccess, TrandisciplinaryDataAccess>();
        builder.Services.AddSingleton<ITraninvestigateDataAccess, TraninvestigateDataAccess>();
        builder.Services.AddSingleton<ITranexonerateDataAccess, TranexonerateDataAccess>();
        builder.Services.AddSingleton<ITrandisciplinaryapprovalDataAccess, TrandisciplinaryapprovalDataAccess>();
        builder.Services.AddSingleton<ITraninvestigateapprovalDataAccess, TraninvestigateapprovalDataAccess>();
        builder.Services.AddSingleton<ITranexonerateapprovalDataAccess, TranexonerateapprovalDataAccess>();
        builder.Services.AddSingleton<ITrandisciplinaryapprovalhistoryDataAccess, TrandisciplinaryapprovalhistoryDataAccess>();
        builder.Services.AddSingleton<ITraninvestigateapprovalhistoryDataAccess, TraninvestigateapprovalhistoryDataAccess>();
        builder.Services.AddSingleton<ITranexonerateapprovalhistoryDataAccess, TranexonerateapprovalhistoryDataAccess>();
        builder.Services.AddSingleton<ITranreinstatementDataAccess, TranreinstatementDataAccess>();
        builder.Services.AddSingleton<ITranreinstatementapprovalDataAccess, TranreinstatementapprovalDataAccess>();
        builder.Services.AddSingleton<ITranreinstatementapprovalhistoryDataAccess, TranreinstatementapprovalhistoryDataAccess>();

        builder.Services.AddSingleton<IEmptranmovementDataAccess, EmptranmovementDataAccess>();
        builder.Services.AddSingleton<IParaDataAccess, ParaDataAccess>();
        builder.Services.AddSingleton<IDesignationDataAccess, DesignationDataAccess>();
        builder.Services.AddSingleton<IPenaltyDataAccess, RpenaltyDataAccess>();

        builder.Services.AddSingleton<IRempstat_baseDataAccess, Rempstat_baseDataAccess>();
        
        builder.Services.AddSingleton<DA222>();
        
        //-- Accountability ------------------------------------------------------------
        builder.Services.AddSingleton<IInvDataAccess, InvDataAccess>();
        builder.Services.AddSingleton<IInvdtlDataAccess, InvdtlDataAccess>();
        builder.Services.AddSingleton<IInv_typeDataAccess, Inv_typeDataAccess>();
        builder.Services.AddSingleton<IInv_brandDataAccess, Inv_brandDataAccess>();
        builder.Services.AddSingleton<IInv_categoryDataAccess, Inv_categoryDataAccess>();
        builder.Services.AddSingleton<IInv_makeDataAccess, Inv_makeDataAccess>();
        builder.Services.AddSingleton<IInv_statusDataAccess, Inv_statusDataAccess>();
        


        //-- PIS Personal ------------------------------------------------------------
        builder.Services.AddSingleton<IEmpmovementDataAccess, EmpmovementDataAccess>();
        builder.Services.AddSingleton<IMymovementDataAccess, MymovementDataAccess>();

        //-- Pay Transaction ------------------------------------------------------------
        builder.Services.AddSingleton<I_20_001_PayDataAccess, _20_001_PayDataAccess>();
        
        builder.Services.AddSingleton<ITbltranDataAccess, TbltranDataAccess>();
        builder.Services.AddSingleton<IEmprateshistDataAccess, EmprateshistDataAccess>();
        builder.Services.AddSingleton<ILoanhdrDataAccess, LoanhdrDataAccess>();
        builder.Services.AddSingleton<ILoansDataAccess, LoansDataAccess>();
        builder.Services.AddSingleton<IUserpayinprocessDataAccess, UserpayinprocessDataAccess>();
        builder.Services.AddSingleton<IPaymainhistoryDataAccess, PaymainhistoryDataAccess>();
        builder.Services.AddSingleton<IFixedearnings_grpDataAccess, Fixedearnings_grpDataAccess>();
        builder.Services.AddSingleton<IDedmandatoryDataAccess, DedmandatoryDataAccess>();
        builder.Services.AddSingleton<ISettingsDataAccess, SettingsDataAccess>();
        builder.Services.AddSingleton<IPayrollprdDataAccess, PayrollprdDataAccess>();

        
        //-- Pay ------------------------------------------------------------------------
        builder.Services.AddSingleton<I_20_002_PayTblMaker, _20_002_PayTblMaker>();
        builder.Services.AddSingleton<IEmpratesDataAccess, EmpratesDataAccess>();
        builder.Services.AddSingleton<IEmprateshistDataAccess, EmprateshistDataAccess>();
        builder.Services.AddSingleton<ICoaDataAccess, CoaDataAccess>();
        builder.Services.AddSingleton<IPaymaindtlDataAccess, PaymaindtlDataAccess>();
        builder.Services.AddSingleton<IPaymainhdrDataAccess, PaymainhdrDataAccess>();
        builder.Services.AddSingleton<IPayrollgrpDataAccess, PayrollgrpDataAccess>();
        builder.Services.AddSingleton<IPayrollgrpratesDataAccess, PayrollgrpratesDataAccess>();
        builder.Services.AddSingleton<IPayrateDataAccess, PayrateDataAccess>();
        builder.Services.AddSingleton<IEmpratesdtlDataAccess, EmpratesdtlDataAccess>();
        builder.Services.AddSingleton<IFixedearningsDataAccess, FixedearningsDataAccess>();
        builder.Services.AddSingleton<IFixedearnings_grp_empDataAccess, Fixedearnings_grp_empDataAccess>();
        builder.Services.AddSingleton<IPaymainvisacctDataAccess, PaymainvisacctDataAccess>();
        builder.Services.AddSingleton<IPaytranDataAccess, PaytranDataAccess>();
        builder.Services.AddSingleton<IDa605DataAccess, Da605DataAccess>();
        builder.Services.AddSingleton<IDutyrenderedDataAccess, DutyrenderedDataAccess>();
        builder.Services.AddSingleton<IMatrixpagibigDataAccess, MatrixpagibigDataAccess>();
        builder.Services.AddSingleton<IMatrixphicDataAccess, MatrixphicDataAccess>();
        builder.Services.AddSingleton<IMatrixsssDataAccess, MatrixsssDataAccess>();
        builder.Services.AddSingleton<IMatrixwtaxDataAccess, MatrixwtaxDataAccess>();
        
        //-- Pay Report --------------------------------------------------------------------
        builder.Services.AddSingleton<IReportDataAccess, ReportDataAccess>();
        builder.Services.AddSingleton<IMainmenuDataAccess, MainmenuDataAccess>();


        //--- Accounting -------------------------------------------------------------------
        builder.Services.AddSingleton<I_AcctgTableMaker, _AcctgTableMaker>();
        // builder.Services.AddSingleton<IMainmenuDataAccess, IMainmenuDataAccess>();
        
        
       
        
    }
}
