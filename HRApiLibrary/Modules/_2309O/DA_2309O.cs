using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._2309O;

public class DA_2309O : IDA_2309O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2309O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    static DateTime? Clean(DateTime? d) => d is { } x && x.Year < 2 ? null : d;

    public async Task<List<M2309_Violation>> _02Violations(string opis, string conn)
        => await _sql.FetchLegacy<M2309_Violation, dynamic>($"select trim(dev_no) DevNo, trim(dev_name) DevName from {opis}.devdata order by dev_name", new { }, conn) ?? [];

    public async Task<List<M2309_Penalty>> _02Penalties(string opis, string conn)
        => await _sql.FetchLegacy<M2309_Penalty, dynamic>($@"select distinct trim(penalty_no) PenaltyNo, trim(desc_) Desc_, resetregref ResetRegRef, isterminated IsTerminated, days Days
                                                             from {opis}.penalty order by desc_", new { }, conn) ?? [];

    public async Task<List<M2309_Status>> _02DeviationStatuses(string opis, string conn)
        => await _sql.FetchLegacy<M2309_Status, dynamic>($"select distinct trim(code) Code, trim(name) Name from {opis}.empstat where isDeviation = 1", new { }, conn) ?? [];

    public async Task<M2309_Rtu?> _02Rtu(string number, string opis, string conn)
    {
        string sql = $@"select trim(h.number) Number, h.prepdate PrepDate, trim(ifnull(h.clnumber, '')) ClNumber, trim(ifnull(d.empnumber, '')) EmpNumber, ifnull(d.reason, '') Reason
                        from {opis}.movehdr h join {opis}.movedtl d on d.number = h.number and d.mode = 'T'
                        where trim(h.number) = @N and h.mode = 'T' limit 1";
        var r = (await _sql.FetchLegacy<M2309_Rtu, dynamic>(sql, new { N = (number ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        if (r == null) return null;
        r.PrepDate = Clean(r.PrepDate);
        var dev = (await _sql.FetchLegacy<M2309_Dev, dynamic>($@"select trim(dev_no) DevNo, trim(penalty_no) PenaltyNo, occur_dt OccurDt, devstart DevStart, devend DevEnd, trim(freq_no) FreqNo
                                                                 from {opis}.devtran where trim(control_no) = @N limit 1", new { N = r.Number }, conn) ?? []).FirstOrDefault();
        if (dev != null) { dev.OccurDt = Clean(dev.OccurDt); dev.DevStart = Clean(dev.DevStart); dev.DevEnd = Clean(dev.DevEnd); }
        r.Existing = dev;
        return r;
    }

    public async Task<int> _02Times(string empNumber, int year, string devNo, string opis, string conn)
    {
        var n = (await _sql.FetchLegacy<int, dynamic>($@"select count(*) from {opis}.devtran where trim(empnumber) = @E and year(occur_dt) = @Y and trim(dev_no) = @D",
            new { E = (empNumber ?? "").Trim(), Y = year, D = (devNo ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        return n + 1;
    }

    public async Task _01Save(M2309_Save m, string opis, string conn)
    {
        await _sql.ExecuteLegacyCmd<dynamic>($@"insert into {opis}.devtran (control_no, prep_dt, clnumber, empnumber, dev_no, occur_dt, freq_no, penalty_no, devstart, devend)
                                                values (@C, @Prep, @Cl, @Emp, @Dev, @Occ, @Freq, @Pen, @S, @E)",
            new
            {
                C = m.ControlNo, Prep = DateTime.Today, Cl = m.ClNumber, Emp = m.EmpNumber, Dev = m.DevNo, Occ = m.OccurDt.Date, Freq = m.Freq.ToString(), Pen = m.PenaltyNo,
                S = m.DevStart?.Date ?? new DateTime(1, 1, 1), E = m.DevEnd?.Date ?? new DateTime(1, 1, 1)
            }, conn);
        if (m.DevStart == null)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.devtran set devstart = '0000-00-00', devend = '0000-00-00' where trim(control_no) = @C and trim(empnumber) = @Emp", new { C = m.ControlNo, Emp = m.EmpNumber }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set empstat_ = @St, movdate = @S, movend = @E, remarks = @Rm where trim(empnumber) = @Emp",
            new { St = m.EmpStat, S = m.DevStart?.Date, E = m.DevEnd?.Date, Rm = m.Remarks, Emp = m.EmpNumber }, conn);
        if (m.EmpStat == "A")
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set movmode = 'A' where trim(empnumber) = @Emp", new { Emp = m.EmpNumber }, conn);
    }

    public async Task<List<M2309_LineUp>> _02LineUp(string opis, string conn)
        => await _sql.FetchLegacy<M2309_LineUp, dynamic>($@"select trim(movnumber) MovNumber, movdate MovDate, trim(empnumber) EmpNumber,
                                                                   concat(trim(ifnull(emplastnm, '')), ', ', trim(ifnull(empfirstnm, '')), ' ', left(trim(ifnull(empmidnm, '')), 1), '.') EmpName
                                                            from {opis}.empmas where movmode = 'T' and empstat_ = ''", new { }, conn) ?? [];
}

public interface IDA_2309O
{
    Task<List<M2309_Violation>> _02Violations(string opis, string conn);
    Task<List<M2309_Penalty>> _02Penalties(string opis, string conn);
    Task<List<M2309_Status>> _02DeviationStatuses(string opis, string conn);
    Task<M2309_Rtu?> _02Rtu(string number, string opis, string conn);
    Task<int> _02Times(string empNumber, int year, string devNo, string opis, string conn);
    Task _01Save(M2309_Save m, string opis, string conn);
    Task<List<M2309_LineUp>> _02LineUp(string opis, string conn);
}

public class M2309_Violation { public string DevNo { get; set; } = ""; public string DevName { get; set; } = ""; }
public class M2309_Penalty { public string PenaltyNo { get; set; } = ""; public string Desc_ { get; set; } = ""; public string ResetRegRef { get; set; } = "0"; public string IsTerminated { get; set; } = "0"; public double Days { get; set; } }
public class M2309_Status { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public class M2309_Dev { public string DevNo { get; set; } = ""; public string PenaltyNo { get; set; } = ""; public DateTime? OccurDt { get; set; } public DateTime? DevStart { get; set; } public DateTime? DevEnd { get; set; } public string FreqNo { get; set; } = ""; }
public class M2309_Rtu { public string Number { get; set; } = ""; public DateTime? PrepDate { get; set; } public string ClNumber { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string Reason { get; set; } = ""; public M2309_Dev? Existing { get; set; } }
public class M2309_LineUp { public string MovNumber { get; set; } = ""; public DateTime? MovDate { get; set; } public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; }

public class M2309_Save
{
    public string ControlNo { get; set; } = ""; public string ClNumber { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string DevNo { get; set; } = ""; public string PenaltyNo { get; set; } = "";
    public DateTime OccurDt { get; set; } public int Freq { get; set; } public DateTime? DevStart { get; set; } public DateTime? DevEnd { get; set; } public string EmpStat { get; set; } = ""; public string Remarks { get; set; } = "";
}
