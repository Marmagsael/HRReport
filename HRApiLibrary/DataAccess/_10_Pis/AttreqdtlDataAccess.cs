using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;


public class AttreqdtlDataAccess : IAttreqdtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqdtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqdtlModel?> _01(AttreqdtlModel attreqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqdtl 
                            (AttReqHdrId,  DStart,  DEnd,  TotHrs,  AttReqTypeId) values 
                            (@AttReqHdrId, @DStart, @DEnd, @TotHrs, @AttReqTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqdtl, conn);
        sql = $@"SELECT * FROM {schema}.Attreqdtl WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<AttreqdtlModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();
    }
    
    public async Task _01In(AttreqdtlModel attreqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqdtl (AttReqHdrId,  DStart,  AttReqTypeId) values (@AttReqHdrId, @DStart, @AttReqTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqdtl, conn);
        
    }
    public async Task _01Out(AttreqdtlModel attreqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqdtl (AttReqHdrId,  DEnd,  AttReqTypeId) values (@AttReqHdrId, @DEnd, @AttReqTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqdtl, conn);
    }
    
    public async Task _01InOut(AttreqdtlModel attreqdtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Attreqdtl 
                        (AttReqHdrId,  DStart,  DEnd,  TotHrs,  AttReqTypeId) values (@AttReqHdrId, @DStart, @DEnd, @TotHrs, @AttReqTypeId)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqdtl, conn);
        
    }

    public async Task<List<AttreqdtlModel?>> _02s(int id, string schema, string conn)
    {
        string sql = $@"select  Id, AttReqHdrId, DStart, DEnd, TotHrs, AttReqTypeId from {schema}.Attreqdtl where Id = @Id";
        var data = await _sql.FetchData<AttreqdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data ?? [];
    }
    
    public async Task<List<AttreqdtlModel?>> _02ByAttReqHdrIds(int attReqHdrId, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Attreqdtl where AttReqHdrId = @AttReqHdrId ";
        var data = await _sql.FetchData<AttreqdtlModel?, dynamic>(sql, new { AttReqHdrId = attReqHdrId }, conn);
        return data ?? [];
    }


    public async Task<AttreqdtlModel?> _03(int id, AttreqdtlModel attreqdtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Attreqdtl set 
							AttReqHdrId 	= @AttReqHdrId, 
							DStart 			= @DStart, 
							DEnd 			= @DEnd, 
							TotHrs 			= @TotHrs, 
							AttReqTypeId 	= @AttReqTypeId 
						where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attreqdtl, conn);

        sql = $@" select  * from {schema}.Attreqdtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqdtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attreqdtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

    }
    
    public async Task _04ByAttReqHdrId(int attReqHdrId, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Attreqdtl where AttReqHdrId 	= @AttReqHdrId;";
        await _sql.ExecuteCmd<dynamic>(sql, new { AttReqHdrId 	= attReqHdrId }, conn);

    }

}

public interface IAttreqdtlDataAccess
{
    Task<AttreqdtlModel?>       _01(AttreqdtlModel attreqdtl, string schema, string conn);
    Task                        _01In(AttreqdtlModel attreqdtl, string schema, string conn); 
    Task                        _01Out(AttreqdtlModel attreqdtl, string schema, string conn);
    Task<List<AttreqdtlModel?>> _02s(int id, string schema, string conn);
    Task<List<AttreqdtlModel?>> _02ByAttReqHdrIds(int attReqHdrId, string schema, string conn); 
    Task<AttreqdtlModel?>       _03(int id, AttreqdtlModel attreqdtl, string schema, string conn);
    Task                        _04(int id, string schema, string conn);
    Task                        _04ByAttReqHdrId(int attReqHdrId, string schema, string conn); 
    
}
