using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._12006O;

public class DA_12006O : IDA_12006O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_12006O(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    // ─── Sites ───────────────────────────────────────────────

    public async Task _01_Site(M12006_Site site, string pisdb, string conn)
    {
        string sql = $@"
            INSERT INTO {pisdb}.Sites (Code, Name, Address, Latitude, Longitude, RadiusMeters, Status)
            VALUES (@Code, @Name, @Address, @Latitude, @Longitude, @RadiusMeters, @Status)
            ON DUPLICATE KEY UPDATE
                Name            = @Name,
                Address         = @Address,
                Latitude        = @Latitude,
                Longitude       = @Longitude,
                RadiusMeters    = @RadiusMeters,
                Status          = @Status;";
        await _sql.ExecuteCmd<dynamic>(sql, site, conn);
    }

    public async Task<List<M12006_Site>?> _02s_Sites(string pisdb, string conn)
    {
        string sql = $@"
            SELECT Id, Code, Name, Address, Latitude, Longitude, RadiusMeters, Status
            FROM {pisdb}.Sites
            ORDER BY Name;";
        return await _sql.FetchData<M12006_Site, dynamic>(sql, new { }, conn);
    }

    public async Task<M12006_Site?> _02_Site(int id, string pisdb, string conn)
    {
        string sql = $@"
            SELECT Id, Code, Name, Address, Latitude, Longitude, RadiusMeters, Status
            FROM {pisdb}.Sites
            WHERE Id = @Id;";
        var data = await _sql.FetchData<M12006_Site, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _03_Site(M12006_Site site, string pisdb, string conn)
    {
        string sql = $@"
            UPDATE {pisdb}.Sites SET
                Code            = @Code,
                Name            = @Name,
                Address         = @Address,
                Latitude        = @Latitude,
                Longitude       = @Longitude,
                RadiusMeters    = @RadiusMeters,
                Status          = @Status
            WHERE Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, site, conn);
    }

    public async Task _04_Site(int id, string pisdb, string conn)
    {
        string sql = $@"DELETE FROM {pisdb}.Sites WHERE Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }

    // ─── EmpSites ─────────────────────────────────────────────

    public async Task<List<M12006_EmpSite>?> _02s_EmpSites(int empmasId, string pisdb, string conn)
    {
        string sql = $@"
            SELECT es.Id, es.EmpmasId, es.SiteId, s.Name AS SiteName, s.Code AS SiteCode
            FROM {pisdb}.EmpSites es
            INNER JOIN {pisdb}.Sites s ON s.Id = es.SiteId
            WHERE es.EmpmasId = @EmpmasId
            ORDER BY s.Name;";
        return await _sql.FetchData<M12006_EmpSite, dynamic>(sql, new { EmpmasId = empmasId }, conn);
    }

    public async Task _01_EmpSite(M12006_EmpSite empSite, string pisdb, string conn)
    {
        string sql = $@"
            INSERT INTO {pisdb}.EmpSites (EmpmasId, SiteId)
            VALUES (@EmpmasId, @SiteId)
            ON DUPLICATE KEY UPDATE SiteId = @SiteId;";
        await _sql.ExecuteCmd<dynamic>(sql, empSite, conn);
    }

    public async Task _04_EmpSite(int id, string pisdb, string conn)
    {
        string sql = $@"DELETE FROM {pisdb}.EmpSites WHERE Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);
    }

    // ─── Geofence check (used at login) ───────────────────────

    public async Task<List<M12006_Site>?> _02s_SitesByEmpmas(int empmasId, string pisdb, string conn)
    {
        string sql = $@"
            SELECT s.Id, s.Code, s.Name, s.Address, s.Latitude, s.Longitude, s.RadiusMeters, s.Status
            FROM {pisdb}.EmpSites es
            INNER JOIN {pisdb}.Sites s ON s.Id = es.SiteId
            WHERE es.EmpmasId = @EmpmasId AND s.Status = 'A';";
        return await _sql.FetchData<M12006_Site, dynamic>(sql, new { EmpmasId = empmasId }, conn);
    }
}

public interface IDA_12006O
{
    Task                        _01_Site(M12006_Site site, string pisdb, string conn);
    Task<List<M12006_Site>?>    _02s_Sites(string pisdb, string conn);
    Task<M12006_Site?>          _02_Site(int id, string pisdb, string conn);
    Task                        _03_Site(M12006_Site site, string pisdb, string conn);
    Task                        _04_Site(int id, string pisdb, string conn);

    Task<List<M12006_EmpSite>?> _02s_EmpSites(int empmasId, string pisdb, string conn);
    Task                        _01_EmpSite(M12006_EmpSite empSite, string pisdb, string conn);
    Task                        _04_EmpSite(int id, string pisdb, string conn);

    Task<List<M12006_Site>?>    _02s_SitesByEmpmas(int empmasId, string pisdb, string conn);
}
