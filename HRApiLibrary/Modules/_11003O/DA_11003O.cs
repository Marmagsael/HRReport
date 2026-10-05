using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._11003O;
public class DA_11003O : IDA_11003O
{
    private readonly I_90_001_MySqlDataAccess _sql;
    public DA_11003O(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task _01CS_AtttemplateAtt(M11003CS_Atttemplate atttemplate, string pisdb, string conn)
    {
        string sql = $@"
        INSERT INTO {pisdb}.Atttemplate 
                (EmpmasId, AttendanceTypeId, 
                    D1_In, D1_HrsLength, D1_DutyType, D2_In, D2_HrsLength, D2_DutyType, D3_In, D3_HrsLength, D3_DutyType,
                    D4_In, D4_HrsLength, D4_DutyType, D5_In, D5_HrsLength, D5_DutyType, D6_In, D6_HrsLength, D6_DutyType,
                    D7_In, D7_HrsLength, D7_DutyType, 
                    AttschedweeklyhdrId, AttschedweeklyhdrIdAdv, ChangeSchedEffectivity, ChangeSchedEnd ) 
        SELECT @Empmasid, 1, 
                    D1_In, D1_HrsLength, D1_DutyType, D2_In, D2_HrsLength, D2_DutyType, D3_In, D3_HrsLength, D3_DutyType, 
                    D4_In, D4_HrsLength, D4_DutyType, D5_In, D5_HrsLength, D5_DutyType, D6_In, D6_HrsLength, D6_DutyType,
                    D7_In, D7_HrsLength, D7_DutyType, 
                    @AttschedweeklyhdrIdAdv, @AttschedweeklyhdrIdAdv, @ChangeSchedEffectivity, @ChangeSchedEnd
        FROM {pisdb}.attschedweeklydtl where AttSchedWeeklyHdrId = @Attschedweeklyhdrid limit 1
        ON DUPLICATE KEY UPDATE AttschedweeklyhdrIdAdv = @AttschedweeklyhdrIdAdv, ChangeSchedEffectivity = @ChangeSchedEffectivity, 
                ChangeSchedEnd = @ChangeSchedEnd; ";

        await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);
    }

    public async Task<List<M11003_Payrollgrp>?> _02Payrollgrps(string schema, string conn)
    {
        string sql = $@"SELECT Id, Code, ClNumber, Name FROM {schema}.Payrollgrp WHERE Status = 'A' ORDER BY Name";
        var data = await _sql.FetchData<M11003_Payrollgrp, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<M11003_Payrollgrp?> _02Payrollgrp(int id, string schema, string conn)
    {
        string sql = $@"SELECT Id, Code, ClNumber, Name FROM {schema}.Payrollgrp WHERE Id = @Id";
        var data = await _sql.FetchData<M11003_Payrollgrp, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<M11003_Empmas>?> _02Deprec_ByFieldId(string? fieldName, int? fieldIdValue, string? pisdb, string? opisdb, string? conn)
    {
        string? sql = $@"SELECT COALESCE(e1.systemid, 0) AS Systemid, d.Empnumber, d.PayrollgrpId, d.IdDeployment,
                            CONCAT(TRIM(COALESCE(e.EmpLastNm, '')), ', ', 
                            TRIM(COALESCE(e.EmpFirstNm, '')), ' ', 
                            TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName
                            FROM {opisdb}.Deprec d 
                            LEFT JOIN {opisdb}.Empmas e ON e.Empnumber = d.Empnumber 
                            left join {pisdb}.empmas e1 on e1.EmpNumber = e.empnumber
                            WHERE d.{fieldName} = @FieldIdValue ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm ";

        var data = await _sql.FetchData<M11003_Empmas, dynamic>(sql, new { FieldIdValue = fieldIdValue }, conn);
        return data ?? [];
    }

    public async Task<List<M11003_Empmas>?> _02EmpmasByKeyword(string keyword, string pisdb, string opisdb, string conn)
    {
        string sql = $@"SELECT COALESCE(e1.SystemId, 0) AS SystemId,
                               e.EmpNumber,
                               CONCAT(TRIM(COALESCE(e.EmpLastNm, '')), ', ',
                                      TRIM(COALESCE(e.EmpFirstNm, '')), ' ',
                                      TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName
                        FROM {opisdb}.Empmas e
                        LEFT JOIN {pisdb}.empmas e1 ON e1.EmpNumber = e.EmpNumber
                        WHERE e.EmpNumber   LIKE @Keyword
                           OR e.EmpLastNm   LIKE @Keyword
                           OR e.EmpFirstNm  LIKE @Keyword
                           OR e.EmpMidNm    LIKE @Keyword
                        ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm";

        var data = await _sql.FetchData<M11003_Empmas, dynamic>(sql, new { Keyword = $"%{keyword}%" }, conn);
        return data ?? [];
    }

    public async Task<List<M11003CS_Atttemplate>?> _02CS_EmplistPerPayrollgrp(int payrollgrpId, string pisdb, string opisdb, string conn)
    {
        string? sql = $@"SELECT COALESCE(e1.systemid, 0) AS Systemid, COALESCE(e1.systemid, 0) AS Empmasid, 
                            d.Empnumber, 
                            d.PayrollgrpId, 
                            d.IdDeployment,
                            h.Description AS CurrentScheduleName,
                            ha.Description AS AdvanceScheduleName,
                            CONCAT(TRIM(COALESCE(e.EmpLastNm, '')), ', ', 
                            TRIM(COALESCE(e.EmpFirstNm, '')), ' ', 
                            TRIM(COALESCE(e.EmpMidNm, ''))) AS EmpName,
                            at.ChangeSchedEffectivity,
                            at.ChangeSchedEnd 
                            FROM {opisdb}.Deprec d 
                            LEFT JOIN {opisdb}.Empmas             e   ON e.Empnumber    = d.Empnumber 
                            left join {pisdb}.empmas              e1  on e1.EmpNumber   = e.empnumber
                            left join {pisdb}.atttemplate         at  on at.EmpmasId    = e1.systemid
                            left join {pisdb}.attschedweeklyhdr   h   on h.id           = at.AttschedweeklyhdrId
                            left join {pisdb}.attschedweeklyhdr   ha  on ha.id          = at.AttschedweeklyhdrIdAdv
                            WHERE d.PayrollgrpId = @PayrollgrpId ORDER BY e.EmpLastNm, e.EmpFirstNm, e.EmpMidNm ";

        var data = await _sql.FetchData<M11003CS_Atttemplate, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data ?? [];
    }

    public async Task _03CS_AtttemplateAtt(M11003CS_Atttemplate atttemplate, string pisdb, string conn)
    {
        string? sql = $@"update {pisdb}.atttemplate set 
                                Attschedweeklyhdrid     = @Attschedweeklyhdrid, 
                                Attschedweeklyhdridadv  = @Attschedweeklyhdridadv, 
                                ChangeSchedEffectivity  = @ChangeSchedEffectivity
                         where empmasId = @Empmasid";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);
    }

    public async Task _03CS_AtttemplateAttAdvOnly(M11003CS_Atttemplate atttemplate, string pisdb, string conn)
    {
        string? sql = $@"update {pisdb}.atttemplate set 
                                Attschedweeklyhdridadv  = @Attschedweeklyhdridadv, 
                                ChangeSchedEffectivity  = @ChangeSchedEffectivity
                         where empmasId = @Empmasid";
        await _sql.ExecuteCmd<dynamic>(sql, atttemplate, conn);
    }

    public async Task _03CS_Atttemplate_CurrentSchedule(int systemid, int attschedweeklyhdrid, string pisdb, string conn)
    {
        string? sql = $@"update {pisdb}.atttemplate set Attschedweeklyhdrid = @Attschedweeklyhdrid where empmasId = @Empmasid";
        await _sql.ExecuteCmd<dynamic>(sql, new { EmpmasId = systemid, Attschedweeklyhdrid = attschedweeklyhdrid }, conn);
    }
}

public interface IDA_11003O
{
    Task                                _01CS_AtttemplateAtt(M11003CS_Atttemplate atttemplate, string pisdb, string conn);
    Task<List<M11003CS_Atttemplate>?>   _02CS_EmplistPerPayrollgrp(int payrollgrpId, string pisdb, string opisdb, string conn);
    Task<List<M11003_Empmas>?>          _02Deprec_ByFieldId(string? fieldName, int? fieldIdValue, string? pisdb, string? opisdb, string? conn);
    Task<List<M11003_Empmas>?>          _02EmpmasByKeyword(string keyword, string pisdb, string opisdb, string conn);
    Task<M11003_Payrollgrp?>            _02Payrollgrp(int id, string schema, string conn);
    Task<List<M11003_Payrollgrp>?>      _02Payrollgrps(string schema, string conn);
    Task                                _03CS_AtttemplateAtt(M11003CS_Atttemplate atttemplate, string pisdb, string conn);
    Task                                _03CS_AtttemplateAttAdvOnly(M11003CS_Atttemplate atttemplate, string pisdb, string conn);
    Task                                _03CS_Atttemplate_CurrentSchedule(int systemid, int attschedweeklyhdrid, string pisdb, string conn);
}