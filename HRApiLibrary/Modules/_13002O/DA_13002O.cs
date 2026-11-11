using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._13002O;

public class DA_13002O : IDA_13002O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_13002O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M13002_Client>> _02Clients(string opis, string conn)
    {
        string sql = $"select trim(clnumber) ClNumber, trim(clname) ClName from {opis}.client where status = 'Active' order by clname";
        return await _sql.FetchLegacy<M13002_Client, dynamic>(sql, new { }, conn) ?? [];
    }

    public async Task<List<M13002_Row>> _02Dtr(string clNumber, DateTime from, DateTime to, string pisdb, string opis, string conn)
    {
        string sql = $@"select trim(e.EmpNumber) EmpNumber,
                               concat(trim(ifnull(o.EmpLastNm, '')), ', ', trim(ifnull(o.EmpFirstNm, '')), ' ', trim(ifnull(o.EmpMidNm, ''))) EmpName,
                               trim(ifnull(c.clname, '')) Client,
                               a.PunchDate PunchDate, dayname(a.PunchDate) DayName, trim(ifnull(d.Name, '')) DutyType,
                               a.TimeIn TimeIn, a.TimeOut TimeOut,
                               if(a.TimeIn is not null and a.TimeOut is not null and a.TimeOut > a.TimeIn, round(timestampdiff(minute, a.TimeIn, a.TimeOut) / 60, 2), null) Hours
                        from {pisdb}.AttDaily a
                             join {pisdb}.Empmas e on e.Id = a.EmpmasId
                             left join {opis}.Empmas o on trim(o.EmpNumber) = trim(e.EmpNumber)
                             left join {opis}.client c on c.clnumber = o.client_
                             left join {pisdb}.AttDutyType d on d.Id = a.DutyTypeId
                        where a.PunchDate between @From and @To
                          and (@Cl = '' or trim(o.client_) = @Cl)
                        order by o.EmpLastNm, o.EmpFirstNm, e.EmpNumber, a.PunchDate";
        var r = await _sql.FetchLegacy<M13002_Row, dynamic>(sql, new { From = from.Date, To = to.Date, Cl = clNumber ?? "" }, conn) ?? [];
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
    Task<List<M13002_Client>> _02Clients(string opis, string conn);
    Task<List<M13002_Row>> _02Dtr(string clNumber, DateTime from, DateTime to, string pisdb, string opis, string conn);
}

public class M13002_Client { public string ClNumber { get; set; } = ""; public string ClName { get; set; } = ""; }

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
