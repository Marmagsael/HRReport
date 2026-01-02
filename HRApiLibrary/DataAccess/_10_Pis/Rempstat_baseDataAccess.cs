using Dapper;
using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class Rempstat_baseDataAccess : IRempstat_baseDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public Rempstat_baseDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<Rempstat_baseModel?> _01(string key, Rempstat_baseModel Rempstat_base, string schema, string conn)
    {

        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sql = $@"Insert into {schema}.{table} (RempstatId) values (@RempstatId); 
                        SELECT * FROM {schema}.{table};";
        var res = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, Rempstat_base, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<Rempstat_baseModel?>?> _01(string key, List<int> ids, string schema, string conn)
    {
        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        if (ids == null || !ids.Any())
            return new List<Rempstat_baseModel?>();

        var valuePlaceholders = new List<string>();
        var parameters = new DynamicParameters();

        for (int i = 0; i < ids.Count; i++)
        {
            string paramName = $"@Id{i}";
            valuePlaceholders.Add($"({paramName})");
            parameters.Add(paramName, ids[i]);
        }

        string valuesClause = string.Join(", ", valuePlaceholders);
        string sql = $@"
        INSERT INTO {schema}.{table} (RempstatId)
        VALUES {valuesClause};
        
        SELECT * FROM {schema}.{table};
         ";

        var result = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, parameters, conn);
        return result;
    }



    public async Task<Rempstat_baseModel?> _02(string key, int id, string schema, string conn)
    {
        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sql = $@"select  Id, RempstatId from {schema}.{table} where Id = @Id";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<Rempstat_baseModel?>?> _02(string key, string schema, string conn)
    {
        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table      = "rempstat_fordeployment";
        if (key == "deviation") table       = "rempstat_fordeviation";
        if (key == "disciplinary") table    = "rempstat_fordisciplinary";
        if (key == "exonerate") table       = "rempstat_forexonerate";
        if (key == "investigate") table     = "rempstat_forinvestigate";
        if (key == "reinstatement") table   = "rempstat_forreinstatement";

        string sql = $@"select  * from {schema}.{table} order by RempstatId";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, new { }, conn);
        return data;
    }



    public async Task<Rempstat_baseModel?> _03(string key, int id, Rempstat_baseModel Rempstat_base, string schema, string conn)
    {
        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sql = $@"Update {schema}.{table} set RempstatId = @RempstatId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, Rempstat_base, conn);

        sql = $@" select  * from {schema}.{table} x where x.Id = @Id ;";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<Rempstat_baseModel?> _04(string key, int id, string schema, string conn)
    {

        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sql = $@"Delete from {schema}.{table} where RempstatId = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.{table} x where x.RempstatId = @Id ;";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<Rempstat_baseModel?>?> _04(string key, List<int> ids, string schema, string conn)
    {


        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sqlDelete = $@"DELETE FROM {schema}.{table} WHERE RempstatId IN @Ids;";
        await _sql.ExecuteCmd<dynamic>(sqlDelete, new { Ids = ids }, conn);

        string sqlSelect = $@"SELECT * FROM {schema}.{table};";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sqlSelect, new { }, conn);

        return data;
    }

    public async Task<List<Rempstat_baseModel?>?> _04(string key, string schema, string conn)
    {

        string table = string.Empty;

        key = key.ToLower();
        if (key == "deployment") table = "rempstat_fordeployment";
        if (key == "deviation") table = "Rempstat_fordeviation";
        if (key == "disciplinary") table = "rempstat_fordisciplinary";
        if (key == "exonerate") table = "rempstat_forexonerate";
        if (key == "investigate") table = "rempstat_forinvestigate";
        if (key == "reinstatement") table = "rempstat_forreinstatement";

        string sqlDelete = $@"DELETE FROM {schema}.{table} ;";
        await _sql.ExecuteCmd<dynamic>(sqlDelete, new { }, conn);

        string sqlSelect = $@"SELECT * FROM {schema}.{table};";
        var data = await _sql.FetchData<Rempstat_baseModel?, dynamic>(sqlSelect, new { }, conn);

        return data;
    }
}
