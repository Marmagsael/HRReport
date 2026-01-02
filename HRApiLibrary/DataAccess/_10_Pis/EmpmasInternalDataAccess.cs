using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._00_Main;
using HRApiLibrary.Models._00_MainPis;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class EmpmasInternalDataAccess : IEmpmasInternalDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpmasInternalDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmpmasInternalModel?> _01(EmpmasInternalModel empmas, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Empmas 
                            (SystemId, EmpNumber, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias) values 
                            (@SystemId, @EmpNumber, @EmpLastNm, @EmpFirstNm, @EmpMidNm, @Suffix, @EmpAlias); 
                        SELECT * FROM {schema}.Empmas WHERE ID = (SELECT @@IDENTITY);";
        var res = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, empmas, conn);

        return res.FirstOrDefault();
    }

    public async Task<EmpmasInternalModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  * from {schema}.Empmas where Id = @Id";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<EmpmasInternalModel?> _02(string empnumber, string schema, string conn)
    {
        string sql = $@"select  Id, SystemId, EmpNumber, EmpLastNm, EmpFirstNm, EmpMidNm, Suffix, EmpAlias 
                        from {schema}.Empmas where EmpNumber = @EmpNumber";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task<List<EmpmasInternalModel?>?> _02(string schema, string conn)
    {
        string sql = $@"select  e.concat(trim(EmpLastNm),', ' trim(EmpFirstNm),' ' , trim(EmpMidNm)) Fullname, e.*  
                        from {schema}.Empmas e
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new {  }, conn);
        return data;
    }
    
    public async Task<List<EmpmasInternalModel?>?> _02byEmpnumber(string empnumber, string schema, string conn)
    {
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, 
                            e.*, s.Name EmpStatus  
                        from {schema}.Empmas e
                            left join {schema}.deprec    d on d.EmpmasId = e.Id
                            left join {schema}.rempstat  s on s.id       = d.empstatusId
                        where e.EmpNumber = @Empnumber 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return data;
    }

    public async Task<List<EmpmasInternalModel?>?> _02byEmpnumber(string empnumber, string pisdb, string paydb, string conn)
    {
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, 
                            e.*, s.Name EmpStatus, d.PayrollgrpId, g.Name  PayrollGrp 
                        from {pisdb}.Empmas e
                            left join {pisdb}.deprec        d on d.EmpmasId = e.Id
                            left join {pisdb}.rempstat      s on s.id       = d.empstatusId
                            left join {paydb}.payrollgrp    g on g.Id       = d.PayrollgrpId 
                        where e.EmpNumber = @Empnumber 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { EmpNumber = empnumber }, conn);
        return data;
    }

    public async Task<List<EmpmasInternalModel?>?> _02FilterByName(string name, string schema, string conn)
    {
        string vname = "%"+name.Trim()+"%";
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where e.EmplastNm like @Vname or e.EmpfirstNm like @Vname
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Vname = vname }, conn);
        return data;
    }
    
    public async Task<List<EmpmasInternalModel?>?> _02FilterByName(string name, string pisdb, string paydb, string conn)
    {
        string vname = "%"+name.Trim()+"%";
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, 
                            e.*, s.Name EmpStatus, d.PayrollgrpId, g.Name  PayrollGrp 
                        from {pisdb}.Empmas e
                            left join {pisdb}.deprec        d on d.EmpmasId = e.Id
                            left join {pisdb}.rempstat      s on s.id       = d.empstatusId
                            left join {paydb}.payrollgrp    g on g.Id       = d.PayrollgrpId 
                        where   e.EmplastNm like @Vname     or 
                                e.EmpfirstNm like @Vname    or 
                                concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) like @Vname 
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Vname = vname }, conn);
        return data;
    }
    

    public async Task<List<EmpmasInternalModel?>?> _02FilterByName(string name, int approverlvl, string schema, string conn)
    {
        string vname = "%"+name.Trim()+"%";
        string sql = $@"select  concat(trim(e.Emplastnm),', ',trim(e.Empfirstnm), ' ', trim(e.Empmidnm)) Fullname, e.*  
                        from {schema}.Empmas e
                        where (e.EmplastNm like @Vname or e.EmpfirstNm like @Vname) and 
                                Id not in (select EmpmasId from {schema}.LeaveDefaultApprover where Lvl = @Lvl )
                        order by EmpLastNm, EmpFirstNm, EmpMidNm";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Vname = vname, Lvl=approverlvl }, conn);
        return data;
    }



    public async Task<EmpmasInternalModel?> _03(int id, EmpmasInternalModel empmas, string schema, string conn)
    {
        empmas.Id = id;

        string sql = $@"Update {schema}.Empmas set 
                            SystemId    = @SystemId, 
                            EmpNumber   = @EmpNumber, 
                            EmpLastNm   = @EmpLastNm, 
                            EmpFirstNm  = @EmpFirstNm, 
                            EmpMidNm    = @EmpMidNm, 
                            Suffix      = @Suffix, 
                            EmpAlias    = @EmpAlias where Id = @Id;
                        select  * from {schema}.Empmas x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, empmas, conn);
        return data?.FirstOrDefault();
    }
    public async Task<EmpmasInternalModel?> _03SystemId(int systemId, string schema, string conn)
    {
        var id      = systemId<1 ? -10 : systemId; 
        var query   = "Select * from Main.Users where Id = @Id";
        var user    = await _sql.FetchData<UsersModel?, dynamic>(query, new { Id = systemId }, conn);
        var email   = user.FirstOrDefault()?.Email ?? "sample@email.test"; 
        
        var sql = $@"Update {schema}.Empmas set SystemId = @SystemId 
                     where Id in (select Id from {schema}.EmpmasAddress where EmailAdd = @EmailAdd ); 
                    Select * from {schema}.Empmas where SystemId = @SystemId ";
        
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new{SystemId = systemId, EmailAdd = email}, conn); 
        return data?.FirstOrDefault();
    }
    
    public async Task<EmpmasInternalModel?> _03SystemId(int empmasId, int systemId, string schema, string conn)
    {
        var sql = $@"Update {schema}.Empmas set SystemId = @SystemId where Id = @Id; 
                    Select * from {schema}.Empmas where Id = @Id ";
        
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new{SystemId = systemId, Id = empmasId}, conn); 
        return data?.FirstOrDefault();
    }
    
    public async Task<EmpmasInternalModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Empmas where Id = @Id;
                        select  * from {schema}.Empmas x where x.Id = @Id ; ";
        var data = await _sql.FetchData<EmpmasInternalModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

}
