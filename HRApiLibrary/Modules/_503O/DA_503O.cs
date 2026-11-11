using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._503O;

public class DA_503O : IDA_503O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_503O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M503_Acct>> _02Accounts(string opay, string conn)
    {
        string sql = $"select trim(AcctNumber) AcctNumber, trim(AcctName) AcctName from {opay}.chartofacct where AcctType = 'E' order by AcctName";
        return await _sql.FetchLegacy<M503_Acct, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<string> _02Setting(string opay, string conn)
    {
        string sql = $"select trim(ifnull(_13thMo, '')) from {opay}.paymaindtlsetup limit 1";
        var r = await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn);
        return r?.FirstOrDefault() ?? "";
    }

    public async Task _01Setting(string acctNumber, string opay, string conn)
    {
        await _sql.ExecuteLegacyCmd($"update {opay}.paymaindtlsetup set _13thMo = @A", new { A = acctNumber }, conn);
    }

    public async Task<List<string>> _02Periods(string opay, string conn)
    {
        string sql = $"select trim(PdNumber) from {opay}.period where PdNumber != '00' order by PdNumber";
        return await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<string>> _02TranYears(string opay, string conn)
    {
        string sql = $"select distinct left(trn, 2) from {opay}.tbltran order by left(trn, 2) desc";
        return await _sql.FetchLegacy<string, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M503_Client>> _02Clients(string opis, string conn)
    {
        string sql = $@"select trim(clnumber) ClNumber, trim(ifnull(clname, '')) ClName, ifnull(trim(status), 'No Status') Status
                        from {opis}.client order by ifnull(trim(status), 'No Status'), clname";
        return await _sql.FetchLegacy<M503_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M503_Status>> _02Statuses(string opis, string conn)
    {
        string sql = $"select trim(Code) Code, trim(Name) Name, if(IsInPayroll = 1, 1, 0) Mrk from {opis}.empstat order by Name";
        return await _sql.FetchLegacy<M503_Status, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M503_Row>> _02Fetch(string year4, int month, string periodId, string acct,
                                               List<string> prds, List<string> cls, List<string> statuses,
                                               string opis, string opay, string conn)
    {
        if (prds.Count == 0 || cls.Count == 0 || statuses.Count == 0) return [];

        string yr = year4.Length >= 2 ? year4[^2..] : year4;
        string prev = (int.Parse(yr) - 1).ToString().PadLeft(2, '0') + "12";

        string sql = $@"select trim(t.empnumber) EmpNumber,
                               concat(trim(e.emplastnm), ', ', trim(e.empfirstnm), ' ', ifnull(trim(e.empmidnm), '')) EmpName,
                               t.TotSal / 12 NewAmt, t.TotSal Bsal,
                               trim(ifnull(c.clname, '')) ClName, trim(ifnull(e.acctcode, '')) AcctCode, trim(ifnull(s.name, '')) EmpStatus
                        from (select empnumber, sum(amount) TotSal
                              from (select trn, acctNumber, empnumber, amount from {opay}.tbltran
                                    where (left(trn, 4) = @Prev or left(trn, 2) = @Yr) and acctNumber = 'E001'
                                    union
                                    select trn, acctNumber, empnumber, amount from {opay}.tmptbltran
                                    where (left(trn, 4) = @Prev or left(trn, 2) = @Yr) and acctNumber = 'E001') x
                              where left(trim(x.trn), 6) in @Prds and right(trim(x.trn), 5) in @Cls
                              group by empnumber) t
                             join {opis}.empmas e on trim(e.empnumber) = trim(t.empnumber)
                             left join {opis}.client c on c.clnumber = e.client_
                             left join {opis}.empstat s on s.code = e.empstat_
                        where trim(e.empstat_) in @Stats
                        order by EmpName";
        var rows = await _sql.FetchLegacy<M503_Row, dynamic>(sql, new { Prev = prev, Yr = yr, Prds = prds, Cls = cls, Stats = statuses }, conn) ?? [];

        string psql = $@"select trim(empNumber) EmpNumber, ifnull(amount, 0) Amount, ifnull(processed, 0) Processed, CONTROL_NO ControlNo
                         from {opay}.tblotherearnings
                         where acctNumber = @Acct and year = @Year and month = @Month and periodID = @Pid";
        var prevRows = await _sql.FetchLegacy<M503_Prev, dynamic>(psql, new { Acct = acct, Year = int.Parse(year4), Month = month, Pid = periodId }, conn) ?? [];
        foreach (var p in prevRows)
        {
            foreach (var r in rows.Where(x => x.EmpNumber == p.EmpNumber))
            {
                r.PrevAmt = p.Amount; r.IsPosted = p.Processed; r.ControlNo = p.ControlNo;
            }
        }
        return rows;
    }

    public async Task _01Save(List<M503_Row> rows, string year4, int month, string periodId, int periodIndex, string acct, string opay, string conn)
    {
        int pi = periodIndex < 1 ? 1 : periodIndex > 5 ? 5 : periodIndex;
        string flags = string.Join(", ", Enumerable.Range(1, 5).Select(i => $"p{i}={(i == pi ? 1 : 0)}"));
        foreach (var r in rows)
        {
            if (r.IsPosted == 1) continue;
            if (r.PrevAmt > 0)
            {
                await _sql.ExecuteLegacyCmd($@"update {opay}.tblotherearnings
                        set amount = @NewAmt, {flags}, remarks = concat('Prev. ', trim(amount))
                        where control_no = @ControlNo",
                    new { NewAmt = r.NewAmt, ControlNo = r.ControlNo }, conn);
            }
            else
            {
                await _sql.ExecuteLegacyCmd($@"insert into {opay}.tblotherearnings
                        (year, month, periodID, empnumber, acctNumber, amount, processed, remarks, p{pi}) values
                        (@Year, @Month, @Pid, @Emp, @Acct, @NewAmt, 0, '13th mo. pay', 1)",
                    new { Year = int.Parse(year4), Month = month, Pid = periodId, Emp = r.EmpNumber, Acct = acct, NewAmt = r.NewAmt }, conn);
            }
        }
    }

    public async Task _04Remove(List<int> controlNos, string opay, string conn)
    {
        foreach (var n in controlNos)
            await _sql.ExecuteLegacyCmd($"delete from {opay}.tblotherearnings where control_no = @N", new { N = n }, conn);
    }
}

public interface IDA_503O
{
    Task<List<M503_Acct>> _02Accounts(string opay, string conn);
    Task<string> _02Setting(string opay, string conn);
    Task _01Setting(string acctNumber, string opay, string conn);
    Task<List<string>> _02Periods(string opay, string conn);
    Task<List<string>> _02TranYears(string opay, string conn);
    Task<List<M503_Client>> _02Clients(string opis, string conn);
    Task<List<M503_Status>> _02Statuses(string opis, string conn);
    Task<List<M503_Row>> _02Fetch(string year4, int month, string periodId, string acct, List<string> prds, List<string> cls, List<string> statuses, string opis, string opay, string conn);
    Task _01Save(List<M503_Row> rows, string year4, int month, string periodId, int periodIndex, string acct, string opay, string conn);
    Task _04Remove(List<int> controlNos, string opay, string conn);
}

public class M503_Acct { public string AcctNumber { get; set; } = ""; public string AcctName { get; set; } = ""; }
public class M503_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; public string Status { get; set; } = ""; }
public class M503_Status { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public bool Mrk { get; set; } }
public class M503_Prev { public string EmpNumber { get; set; } = ""; public decimal Amount { get; set; } public int Processed { get; set; } public int ControlNo { get; set; } }

public class M503_Row
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public decimal NewAmt { get; set; }
    public decimal PrevAmt { get; set; }
    public int IsPosted { get; set; }
    public decimal Bsal { get; set; }
    public string ClName { get; set; } = "";
    public int ControlNo { get; set; }
    public string AcctCode { get; set; } = "";
    public string EmpStatus { get; set; } = "";
}
