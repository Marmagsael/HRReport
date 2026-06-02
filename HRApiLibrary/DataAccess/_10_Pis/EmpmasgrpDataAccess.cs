using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;
namespace HRApiLibrary.DataAccess._10_Pis;

public class EmpmasgrpDataAccess : IEmpmasgrpDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public EmpmasgrpDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<EmpmasgrpModel?> _01(EmpmasgrpModel empmasgrp, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Empmasgrp 
            (EmpmasId,  SecId,  DepId,  DivId,  LeaveGrpId,  EmpstatId) values 
            (@EmpmasId, @SecId, @DepId, @DivId, @LeaveGrpId, @EmpstatId)";
        await _sql.ExecuteCmd<dynamic>(sql, empmasgrp, conn);

        sql = $@"SELECT * FROM {schema}.Empmasgrp WHERE ID = (SELECT @@IDENTITY)";
        var res = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { }, conn);
        return res.FirstOrDefault();

    }
    
    public async Task _01FromLvGrp(EmpmasgrpModel empmasgrp, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Empmasgrp 
            (EmpmasId,  SecId,  DepId,  DivId,  LeaveGrpId  ) values 
            (@EmpmasId, 0,      0,      0,      @LeaveGrpId) 
            on duplicate key update LeaveGrpId = @LeaveGrpId";
        await _sql.ExecuteCmd<dynamic>(sql, empmasgrp, conn);

    }


    public async Task<EmpmasgrpModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  EmpmasId, SecId, DepId, DivId, LeaveGrpId, EmpstatId from {schema}.Empmasgrp where Id = @Id";
        var data = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<EmpmasgrpModel?>?> _02ByFldId(string? fldName, int? id, string? schema, string? conn)
    {
        string? sql = $@"SELECT g.*, concat(e.EmpLastNm, ', ', e.EmpFirstNm) EmpmasName, 
                            s.name SecName, d.Name DepName, d.Name DivName, l.Name LeavegrpName
                        FROM {schema}.Empmasgrp g
                        left join  {schema}.Empmas      e  on e.Id = g.EmpmasId
                        left join  {schema}.RSection    s  on s.Id = g.SecId
                        left join  {schema}.RDivision   d  on d.Id = g.DivId
                        left join  {schema}.Leavegrp    l  on l.Id = g.LeavegrpId
                        left join  {schema}.rempstat    es on es.Id = g.EmpstatId
                        where {fldName} = @Id ;";
        var data = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data;
    }
    
    public async Task<List<EmpmasgrpModel?>?> _02ByFldIds(string? fldName, List<int> ids, string? schema, string? conn)
    {
        string? sql = $@"SELECT g.*, concat(e.EmpLastNm, ', ', e.EmpFirstNm) EmpmasName, 
                            s.name SecName, d.Name DepName, d.Name DivName, l.Name LeavegrpName
                        FROM {schema}.Empmasgrp g
                        left join  {schema}.Empmas      e  on e.Id = g.EmpmasId
                        left join  {schema}.RSection    s  on s.Id = g.SecId
                        left join  {schema}.RDivision   d  on d.Id = g.DivId
                        left join  {schema}.Leavegrp    l  on l.Id = g.LeavegrpId
                        left join  {schema}.rempstat    es on es.Id = g.EmpstatId
                        where {fldName} in @Ids 
                        order by EmpmasName;";
        var data = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { Ids = ids }, conn);
        return data;
    }



    public async Task<EmpmasgrpModel?> _03(int? id, EmpmasgrpModel empmasgrp, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Empmasgrp set EmpmasId = @EmpmasId, SecId = @SecId, DepId = @DepId, DivId = @DivId, LeaveGrpId = @LeaveGrpId, EmpstatId = @EmpstatId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, empmasgrp, conn);

        sql = $@" select  * from {schema}.Empmasgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
    
    public async Task _03RemoveLvgrp(int? empmasId, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Empmasgrp set LeaveGrpId = 0 where EmpmasId = @EmpmasId;";
        await _sql.ExecuteCmd<dynamic>(sql, new {EmpmasId = empmasId}, conn);
    }


    public async Task<EmpmasgrpModel?> _04(int? id, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Empmasgrp where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Empmasgrp x where x.Id = @Id ;";
        var data = await _sql.FetchData<EmpmasgrpModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IEmpmasgrpDataAccess
{
    Task<EmpmasgrpModel?>           _01(EmpmasgrpModel empmasgrp, string? schema, string? conn);
    Task                            _01FromLvGrp(EmpmasgrpModel empmasgrp, string? schema, string? conn);
    Task<EmpmasgrpModel?>           _02(int? id, string? schema, string? conn);
    Task<List<EmpmasgrpModel?>?>    _02ByFldId(string? fldName, int? id, string? schema, string? conn);
    Task<List<EmpmasgrpModel?>?>    _02ByFldIds(string? fldName, List<int> ids, string? schema, string? conn); 
    Task<EmpmasgrpModel?>           _03(int? id, EmpmasgrpModel empmasgrp, string? schema, string? conn);
    Task                            _03RemoveLvgrp(int? empmasId, string? schema, string? conn); 
    Task<EmpmasgrpModel?>           _04(int? id, string? schema, string? conn);
}
