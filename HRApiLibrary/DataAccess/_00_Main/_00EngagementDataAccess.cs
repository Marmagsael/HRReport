using System.Text.RegularExpressions;
using Dapper;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00EngagementDataAccess : I_00EngagementDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00EngagementDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // -- 02 Companies of a user : companyusers + userscompany ----------------------------------
    public async Task<List<MyEngagementModel>> _02ByUserId(int userId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  uc.Id                   CompanyId,
                                uc.CompanyName,
                                uc.CompanyCode,
                                cu.CompanyUserTypeId,
                                t.Name                  UserType,
                                cu.Status,
                                cu.DateInvited,
                                cu.DateAccepted,
                                (u.DefaultCoId = uc.Id) IsDefault
                        from    {schema}.CompanyUsers cu
                                join      {schema}.UsersCompany    uc on uc.Id = cu.CompanyId
                                join      {schema}.Users           u  on u.Id  = cu.UserId
                                left join {schema}.CompanyUserType t  on t.Id  = cu.CompanyUserTypeId
                        where   cu.UserId = @UserId
                        order by IsDefault desc, uc.CompanyName";
        return await _sql.FetchData<MyEngagementModel, dynamic>(sql, new { UserId = userId }, connName);
    }

    // -- 03 Switch company : users.DefaultCoId, only to a company the user is approved in ------
    // -- Returns false when the user is not an approved member of the company -------------------
    public async Task<bool> _03DefaultCompany(int userId, int companyId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"update  {schema}.Users u
                                join {schema}.CompanyUsers cu on cu.UserId = u.Id and cu.CompanyId = @CompanyId and cu.Status = 'A'
                        set     u.DefaultCoId = @CompanyId
                        where   u.Id = @UserId;
                        select  count(*) from {schema}.CompanyUsers where UserId = @UserId and CompanyId = @CompanyId and Status = 'A';";
        var res = await _sql.FetchData<long, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, connName);
        return res.FirstOrDefault() > 0;
    }


    // **********************************************************************************************
    // --- Invites : the company invites an existing user, matched by the email in its old PIS Empmas ---
    // **********************************************************************************************

    // -- 02 Search the company's employees in the old PIS (the source of truth) : employee no., last name --
    // -- or first name, 30 rows at most --
    public async Task<List<InviteCandidateModel>> _02InviteSearch(string oldPisSchema, string term, string? schema, string? connName = "MySqlConn")
    {
        // -- % and _ in the typed text are matched as plain characters --
        string starts = term.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";

        string sql = $@"select  trim(e.EMPNUMBER)       EmpNumber,
                                trim(e.EMPLASTNM)       LastName,
                                trim(e.EMPFIRSTNM)      FirstName,
                                trim(s.NAME)            EmpStatus
                        from    {oldPisSchema}.Empmas e
                                left join {oldPisSchema}.empstat s on s.CODE = e.EMPSTAT_
                        where   e.EMPNUMBER like @Starts
                                or e.EMPLASTNM like @Starts
                                or e.EMPFIRSTNM like @Starts
                                or concat(trim(e.EMPLASTNM), ', ', trim(e.EMPFIRSTNM)) like @Starts
                        order by e.EMPLASTNM, e.EMPFIRSTNM
                        limit 30";
        return await _sql.FetchData<InviteCandidateModel, dynamic>(sql, new { Starts = starts }, connName);
    }

    // -- 02 One employee and why it can or cannot be invited ----------------------------------------
    // -- Name, email and status : {OldPis}.Empmas; link and name check : {PISSchema}.Empmas of the same EmpNumber --
    // -- Already a user / invited : CompanyUsers / CompanyInvite of this company, by the employee (EmpmasId) or the account (UserId) --
    public async Task<InviteCandidateModel?> _02InviteCandidate(int companyId, string empNumber, string pisSchema, string oldPisSchema, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  x.*,
                                (select cu.Status from {schema}.CompanyUsers cu
                                 where  cu.CompanyId = @CompanyId and ((x.EmpmasId > 0 and cu.EmpmasId = x.EmpmasId) or (x.UserId > 0 and cu.UserId = x.UserId))
                                 limit 1)                                                       MemberStatus,
                                (select ci.Status from {schema}.CompanyInvite ci
                                 where  ci.CompanyId = @CompanyId and ((x.EmpmasId > 0 and ci.EmpmasId = x.EmpmasId) or (x.UserId > 0 and ci.UserId = x.UserId))
                                 order by ci.DateInvited desc limit 1)                          InviteStatus
                        from   (select  trim(o.EMPNUMBER)           EmpNumber,
                                        trim(o.EMPLASTNM)           LastName,
                                        trim(o.EMPFIRSTNM)          FirstName,
                                        trim(o.EMPMIDNM)            MiddleName,
                                        trim(o.suffix)              Suffix,
                                        trim(o.email)               Email,
                                        trim(s.NAME)                EmpStatus,
                                        if(s.ISRESIGNED = '1', 1, 0) IsResigned,
                                        ifnull((select min(u.Id) from {schema}.Users u
                                                where  trim(ifnull(o.email, '')) <> ''
                                                       and lower(trim(u.Email)) = lower(trim(o.email))), 0) UserId,
                                        ifnull(e.Id, 0)             EmpmasId,
                                        ifnull(e.SystemId, 0)       LinkedUserId,
                                        trim(e.EmpLastNm)           PisLastName,
                                        trim(e.EmpFirstNm)          PisFirstName,
                                        trim(e.EmpMidNm)            PisMiddleName,
                                        trim(e.Suffix)              PisSuffix
                                from    {oldPisSchema}.Empmas o
                                        left join {oldPisSchema}.empstat s on s.CODE = o.EMPSTAT_
                                        left join {pisSchema}.Empmas e on e.Id = (select min(p.Id) from {pisSchema}.Empmas p where p.EmpNumber = o.EMPNUMBER)
                                where   o.EMPNUMBER = @EmpNumber) x";
        var rows = await _sql.FetchData<InviteCandidateModel, dynamic>(sql, new { CompanyId = companyId, EmpNumber = empNumber }, connName);
        return rows.FirstOrDefault();
    }

    // -- 01 Invite : the employee's {PISSchema}.Empmas row is made from the old PIS when missing ------
    // -- A declined invite of the same user is sent again; invite.EmpmasId is set here -------------
    public async Task _01Invite(CompanyInviteModel invite, V01EmpmasLookup oldPis, string pisSchema, string? schema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            invite.EmpmasId = await PisEmpmasLink.EnsureAsync(cn, tran, pisSchema, oldPis);

            string sql = $@"insert into {schema}.CompanyInvite (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                                WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess, EmpmasId)
                                                        values (@UserId, @CompanyId, 'I', now(), null, @CompanyUserTypeId, @InvitedById,
                                                                @WithPISAccess, @WithPayAccess, @WithAMSAccess, @WithAccountingAccess, @WithApplicantAccess, @EmpmasId)
                            on duplicate key update Status = 'I', DateInvited = now(), DateAccepted = null, CompanyUserTypeId = @CompanyUserTypeId, InvitedById = @InvitedById,
                                                    WithPISAccess = @WithPISAccess, WithPayAccess = @WithPayAccess, WithAMSAccess = @WithAMSAccess,
                                                    WithAccountingAccess = @WithAccountingAccess, WithApplicantAccess = @WithApplicantAccess, EmpmasId = @EmpmasId";
            await cn.ExecuteAsync(sql, invite, tran);

            tran.Commit();
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 02 My Invitations : invites waiting for the user's answer ---------------------------------
    public async Task<List<MyInvitationModel>> _02InvitesByUserId(int userId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  ci.CompanyId,
                                uc.CompanyName,
                                uc.CompanyCode,
                                trim(inv.Email)         InvitedBy,
                                ci.DateInvited
                        from    {schema}.CompanyInvite ci
                                join      {schema}.UsersCompany uc  on uc.Id  = ci.CompanyId
                                left join {schema}.Users        inv on inv.Id = ci.InvitedById
                        where   ci.UserId = @UserId and ci.Status = 'I'
                        order by ci.DateInvited desc";
        return await _sql.FetchData<MyInvitationModel, dynamic>(sql, new { UserId = userId }, connName);
    }

    // -- 03 Accept : companyusers row + Empmas.SystemId, in one transaction ------------------------
    // -- Returns an error message, or null when accepted ------------------------------------------
    public async Task<string?> _03AcceptInvite(int userId, int companyId, string? schema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            var p = new { UserId = userId, CompanyId = companyId };

            string sql = $@"select ci.EmpmasId, uc.PISSchema
                            from   {schema}.CompanyInvite ci
                                   join {schema}.UsersCompany uc on uc.Id = ci.CompanyId
                            where  ci.UserId = @UserId and ci.CompanyId = @CompanyId and ci.Status = 'I'
                            for update";
            var invite = await cn.QueryFirstOrDefaultAsync<InviteRow>(sql, p, tran);

            if (invite == null)                                                     { tran.Rollback(); return "This invitation is no longer available."; }
            if (string.IsNullOrEmpty(invite.PISSchema) || !Regex.IsMatch(invite.PISSchema, @"^\w+$"))
                                                                                    { tran.Rollback(); return "This company is not set up for invitations yet."; }

            sql = $@"select count(*) from {schema}.CompanyUsers where UserId = @UserId and CompanyId = @CompanyId";
            if (await cn.ExecuteScalarAsync<long>(sql, p, tran) > 0)                { tran.Rollback(); return "You already belong to this company."; }

            // -- Link the employee record to this user; blocked only when another user is linked to it --
            sql = $@"select SystemId from {invite.PISSchema}.Empmas where Id = @EmpmasId for update";
            int? linked = await cn.ExecuteScalarAsync<int?>(sql, new { invite.EmpmasId }, tran);

            if (linked == null)                                                     { tran.Rollback(); return "The employee record of this invitation was not found. Please contact the company."; }
            if (linked > 0 && linked != userId)                                     { tran.Rollback(); return "This employee record is already linked to another account. Please contact the company."; }

            sql = $@"update {invite.PISSchema}.Empmas set SystemId = @UserId where Id = @EmpmasId";
            await cn.ExecuteAsync(sql, new { UserId = userId, invite.EmpmasId }, tran);

            sql = $@"insert into {schema}.CompanyUsers (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                        WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess, EmpmasId)
                     select UserId, CompanyId, 'A', DateInvited, now(), CompanyUserTypeId, InvitedById,
                            WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess, EmpmasId
                     from   {schema}.CompanyInvite where UserId = @UserId and CompanyId = @CompanyId";
            await cn.ExecuteAsync(sql, p, tran);

            sql = $@"update {schema}.CompanyInvite set Status = 'A', DateAccepted = now() where UserId = @UserId and CompanyId = @CompanyId";
            await cn.ExecuteAsync(sql, p, tran);

            // -- First company of the user : the login opens it --
            sql = $@"update {schema}.Users set DefaultCoId = @CompanyId where Id = @UserId and ifnull(DefaultCoId, 0) = 0";
            await cn.ExecuteAsync(sql, p, tran);

            tran.Commit();
            return null;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 03 Decline -------------------------------------------------------------------------------
    public async Task _03DeclineInvite(int userId, int companyId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"update {schema}.CompanyInvite set Status = 'D', DateAccepted = now()
                        where  UserId = @UserId and CompanyId = @CompanyId and Status = 'I'";
        await _sql.ExecuteCmd(sql, new { UserId = userId, CompanyId = companyId }, connName);
    }

    // -- 02 Login check : 'M' = approved member (CompanyUsers A), P / D = self-registered invite ----
    // -- companyId null : any company ------------------------------------------------------------
    public async Task<List<string>> _02ApprovalStatus(int userId, int? companyId, string? schema, string? connName = "MySqlConn")
    {
        string coFilter = companyId == null ? "" : "and CompanyId = @CompanyId";
        string sql = $@"select 'M' from {schema}.CompanyUsers  where UserId = @UserId and Status = 'A' {coFilter}
                        union all
                        select Status from {schema}.CompanyInvite where UserId = @UserId and Status in ('P', 'D') and InvitedById = 0 {coFilter}";
        var data = await _sql.FetchData<string, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, connName);
        return data.ToList();
    }

    // -- 02 First company where the user is an approved member ------------------------------------
    public async Task<int?> _02FirstMemberCompany(int userId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select CompanyId from {schema}.CompanyUsers where UserId = @UserId and Status = 'A' order by DateAccepted limit 1";
        var data = await _sql.FetchData<int?, dynamic>(sql, new { UserId = userId }, connName);
        return data.FirstOrDefault();
    }

    // **********************************************************************************************
    // -- 01.08 Registration Requests : employees who registered (CompanyInvite P), approved by the owner --
    // **********************************************************************************************

    // -- 02 Pending requests of a company, oldest first --------------------------------------------
    public async Task<List<RegistrationRequestModel>> _02Requests(int companyId, string pisSchema, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  ci.UserId, ci.EmpmasId, e.EmpNumber, e.EmpLastNm LastName, e.EmpFirstNm FirstName, e.EmpMidNm MiddleName,
                                trim(u.Email) Email, ci.DateInvited
                        from    {schema}.CompanyInvite ci
                                join      {schema}.Users     u on u.Id = ci.UserId
                                left join {pisSchema}.Empmas e on e.Id = ci.EmpmasId
                        where   ci.CompanyId = @CompanyId and ci.Status = 'P'
                        order by ci.DateInvited";
        return await _sql.FetchData<RegistrationRequestModel, dynamic>(sql, new { CompanyId = companyId }, connName);
    }

    // -- 02 One pending request ---------------------------------------------------------------------
    public async Task<RegistrationRequestModel?> _02Request(int userId, int companyId, string pisSchema, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  ci.UserId, ci.EmpmasId, e.EmpNumber, e.EmpLastNm LastName, e.EmpFirstNm FirstName, e.EmpMidNm MiddleName,
                                trim(u.Email) Email, ci.DateInvited
                        from    {schema}.CompanyInvite ci
                                join      {schema}.Users     u on u.Id = ci.UserId
                                left join {pisSchema}.Empmas e on e.Id = ci.EmpmasId
                        where   ci.UserId = @UserId and ci.CompanyId = @CompanyId and ci.Status = 'P'";
        var data = await _sql.FetchData<RegistrationRequestModel, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Count for the My Engagement badge ------------------------------------------------------
    public async Task<int> _02RequestCount(int companyId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select count(*) from {schema}.CompanyInvite where CompanyId = @CompanyId and Status = 'P'";
        var data = await _sql.FetchData<int, dynamic>(sql, new { CompanyId = companyId }, connName);
        return data.FirstOrDefault();
    }

    // -- 03 Approve : link Empmas, copy to CompanyUsers (A) with the access picked by the owner --------
    // -- approval.InvitedById = the approving owner; returns an error message, or null when approved --
    public async Task<string?> _03ApproveRequest(CompanyInviteModel approval, string pisSchema, string? schema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            string sql = $@"select EmpmasId from {schema}.CompanyInvite
                            where  UserId = @UserId and CompanyId = @CompanyId and Status = 'P'
                            for update";
            int? empmasId = await cn.ExecuteScalarAsync<int?>(sql, approval, tran);

            if (empmasId == null)                                                   { tran.Rollback(); return "This request is no longer pending."; }

            sql = $@"select count(*) from {schema}.CompanyUsers where UserId = @UserId and CompanyId = @CompanyId";
            if (await cn.ExecuteScalarAsync<long>(sql, approval, tran) > 0)         { tran.Rollback(); return "This user already belongs to the company."; }

            // -- Link the employee record to this user; blocked when another user is linked to it --
            sql = $@"select SystemId from {pisSchema}.Empmas where Id = @EmpmasId for update";
            int? linked = await cn.ExecuteScalarAsync<int?>(sql, new { EmpmasId = empmasId }, tran);

            if (linked == null)                                                     { tran.Rollback(); return "The employee record of this request was not found."; }
            if (linked > 0 && linked != approval.UserId)                            { tran.Rollback(); return "This employee record is already linked to another account."; }

            sql = $@"update {pisSchema}.Empmas set SystemId = @UserId where Id = @EmpmasId";
            await cn.ExecuteAsync(sql, new { approval.UserId, EmpmasId = empmasId }, tran);

            sql = $@"insert into {schema}.CompanyUsers (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                        WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess, EmpmasId)
                     select UserId, CompanyId, 'A', DateInvited, now(), @CompanyUserTypeId, @InvitedById,
                            @WithPISAccess, @WithPayAccess, @WithAMSAccess, @WithAccountingAccess, @WithApplicantAccess, EmpmasId
                     from   {schema}.CompanyInvite where UserId = @UserId and CompanyId = @CompanyId";
            await cn.ExecuteAsync(sql, approval, tran);

            sql = $@"update {schema}.CompanyInvite set Status = 'A', DateAccepted = now(), CompanyUserTypeId = @CompanyUserTypeId,
                            WithPISAccess = @WithPISAccess, WithPayAccess = @WithPayAccess, WithAMSAccess = @WithAMSAccess,
                            WithAccountingAccess = @WithAccountingAccess, WithApplicantAccess = @WithApplicantAccess
                     where  UserId = @UserId and CompanyId = @CompanyId";
            await cn.ExecuteAsync(sql, approval, tran);

            // -- First company of the user : the login opens it --
            sql = $@"update {schema}.Users set DefaultCoId = @CompanyId where Id = @UserId and ifnull(DefaultCoId, 0) = 0";
            await cn.ExecuteAsync(sql, approval, tran);

            tran.Commit();
            return null;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 03 Decline : D, the login then shows "not approved" ----------------------------------------
    public async Task _03DeclineRequest(int userId, int companyId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"update {schema}.CompanyInvite set Status = 'D', DateAccepted = now()
                        where  UserId = @UserId and CompanyId = @CompanyId and Status = 'P'";
        await _sql.ExecuteCmd(sql, new { UserId = userId, CompanyId = companyId }, connName);
    }

    // **********************************************************************************************
    // -- 01.09 Company Users : members of the current company, their role and access ---------------
    // **********************************************************************************************

    // -- 02 Role of the user in a company (approved members only), null when there is no row --------
    public async Task<int?> _02CompanyUserType(int userId, int companyId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select CompanyUserTypeId from {schema}.CompanyUsers where UserId = @UserId and CompanyId = @CompanyId and Status = 'A'";
        var data = await _sql.FetchData<int?, dynamic>(sql, new { UserId = userId, CompanyId = companyId }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Members : name from the company's Empmas (EmpmasId), else MainPis.Empmas (Id = users.Id) --
    public async Task<List<CompanyMemberModel>> _02Members(int companyId, string pisSchema, string? mainPisSchema, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select  cu.UserId, trim(u.Email) Email, trim(e.EmpNumber) EmpNumber,
                                coalesce(nullif(trim(e.EmpLastNm), ''), trim(m.EmpLastNm))   LastName,
                                coalesce(nullif(trim(e.EmpFirstNm), ''), trim(m.EmpFirstNm)) FirstName,
                                cu.CompanyUserTypeId, t.Name UserType, cu.Status, cu.DateAccepted,
                                cu.WithPISAccess, cu.WithPayAccess, cu.WithAMSAccess, cu.WithAccountingAccess, cu.WithApplicantAccess
                        from    {schema}.CompanyUsers cu
                                join      {schema}.Users           u on u.Id = cu.UserId
                                left join {schema}.CompanyUserType t on t.Id = cu.CompanyUserTypeId
                                left join {pisSchema}.Empmas       e on e.Id = cu.EmpmasId and cu.EmpmasId > 0
                                left join {mainPisSchema}.Empmas   m on m.Id = cu.UserId
                        where   cu.CompanyId = @CompanyId
                        order by cu.CompanyUserTypeId = 1 desc, cu.CompanyUserTypeId = 5 desc, LastName, FirstName";
        return await _sql.FetchData<CompanyMemberModel, dynamic>(sql, new { CompanyId = companyId }, connName);
    }

    // -- 03 Role and access of one member; the Owner row is never changed here -----------------------
    // -- setType false : access only (a System Admin); the role must be 2 User, 3 Employee or 5 System Admin --
    // -- An Employee (3) has no module access : the role is set first, so the flags see the new role --
    public async Task<bool> _03Member(CompanyInviteModel member, bool setType, string? schema, string? connName = "MySqlConn")
    {
        if (setType && member.CompanyUserTypeId is not (2 or 3 or 5)) return false;

        string type = setType ? "CompanyUserTypeId = @CompanyUserTypeId," : "";
        string sql  = $@"update {schema}.CompanyUsers
                         set    {type}
                                WithPISAccess        = if(CompanyUserTypeId = 3, 0, @WithPISAccess),
                                WithPayAccess        = if(CompanyUserTypeId = 3, 0, @WithPayAccess),
                                WithAMSAccess        = if(CompanyUserTypeId = 3, 0, @WithAMSAccess),
                                WithAccountingAccess = if(CompanyUserTypeId = 3, 0, @WithAccountingAccess),
                                WithApplicantAccess  = if(CompanyUserTypeId = 3, 0, @WithApplicantAccess)
                         where  UserId = @UserId and CompanyId = @CompanyId and CompanyUserTypeId <> 1";
        using var cn = _sql.GetConnString(connName);
        return await cn.ExecuteAsync(sql, member) > 0;
    }

    private class InviteRow
    {
        public int      EmpmasId    { get; set; }
        public string?  PISSchema   { get; set; }
    }
}
