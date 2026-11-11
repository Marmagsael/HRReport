using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._2308O;

public class DA_2308O : IDA_2308O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2308O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    static DateTime? Clean(DateTime? d) => d is { } x && x.Year < 2 ? null : d;

    public async Task<List<M2308_Client>> _02Clients(string opis, string conn)
    {
        string sql = $@"select trim(c.clnumber) ClNumber, trim(c.clname) ClName, trim(ifnull(c.addr1, '')) Addr1,
                               ifnull(concat(trim(c.clname), ' - ', trim(a.areaname)), trim(c.clname)) ClName2
                        from {opis}.client c left join {opis}.area a on a.areacode = c.areacode
                        where c.status = 'Active' order by c.clname";
        return await _sql.FetchLegacy<M2308_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M2308_Desig>> _02Designations(string opis, string conn)
        => await _sql.FetchLegacy<M2308_Desig, dynamic>($"select trim(Code) Code, trim(Name) Name from {opis}.designation order by Name", new { }, conn) ?? [];

    public async Task<List<M2308_Signatory>> _02Signatories(string opis, string conn)
        => await _sql.FetchLegacy<M2308_Signatory, dynamic>($"select staff_id StaffId, trim(staff_nm) StaffNm, trim(staff_pos) StaffPos, defa Defa from {opis}.ofstaff3 order by staff_nm", new { }, conn) ?? [];

    public async Task _01Signatory(M2308_Signatory m, string opis, string conn)
    {
        if (m.Defa == 1) await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.ofstaff3 set defa = 0", new { }, conn);
        if (m.StaffId > 0)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.ofstaff3 set staff_nm = @StaffNm, staff_pos = @StaffPos, defa = @Defa where staff_id = @StaffId", m, conn);
        else
            await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.ofstaff3 (staff_nm, staff_pos, defa) values (@StaffNm, @StaffPos, @Defa)", m, conn);
    }

    public async Task _04Signatory(int id, string opis, string conn)
        => await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opis}.ofstaff3 where staff_id = @Id", new { Id = id }, conn);

    public async Task<(string Name, string Address)> _02Company(string opis, string conn)
    {
        var r = (await _sql.FetchLegacy<M2308_Company, dynamic>($"select trim(ifnull(CoName, '')) CoName, trim(ifnull(CoAdd, '')) CoAdd from {opis}.coinfo limit 1", new { }, conn) ?? []).FirstOrDefault();
        return (r?.CoName ?? "", r?.CoAdd ?? "");
    }

    public async Task<List<string>> _02AllowedStatuses(string opis, string conn)
        => (await _sql.FetchLegacy<string, dynamic>($"select trim(Code) from {opis}.movehdr_status", new { }, conn) ?? []).ToList();

    public async Task<M2308_Employee?> _02Employee(string empNumber, string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber, trim(ifnull(e.emplastnm, '')) LastNm, trim(ifnull(e.empfirstnm, '')) FirstNm, trim(ifnull(e.empmidnm, '')) MidNm,
                               trim(ifnull(e.empstat_, '')) StatCode, ifnull(s.Name, '---Undefined----') StatName,
                               trim(ifnull(e.movnumber, '')) MovNumber, trim(ifnull(e.movmode, '')) MovMode, e.movdate MovDate, e.movend MovEnd,
                               trim(ifnull(e.client_, '')) ClNumber, trim(ifnull(e.seclicense, '')) License, e.licexpire LicExpire,
                               e.datehired DateHired, trim(ifnull(e.desig_, '')) Desig
                        from {opis}.empmas e left join {opis}.empstat s on s.code = e.empstat_
                        where trim(e.empnumber) = @Emp";
        var r = (await _sql.FetchLegacy<M2308_Employee, dynamic>(sql, new { Emp = (empNumber ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        if (r == null) return null;
        r.MovDate = Clean(r.MovDate); r.MovEnd = Clean(r.MovEnd); r.LicExpire = Clean(r.LicExpire); r.DateHired = Clean(r.DateHired);
        return r;
    }

    public async Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber, trim(ifnull(e.emplastnm, '')) LastNm, trim(ifnull(e.empfirstnm, '')) FirstNm, trim(ifnull(e.empmidnm, '')) MidNm,
                               trim(ifnull(e.empstat_, '')) StatCode, ifnull(s.Name, '') StatName
                        from {opis}.empmas e left join {opis}.empstat s on s.code = e.empstat_
                        where e.empstat_ in (select code from {opis}.movehdr_status)
                          and (trim(e.empnumber) like @T or e.emplastnm like @T or e.empfirstnm like @T)
                        order by e.emplastnm, e.empfirstnm limit 200";
        return (await _sql.FetchLegacy<M2308_Employee, dynamic>(sql, new { T = "%" + (text ?? "").Trim() + "%" }, conn) ?? []).ToList();
    }

    public async Task<M2308_Order?> _02Order(string number, string opis, string conn)
    {
        string sql = $@"select trim(h.number) Number, h.expdate ExpDate, trim(ifnull(h.clnumber, '')) ClNumber, d.date DutyDate, trim(ifnull(d.empnumber, '')) EmpNumber, ifnull(d.reason, '') Note
                        from {opis}.movehdr h join {opis}.movedtl d on d.number = h.number and d.mode = 'A'
                        where trim(h.number) = @N and h.mode = 'A' limit 1";
        var r = (await _sql.FetchLegacy<M2308_Order, dynamic>(sql, new { N = (number ?? "").Trim() }, conn) ?? []).FirstOrDefault();
        if (r == null) return null;
        r.ExpDate = Clean(r.ExpDate); r.DutyDate = Clean(r.DutyDate);
        return r;
    }

    // -- the new number is A + yy + - + mm + a 4-digit sequence --
    public async Task<string> _01Save(M2308_Save m, string opis, string conn)
    {
        string prefix = "A" + (m.DutyDate.Year % 100).ToString("00") + "-" + m.DutyDate.Month.ToString("00");
        var max = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(number, length(trim(number)) - 6), unsigned)) from {opis}.movehdr
                                                            where left(trim(number), 6) = @P and mode = 'A'", new { P = prefix }, conn) ?? []).FirstOrDefault();
        string number = prefix + ((max ?? 0) + 1).ToString("0000");

        await _sql.ExecuteLegacyCmd<dynamic>($@"update {opis}.empmas set client_ = @Cl, movnumber = @N, movmode = 'A', movdate = @D1, movend = @D2, empstat_ = 'A',
                                                       desig_ = @Desig, remarks = '', clname = @ClName where trim(empnumber) = @Emp",
            new { Cl = m.ClNumber, N = number, D1 = m.DutyDate.Date, D2 = m.DutyEnd.Date, Desig = m.Desig, ClName = m.ClName, Emp = m.EmpNumber }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movehdr (number, prepdate, reqnumber, clnumber, mode, report, expdate, grpNo) values (@N, @D1, '', @Cl, 'A', '', @D2, '')",
            new { N = number, D1 = m.DutyDate.Date, Cl = m.ClNumber, D2 = m.DutyEnd.Date }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movedtl (number, empnumber, mode, date, time, sadate, reason) values (@N, @Emp, 'A', @D1, '', @D2, @Note)",
            new { N = number, Emp = m.EmpNumber, D1 = m.DutyDate.Date, D2 = m.DutyEnd.Date, Note = m.Note ?? "" }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($@"update {opis}.floatnotice set transno = '', date1 = '0000-00-00', date2 = '0000-00-00', date3 = '0000-00-00', date4 = '0000-00-00',
                                                       d1no = '', d2no = '', d3no = '', d4no = '' where trim(empnumber) = @Emp", new { Emp = m.EmpNumber }, conn);
        if (m.SetHired)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set datehired = @D1, regref = @R where trim(empnumber) = @Emp",
                new { D1 = m.DutyDate.Date, R = m.DutyDate.Date.AddMonths(6), Emp = m.EmpNumber }, conn);
        return number;
    }

    public async Task _04Delete(string number, string empNumber, string opis, string conn)
    {
        await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opis}.movehdr where trim(number) = @N and mode = 'A'", new { N = number }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opis}.movedtl where trim(number) = @N and mode = 'A'", new { N = number }, conn);
        var e = (await _sql.FetchLegacy<M2308_Employee, dynamic>($"select trim(movnumber) MovNumber, trim(movmode) MovMode from {opis}.empmas where trim(empnumber) = @Emp", new { Emp = empNumber }, conn) ?? []).FirstOrDefault();
        if (e != null && e.MovMode == "A" && e.MovNumber == number)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opis}.empmas set client_ = '', movmode = 'R', movnumber = '', empstat_ = 'B' where trim(empnumber) = @Emp", new { Emp = empNumber }, conn);
    }

    public async Task<List<M2308_Status>> _02Statuses(string opis, string conn)
    {
        string sql = $@"select trim(s.code) Code, trim(s.name) Name, if(m.code is null, 0, 1) Marked
                        from {opis}.empstat s left join {opis}.movehdr_status m on m.code = s.code order by Marked desc, s.name";
        return (await _sql.FetchLegacy<M2308_Status, dynamic>(sql, new { }, conn) ?? []).ToList();
    }

    public async Task _03Status(string code, bool marked, string user, string opis, string conn)
    {
        if (marked) await _sql.ExecuteLegacyCmd<dynamic>($"insert ignore into {opis}.movehdr_status (code) values (@C)", new { C = code }, conn);
        else await _sql.ExecuteLegacyCmd<dynamic>($"delete from {opis}.movehdr_status where code = @C", new { C = code }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opis}.movehdr_status_trail (usr, code, action, activity) values (@U, @C, @A, now())",
            new { U = user, C = code, A = marked ? "I" : "D" }, conn);
    }
}

public interface IDA_2308O
{
    Task<List<M2308_Client>> _02Clients(string opis, string conn);
    Task<List<M2308_Desig>> _02Designations(string opis, string conn);
    Task<List<M2308_Signatory>> _02Signatories(string opis, string conn);
    Task _01Signatory(M2308_Signatory m, string opis, string conn);
    Task _04Signatory(int id, string opis, string conn);
    Task<(string Name, string Address)> _02Company(string opis, string conn);
    Task<List<string>> _02AllowedStatuses(string opis, string conn);
    Task<M2308_Employee?> _02Employee(string empNumber, string opis, string conn);
    Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn);
    Task<M2308_Order?> _02Order(string number, string opis, string conn);
    Task<string> _01Save(M2308_Save m, string opis, string conn);
    Task _04Delete(string number, string empNumber, string opis, string conn);
    Task<List<M2308_Status>> _02Statuses(string opis, string conn);
    Task _03Status(string code, bool marked, string user, string opis, string conn);
}

public class M2308_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; public string Addr1 { get; set; } = ""; public string ClName2 { get; set; } = ""; }
public class M2308_Desig { public string Code { get; set; } = ""; public string Name { get; set; } = ""; }
public class M2308_Signatory { public int StaffId { get; set; } public string StaffNm { get; set; } = ""; public string StaffPos { get; set; } = ""; public int Defa { get; set; } }
public class M2308_Company { public string CoName { get; set; } = ""; public string CoAdd { get; set; } = ""; }
public class M2308_Status { public string Code { get; set; } = ""; public string Name { get; set; } = ""; public int Marked { get; set; } public bool IsMarked { get => Marked == 1; set => Marked = value ? 1 : 0; } }

public class M2308_Employee
{
    public string EmpNumber { get; set; } = ""; public string LastNm { get; set; } = ""; public string FirstNm { get; set; } = ""; public string MidNm { get; set; } = "";
    public string StatCode { get; set; } = ""; public string StatName { get; set; } = ""; public string MovNumber { get; set; } = ""; public string MovMode { get; set; } = "";
    public DateTime? MovDate { get; set; } public DateTime? MovEnd { get; set; } public string ClNumber { get; set; } = "";
    public string License { get; set; } = ""; public DateTime? LicExpire { get; set; } public DateTime? DateHired { get; set; } public string Desig { get; set; } = "";
    public string FullName => LastNm == "" ? "" : $"{LastNm}, {FirstNm} {(MidNm.Length > 0 ? MidNm[..1] + "." : "")}".Trim();
}

public class M2308_Order
{
    public string Number { get; set; } = ""; public DateTime? ExpDate { get; set; } public string ClNumber { get; set; } = "";
    public DateTime? DutyDate { get; set; } public string EmpNumber { get; set; } = ""; public string Note { get; set; } = "";
}

public class M2308_Save
{
    public string EmpNumber { get; set; } = ""; public string ClNumber { get; set; } = ""; public string ClName { get; set; } = "";
    public DateTime DutyDate { get; set; } public DateTime DutyEnd { get; set; } public string Desig { get; set; } = ""; public string? Note { get; set; }
    public bool SetHired { get; set; }
}
