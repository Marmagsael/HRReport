using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class RdepapproverDataAccess : IRdepapproverDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdepapproverDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdepapproverModel?> _01(RdepapproverModel rdepapprover, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdepapprover (SystemId, Module) values (@SystemId, @Module) 
                            on duplicate key update Module = @Module; 
                        SELECT  a.Module, e.*, concat(trim(e.EmplastNm),', ',trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) as FullName
                        FROM {schema}.Rdepapprover a
                            left join {schema}.Empmas e on e.SystemId = a.SystemId
                        WHERE a.SystemId = @SystemId and Module = @Module;";

        var res = await _sql.FetchData<RdepapproverModel?, dynamic>(sql, rdepapprover, conn);

        return res.FirstOrDefault();
    }


    public async Task<List<RdepapproverModel?>?> _02(string schema, string conn)
    {
        string sql = $@"SELECT a.Module, e.*, concat(trim(e.EmplastNm),', ',trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) as FullName
                        FROM {schema}.Rdepapprover a
                            left join {schema}.Empmas e on e.SystemId = a.SystemId ;";
        var data = await _sql.FetchData<RdepapproverModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    public async Task<List<RdepapproverModel?>?> _02ByModule(string module, string schema, string conn)
    {
        string sql = $@"SELECT a.Module, e.*, concat(trim(e.EmplastNm),', ',trim(e.EmpFirstNm),' ',trim(e.EmpMidNm)) as FullName
                        FROM {schema}.Rdepapprover a
                            left join {schema}.Empmas e on e.SystemId = a.SystemId 
                        where Module = @Module ;";
        var data = await _sql.FetchData<RdepapproverModel?, dynamic>(sql, new { Module = module}, conn);
        return data;
    }


    public async Task<RdepapproverModel?> _04(int systemid, string module, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdepapprover where SystemId = @SystemId;
                        Select  * from {schema}.Rdepapprover where SystemId = @SystemId and Module = @Module ;";
        var data = await _sql.FetchData<RdepapproverModel?, dynamic>(sql, new { SystemId = systemid, Module = module }, conn);
        return data?.FirstOrDefault();
    }
}
