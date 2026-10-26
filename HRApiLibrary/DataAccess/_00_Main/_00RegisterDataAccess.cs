using Dapper;
using HRApiLibrary.DataAccess._00_Main.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;

namespace HRApiLibrary.DataAccess._00_Main;

public class _00RegisterDataAccess : I_00RegisterDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public _00RegisterDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // -- 01 Owner : users + userscompany + companyusers + MainPis.Empmas, in one transaction ------
    // -- One connection is kept open so LAST_INSERT_ID() belongs to these inserts -----------------
    public async Task<RegisterOwnerModel> _01Owner(RegisterOwnerModel owner, string? schema, string? mainPisSchema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            // -- users --
            string sql = $@"insert into {schema}.Users (LoginName, Password, Email, Domain, UserType, Status, DefaultCoId)
                                               values (@Email,    @Password, @Email, @Domain, 1,       'A',    0);
                            select last_insert_id();";
            owner.UserId = await cn.ExecuteScalarAsync<int>(sql, owner, tran);

            // -- userscompany : schemas are prefixed by the company code --
            sql = $@"insert into {schema}.UsersCompany (OwnerId, CompanySName, CompanyName, CompanyCode, CountryId, CurrencyId,
                                                        AMSSchema, ApplicantSchema, PISSchema, PaySchema, AcctgSchema, OldPis, OldPay)
                                                values (@UserId, @CompanyCode, @CompanyName, @CompanyCode, @CountryId, @CurrencyId,
                                                        @AMSSchema, @ApplicantSchema, @PISSchema, @PaySchema, @AcctgSchema, @OldPis, @OldPay);
                     select last_insert_id();";
            owner.CompanyId = await cn.ExecuteScalarAsync<int>(sql, owner, tran);

            // -- companyusers : owner, all modules --
            sql = $@"insert into {schema}.CompanyUsers (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                        WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess)
                                                values (@UserId, @CompanyId, 'A', now(), now(), 1, 0,
                                                        1, 1, 1, 1, 1)";
            await cn.ExecuteAsync(sql, owner, tran);

            sql = $@"update {schema}.Users set DefaultCoId = @CompanyId where Id = @UserId";
            await cn.ExecuteAsync(sql, owner, tran);

            // -- MainPis.Empmas : Id = users.Id --
            sql = $@"insert into {mainPisSchema}.Empmas (Id, SystemId, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix)
                                                 values (@UserId, 0, @LastName, @FirstName, @MiddleName, ifnull(@Suffix, ''))";
            await cn.ExecuteAsync(sql, owner, tran);

            tran.Commit();
            return owner;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 01 New company of a logged-in user : userscompany + companyusers, in one transaction ----
    // -- owner.UserId is the existing users.Id; DefaultCoId is left as is -------------------------
    public async Task<RegisterOwnerModel> _01Company(RegisterOwnerModel owner, string? schema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            string sql = $@"insert into {schema}.UsersCompany (OwnerId, CompanySName, CompanyName, CompanyCode, CountryId, CurrencyId,
                                                               AMSSchema, ApplicantSchema, PISSchema, PaySchema, AcctgSchema, OldPis, OldPay)
                                                       values (@UserId, @CompanyCode, @CompanyName, @CompanyCode, @CountryId, @CurrencyId,
                                                               @AMSSchema, @ApplicantSchema, @PISSchema, @PaySchema, @AcctgSchema, @OldPis, @OldPay);
                            select last_insert_id();";
            owner.CompanyId = await cn.ExecuteScalarAsync<int>(sql, owner, tran);

            sql = $@"insert into {schema}.CompanyUsers (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                        WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess)
                                                values (@UserId, @CompanyId, 'A', now(), now(), 1, 0,
                                                        1, 1, 1, 1, 1)";
            await cn.ExecuteAsync(sql, owner, tran);

            tran.Commit();
            return owner;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 02 Owner in the company's own Empmas : SystemId = users.Id (run after the PIS table maker) --
    // -- companyusers.EmpmasId is set to the new Empmas row (same command, so LAST_INSERT_ID() is this insert) --
    public async Task _01OwnerEmpmas(RegisterOwnerModel owner, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"insert into {owner.PISSchema}.Empmas (SystemId, EmpNumber, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix)
                                                      values (@UserId,  '0001',    @LastName, @FirstName, @MiddleName, ifnull(@Suffix, ''));
                        update {schema}.CompanyUsers set EmpmasId = last_insert_id() where UserId = @UserId and CompanyId = @CompanyId;";
        await _sql.ExecuteCmd(sql, owner, connName);
    }

    // -- 03 Owner in the company's old PIS Empmas : no SystemId there, the login finds it by email --
    // -- EMPSTAT_ '_' = Newly Hired (empstat) --
    public async Task _01OwnerOldEmpmas(RegisterOwnerModel owner, string? connName = "MySqlConn")
    {
        string sql = $@"insert into {owner.OldPis}.Empmas (EMPNUMBER, EMPLASTNM, EMPFIRSTNM, EMPMIDNM, suffix, email, EMPSTAT_)
                                                   values ('0001',    @LastName, @FirstName, @MiddleName, ifnull(@Suffix, ''), @Email, '_')";
        await _sql.ExecuteCmd(sql, owner, connName);
    }

    // -- 04 Employee : users + CompanyInvite (P pending) + MainPis.Empmas, in one transaction ------
    // -- The employee comes from {OldPis}.Empmas; {PISSchema}.Empmas gets a row (by EmpNumber) when it has none --
    // -- DefaultCoId, CompanyUsers and {PISSchema}.Empmas.SystemId wait for the company's approval --
    public async Task<RegisterEmployeeModel> _01Employee(RegisterEmployeeModel emp, string? schema, string? mainPisSchema, string? connName = "MySqlConn")
    {
        using var cn = _sql.GetConnString(connName);
        cn.Open();
        using var tran = cn.BeginTransaction();

        try
        {
            // -- {PISSchema}.Empmas : the row of this EmpNumber, made from the old PIS (the source of truth) when missing --
            // -- An existing row was already compared with the old PIS by the register check --
            emp.EmpmasId = await PisEmpmasLink.EnsureAsync(cn, tran, emp.PISSchema, new V01EmpmasLookup
            {
                EmpNumber = emp.EmpNumber, LastName = emp.LastName, FirstName = emp.FirstName, MiddleName = emp.MiddleName, Suffix = emp.Suffix
            });

            // -- users --
            string sql = $@"insert into {schema}.Users (LoginName, Password, Email, Domain, UserType, Status, DefaultCoId)
                                        values (@Email,    @Password, @Email, @Domain, 2,       'A',    0);
                     select last_insert_id();";
            emp.UserId = await cn.ExecuteScalarAsync<int>(sql, emp, tran);

            // -- CompanyInvite : registered by the employee (InvitedById 0), Employee (3), no module access yet --
            sql = $@"insert into {schema}.CompanyInvite (UserId, CompanyId, Status, DateInvited, DateAccepted, CompanyUserTypeId, InvitedById,
                                                         WithPISAccess, WithPayAccess, WithAMSAccess, WithAccountingAccess, WithApplicantAccess, EmpmasId)
                                                 values (@UserId, @CompanyId, 'P', now(), null, 3, 0,
                                                         0, 0, 0, 0, 0, @EmpmasId)";
            await cn.ExecuteAsync(sql, emp, tran);

            // -- MainPis.Empmas : Id = users.Id, name from the old PIS (cut to the MainPis column sizes) --
            sql = $@"insert into {mainPisSchema}.Empmas (Id, SystemId, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix)
                                                 values (@UserId, 0, left(@LastName, 25), left(@FirstName, 25), left(@MiddleName, 15), ifnull(@Suffix, ''))";
            await cn.ExecuteAsync(sql, emp, tran);

            tran.Commit();
            return emp;
        }
        catch
        {
            tran.Rollback();
            throw;
        }
    }

    // -- 02 Register is for new emails only ---------------------------------------------------------
    public async Task<bool> _02EmailExists(string email, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select count(*) from {schema}.Users where lower(trim(Email)) = lower(trim(@Email))";
        var data = await _sql.FetchData<long, dynamic>(sql, new { Email = email }, connName);
        return data.FirstOrDefault() > 0;
    }

    // -- 02 Company by its code ---------------------------------------------------------------------
    public async Task<V01CompanyLookup?> _02CompanyByCode(string companyCode, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select Id, CompanyName, PISSchema, OldPis from {schema}.UsersCompany where CompanyCode = @CompanyCode limit 1";
        var data = await _sql.FetchData<V01CompanyLookup, dynamic>(sql, new { CompanyCode = companyCode }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Employee no. + last name in the company's old PIS Empmas, with its email and status ----
    public async Task<V01EmpmasLookup?> _02OldEmpmasForRegister(string empNumber, string lastName, string oldPisSchema, string? connName = "MySqlConn")
    {
        string sql = $@"select  trim(e.EMPNUMBER) EmpNumber, trim(e.EMPLASTNM) LastName, trim(e.EMPFIRSTNM) FirstName,
                                trim(e.EMPMIDNM) MiddleName, trim(e.suffix) Suffix, trim(e.email) Email,
                                trim(s.NAME) EmpStatus, if(s.ISRESIGNED = '1', 1, 0) IsResigned
                        from    {oldPisSchema}.Empmas e
                                left join {oldPisSchema}.empstat s on s.CODE = e.EMPSTAT_
                        where   e.EMPNUMBER = @EmpNumber and upper(trim(e.EMPLASTNM)) = upper(trim(@LastName)) limit 1";
        var data = await _sql.FetchData<V01EmpmasLookup, dynamic>(sql, new { EmpNumber = empNumber, LastName = lastName }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 The employee's row in the new PIS Empmas (by EmpNumber), null when there is none yet ----
    // -- The name is compared with the old PIS by the register --------------------------------------
    public async Task<V01EmpmasLookup?> _02PisEmpmasByEmpNumber(string empNumber, string pisSchema, string? connName = "MySqlConn")
    {
        string sql = $@"select  Id, SystemId, trim(EmpNumber) EmpNumber, trim(EmpLastNm) LastName, trim(EmpFirstNm) FirstName,
                                trim(EmpMidNm) MiddleName, trim(Suffix) Suffix
                        from    {pisSchema}.Empmas
                        where   EmpNumber = @EmpNumber order by Id limit 1";
        var data = await _sql.FetchData<V01EmpmasLookup, dynamic>(sql, new { EmpNumber = empNumber }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Employee status : {OldPis}.Empmas.EMPSTAT_ -> {OldPis}.empstat of the same EmpNumber ---
    public async Task<V01EmpmasLookup?> _02OldEmpStatus(string empNumber, string oldPisSchema, string? connName = "MySqlConn")
    {
        string sql = $@"select trim(s.NAME) EmpStatus, if(s.ISRESIGNED = '1', 1, 0) IsResigned
                        from   {oldPisSchema}.Empmas e
                               join {oldPisSchema}.empstat s on s.CODE = e.EMPSTAT_
                        where  e.EMPNUMBER = @EmpNumber limit 1";
        var data = await _sql.FetchData<V01EmpmasLookup, dynamic>(sql, new { EmpNumber = empNumber }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Someone already registered this employee and is waiting for approval ------------------
    public async Task<bool> _02PendingInvite(int companyId, int empmasId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select count(*) from {schema}.CompanyInvite where CompanyId = @CompanyId and EmpmasId = @EmpmasId and Status = 'P'";
        var data = await _sql.FetchData<long, dynamic>(sql, new { CompanyId = companyId, EmpmasId = empmasId }, connName);
        return data.FirstOrDefault() > 0;
    }

    // -- 02 Company code already used by another company ------------------------------------------
    public async Task<bool> _02CodeTaken(string companyCode, string? schema, string? connName = "MySqlConn")
    {
        string sql = $"select count(*) from {schema}.UsersCompany where CompanyCode = @CompanyCode";
        var data = await _sql.FetchData<long, dynamic>(sql, new { CompanyCode = companyCode }, connName);
        return data.FirstOrDefault() > 0;
    }

    // -- 02 Logged-in user's name from MainPis.Empmas (Id = users.Id), used as the owner of a new company --
    public async Task<RegisterOwnerModel?> _02OwnerByUserId(int userId, string? schema, string? mainPisSchema, string? connName = "MySqlConn")
    {
        string sql = $@"select e.Id as UserId, e.EmpFirstNm as FirstName, e.EmpMidNm as MiddleName, e.EmpLastNm as LastName, e.Suffix,
                               trim(u.Email) as Email
                        from   {mainPisSchema}.Empmas e
                               join {schema}.Users u on u.Id = e.Id
                        where  e.Id = @UserId limit 1";
        var data = await _sql.FetchData<RegisterOwnerModel, dynamic>(sql, new { UserId = userId }, connName);
        return data.FirstOrDefault();
    }

    // -- 02 Current company (users.DefaultCoId) when the user is its approved Owner (1) or System Admin (5), else null --
    public async Task<V01CompanyLookup?> _02CurrentAdminCompany(int userId, string? schema, string? connName = "MySqlConn")
    {
        string sql = $@"select uc.Id, uc.CompanyName, uc.PISSchema, uc.OldPis, cu.CompanyUserTypeId
                        from   {schema}.Users u
                               join {schema}.CompanyUsers cu on cu.UserId = u.Id and cu.CompanyId = u.DefaultCoId
                                                           and cu.Status = 'A' and cu.CompanyUserTypeId in (1, 5)
                               join {schema}.UsersCompany uc on uc.Id = u.DefaultCoId
                        where  u.Id = @UserId limit 1";
        var data = await _sql.FetchData<V01CompanyLookup, dynamic>(sql, new { UserId = userId }, connName);
        return data.FirstOrDefault();
    }
}
