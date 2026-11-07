using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class OLeaveTypeSyncDataAccess : IOLeaveTypeSyncDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public OLeaveTypeSyncDataAccess(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    // OPis levtbl.LVCODE is 2 characters; a longer code cannot be kept in the old PIS
    public bool CodeFits(string? code) => !string.IsNullOrWhiteSpace(code) && code.Trim().Length <= 2;

    public async Task _01Upsert(string? code, string? name, string? opisdb, string? conn)
    {
        if (!CodeFits(code)) return;
        var has = await _sql.FetchLegacy<long, dynamic>($"select count(*) from {opisdb}.levtbl where trim(upper(LVCODE)) = trim(upper(@Code))", new { Code = code }, conn);
        if (has.FirstOrDefault() > 0)
            await _sql.ExecuteLegacyCmd<dynamic>($"update {opisdb}.levtbl set LVNAME = left(trim(@Name), 20) where trim(upper(LVCODE)) = trim(upper(@Code))", new { Code = code, Name = name }, conn);
        else
            await _sql.ExecuteLegacyCmd<dynamic>($"insert into {opisdb}.levtbl (LVCODE, LVNAME, EARNCODE, NO_INCDATE) values (trim(@Code), left(trim(@Name), 20), '  ', '0')", new { Code = code, Name = name }, conn);
    }

    // the code was changed in the PIS : the old PIS levtbl and its leave1 rows follow
    public async Task _03Rename(string? oldCode, string? newCode, string? name, string? opisdb, string? conn)
    {
        if (string.Equals(oldCode?.Trim(), newCode?.Trim(), StringComparison.OrdinalIgnoreCase)) { await _01Upsert(newCode, name, opisdb, conn); return; }
        if (!CodeFits(newCode)) return;
        await _sql.ExecuteLegacyCmd<dynamic>($"update {opisdb}.leave1 set LVCODE = trim(@New) where trim(upper(LVCODE)) = trim(upper(@Old))", new { Old = oldCode, New = newCode }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>($"update {opisdb}.levtbl set LVCODE = trim(@New), LVNAME = left(trim(@Name), 20) where trim(upper(LVCODE)) = trim(upper(@Old))", new { Old = oldCode, New = newCode, Name = name }, conn);
        await _01Upsert(newCode, name, opisdb, conn);
    }

    public Task _04Delete(string? code, string? opisdb, string? conn) =>
        _sql.ExecuteLegacyCmd<dynamic>($"delete from {opisdb}.levtbl where trim(upper(LVCODE)) = trim(upper(@Code))", new { Code = code }, conn);

    // one time : both lists get the leave types the other one has (2 character codes only)
    public async Task _05SyncAll(string? pisdb, string? opisdb, string? conn)
    {
        await _sql.ExecuteLegacyCmd<dynamic>(
            $@"insert into {opisdb}.levtbl (LVCODE, LVNAME, EARNCODE, NO_INCDATE)
               select trim(p.Code), left(p.LeaveName, 20), '  ', '0' from {pisdb}.Leavetype p
               where char_length(trim(p.Code)) <= 2
                 and not exists (select 1 from {opisdb}.levtbl o where trim(upper(o.LVCODE)) = trim(upper(p.Code)))", new { }, conn);
        await _sql.ExecuteLegacyCmd<dynamic>(
            $@"insert into {pisdb}.Leavetype (Code, LeaveName, AnivStart, AnivEnd, DefValue)
               select trim(o.LVCODE), trim(o.LVNAME), makedate(year(now()), 1), concat(year(now()), '-12-31'), 0 from {opisdb}.levtbl o
               where trim(ifnull(o.LVCODE, '')) <> ''
                 and not exists (select 1 from {pisdb}.Leavetype p where trim(upper(p.Code)) = trim(upper(o.LVCODE)))", new { }, conn);
    }
}

public interface IOLeaveTypeSyncDataAccess
{
    bool CodeFits(string? code);
    Task _01Upsert(string? code, string? name, string? opisdb, string? conn);
    Task _03Rename(string? oldCode, string? newCode, string? name, string? opisdb, string? conn);
    Task _04Delete(string? code, string? opisdb, string? conn);
    Task _05SyncAll(string? pisdb, string? opisdb, string? conn);
}
