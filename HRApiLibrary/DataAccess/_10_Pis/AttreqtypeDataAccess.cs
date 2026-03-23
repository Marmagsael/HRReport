using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;


public class AttreqtypeDataAccess : IAttreqtypeDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqtypeDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqtypeModel?> _01(AttreqtypeModel attreqtype, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqtype (Code, Category, Name) values (@Code, @Category, @Name)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqtype, conn);

        sql = $@"SELECT * FROM {schema}.Attreqtype WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<AttreqtypeModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<AttreqtypeModel?>> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Code, Category, Name from {schema}.Attreqtype where Id = @Id";
        var data = await _sql.FetchData<AttreqtypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data ?? [];
    }
	
    public async Task<List<AttreqtypeModel?>> _02s(string schema, string conn)
    {
        string sql = $@"select  Id, Code, Category, Name from {schema}.Attreqtype ";
        var data = await _sql.FetchData<AttreqtypeModel?, dynamic>(sql, new { }, conn);
        return data ?? [];
    }


    public async Task<AttreqtypeModel?> _03(int id, AttreqtypeModel attreqtype, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attreqtype set Code = @Code, Category = @Category, Name = @Name where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attreqtype, conn);

        sql = $@" select  * from {schema}.Attreqtype x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqtypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttreqtypeModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attreqtype where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqtype x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqtypeModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IAttreqtypeDataAccess
{
    Task<AttreqtypeModel?> _01(AttreqtypeModel attreqtype, string schema, string conn);
    Task<List<AttreqtypeModel?>> _02s(int id, string schema, string conn);
	Task<List<AttreqtypeModel?>> _02s(string schema, string conn); 
    Task<AttreqtypeModel?> _03(int id, AttreqtypeModel attreqtype, string schema, string conn);
    Task<AttreqtypeModel?> _04(int id, string schema, string conn);
}
