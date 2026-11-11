using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._11_AMS;
using HRApiLibrary.Modules._11003AME;
using HRApiLibrary.Modules._11003O;
using HRApiLibrary.Modules._12005O;
using HRApiLibrary.Modules._2306O;
using HRApiLibrary.Modules._2310O;
using HRApiLibrary.Modules._12008O;
using HRApiLibrary.Modules._11000O;
using HRApiLibrary.Modules._552O;
using HRApiLibrary.Modules._553O;
using HRApiLibrary.Modules._554O;
using HRApiLibrary.Modules._555O;
using HRApiLibrary.Modules._503O;
using HRApiLibrary.Modules._1303O;
using HRApiLibrary.Modules._1306O;
using HRApiLibrary.Modules._1304O;
using HRApiLibrary.Modules._1305O;
using HRApiLibrary.Modules._1302O;
using HRApiLibrary.Modules._13002O;
using HRApiLibrary.Modules._13003O;
using HRApiLibrary.Modules._2308O;
using HRApiLibrary.Modules._602O;
using HRApiLibrary.Modules._603O;
using HRApiLibrary.Modules._604O;
using HRApiLibrary.Modules._605O;
using HRApiLibrary.Modules._12006O;
using HRMvc.StartupConfig.Library;

namespace HRMvc.StartupConfig;

public static class HRMvcScope
{
    public static IServiceCollection AddHRMvcScope(this IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // kung kailangan
        services.AddScoped<SessionService>();
        services.AddScoped<UserClaimsContextService>();
        services.AddScoped<PisAccessService>();
        services.AddScoped<AmsAccessService>();
        services.AddScoped<PayAccessService>();
        services.AddScoped<ClientTerminalService>();
        services.AddScoped<HRMvc.Applications._01Register.Library.L01CompanySetup>();
        services.AddScoped<L12_101>();
        services.AddScoped<L12_102>();
        services.AddScoped<L12_103>();
        services.AddScoped<L12_103NonExlusive>();
        services.AddScoped<L12_120>();
        services.AddScoped<L12>();


        //--- AMS -------------------------------------------------------------------
        services.AddScoped<IAMSTableMaker, AMSTableMaker>();
        services.AddScoped<IAtttemplateDataAccess, AtttemplateDataAccess>();
        services.AddScoped<IAttscheddefaultDataAccess, AttscheddefaultDataAccess>();
        services.AddScoped<IAttscheddailyDataAccess, AttscheddailyDataAccess>();
        services.AddScoped<IAttschedweeklydtlDataAccess, AttschedweeklydtlDataAccess>();
        services.AddScoped<IAttschedweeklyhdrDataAccess, AttschedweeklyhdrDataAccess>();
        services.AddScoped<IAttadvancescheduleDataAccess, AttadvancescheduleDataAccess>();
        services.AddScoped<IDA_11003AME, DA_11003AME>();
        services.AddScoped<IDA_11003O, DA_11003O>();
        services.AddScoped<IDA_12005O, DA_12005O>();
        services.AddScoped<IDA_2306O, DA_2306O>();
        services.AddScoped<IDA_2310O, DA_2310O>();
        services.AddScoped<IDA_12008O, DA_12008O>();
        services.AddScoped<IDA_11000O, DA_11000O>();
        services.AddScoped<IDA_552O, DA_552O>();
        services.AddScoped<IDA_553O, DA_553O>();
        services.AddScoped<IDA_554O, DA_554O>();
        services.AddScoped<IDA_555O, DA_555O>();
        services.AddScoped<IDA_503O, DA_503O>();
        services.AddScoped<IDA_1303O, DA_1303O>();
        services.AddScoped<IDA_1306O, DA_1306O>();
        services.AddScoped<IDA_1304O, DA_1304O>();
        services.AddScoped<IDA_1305O, DA_1305O>();
        services.AddScoped<IDA_1302O, DA_1302O>();
        services.AddScoped<IDA_13002O, DA_13002O>();
        services.AddScoped<IDA_13003O, DA_13003O>();
        services.AddScoped<IDA_2308O, DA_2308O>();
        services.AddScoped<IDA_602O, DA_602O>();
        services.AddScoped<IDA_603O, DA_603O>();
        services.AddScoped<IDA_604O, DA_604O>();
        services.AddScoped<IDA_605O, DA_605O>();
        services.AddScoped<IAms_otsettingsDataAccess, Ams_otsettingsDataAccess>();
        services.AddScoped<IBiologDataAccess, BiologDataAccess>();
        services.AddScoped<IBioManHourDataAccess, BioManHourDataAccess>();
        services.AddScoped<IBioManHourHdrDataAccess, BioManHourHdrDataAccess>();
        services.AddScoped<IDA_12006O, DA_12006O>();


        return services;
    }
}