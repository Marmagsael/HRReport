using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.Interface
{
    public interface IEmpratesDataAccess
    {
        Task<EmpratesModel?> _01(EmpratesModel emprates, int userId, string paySchema, string pisSchema, string conn);
        Task<EmpratesModel?> _01(EmpratesModel emprates, string paySchema, string pisSchema, string conn);
        Task<EmpratesModel?> _02(int empmasId, string paySchema, string pisSchema, string conn);
        Task<EmpratesModel?> _02ByEmpNumber(string empnumber, string paySchema, string pisSchema, string conn);
        Task<List<EmpratesModel?>?> _02ByName(string name, string paySchema, string pisSchema, string conn);
        Task<List<EmpratesEmpCntPerPGModel?>?> _02EmpCnt_Per_PG(string paySchema, string conn);
        Task<List<EmpratesEmpCntPerPGModel?>?> _02EmpCnt_Per_Tbltran(string trn, string paySchema, string conn);
        Task<List<EmpratesModel?>?> _02NotInTmpTbltran(string trn, string paySchema, string pisSchema, string conn);
        Task<List<EmpratesModel?>?> _02PerPG(int payrollgrpId, string paySchema, string pisSchema, string conn);
        Task<List<EmpratesModel?>?> _02Deployed(int payrollgrpId, string paySchema, string pisSchema, string conn); 
        Task<EmpratesModel?> _03(EmpratesModel emprates, string schema, string conn);
        Task<EmpratesModel?> _03Rates(EmpratesModel emprates, string schema, string conn); 
        Task<EmpratesModel?> _04(int empmasId, string schema, string conn);
        Task _04ByFK(int empmasId, int payrollgrpId, string schema, string conn);
    }
}