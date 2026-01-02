using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;
using Org.BouncyCastle.Asn1.Misc;

namespace HRApiLibrary.DataAccess._10_Pis;

public class PisEmpmasDataAccess : IPisEmpmasDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PisEmpmasDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }


    public async Task<PisEmpmasModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  e.* from {schema}.Empmas e where Id = @Id";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<List<PisEmpmasModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ' , trim(e.EmpMidNm)) Fullname, e.*,  s.Name EmpStat_,  
                                d.IsOnDeviation, d.IdDeviation, d.IsOnDiciplinary, d.IsOnInvestigation
                        from {schema}.Empmas e
                        left join {schema}.Deprec          d      on d.EmpmasId    = e.Id
                        left join {schema}.REmpstat        s      on s.Id          = d.Empstatusid 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<PisEmpmasModel?> _02ByEmpnumber(string empnumber, string schema, string conn)
    {
        string sql = $@"SELECT CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm), ' ', TRIM(e.EmpMidNm)) AS Fullname, e.*, 
                        s.Name AS EmpStat_, s.Id AS EmpStatId,
                        d.IsOnDeviation, d.IdDeviation, d.IsOnDiciplinary, d.IsOnInvestigation, d.DepDate DeploymentDate,
                        rd.Name AS DeploymentName
                        FROM {schema}.Empmas e
                        LEFT JOIN {schema}.Deprec d ON d.EmpmasId = e.Id
                        LEFT JOIN {schema}.REmpstat s ON s.Id = d.EmpStatusId 
                        LEFT JOIN {schema}.RDeployment rd ON d.IdDeployment = rd.Id
                        where Empnumber = @Empnumber 
                        order by EmpLastNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<PisEmpmasModel?>?> _02EmpByStatus(List<int> empstatusId, string schema, string conn)
    {
        string sql = $@"SELECT CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm), ' ', TRIM(e.EmpMidNm)) AS Fullname, e.*    
                        FROM {schema}.Empmas e    
                        LEFT JOIN {schema}.Deprec d ON d.EmpmasId = e.Id
                        WHERE d.EmpStatusId IN @Ids
                        ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm;";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Ids = empstatusId }, conn);
        return data;
    }
    
    // Detailed 
    public async Task<List<PisEmpmasModel?>?> _02ByStatus(List<int> empstatusId, string schema, string conn)
    {
        string sql = $@"SELECT CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm), ' ', TRIM(e.EmpMidNm)) AS Fullname, e.*, 
                        s.Name AS EmpStat_, s.Id AS EmpStatId,
                        d.IsOnDeviation, d.IdDeviation, d.IsOnDiciplinary, d.IsOnInvestigation, d.DepDate DeploymentDate,
                        rd.Name AS DeploymentName
                        FROM {schema}.Empmas e
                        LEFT JOIN {schema}.Deprec d ON d.EmpmasId = e.Id
                        LEFT JOIN {schema}.REmpstat s ON s.Id = d.EmpStatusId 
                        LEFT JOIN {schema}.RDeployment rd ON d.IdDeployment = rd.Id
                        WHERE d.EmpStatusId IN @Ids
                        ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm;
";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Ids = empstatusId  }, conn);
        return data;
    }

    public async Task<PisEmpmasModel?> _02BySystemId(int systemId, string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm),', ' ,trim(e.EmpFirstNm),' ' , trim(e.EmpMidNm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where SystemId = @SystemId 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { SystemId = systemId}, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<PisEmpmasModel?>?> _02BySystemIds(int systemId, string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm),', ' ,trim(e.EmpFirstNm),' ' , trim(e.EmpMidNm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where SystemId = @SystemId 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { SystemId = systemId}, conn);
        return data;
    }
    

    public async Task<List<PisEmpmasModel?>?> _02BySystemIdLst(int systemId, string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm), ', ', trim(e.EmpFirstNm),' ' , trim(e.EmpMidNm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where SystemId = @SystemId 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { SystemId = systemId}, conn);
        return data;
    }
    
    public async Task<List<PisEmpmasModel?>?> _02ByEmpnumbers(string empnumber, string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.EmpLastNm), ', ', trim(e.EmpFirstNm),' ' , trim(e.EmpMidNm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where Empnumber = @Empnumber 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Empnumber = empnumber}, conn);
        return data;
    }


    public async Task<List<PisEmpmasModel?>?> _02ByEmpIds(List<int> ids, string schema, string conn)
    {
        string sql = $@"SELECT CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm), ' ', TRIM(e.EmpMidNm)) AS Fullname, e.*, 
                        s.Name AS EmpStat_, s.Id AS EmpStatId,
                        d.IsOnDeviation, d.IdDeviation, d.IsOnDiciplinary, d.IsOnInvestigation, d.DepDate DeploymentDate,
                        rd.Name AS DeploymentName
                        FROM {schema}.Empmas e
                        LEFT JOIN {schema}.Deprec d ON d.EmpmasId = e.Id
                        LEFT JOIN {schema}.REmpstat s ON s.Id = d.EmpStatusId 
                        LEFT JOIN {schema}.RDeployment rd ON d.IdDeployment = rd.Id
                        where e.Id in @Ids 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Ids = ids }, conn);
        return data;
    }

    public async Task<List<PisEmpmasModel?>?> _02FilterByName(string name, string schema, string conn)
    {
        string vname = "%" + name.Trim() + "%";
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where e.EmplastNm like @Vname or e.EmpfirstNm like @Vname or e.Empmidnm like @Vname
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Vname = vname }, conn);
        return data;
    }

    public async Task<List<PisEmpmasModel?>?> _02FilterByName(string name, int approverlvl, string schema, string conn)
    {
        string vname = "%" + name.Trim() + "%";
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where (e.EmplastNm like @Vname or e.EmpfirstNm like @Vname) and 
                                Id not in (select EmpmasId from {schema}.LeaveDefaultApprover where Lvl = @Lvl )
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<PisEmpmasModel?, dynamic>(sql, new { Vname = vname, Lvl = approverlvl }, conn);
        return data;
    }



}
