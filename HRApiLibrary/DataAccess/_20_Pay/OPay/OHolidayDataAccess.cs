using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OHolidayDataAccess : IOHolidayDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public OHolidayDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<HolidayGlobalModel>> _02Global(int year, string schema, string conn)
    {
        var sql = $@"select Id, HDate, Name, TypeCode from {schema}.holidaysetting where Scope = 'G' and year(HDate) = @Year order by HDate";
        return await _sql.FetchLegacy<HolidayGlobalModel, dynamic>(sql, new { Year = year }, conn) ?? new();
    }

    public async Task<string?> _01Global(HolidayGlobalModel m, string schema, string conn)
    {
        var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.holidaysetting where Scope = 'G' and HDate = @HDate", new { m.HDate }, conn);
        if (dup.FirstOrDefault() > 0) return $"A global holiday already exists on {m.HDate:MMM dd, yyyy}.";

        await _sql.ExecuteLegacyCmd($"insert into {schema}.holidaysetting (HDate, Name, TypeCode, Scope, GlobalDate) values (@HDate, @Name, @TypeCode, 'G', @HDate)", m, conn);
        return null;
    }

    public async Task<string?> _03Global(HolidayGlobalModel m, string schema, string conn)
    {
        var dup = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {schema}.holidaysetting where Scope = 'G' and HDate = @HDate and Id <> @Id", new { m.HDate, m.Id }, conn);
        if (dup.FirstOrDefault() > 0) return $"A global holiday already exists on {m.HDate:MMM dd, yyyy}.";

        await _sql.ExecuteLegacyCmd($"update {schema}.holidaysetting set HDate = @HDate, GlobalDate = @HDate, Name = @Name, TypeCode = @TypeCode where Scope = 'G' and Id = @Id", m, conn);
        return null;
    }

    public async Task _04Global(int id, string schema, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {schema}.holidaysetting where Scope = 'G' and Id = @Id", new { Id = id }, conn);
    }

    public async Task<List<HolidayLocalModel>> _02Local(int year, string schema, string conn)
    {
        var sql = $@"select h.Id, h.HDate, h.Name, h.TypeCode,
                            group_concat(g.PayrollgrpId order by p.Name) as GroupIds,
                            group_concat(p.Name order by p.Name separator ', ') as GroupNames,
                            count(g.Id) as GroupCount
                     from   {schema}.holidaysetting h
                     left join {schema}.holidaysettinggrp g on g.HolidayId = h.Id
                     left join {schema}.payrollgrp p on p.Id = g.PayrollgrpId
                     where  h.Scope = 'L' and year(h.HDate) = @Year
                     group by h.Id, h.HDate, h.Name, h.TypeCode
                     order by h.HDate, h.Name";
        return await _sql.FetchLegacy<HolidayLocalModel, dynamic>(sql, new { Year = year }, conn) ?? new();
    }

    public async Task<string?> _01Local(HolidayLocalModel m, List<int> groupIds, string schema, string conn)
    {
        var msg = await ConflictMessage(m.HDate, groupIds, null, schema, conn);
        if (msg != null) return msg;

        var res = await _sql.FetchLegacy<long, dynamic>($@"insert into {schema}.holidaysetting (HDate, Name, TypeCode, Scope) values (@HDate, @Name, @TypeCode, 'L');
                                                           select last_insert_id();", m, conn);
        await SaveGroups(res.First(), m.HDate, groupIds, schema, conn);
        return null;
    }

    public async Task<string?> _03Local(HolidayLocalModel m, List<int> groupIds, string schema, string conn)
    {
        var msg = await ConflictMessage(m.HDate, groupIds, m.Id, schema, conn);
        if (msg != null) return msg;

        await _sql.ExecuteLegacyCmd($"update {schema}.holidaysetting set HDate = @HDate, Name = @Name, TypeCode = @TypeCode where Scope = 'L' and Id = @Id", m, conn);
        await _sql.ExecuteLegacyCmd($"delete from {schema}.holidaysettinggrp where HolidayId = @Id", new { m.Id }, conn);
        await SaveGroups(m.Id!.Value, m.HDate, groupIds, schema, conn);
        return null;
    }

    public async Task _04Local(int id, string schema, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {schema}.holidaysettinggrp where HolidayId = @Id", new { Id = id }, conn);
        await _sql.ExecuteLegacyCmd($"delete from {schema}.holidaysetting where Scope = 'L' and Id = @Id", new { Id = id }, conn);
    }

    private async Task<string?> ConflictMessage(DateTime hDate, List<int> groupIds, int? exceptId, string schema, string conn)
    {
        if (groupIds.Count == 0) return "Select at least one payroll group.";

        var sql = $@"select p.Name from {schema}.holidaysettinggrp g
                     join {schema}.payrollgrp p on p.Id = g.PayrollgrpId
                     where g.HDate = @HDate and g.PayrollgrpId in @GroupIds and g.HolidayId <> @ExceptId
                     order by p.Name";
        var names = await _sql.FetchLegacy<string, dynamic>(sql, new { HDate = hDate, GroupIds = groupIds, ExceptId = exceptId ?? 0 }, conn);
        if (names.Count == 0) return null;

        return $"Already has a local holiday on {hDate:MMM dd, yyyy}: {string.Join(", ", names.Take(5))}{(names.Count > 5 ? $" and {names.Count - 5} more" : "")}.";
    }

    private async Task SaveGroups(long holidayId, DateTime hDate, List<int> groupIds, string schema, string conn)
    {
        foreach (var gid in groupIds.Distinct())
            await _sql.ExecuteLegacyCmd($"insert into {schema}.holidaysettinggrp (HolidayId, PayrollgrpId, HDate) values (@HolidayId, @PayrollgrpId, @HDate)",
                                        new { HolidayId = holidayId, PayrollgrpId = gid, HDate = hDate }, conn);
    }
}

public interface IOHolidayDataAccess
{
    Task<List<HolidayGlobalModel>>  _02Global(int year, string schema, string conn);
    Task<string?>                   _01Global(HolidayGlobalModel m, string schema, string conn);
    Task<string?>                   _03Global(HolidayGlobalModel m, string schema, string conn);
    Task                            _04Global(int id, string schema, string conn);
    Task<List<HolidayLocalModel>>   _02Local(int year, string schema, string conn);
    Task<string?>                   _01Local(HolidayLocalModel m, List<int> groupIds, string schema, string conn);
    Task<string?>                   _03Local(HolidayLocalModel m, List<int> groupIds, string schema, string conn);
    Task                            _04Local(int id, string schema, string conn);
}
