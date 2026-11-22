using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Modules._2308O;

namespace HRApiLibrary.Modules._2307O;

public class DA_2307O : IDA_2307O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_2307O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn)
    {
        string sql = $@"select trim(e.empnumber) EmpNumber, trim(ifnull(e.emplastnm, '')) LastNm, trim(ifnull(e.empfirstnm, '')) FirstNm, trim(ifnull(e.empmidnm, '')) MidNm,
                               trim(ifnull(e.empstat_, '')) StatCode, ifnull(s.Name, '') StatName
                        from {opis}.empmas e left join {opis}.empstat s on s.code = e.empstat_
                        where (trim(e.empnumber) like @T or e.emplastnm like @T or e.empfirstnm like @T)
                        order by e.emplastnm, e.empfirstnm limit 200";
        return (await _sql.FetchLegacy<M2308_Employee, dynamic>(sql, new { T = "%" + (text ?? "").Trim() + "%" }, conn) ?? []).ToList();
    }

    public async Task<string> _01Save(M2308_Save m, string opis, string conn)
    {
        string prefix = "A" + (m.DutyDate.Year % 100).ToString("00") + "-" + m.DutyDate.Month.ToString("00");
        var max = (await _sql.FetchLegacy<int?, dynamic>($@"select max(convert(right(number, length(trim(number)) - 6), unsigned)) from {opis}.movehdr
                                                            where left(trim(number), 6) = @P and mode = 'A'", new { P = prefix }, conn) ?? []).FirstOrDefault();
        string number = prefix + ((max ?? 0) + 1).ToString("0000");

        await _sql.ExecuteLegacyCmd<dynamic>($@"update {opis}.empmas set client_ = @Cl, movnumber = @N, movmode = 'A', movdate = @D1, movend = @D2, empstat_ = 'A',
                                                       remarks = '', clname = @ClName where trim(empnumber) = @Emp",
            new { Cl = m.ClNumber, N = number, D1 = m.DutyDate.Date, D2 = m.DutyEnd.Date, ClName = m.ClName, Emp = m.EmpNumber }, conn);
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
}

public interface IDA_2307O
{
    Task<List<M2308_Employee>> _02SearchEmployees(string text, string opis, string conn);
    Task<string> _01Save(M2308_Save m, string opis, string conn);
}
