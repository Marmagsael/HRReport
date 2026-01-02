using HRApiLibrary.DataAccess._00_Login;
using HRApiLibrary.DataAccess._00_Login.Interface;
using HRApiLibrary.DataAccess._00_Main;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._10_Pis;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._20_Pay;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace MysqlApi.StartupConfig;

public static class DependencyExt
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddSwaggerGen();

    }

    public static void AddInjectionServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<I_90_001_MySqlDataAccess, _90_001_MySqlDataAccess>();
        builder.Services.AddSingleton<I_00_001_LoginAccess,     _00_001_LoginAccess>();

        builder.Services.AddSingleton<I_00MainTblMakerAccess,   _00MainTblMakerAccess>();
        builder.Services.AddSingleton<I_00MainDataMakerAccess,  _00MainDataMakerAccess>();

        builder.Services.AddSingleton<I_00MainPisTblMakerAccess, _00MainPisTblMakerAccess>();

        //-- Main ---------------------------------------------------------------------
        builder.Services.AddSingleton<I_00UsersAccess, _00UsersAccess>();
        builder.Services.AddSingleton<I_00MainDA,      _00MainDA>();
        builder.Services.AddSingleton<I_00CountryDataAccess, _00CountryDataAccess>();

        //-- MainPis ---------------------------------------------------------------------
        builder.Services.AddSingleton<I_10_EmpmasDataAccess, _10_EmpmasDataAccess>();


        //-- Pis ------------------------------------------------------------------------


        //-- Pay ------------------------------------------------------------------------
        builder.Services.AddSingleton<I_20_002_PayTblMaker, _20_002_PayTblMaker>(); 

    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(p => p.AddPolicy("FreeForAll", build =>
        {
            build.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
        }));

    }


    public static void AddAuthenticationServices(this WebApplicationBuilder builder)
    {

    }


}
