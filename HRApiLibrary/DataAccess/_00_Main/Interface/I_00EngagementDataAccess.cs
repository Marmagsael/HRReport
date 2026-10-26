using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main.Interface
{
    public interface I_00EngagementDataAccess
    {
        Task<List<MyEngagementModel>> _02ByUserId(int userId, string? schema, string? connName = "MySqlConn");
        Task<bool> _03DefaultCompany(int userId, int companyId, string? schema, string? connName = "MySqlConn");

        Task<List<InviteCandidateModel>> _02InviteSearch(string oldPisSchema, string term, string? schema, string? connName = "MySqlConn");
        Task<InviteCandidateModel?>      _02InviteCandidate(int companyId, string empNumber, string pisSchema, string oldPisSchema, string? schema, string? connName = "MySqlConn");
        Task                             _01Invite(CompanyInviteModel invite, V01EmpmasLookup oldPis, string pisSchema, string? schema, string? connName = "MySqlConn");
        Task<List<MyInvitationModel>>    _02InvitesByUserId(int userId, string? schema, string? connName = "MySqlConn");
        Task<string?>                    _03AcceptInvite(int userId, int companyId, string? schema, string? connName = "MySqlConn");
        Task                             _03DeclineInvite(int userId, int companyId, string? schema, string? connName = "MySqlConn");

        Task<List<string>>               _02ApprovalStatus(int userId, int? companyId, string? schema, string? connName = "MySqlConn");
        Task<int?>                       _02FirstMemberCompany(int userId, string? schema, string? connName = "MySqlConn");

        Task<List<RegistrationRequestModel>> _02Requests(int companyId, string pisSchema, string? schema, string? connName = "MySqlConn");
        Task<RegistrationRequestModel?>      _02Request(int userId, int companyId, string pisSchema, string? schema, string? connName = "MySqlConn");
        Task<int>                            _02RequestCount(int companyId, string? schema, string? connName = "MySqlConn");
        Task<string?>                        _03ApproveRequest(CompanyInviteModel approval, string pisSchema, string? schema, string? connName = "MySqlConn");
        Task                                 _03DeclineRequest(int userId, int companyId, string? schema, string? connName = "MySqlConn");

        Task<int?>                           _02CompanyUserType(int userId, int companyId, string? schema, string? connName = "MySqlConn");
        Task<List<CompanyMemberModel>>       _02Members(int companyId, string pisSchema, string? mainPisSchema, string? schema, string? connName = "MySqlConn");
        Task<bool>                           _03Member(CompanyInviteModel member, bool setType, string? schema, string? connName = "MySqlConn");
    }
}
