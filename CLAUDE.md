# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

HRSeries is an HR / Payroll / Attendance (AMS) / Employee Portal system for Philippine payroll (SSS, PHIC, Pag-IBIG, withholding tax). It is ASP.NET Core (net8.0, SDK pinned to 9.0.308 in `global.json`) on MySQL, accessed with Dapper. There is no ORM.

## Commands

```bash
dotnet build HRSys.sln                 # build everything
dotnet run --project HRMvc             # main app: https://localhost:7065 (Swagger at /swagger)
dotnet run --project HRSys             # standalone API host: https://localhost:7101
```

- `HRApiLibrary.Tests` contains no test files, so there are no tests to run.
- `Directory.Build.props` sets `NoWarn=*`, so warnings are hidden. Treat compiler errors as the only build signal.

## Solution layout

- **HRMvc**: the main web app. It hosts MVC controllers and views, Razor pages, Blazor Server components (Radzen, Telerik UI, Blazorise) and Telerik/FastReport reporting. It also exposes the API controllers from `HRApiLibrary` through `app.MapControllers()`. `ApiAddress` in appsettings (`https://localhost:7065/api/`) points back at HRMvc itself.
- **HRApiLibrary**: the shared class library. It holds data access, models, API controllers, and `Modules/` (feature-specific DA plus model bundles, e.g. `_11003O`, `_12006O`).
- **HRSys** (`HRApi.csproj`): a thin API host over HRApiLibrary with its own `StartupConfig/DependencyExt.cs`.

## Numeric module codes

Names across the code are prefixed by domain or module number. Use these to find related code:

- Data/model domains in HRApiLibrary: `_00_Main`/`_00_MainTrans`/`_00_Login`/`_00_CT` (system, users, companies), `_10_Pis` (personnel info, employee master `Empmas`), `_11_AMS` (attendance), `_20_Pay`/`_20_Pay_Report` (payroll), `_90_Utils` (SQL helpers, session, globals).
- UI modules in HRMvc: `Controllers/_NNController.cs` maps `[Route("NN")]` with sub-codes to views. For example, `_12Controller` maps `/12/{code}` through a `Dictionary` to `~/Applications/_12EmployeeProfile/Pages/_12_{code}_*.cshtml`. A few routes, like `/12/102`, are explicit actions that point at `Applications/EPortal/Pages/`.
- `O`-prefixed types (`OEmpmas`, `OPis`, `OPay`, `uc.OpisDb`) refer to the **old/legacy** PIS/Payroll databases.

## Data access pattern

- Every DA class has an interface in a sibling `Interface/` folder. Each class injects `I_90_001_MySqlDataAccess` and calls `FetchData<T,U>(sql, params, connName)` or `ExecuteCmd(...)`.
- Method names follow a CRUD numbering: `_01` = insert/upsert, `_02*` = select (e.g. `_02ByType`), `_03*` = update, `_04` = delete.
- Most methods take `(…, string? schema, string? conn)`. SQL is written as an interpolated string with `{schema}.Table`. Values always go in as Dapper `@Param` placeholders. The `conn` argument is a **connection-string name** that is looked up in `ConnectionStrings`, not a raw connection string. An overload takes a `db` argument and appends `database=` to the connection string.
- The DB is multi-tenant by schema. Fixed schemas are `Main`, `MainPis` and `MainPay`. Each company has its own Pis/Pay/Ams/Acctg/Applicant schemas.

## Per-user context (claims)

After cookie login (`/login`, with a global fallback policy that requires an authenticated user), the claims carry the user's schemas and connection names. Call `PayClaimsHelper.UserToClaims(User)` (`HRMvc/Applications/PayrollTransaction/Vars/`) to get a `UserClaimsModel` with `SchemaUserPis`, `SchemaUserPay`, `SchemaUserAms`, `Conn`, `OpisDb`, `EmpmasId`, `IsExclusiveCompany` and so on. The schema and conn arguments for DA calls come from this model. `UserClaimsContextService` is the injectable equivalent.

## Page composition (EPortal / Employee Profile)

Typical flow:
1. A controller action returns a `.cshtml` under `HRMvc/Applications/<Area>/Pages/` that uses `_EPortalLayout.cshtml` or another shared layout.
2. The page builds `uc` from claims. It then calls a "Library" service (`HRMvc/StartupConfig/Library/L12*.cs`) that collects the DA results into a view-model (e.g. `V12_102`, defined under `Applications/*/Vars`).
3. It renders a Blazor component with `Html.RenderComponentAsync<…>(RenderMode.ServerPrerendered, new { UserClaims = uc, ... })`. Components live under `Applications/<Area>/Blazor/_NN_NNN/`, with child components in `_NN_NNN_Child/`.

## DI registration

There is no assembly scanning. New DA classes or services must be registered by hand, usually as `AddScoped<IFoo, Foo>()`, in one of these places:
- `HRMvc/StartupConfig/DependencyExt.cs` (`AddInjectServices`): core and main DA
- `HRMvc/StartupConfig/ApiExt.cs` (`AddApiInjectionServices`): most DA used by API and pages
- `HRMvc/StartupConfig/HRMvcScope.cs` (`AddHRMvcScope`): `L12*` library services, AMS DA, `Modules/*` DA
- `HRSys/StartupConfig/DependencyExt.cs`: only if the standalone API host needs the service

## Configuration

`HRMvc/appsettings.json` holds the `ConnectionStrings` (`MySqlConn`, `MySqlConnNoDb`, …), `Schema` names, and `CompanyInfo` (`Exclusive`, `DefaultCoId`, `DeploymentCaption`). Some flows have "Exclusive" and "NonExclusive" variants (e.g. `L12_103` vs `L12_103NonExclusive`, `PisModules/Vars/NonExclusive`), chosen by the company's exclusive flag. Data-protection keys are persisted to `HRMvc/keys`.
