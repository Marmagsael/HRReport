using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._11_AMS;
using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.Models._00_Main;
using HRMvc.Applications._01Register.Vars;
using HRMvc.Models.Main;

namespace HRMvc.Applications._01Register.Library;

// -- 01 Company setup : shared by Register as Company Owner and Create New Company (My Engagement) --
public class L01CompanySetup
{
    public const string SupportedCountry = "PHL";     // Main.Country.Code; other countries are not available yet

    private readonly I_00MainDA                 _mainDA;
    private readonly IConfiguration             _config;
    private readonly I_00RegisterDataAccess     _register;
    private readonly I_00MainPisTblMakerAccess  _pisTblMaker;
    private readonly IAMSTableMaker             _amsTblMaker;
    private readonly I_20_002_PayTblMaker       _payTblMaker;
    private readonly IOldPis_TblMakerDataAccess _oldPisTblMaker;
    private readonly IOldPay_TblMakerDataAccess _oldPayTblMaker;
    private readonly I_AcctgTableMaker          _acctgTblMaker;

    public L01CompanySetup(I_00MainDA mainDA, IConfiguration config, I_00RegisterDataAccess register,
                           I_00MainPisTblMakerAccess pisTblMaker, IAMSTableMaker amsTblMaker, I_20_002_PayTblMaker payTblMaker,
                           IOldPis_TblMakerDataAccess oldPisTblMaker, IOldPay_TblMakerDataAccess oldPayTblMaker,
                           I_AcctgTableMaker acctgTblMaker)
    {
        _mainDA         = mainDA;
        _config         = config;
        _register       = register;
        _pisTblMaker    = pisTblMaker;
        _amsTblMaker    = amsTblMaker;
        _payTblMaker    = payTblMaker;
        _oldPisTblMaker = oldPisTblMaker;
        _oldPayTblMaker = oldPayTblMaker;
        _acctgTblMaker  = acctgTblMaker;
    }

    private string Conn     => _config.GetSection("Schema:DefConn").Value!;
    private string Main     => _config.GetSection("Schema:Main").Value!;
    private string MainPis  => _config.GetSection("Schema:MainPis").Value!;


    // -- Lookups ------------------------------------------------------------------------
    public async Task<List<CountryModel?>> Countries()
    {
        return await _mainDA._02CountryList(Main, Conn);
    }

    public async Task<List<CurrencyModel?>> Currencies()
    {
        return await _mainDA._02CurrencyList(Main, Conn);
    }

    public async Task<CountryModel?> Country(int? id)
    {
        return await _mainDA._02Country(id, Main, Conn);
    }

    // -- Company code already used by another company --
    public async Task<bool> CodeTaken(string companyCode)
    {
        return await _register._02CodeTaken(companyCode, Main, Conn);
    }

    // -- Logged-in user's name from MainPis.Empmas (Id = users.Id), used as the owner of a new company --
    public async Task<RegisterOwnerModel?> OwnerByUserId(int userId)
    {
        return await _register._02OwnerByUserId(userId, Main, MainPis, Conn);
    }


    // -- Current company (users.DefaultCoId) when the user is its approved Owner, else null --
    public async Task<V01CompanyLookup?> CurrentAdminCompany(int userId)
    {
        return await _register._02CurrentAdminCompany(userId, Main, Conn);
    }

    // -- Company schemas : default PIS (AMS in it for now), default Pay + PH payroll, old PIS, old Pay, accounting --
    // -- then the owner in the company's own Empmas and in the old PIS Empmas ------------
    public async Task CreateSchemas(RegisterOwnerModel owner)
    {
        await _pisTblMaker._01MainPisTableInternal(owner.PISSchema, Conn);
        await _pisTblMaker._01MainPisTable(owner.PISSchema, Conn);
        await _amsTblMaker._01AMSTable(owner.AMSSchema, Conn);
        await _payTblMaker._01(owner.PaySchema, "PH", Conn);
        await _payTblMaker._01PH(owner.PaySchema, "PH", Conn);
        await _oldPisTblMaker._01OldPisTable(owner.OldPis, Conn);
        await _oldPisTblMaker._01OldPisDefaults(owner.OldPis, MainPis, Conn);        // masterfile rows from MainPis
        await _oldPayTblMaker._01OldPayTable(owner.OldPay, Conn);
        await _acctgTblMaker.AccountingTableMaker(owner.AcctgSchema, Conn);      // was made on the first login before

        await _register._01OwnerEmpmas(owner, Main, Conn);
        await _register._01OwnerOldEmpmas(owner, Conn);
    }
}
