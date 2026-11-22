using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Modules._2308O;

namespace HRApiLibrary.Modules._2303O;

public class DA_2303O : IDA_2303O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2303O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    static DateTime? Clean(DateTime? d) => d is { } x && x.Year < 2 ? null : d;

    static string Table(int which) => which == 2 ? "ofstaff2" : "ofstaff";

    public async Task<List<M2308_Signatory>> _02Signatories(int which, string opis, string conn)
        => await _sql.FetchLegacy<M2308_Signatory, dynamic>($"select staff_id StaffId, trim(staff_nm) StaffNm, trim(staff_pos) StaffPos, defa Defa from {opis}.{Table(which)} order by staff_nm", new { }, conn) ?? [];

    public async Task _01Signatory(int which, M2308_Signatory m, string opis, string conn)
    {
        var t = Table(which);
        if (m.Defa == 1) await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.{t} set defa = 0", new { }, conn);
        if (m.StaffId > 0)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.{t} set staff_nm = @StaffNm, staff_pos = @StaffPos, defa = @Defa where staff_id = @StaffId", m, conn);
        else
            await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.{t} (staff_nm, staff_pos, defa) values (@StaffNm, @StaffPos, @Defa)", m, conn);
    }

    public async Task _04Signatory(int which, int id, string opis, string conn)
        => await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opis}.{Table(which)} where staff_id = @Id", new { Id = id }, conn);

    public async Task<M2303_Client?> _02Client(string clNumber, string opis, string conn)
    {
        string sql = $@"select trim(c.clnumber) ClNumber, trim(c.clname) ClName, trim(ifnull(c.areacode, '')) AreaCode
                        from {opis}.client c where trim(c.clnumber) = @C";
        return (await _sql.FetchLegacy<M2303_Client, dynamic>(sql, new { C = (clNumber ?? "").Trim() }, conn) ?? []).FirstOrDefault();
    }

    public async Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber, trim(ifnull(e.emplastnm, '')) LastNm, trim(ifnull(e.empfirstnm, '')) FirstNm, trim(ifnull(e.empmidnm, '')) MidNm,
                               trim(ifnull(e.empstat_, '')) StatCode, ifnull(s.Name, '') StatName
                        from {opis}.empmas e left join {opis}.empstat s on s.code = e.empstat_
                        where (trim(e.empnumber) like @T or e.emplastnm like @T or e.empfirstnm like @T)
                        order by e.emplastnm, e.empfirstnm limit 200";
        return (await _sql.FetchLegacy<M2308_Employee, dynamic>(sql, new { T = "%" + (text ?? "").Trim() + "%" }, conn) ?? []).ToList();
    }

    public async Task<M2303_Order?> _02Order(string number, string opis, string conn)
    {
        string sql = $@"select trim(h.number) Number, trim(ifnull(h.clnumber, '')) ClNumber, d.date PrepDate, d.sadate ReportOn, trim(ifnull(d.time, '')) Time,
                               trim(ifnull(d.empnumber, '')) EmpNumber, ifnull(d.reason, '') Reason
                        from {opis}.movehdr h join {opis}.movedtl d on d.number = h.number and d.mode = 'T'
                        where trim(h.number) = @N and h.mode = 'T' limit 1";
        var r = (await _sql.FetchLegacy<M2303_Order, dynamic>(sql, new { N = (number ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        if (r == null) return null;
        r.PrepDate = Clean(r.PrepDate); r.ReportOn = Clean(r.ReportOn);
        return r;
    }

    // -- the new number is T + yy + - + mm + a 4-digit sequence --
    public async Task<string> _01Save(M2303_Save m, string opis, string conn)
    {
        string prefix = "T" + (m.PrepDate.Year % 100).ToString("00") + "-" + m.PrepDate.Month.ToString("00");
        var max = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(number, length(trim(number)) - 6), unsigned)) from {opis}.movehdr
                                                            where left(trim(number), 6) = @P and mode = 'T'", new { P = prefix }, conn) ?? []).FirstOrDefault();
        string number = prefix + ((max ?? 0) + 1).ToString("0000");

        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movehdr (number, prepdate, reqnumber, clnumber, mode, report, expdate, grpNo) values (@N, @D1, '', @Cl, 'T', '', '0000-00-00', '')",
            new { N = number, D1 = m.PrepDate.Date, Cl = m.ClNumber }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movedtl (number, empnumber, mode, date, time, sadate, reason) values (@N, @Emp, 'T', @D1, @Tm, @D2, @Note)",
            new { N = number, Emp = m.EmpNumber, D1 = m.PrepDate.Date, Tm = m.Time, D2 = m.ReportOn.Date, Note = m.Note ?? "" }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set movnumber = @N, movmode = 'T', movdate = @D1, movend = @D2, empstat_ = '' where trim(empnumber) = @Emp",
            new { N = number, D1 = m.PrepDate.Date, D2 = m.ReportOn.Date, Emp = m.EmpNumber }, conn);
        return number;
    }
}

public interface IDA_2303O
{
    Task<List<M2308_Signatory>> _02Signatories(int which, string opis, string conn);
    Task _01Signatory(int which, M2308_Signatory m, string opis, string conn);
    Task _04Signatory(int which, int id, string opis, string conn);
    Task<M2303_Client?> _02Client(string clNumber, string opis, string conn);
    Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn);
    Task<M2303_Order?> _02Order(string number, string opis, string conn);
    Task<string> _01Save(M2303_Save m, string opis, string conn);
}

public class M2303_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; public string AreaCode { get; set; } = ""; }

public class M2303_Order
{
    public string Number { get; set; } = ""; public string ClNumber { get; set; } = ""; public DateTime? PrepDate { get; set; } public DateTime? ReportOn { get; set; }
    public string Time { get; set; } = ""; public string EmpNumber { get; set; } = ""; public string Reason { get; set; } = "";
}

public class M2303_Save
{
    public string EmpNumber { get; set; } = ""; public string ClNumber { get; set; } = ""; public DateTime PrepDate { get; set; } public DateTime ReportOn { get; set; }
    public string Time { get; set; } = "0000"; public string? Note { get; set; }
}
