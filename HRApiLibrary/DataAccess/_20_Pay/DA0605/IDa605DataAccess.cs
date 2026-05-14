using HRApiLibrary.Models._20_Pay;
using HRApiLibrary.Models._20_Pay.M0605;

namespace HRApiLibrary.DataAccess._20_Pay.DA0605
{
    public interface IDa605DataAccess
    {
        Task _01CreatePayroll(Model605? m605);

        Task<Model605> _01EmpByCurrPayrollgrp(string? trn, int payrollgrpId, int empmasId, string? empNumber, double rate, string? paydb,
            string? pisdb, string? conn, int userId);

        Task<Model605> _01EmpAssignedRates(string? trn, EmpratesModel empRates, int userId, string? paydb,
            string? pisdb, string? conn);

        Task<Model605> _01EmpByPayrollgrpRate(string? trn, int payrollgrpId, int empmasId, string empNumber, int userId,
            string? paydb, string? pisdb, string? conn);

        Task _01_FE_to_Tmptbltran(string trn, string fldName, string? paydb, string pisdb, int idUser,
            string? conn);

        Task _01_FEG_to_Tmptbltran(string trn, string fldName, int payrollgrpId, string? paydb, string pisdb, int idUser,
            SettingsModel s, string? conn);

        Task _01_Account_to_Tmptbltran(string trn, string acctNumber, string source, int idUser, string? paydb,
            string pisdb, string? conn);
        Task _01_Loans(string trn, int idUser, string source, string? paydb, string pisdb, string? conn);
        Task _01_MandatoryDeduction(string trn, int idUser, string? paydb, string? conn, string source = "MDed");
        Task _01_PostTrn(string trn, int idUser, string? paydb, string? conn); 
       
        Task<Model605> _02TmpPayroll(string? trn, string? paydb, string? pisdb, string? conn); 
        Task<Model605> _02NewPayroll(string? trn, string? paydb, string? pisdb, string? conn);

        Task<List<EmpratesModel>> _02EmployeeToAdd(string skey, string? trn, int payrollgrpId, string? paydb,
            string? pisdb, string? conn);

        Task<List<CoaModel?>?> _02PrdAccts_ByTrn(string trn, string? paydb, string? conn);
        Task<List<DeprecsettingsModel>> _02TaxableEmployees(string trn, string? paydb, string? conn);
        Task<List<DeprecsettingsModel>> _02TaxableEmployees_per_Trn(string trn, string? paydb, string? conn);
        Task<List<DeprecsettingsModel>> _02WithPremEmployees_per_Trn(string trn, string? paydb, string? conn,
            string premSw = "wSSS");
        Task<List<LoansModel?>?> _02LoansCheck(string? trn, string? paydb, string? conn);
        Task<List<DedmandatorytranModel?>?> _02MandatoryDedutionCheck(string? trn, string? paydb, string? conn); 
        
        Task _03TmpTbltran(TbltranModel tbltran, string? paydb, string? conn); 
        Task _03PaymainhdrCoverage(PaymainhdrModel paymainhdr, string? paydb, string? conn); 
        Task _04PayrollDtlRecord(string trn, int empmasId, string? paydb, string? conn); 
        Task _04UnlockedPayroll(string trn, string? paydb, string? conn);
        Task<Model605> _04Tmp_Employee(string trn, int empmasId, string? paydb, string pisdb, string? conn);
        Task<Model605> _04Acct_per_Trn(string trn, string acctNumber, string? paydb, string? pisdb, string? conn);
        Task _04Deductions_per_Trn(string trn, string? paydb, string? conn); 
    }
}