using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._604O;

public class DA_604O : IDA_604O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_604O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    private const string LoanCols = @"l.NUMBER Number, trim(l.EMPNUMBER) EmpNumber, trim(l.DEDNCODE) DednCode, ifnull(c.AcctName, trim(l.DEDNDESC)) AcctName,
                l.`DATE` LoanDate, l.payStart PayStart, l.payRes PayRes,
                ifnull(l.AMOUNT, 0) Amount, ifnull(l.AMORT, 0) Amort, ifnull(l.BALANCE, 0) Balance,
                ifnull(l.Principal, 0) Principal, ifnull(l.InterestRate, 0) InterestRate, ifnull(l.MoToPay, 0) MoToPay,
                cast(ifnull(nullif(trim(l.cvno), ''), '0') as unsigned) CvNo,
                ifnull(l.p1, 0) P1, ifnull(l.p2, 0) P2, ifnull(l.p3, 0) P3, ifnull(l.p4, 0) P4, ifnull(l.p5, 0) P5,
                l.REMARKS Remarks, ifnull(l.STATUS, 'A') Status";

    public async Task _04AutoInactive(string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.loans set STATUS = 'I' where ifnull(BALANCE, 0) <= 0 and STATUS = 'A'", new { }, conn);
    }

    public async Task<List<M604_Emp>> _02Employees(string opis, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(e.EmpLastNm, '')), ', ', trim(ifnull(e.EmpFirstNm, '')), ' ', left(trim(ifnull(e.EmpMidNm, '')), 1), '.') EmpName,
                               if(ifnull(s.ISRESIGNED, '0') = '1', 1, 0) Resigned
                        from {opis}.Empmas e left join {opis}.empstat s on s.CODE = e.EmpStat_
                        order by e.EmpLastNm, e.EmpFirstNm";
        return await _sql.FetchLegacy<M604_Emp, dynamic>(sql, new { }, conn) ?? [];
    }

    // -- Chargeable debit accounts; Mrk = 1 when the user may use it (no restriction = all) --
    public async Task<List<M604_Acct>> _02Accounts(string opay, string username, bool unrestricted, string conn)
    {
        string sql = unrestricted
            ? $@"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName, 1 Mrk from {opay}.chartofacct where ischargeable = '1' and acctType = 'D' order by AcctName"
            : $@"select trim(c.AcctNumber) AcctNumber, trim(c.AcctName) AcctName,
                        if(a.AcctNumber is null, 0, 1) Mrk
                 from {opay}.chartofacct c
                      left join {opay}.loansacctaccess a on trim(a.AcctNumber) = trim(c.AcctNumber) and a.username = @User
                 where c.ischargeable = '1' and c.acctType = 'D' order by c.AcctName";
        return await _sql.FetchLegacy<M604_Acct, dynamic>(sql, new { User = username }, conn) ?? [];
    }

    public async Task<List<string>> _02AccessOf(string username, string opay, string conn)
    {
        var r = await _sql.FetchLegacy<string, dynamic>($"select trim(AcctNumber) from {opay}.loansacctaccess where username = @User", new { User = username }, conn);
        return r ?? [];
    }

    public async Task _03SaveAccess(string username, List<string> accts, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {opay}.loansacctaccess where username = @User", new { User = username }, conn);
        foreach (var a in accts.Distinct())
            await _sql.ExecuteLegacyCmd($"insert into {opay}.loansacctaccess (username, AcctNumber) values (@User, @Acct)", new { User = username, Acct = a }, conn);
    }

    // -- Loans of an employee; a user only sees the loans of the accounts they have access to --
    public async Task<List<M604_Loan>> _02Loans(string empNumber, string username, bool unrestricted, string opay, string conn)
    {
        string sql = $@"select {LoanCols}
                        from {opay}.loans l left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(l.DEDNCODE)
                        where trim(l.EMPNUMBER) = @Emp
                          {(unrestricted ? "" : $"and trim(l.DEDNCODE) in (select trim(AcctNumber) from {opay}.loansacctaccess where username = @User)")}
                        order by AcctName, l.NUMBER";
        var r = await _sql.FetchLegacy<M604_Loan, dynamic>(sql, new { Emp = empNumber, User = username }, conn) ?? [];
        foreach (var l in r)
        {
            if (l.LoanDate is { } d && d.Year < 2) l.LoanDate = null;
            if (l.PayStart is { } s && s.Year < 2) l.PayStart = null;
            if (l.PayRes is { } e && e.Year < 2) l.PayRes = null;
        }
        return r;
    }

    // -- Insert (isNew) or update; returns an error message, or null when saved --
    public async Task<(string? Error, int Number)> _01Save(M604_Loan f, bool isNew, string username, string opay, string conn)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        if (f.CvNo > 0)
        {
            var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.loans where NUMBER <> @Number and cast(ifnull(nullif(trim(cvno), ''), '0') as unsigned) = @Cv",
                new { Number = f.Number, Cv = f.CvNo }, conn);
            if (dup.FirstOrDefault() > 0) return ("CV number already in used!", f.Number);
        }

        if (isNew)
        {
            var exists = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.loans where trim(DEDNCODE) = @Acct and STATUS = 'A' and trim(EMPNUMBER) = @Emp",
                new { Acct = f.DednCode, Emp = f.EmpNumber }, conn);
            if (exists.FirstOrDefault() > 0) return ("Unable to save transaction! Existing account found in database.", 0);

            var p = Params(f, username, now);
            await _sql.ExecuteLegacyCmd($@"insert into {opay}.loans
                    (EMPNUMBER, `DATE`, DEDNCODE, DEDNDESC, AMOUNT, AMORT, REMARKS, payStart, payRes, BALANCE, STATUS, cvno, Principal, InterestRate, MoToPay, p1, p2, p3, p4, p5, ENCODEBY, ENCODEDT)
                values (@EmpNumber, @LoanDate, @DednCode, left(@AcctName, 20), @Amount, @Amort, @Remarks, @PayStart, ifnull(@PayRes, '0000-00-00'), @Amount, 'A', @CvNo, @Principal, @InterestRate, @MoToPay, @P1, @P2, @P3, @P4, @P5, @User, @Now)", p, conn);
            var id = await _sql.FetchLegacy<long, dynamic>($"select cast(max(NUMBER) as signed) from {opay}.loans where trim(EMPNUMBER) = @Emp and trim(DEDNCODE) = @Acct", new { Emp = f.EmpNumber, Acct = f.DednCode }, conn);
            f.Number = (int)id.FirstOrDefault();
            await RebuildSchedule(f.Number, f.Amount, f.Amort, opay, conn);
        }
        else
        {
            var old = (await _sql.FetchLegacy<M604_Loan, dynamic>($"select {LoanCols} from {opay}.loans l left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(l.DEDNCODE) where l.NUMBER = @Number", new { Number = f.Number }, conn)).FirstOrDefault();
            bool rebuild = old == null || old.Amort != f.Amort || old.Balance != f.Balance;
            var p = Params(f, username, now);
            await _sql.ExecuteLegacyCmd($@"update {opay}.loans set
                    `DATE` = @LoanDate, AMORT = @Amort, AMOUNT = @Amount, Principal = @Principal, InterestRate = @InterestRate, MoToPay = @MoToPay,
                    BALANCE = @Balance, REMARKS = @Remarks, payStart = @PayStart, payRes = ifnull(@PayRes, '0000-00-00'), STATUS = @Status, cvno = @CvNo,
                    CHANGEBY = @User, CHANGEDT = @Now, p1 = @P1, p2 = @P2, p3 = @P3, p4 = @P4, p5 = @P5
                where NUMBER = @Number", p, conn);
            if (rebuild) await RebuildSchedule(f.Number, f.Balance, f.Amort, opay, conn);
        }
        return (null, f.Number);
    }

    private static object Params(M604_Loan f, string username, string now) => new
    {
        f.Number, f.EmpNumber, f.DednCode, f.AcctName, f.LoanDate, f.PayStart, f.PayRes, f.Amount, f.Amort, f.Balance, f.Remarks, f.Status,
        f.CvNo, f.Principal, f.InterestRate, f.MoToPay, f.P1, f.P2, f.P3, f.P4, f.P5, User = username, Now = now
    };

    // -- Payments already deducted (TRN filled) stay; the unpaid rows are rebuilt from what is still owed --
    private async Task RebuildSchedule(int loansId, decimal owed, decimal amort, string opay, string conn)
    {
        var paid = await _sql.FetchLegacy<M604_Sched, dynamic>($"select Id, SchedNo, Amort, TRN Trn from {opay}.loansSchedule where LoansId = @Id and TRN is not null order by SchedNo", new { Id = loansId }, conn);
        int next = (paid ?? []).Count == 0 ? 1 : paid!.Max(x => x.SchedNo) + 1;

        await _sql.ExecuteLegacyCmd($"delete from {opay}.loansSchedule where LoansId = @Id and TRN is null", new { Id = loansId }, conn);

        decimal remaining = owed;
        if (amort <= 0 || remaining <= 0) return;

        while (remaining > 0 && next < 1000)
        {
            decimal a = Math.Min(amort, remaining);
            await _sql.ExecuteLegacyCmd($"insert into {opay}.loansSchedule (LoansId, SchedNo, Amort) values (@Id, @No, @Amort)", new { Id = loansId, No = next, Amort = Math.Round(a, 2) }, conn);
            remaining = Math.Round(remaining - a, 2);
            next++;
        }
    }

    public async Task _04Delete(int number, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {opay}.loansSchedule where LoansId = @Number", new { Number = number }, conn);
        await _sql.ExecuteLegacyCmd($"delete from {opay}.loans where NUMBER = @Number", new { Number = number }, conn);
    }

    // -- Change the amortization of one unpaid payment of the schedule --
    public async Task<bool> _03ScheduleAmort(int id, decimal amort, string opay, string conn)
    {
        var r = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opay}.loansSchedule where Id = @Id and TRN is null", new { Id = id }, conn);
        if (r.FirstOrDefault() == 0) return false;
        await _sql.ExecuteLegacyCmd($"update {opay}.loansSchedule set Amort = @Amort where Id = @Id and TRN is null", new { Id = id, Amort = Math.Round(amort, 2) }, conn);
        return true;
    }

    public async Task<List<M604_Sched>> _02Schedule(int number, string opay, string conn)
    {
        return await _sql.FetchLegacy<M604_Sched, dynamic>($"select Id, SchedNo, Amort, TRN Trn from {opay}.loansSchedule where LoansId = @Id order by SchedNo", new { Id = number }, conn) ?? [];
    }

    // -- Deductions actually made in payroll (tbltran) from the loan date until the next loan of the same account --
    public async Task<M604_Recompute> _02Recompute(int number, string opay, string conn)
    {
        var loan = (await _sql.FetchLegacy<M604_Loan, dynamic>($"select {LoanCols} from {opay}.loans l left join {opay}.chartofacct c on trim(c.AcctNumber) = trim(l.DEDNCODE) where l.NUMBER = @Number", new { Number = number }, conn)).FirstOrDefault();
        var r = new M604_Recompute();
        if (loan == null) return r;

        DateTime start = loan.LoanDate ?? DateTime.Today;
        var next = await _sql.FetchLegacy<DateTime?, dynamic>($@"select min(`DATE`) from {opay}.loans where `DATE` > @Start and trim(EMPNUMBER) = @Emp and trim(DEDNCODE) = @Acct",
            new { Start = start, Emp = loan.EmpNumber, Acct = loan.DednCode }, conn);
        DateTime end = next.FirstOrDefault() is { } d && d.Year > 1 ? d : DateTime.Today;

        var tr = await _sql.FetchLegacy<M604_Tran, dynamic>($@"select * from (
                        select TRN Trn, amount Amount,
                               str_to_date(concat('20', substr(TRN, 1, 2), '-', substr(TRN, 3, 2), '-', substr(TRN, 5, 2)), '%Y-%m-%d') XDate
                        from {opay}.tbltran where trim(empNumber) = @Emp and trim(acctNumber) = @Acct) x
                    where XDate between @Start and @End order by XDate", new { Emp = loan.EmpNumber, Acct = loan.DednCode, Start = start, End = end }, conn) ?? [];

        r.Loan = loan;
        r.Trans = tr;
        r.Deducted = tr.Sum(x => x.Amount);
        r.ExpectedBalance = Math.Max(0, loan.Amount - r.Deducted);
        r.Schedule = await _02Schedule(number, opay, conn);
        return r;
    }

    public async Task _03Reconstruct(int number, decimal balance, string username, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.loans set BALANCE = @Balance, CHANGEBY = @User, CHANGEDT = @Now where NUMBER = @Number",
            new { Number = number, Balance = balance, User = username, Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, conn);
    }
}

public interface IDA_604O
{
    Task _04AutoInactive(string opay, string conn);
    Task<List<M604_Emp>> _02Employees(string opis, string conn);
    Task<List<M604_Acct>> _02Accounts(string opay, string username, bool unrestricted, string conn);
    Task<List<string>> _02AccessOf(string username, string opay, string conn);
    Task _03SaveAccess(string username, List<string> accts, string opay, string conn);
    Task<List<M604_Loan>> _02Loans(string empNumber, string username, bool unrestricted, string opay, string conn);
    Task<(string? Error, int Number)> _01Save(M604_Loan f, bool isNew, string username, string opay, string conn);
    Task _04Delete(int number, string opay, string conn);
    Task<List<M604_Sched>> _02Schedule(int number, string opay, string conn);
    Task<bool> _03ScheduleAmort(int id, decimal amort, string opay, string conn);
    Task<M604_Recompute> _02Recompute(int number, string opay, string conn);
    Task _03Reconstruct(int number, decimal balance, string username, string opay, string conn);
}

public class M604_Emp
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public bool Resigned { get; set; }
}

public class M604_Acct
{
    public string AcctNumber { get; set; } = "";
    public string AcctName { get; set; } = "";
    public int Mrk { get; set; }
}

public class M604_Loan
{
    public int Number { get; set; }
    public string EmpNumber { get; set; } = "";
    public string DednCode { get; set; } = "";
    public string AcctName { get; set; } = "";
    public DateTime? LoanDate { get; set; }
    public DateTime? PayStart { get; set; }
    public DateTime? PayRes { get; set; }
    public decimal Amount { get; set; }
    public decimal Amort { get; set; }
    public decimal Balance { get; set; }
    public decimal Principal { get; set; }
    public decimal InterestRate { get; set; }
    public int MoToPay { get; set; }
    public long CvNo { get; set; }
    public bool P1 { get; set; }
    public bool P2 { get; set; }
    public bool P3 { get; set; }
    public bool P4 { get; set; }
    public bool P5 { get; set; }
    public string? Remarks { get; set; }
    public string Status { get; set; } = "A";

    public M604_Loan Copy() => (M604_Loan)MemberwiseClone();
}

public class M604_Sched
{
    public int Id { get; set; }
    public int SchedNo { get; set; }
    public decimal Amort { get; set; }
    public string? Trn { get; set; }
}

public class M604_Tran
{
    public string? Trn { get; set; }
    public decimal Amount { get; set; }
    public DateTime XDate { get; set; }
}

public class M604_Recompute
{
    public M604_Loan? Loan { get; set; }
    public List<M604_Tran> Trans { get; set; } = new();
    public List<M604_Sched> Schedule { get; set; } = new();
    public decimal Deducted { get; set; }
    public decimal ExpectedBalance { get; set; }
}
