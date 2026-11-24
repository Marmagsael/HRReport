using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._13002O;

public class DA_13002O : IDA_13002O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_13002O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M13002_Client>> _02Clients(string opay, string conn)
    {
        string sql = $"select cast(Id as char) ClNumber, trim(Name) ClName from {opay}.payrollgrp where ifnull(Status, 'A') = 'A' order by Name";
        return await _sql.FetchLegacy<M13002_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M13002_Emp>> _02Employees(string clNumber, string opis, string conn)
    {
        string sql = $@"select trim(o.EmpNumber) EmpNumber, concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName
                        from {opis}.Empmas o
                             left join {opis}.deprec dp on trim(dp.EmpNumber) = trim(o.EmpNumber)
                        where (@Cl = '' or cast(dp.PayrollgrpId as char) = @Cl)
                        order by o.EmpLastNm, o.EmpFirstNm, o.EmpMidNm";
        return await _sql.FetchLegacy<M13002_Emp, dynamic>(sql, new { Cl = clNumber ?? "" }, conn) ?? [];
    }

    public async Task<List<M13002_Row>> _02Dtr(string clNumber, IEnumerable<string> empNumbers, DateTime from, DateTime to, string pisdb, string opis, string opay, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName,
                               trim(ifnull(pg.Name, '')) Client,
                               date(a.PunchInDate) PunchDate, dayname(a.PunchInDate) DayName, trim(ifnull(d.Name, '')) DutyType,
                               a.PunchInDate TimeIn, a.PunchOutDate TimeOut,
                               if(a.PunchInDate is not null and a.PunchOutDate is not null and a.PunchOutDate > a.PunchInDate, round(timestampdiff(minute, a.PunchInDate, a.PunchOutDate) / 60, 2), null) Hours
                        from {pisdb}.AttPunches1 a
                             join {pisdb}.Empmas e on e.Id = a.EmpmasId
                             left join {opis}.Empmas o on trim(o.EmpNumber) = trim(e.EmpNumber)
                             left join {opis}.deprec dp on trim(dp.EmpNumber) = trim(o.EmpNumber)
                             left join {opay}.payrollgrp pg on pg.Id = dp.PayrollgrpId
                             left join {pisdb}.AttDutyType d on d.Id = a.DutyTypeId
                        where a.PunchInDate >= @From and a.PunchInDate < date_add(@To, interval 1 day)
                          and (@Cl = '' or cast(dp.PayrollgrpId as char) = @Cl)
                          and trim(e.EmpNumber) in @Emps
                        order by o.EmpLastNm, o.EmpFirstNm, e.EmpNumber, a.PunchInDate";
        var r = await _sql.FetchLegacy<M13002_Row, dynamic>(sql, new { From = from.Date, To = to.Date, Cl = clNumber ?? "", Emps = empNumbers.Select(x => x.Trim()).ToList() }, conn) ?? [];
        foreach (var x in r)
        {
            if (x.TimeIn is { } i && i.Year < 2) x.TimeIn = null;
            if (x.TimeOut is { } o2 && o2.Year < 2) x.TimeOut = null;
        }
        return r;
    }
}

public interface IDA_13002O
{
    Task<List<M13002_Client>> _02Clients(string opay, string conn);
    Task<List<M13002_Emp>> _02Employees(string clNumber, string opis, string conn);
    Task<List<M13002_Row>> _02Dtr(string clNumber, IEnumerable<string> empNumbers, DateTime from, DateTime to, string pisdb, string opis, string opay, string conn);
}

public class M13002_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }
public class M13002_Emp { public string EmpNumber { get; set; } = ""; public string EmpName { get; set; } = ""; public string Display => $"{EmpName.Trim()}  ({EmpNumber})"; }

public class M13002_Row
{
    public string EmpNumber { get; set; } = "";
    public string EmpName { get; set; } = "";
    public string Client { get; set; } = "";
    public DateTime PunchDate { get; set; }
    public string DayName { get; set; } = "";
    public string DutyType { get; set; } = "";
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public decimal? Hours { get; set; }
}
