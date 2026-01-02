using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface I_20_001_PayDataAccess
    {
        Task _01EmpmasAndEmpRates(EmpratesModel er, string pisdb, string paydb, string conn);
        Task _01Payroll(PaymainhdrModel phdr, List<TbltranModel?> tmptbltrans, string schema, string conn);
        Task _01Empratesdtl_NewPayroll(int payrollgrpId, int empmasId, double rateHR, double rateDay, string paydb, string conn);
        Task _01TmptbltranCoaList(string trn, string acctNumber, string paydb, string conn);
        Task _01TmptbltranEmpList(string trn, int payrollgrpId, int empmasId, string paydb, string conn);
        Task<PaymaindtlsetupModel?> _02PaymainSetup(string schema, string conn);
        Task<SettingsModel?>        _02Settings(string schema, string conn);
        Task<List<TbltranModel?>?>  _02Tmptbltran(string trn, int payrollgrpId, int userId, string paydb, string pisdb, string conn);
        Task _04ByTrn(string trn, string schema, string conn);
    }
}