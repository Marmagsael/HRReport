using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IPayrollgrpDataAccess
    {
        Task<PayrollgrpModel?>              _01(PayrollgrpModel payrollgrp, string schema, string conn);
        Task<PayrollgrpModel?>              _02(int id, string schema, string conn);
        Task<List<PayrollgrpModel?>?>       _02(string schema, string conn);
        Task<List<PayrollgrpModel?>?>       _02Active(string schema, string conn);
        Task<List<PayrollgrpModel?>?>       _02ByCode(string code, string schema, string conn); 
        Task<List<TbltranModel?>?>          _02CheckToTblTran(string clNumber, string schema, string conn);
        Task<List<PayrollgrpModel?>?>       _02Dashboard(string schema, string conn);
        Task<List<PayrollgrpModel?>?>       _02PayDashboard(string paydb, string pisdb, string conn); 
        Task<PayrollgrpModel?>              _03(int id, PayrollgrpModel payrollgrp, string schema, string conn);
        Task<PayrollgrpModel?>              _04(int id, string schema, string conn);
    }
}