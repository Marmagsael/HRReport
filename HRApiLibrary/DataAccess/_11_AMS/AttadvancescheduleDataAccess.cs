using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._11_AMS;

public class AttadvancescheduleDataAccess : IAttadvancescheduleDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttadvancescheduleDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttadvancescheduleModel?> _01(AttadvancescheduleModel attadvanceschedule, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attadvanceschedule (Empnumber, Date, attscheddailyId, DutyType, PIn, Duration, Pout, ND) values (@Empnumber, @Date, @attscheddailyId, @DutyType, @PIn, @Duration, @Pout, @ND)";
        await _sql.ExecuteCmd<dynamic>(sql, attadvanceschedule, conn);

        sql = $@"SELECT * FROM {schema}.Attadvanceschedule WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<AttadvancescheduleModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<AttadvancescheduleModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Empnumber, Date, attscheddailyId, DutyType, PIn, Duration, Pout, ND from {schema}.Attadvanceschedule where Id = @Id";
        var data = await _sql.FetchData<AttadvancescheduleModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    public async Task<List<AttadvancescheduleModel?>?> _02sEmpNumberDateRange(string empNumber, DateTime startDate, DateTime endDate, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attadvanceschedule where Empnumber = @EmpNumber and Date >= @StartDate and Date <= @EndDate";
        var data = await _sql.FetchData<AttadvancescheduleModel?, dynamic>(sql, new { EmpNumber = empNumber, StartDate = startDate.Date, EndDate = endDate.Date.AddDays(1) }, conn);
        return data;
    }


    public async Task<AttadvancescheduleModel?> _03(int id, AttadvancescheduleModel attadvanceschedule, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attadvanceschedule set Empnumber = @Empnumber, Date = @Date, attscheddailyId = @attscheddailyId, DutyType = @DutyType, PIn = @PIn, Duration = @Duration, Pout = @Pout, ND = @ND where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attadvanceschedule, conn);

        sql = $@" select  * from {schema}.Attadvanceschedule x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttadvancescheduleModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttadvancescheduleModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attadvanceschedule where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attadvanceschedule x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttadvancescheduleModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttadvancescheduleDataAccess
{
    Task<AttadvancescheduleModel?> _01(AttadvancescheduleModel attadvanceschedule, string schema, string conn);
    Task<AttadvancescheduleModel?> _02(int id, string schema, string conn);
    Task<List<AttadvancescheduleModel?>?> _02sEmpNumberDateRange(string empNumber, DateTime startDate, DateTime endDate, string schema, string conn);
    Task<AttadvancescheduleModel?> _03(int id, AttadvancescheduleModel attadvanceschedule, string schema, string conn);
    Task<AttadvancescheduleModel?> _04(int id, string schema, string conn);
}
