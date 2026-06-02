using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
using MimeKit.Cryptography;

public class AttreqhistDataAccess : IAttreqhistDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public AttreqhistDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<AttreqhistModel?> _01(AttreqhistModel attreqhist, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Attreqhist 
                            (AttReqHdrId,  DActionTaken,  SetStatusTo,  Remarks,  Empnumber_Approver) values 
                            (@AttReqHdrId, @DActionTaken, @SetStatusTo, @Remarks, @Empnumber_Approver)";
        await _sql.ExecuteCmd<dynamic>(sql, attreqhist, conn);

        sql = $@"SELECT * FROM {schema}.Attreqhist WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<AttreqhistModel?>> _02s(int? id, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*  
                        from {pisdb}.Attreqhist h 
                        left join {opisdb}.empmas e on e.empnumber = h.empnumber_approver 
                        where Id = @Id";
        var data = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data ?? [];
    }

    public async Task<List<AttreqhistModel?>> _02s(string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*  
                        from {pisdb}.Attreqhist h 
                        left join {opisdb}.empmas e on e.empnumber = h.empnumber_approver 
                        where Id = @Id";
        var data = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { }, conn);
        return data ?? [];
    }
   
    public async Task<List<AttreqhistModel?>> _02ByAttreqhdrId(int? attreqhdrId, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"select  CONCAT_WS(' ', TRIM(e.EmpFirstNm), trim(e.EmpMidNm), TRIM(e.EmpLastNm)) AS ApproverName, h.*  
                        from {pisdb}.Attreqhist h 
                        left join {opisdb}.empmas e on e.empnumber = h.empnumber_approver 
                        where AttReqHdrId = @AttReqHdrId";
        // Console.WriteLine($"SQL : {sql} * ");                 
        var data = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { AttReqHdrId = attreqhdrId }, conn);
        return data ?? [];
    }



    public async Task<AttreqhistModel?> _03(int? id, AttreqhistModel attreqhist, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Attreqhist set 
                            AttReqHdrId         = @AttReqHdrId, 
                            DActionTaken        = @DActionTaken, 
                            SetStatusTo         = @SetStatusTo,
                            Empnumber_Approver  = @Empnumber_Approver,
                            Remarks             = @Remarks where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, attreqhist, conn);

        sql = $@" select  * from {schema}.Attreqhist x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<AttreqhistModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Attreqhist where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Attreqhist x where x.Id = @Id ;";
        var data = await _sql.FetchData<AttreqhistModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}


public interface IAttreqhistDataAccess
{
    Task<AttreqhistModel?>          _01(AttreqhistModel attreqhist, string? schema, string? conn);
    Task<List<AttreqhistModel?>>    _02s(int? id, string? pisdb, string? opisdb, string? conn);
    Task<List<AttreqhistModel?>>    _02s(string? pisdb, string? opisdb, string? conn);
    Task<List<AttreqhistModel?>>    _02ByAttreqhdrId(int? attreqhdrId, string? pisdb, string? opisdb, string? conn); 
    Task<AttreqhistModel?>          _03(int? id, AttreqhistModel attreqhist, string? schema, string? conn);
    Task<AttreqhistModel?>          _04(int? id, string? schema, string? conn);
}
