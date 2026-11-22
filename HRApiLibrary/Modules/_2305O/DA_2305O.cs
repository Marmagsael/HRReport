using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Modules._2303O;

namespace HRApiLibrary.Modules._2305O;

public class DA_2305O : IDA_2305O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2305O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    static DateTime? Clean(DateTime? d) => d is { } x && x.Year < 2 ? null : d;

    public async Task<M2303_Order?> _02Order(string number, string opis, string conn)
    {
        string sql = $@"select trim(h.number) Number, trim(ifnull(h.clnumber, '')) ClNumber, d.date PrepDate, d.sadate ReportOn, trim(ifnull(d.time, '')) Time,
                               trim(ifnull(d.empnumber, '')) EmpNumber, ifnull(d.reason, '') Reason
                        from {opis}.movehdr h join {opis}.movedtl d on d.number = h.number and d.mode = 'R'
                        where trim(h.number) = @N and h.mode = 'R' limit 1";
        var r = (await _sql.FetchLegacy<M2303_Order, dynamic>(sql, new { N = (number ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        if (r == null) return null;
        r.PrepDate = Clean(r.PrepDate); r.ReportOn = Clean(r.ReportOn);
        return r;
    }

    // -- the new number is R + yy + - + mm + a 4-digit sequence; For Training sets the employee status to I, otherwise B --
    public async Task<string> _01Save(M2303_Save m, bool forTraining, string opis, string conn)
    {
        string prefix = "R" + (m.PrepDate.Year % 100).ToString("00") + "-" + m.PrepDate.Month.ToString("00");
        var max = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(number, length(trim(number)) - 6), unsigned)) from {opis}.movehdr
                                                            where left(trim(number), 6) = @P and mode = 'R'", new { P = prefix }, conn) ?? []).FirstOrDefault();
        string number = prefix + ((max ?? 0) + 1).ToString("0000");
        string note = m.Note ?? "";

        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movehdr (number, prepdate, reqnumber, clnumber, mode, report, expdate, grpNo) values (@N, @D1, '', @Cl, 'R', '', '0000-00-00', '')",
            new { N = number, D1 = m.PrepDate.Date, Cl = m.ClNumber }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movedtl (number, empnumber, mode, date, time, sadate, reason) values (@N, @Emp, 'R', @D1, @Tm, @D2, @Note)",
            new { N = number, Emp = m.EmpNumber, D1 = m.PrepDate.Date, Tm = m.Time, D2 = m.ReportOn.Date, Note = note }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set movnumber = @N, movmode = 'R', movdate = @D1, movend = @D2, empstat_ = @St, remarks = @Rm where trim(empnumber) = @Emp",
            new { N = number, D1 = m.PrepDate.Date, D2 = m.ReportOn.Date, St = forTraining ? "I" : "B", Rm = note.Length > 150 ? note[..150] : note, Emp = m.EmpNumber }, conn);
        return number;
    }
}

public interface IDA_2305O
{
    Task<M2303_Order?> _02Order(string number, string opis, string conn);
    Task<string> _01Save(M2303_Save m, bool forTraining, string opis, string conn);
}
