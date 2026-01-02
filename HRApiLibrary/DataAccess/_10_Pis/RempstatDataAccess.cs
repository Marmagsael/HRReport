using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RempstatDataAccess : IRempstatDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RempstatDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RempstatModel?> _01(RempstatModel rempstat, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rempstat 
                            (Code,  Name,  IsResigned,  IsOnLeaved,  IsFloating,  IsSuspended) values 
                            (@Code, @Name, @IsResigned, @IsOnLeaved, @IsFloating, @IsSuspended); 
                        SELECT * FROM {schema}.Rempstat WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<RempstatModel?, dynamic>(sql, rempstat, conn);
        return res.FirstOrDefault();
    }


    public async Task<RempstatModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rempstat where Id = @Id";
        var data = await _sql.FetchData<RempstatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<RempstatModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rempstat order by Name";
        var data = await _sql.FetchData<RempstatModel?, dynamic>(sql, new { }, conn);
        return data;
    }


    public async Task<RempstatModel?> _03(int id, RempstatModel rempstat, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rempstat set Code = @Code, Name = @Name, IsResigned = @IsResigned, IsOnLeaved = @IsOnLeaved, IsFloating = @IsFloating, IsSuspended = @IsSuspended where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rempstat, conn);

        sql = $@" select  * from {schema}.Rempstat x where x.Id = @Id ;";
        var data = await _sql.FetchData<RempstatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<List<RempstatModel>> _03(List<int> ids, string fieldName, int fieldVal, string schema, string conn)
    {
        string sql = $@"UPDATE {schema}.Rempstat SET {fieldName} = @FieldVal  WHERE Id IN @Ids;";

        await _sql.ExecuteCmd<dynamic>(sql, new { Ids = ids, FieldVal = fieldVal }, conn);

        sql = $@"SELECT * FROM {schema}.Rempstat  WHERE {fieldName} = 1;";

        var data = await _sql.FetchData<RempstatModel, dynamic>(sql, new { Ids = ids }, conn);
        return data;
    }


    public async Task<RempstatModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rempstat where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rempstat x where x.Id = @Id ;";
        var data = await _sql.FetchData<RempstatModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}