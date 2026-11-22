using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._2304O;

public class DA_2304O : IDA_2304O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2304O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    static DateTime? Clean(DateTime? d) => d is { } x && x.Year < 2 ? null : d;

    public async Task<List<M2304_Emp>> _02ActiveEmployees(string clNumber, string opis, string conn)
    {
        string sql = $@"select trim(a.empnumber) EmpNumber,
                               concat(trim(ifnull(a.emplastnm, '')), ', ', trim(ifnull(a.empfirstnm, '')), ' ', left(trim(ifnull(a.empmidnm, '')), 1), '.') EmpName,
                               ifnull(b.name, '') Status
                        from {opis}.empmas a left join {opis}.empstat b on a.empstat_ = b.code
                        where trim(a.client_) = @C and a.empstat_ = 'A' order by a.emplastnm, a.empfirstnm";
        return (await _sql.FetchLegacy<M2304_Emp, dynamic>(sql, new { C = (clNumber ?? "").Trim() }, conn) ?? []).ToList();
    }

    public async Task<M2304_Group?> _02Group(string grpNo, string opis, string conn)
    {
        string sql = $@"select trim(a.number) Number, trim(ifnull(a.clnumber, '')) ClNumber, b.date PrepDate, b.sadate ReportOn, trim(ifnull(b.time, '')) Time, ifnull(b.reason, '') Reason,
                               trim(b.empnumber) EmpNumber,
                               concat(trim(ifnull(c.emplastnm, '')), ', ', trim(ifnull(c.empfirstnm, '')), ' ', left(trim(ifnull(c.empmidnm, '')), 1), '.') EmpName
                        from {opis}.movehdr a left join {opis}.movedtl b on a.number = b.number and b.mode = 'R'
                        left join {opis}.empmas c on b.empnumber = c.empnumber
                        where a.mode = 'R' and trim(a.grpNo) = @G order by c.emplastnm, c.empfirstnm";
        var rows = (await _sql.FetchLegacy<M2304_Row, dynamic>(sql, new { G = (grpNo ?? "").Trim() }, conn) ?? []).ToList();
        if (rows.Count == 0) return null;
        var f = rows[0];
        return new M2304_Group
        {
            GrpNo = grpNo.Trim(), ClNumber = f.ClNumber, PrepDate = Clean(f.PrepDate), ReportOn = Clean(f.ReportOn), Time = f.Time, Reason = f.Reason,
            Employees = rows.Select(r => new M2304_Emp { EmpNumber = r.EmpNumber, EmpName = r.EmpName }).ToList()
        };
    }

    // -- group number is R + yy + - + mm + a 4-digit sequence over grpNo; each employee gets its own control number over number --
    public async Task<string> _01Save(M2304_Save m, string opis, string conn)
    {
        string prefix = "R" + (m.PrepDate.Year % 100).ToString("00") + "-" + m.PrepDate.Month.ToString("00");
        var gmax = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(grpNo, length(trim(grpNo)) - 6), unsigned)) from {opis}.movehdr
                                                             where left(trim(grpNo), 6) = @P and mode = 'R'", new { P = prefix }, conn) ?? []).FirstOrDefault();
        string grpNo = prefix + ((gmax ?? 0) + 1).ToString("0000");
        string note = m.Note ?? "";

        foreach (var emp in m.Employees)
        {
            var max = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(number, length(trim(number)) - 6), unsigned)) from {opis}.movehdr
                                                                where left(trim(number), 6) = @P and mode = 'R'", new { P = prefix }, conn) ?? []).FirstOrDefault();
            string number = prefix + ((max ?? 0) + 1).ToString("0000");
            await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movehdr (number, prepdate, reqnumber, clnumber, mode, report, expdate, grpNo) values (@N, @D1, '', @Cl, 'R', '', @D2, @G)",
                new { N = number, D1 = m.PrepDate.Date, Cl = m.ClNumber, D2 = m.ReportOn.Date, G = grpNo }, conn);
            await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movedtl (number, empnumber, mode, date, time, sadate, reason) values (@N, @Emp, 'R', @D1, @Tm, @D2, @Note)",
                new { N = number, Emp = emp, D1 = m.PrepDate.Date, Tm = m.Time, D2 = m.ReportOn.Date, Note = note }, conn);
            await _sql.ExecuteLegacyCmd<dynamic>($@"update {opis}.empmas set movnumber = @N, movmode = 'R', movdate = @D1, movend = @D2, empstat_ = 'B', remarks = @Rm, regref = '0000-00-00'
                                                   where trim(empnumber) = @Emp",
                new { N = number, D1 = m.PrepDate.Date, D2 = m.ReportOn.Date, Rm = note.Length > 150 ? note[..150] : note, Emp = emp }, conn);
        }
        return grpNo;
    }
}

public interface IDA_2304O
{
    Task<List<M2304_Emp>> _02ActiveEmployees(string clNumber, string opis, string conn);
    Task<M2304_Group?> _02Group(string grpNo, string opis, string conn);
    Task<string> _01Save(M2304_Save m, string opis, string conn);
}

public class M2304_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Status { get; set; } = ""; }

public class M2304_Row
{
    public string Number { get; set; } = ""; public string ClNumber { get; set; } = ""; public DateTime? PrepDate { get; set; } public DateTime? ReportOn { get; set; }
    public string Time { get; set; } = ""; public string Reason { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = "";
}

public class M2304_Group
{
    public string GrpNo { get; set; } = ""; public string ClNumber { get; set; } = ""; public DateTime? PrepDate { get; set; } public DateTime? ReportOn { get; set; }
    public string Time { get; set; } = ""; public string Reason { get; set; } = ""; public List<M2304_Emp> Employees { get; set; } = new();
}

public class M2304_Save
{
    public string ClNumber { get; set; } = ""; public DateTime PrepDate { get; set; } public DateTime ReportOn { get; set; }
    public string Time { get; set; } = "0000"; public string? Note { get; set; } public List<string> Employees { get; set; } = new();
}
