using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class RdepDataAccess : IRdepDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public RdepDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<RdepModel?> _01(RdepModel rdep, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Rdep 
                            (Trndate,  MovStart,  MovEnd,  DepmodeId,  EmployTypeId,  DivId,  DepId,  SecId,  PosId,  PayrollgrpId,  ApprSystemId,  UserId,  Rstat,  Datecreated,  Dateapproved) values 
                            (@Trndate, @Movstart, @Movend, @Depmodeid, @Employtypeid, @Divid, @Depid, @Secid, @Posid, @Payrollgrpid, @Apprsystemid, @Userid, @Rstat, @Datecreated, @Dateapproved); 
                        SELECT * FROM {schema}.Rdep WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<RdepModel?, dynamic>(sql, rdep, conn);

        return res.FirstOrDefault();
    }


    public async Task<RdepModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rdep where Id = @Id";
        var data = await _sql.FetchData<RdepModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<RdepModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Rdep";
        var data = await _sql.FetchData<RdepModel?, dynamic>(sql, new { }, conn);
        return data;
    }
    

    public async Task<RdepModel?> _03(int id, RdepModel rdep, string schema, string conn)
    {
        string sql = $@"Update {schema}.Rdep set 
                            Trndate         = @Trndate,  
                            MovStart        = @Movstart,  
                            MovEnd          = @Movend,  
                            DepmodeId       = @DepmodeId,  
                            EmploytypeId    = @Employtypeid,  
                            DivId           = @Divid,  
                            DepId           = @Depid,  
                            SecId           = @Secid,  
                            PosId           = @Posid,  
                            PayrollgrpId    = @Payrollgrpid,  
                            ApprSystemId    = @Apprsystemid,  
                            UserId          = @Userid,  
                            Rstat           = @Rstat,  
                            Datecreated     = @Datecreated,  
                            Dateapproved    = @Dateapproved where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, rdep, conn);

        sql = $@" select  * from {schema}.Rdep x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<RdepModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Rdep where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Rdep x where x.Id = @Id ;";
        var data = await _sql.FetchData<RdepModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}