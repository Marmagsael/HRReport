using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class DeprecDataAccess : IDeprecDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public DeprecDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<DeprecModel?> _01(DeprecModel deprec, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Deprec 
                        (EmpmasId,               DivId,               DepId,                SecId,              LeavegrpId,   PayrollgrpId, Positionid, EmploymentTypeId,  
                         EmpStatusId,            DHired,              DRegularization,      DTraineeStart,      DTraineeEnd,  
                         DContractualStart,      DContractualEnd,     DProbationaryStart,   DProbationaryEnd,  
                         DRegularizationStart,   DRegularizationEnd,  DPermanentStart,      DResigned,          DTerminated,  DSeparated,    Remarks, IsOnDeviation, IdDeviation, IsOnDiciplinary, IsOnInvestigation, TranNumber, IdDeployment, DepDate, IdInvestigate) values 
                        (@EmpmasId,              @DivId,              @DepId,               @SecId,             @LeavegrpId,  @PayrollgrpId, @Positionid, @EmploymentTypeId,  
                         @EmpStatusId,           @DHired,             @DRegularization,     @DTraineeStart,     @DTraineeEnd,  
                         @DContractualStart,     @DContractualEnd,    @DProbationaryStart,  @DProbationaryEnd,  
                         @DRegularizationStart,  @DRegularizationEnd, @DPermanentStart,     @DResigned,         @DTerminated, @DSeparated,   @Remarks, @IsOnDeviation,@IdDeviation, @IsOnDiciplinary, @IsOnInvestigation, @TranNumber, @IdDeployment, @DepDate, @IdInvestigate); 
                        
                        select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId ";

        var res = await _sql.FetchData<DeprecModel?, dynamic>(sql, deprec, conn);
        return res.FirstOrDefault();
    }


    public async Task<DeprecModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  d.*, p.Name Positionname, s.Name Empstatusname, 
                            d2.Name DivName, d1.Name DepName, sec.Name Secname, l.Name LeavegrpName , d3.Name DeploymentName
                        from {schema}.Deprec d 
                        left join {schema}.Position p       on p.Id     = d.Positionid 
                        left join {schema}.REmpstat s       on s.Id     = d.Empstatusid 
                        left join {schema}.RDepartment d1   on d1.Id    = d.DepId  
                        left join {schema}.RDivision   d2   on d2.Id    = d.Divid
                        left join {schema}.RSection    sec  on sec.Id   = d.SecId  
                        left join {schema}.RDeployment d3   on d3.Id    = d.IdDeployment  
                        left join {schema}.Leavegrp l       on l.Id     = d.LeavegrpId  
                        where d.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DeprecModel?> _02(int id, string schema, string payschema, string conn)
    {
        string sql = $@"select  d.*, p.Name Positionname, s.Name Empstatusname, 
                            d2.Name DivName,sec.Name SecName, d1.Name DepName, sec.Name Secname, l.Name LeavegrpName, d3.Name DeploymentName,
                            pgrp.Name Payrollgrpname 
                        from {schema}.Deprec d 
                            left join {schema}.Position      p      on p.Id     = d.Positionid 
                            left join {schema}.REmpstat      s      on s.Id     = d.Empstatusid 
                            left join {schema}.RDepartment   d1     on d1.Id    = d.DepId  
                            left join {schema}.RDivision     d2     on d2.Id    = d.Divid
                            left join {schema}.RSection      sec    on sec.Id   = d.SecId  
                            left join {schema}.RDeployment   d3     on d3.Id    = d.IdDeployment  
                            left join {schema}.Leavegrp      l      on l.Id     = d.LeavegrpId  
                            left join {payschema}.Payrollgrp pgrp   on pgrp.Id  = d.PayrollgrpId   
                        where d.EmpmasId = @EmpmasId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<DeprecModel?> _02ByEmpnumber(string empnumber, string schema, string conn)
    {
        string sql = $@"select d.*, concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', e.EmpMidNm) Empname, e.Empnumber,   s.Name Empstatusname, 
                             d3.Name DeploymentName
                        from {schema}.Deprec d 
                        left join {schema}.Empmas e         on e.Id     = d.EmpmasId 
                        left join {schema}.REmpstat s       on s.Id     = d.Empstatusid 
                        left join {schema}.RDeployment d3   on d3.Id    = d.IdDeployment  
                        where e.Empnumber = @Empnumber";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<DeprecModel?>?> _02ByEmpmasIds(List<int> empmasId, string schema, string conn)
    {
        string sql = $@"select d.*, concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', e.EmpMidNm) Empname, e.Empnumber,   s.Name Empstatusname, 
                             d3.Name DeploymentName
                        from {schema}.Deprec d 
                        left join {schema}.Empmas e         on e.Id     = d.EmpmasId 
                        left join {schema}.REmpstat s       on s.Id     = d.Empstatusid 
                        left join {schema}.RDeployment d3   on d3.Id    = d.IdDeployment  
                        where d.EmpmasId IN @EmpmasId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = empmasId }, conn);
        return data;
    }

    public async Task<List<DeprecModel?>?> _02ByStatusIds(List<int> statusIds, string schema, string conn)
    {
        string sql = $@"select  d.*, concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', e.EmpMidNm) Empname, e.Empnumber,  s.Name Empstatusname, 
                             d3.Name DeploymentName
                        from {schema}.Deprec d 
                        left join {schema}.Empmas e         on e.Id     = d.EmpmasId 
                        left join {schema}.REmpstat s       on s.Id     = d.Empstatusid 
                        left join {schema}.RDeployment d3   on d3.Id    = d.IdDeployment  
                        where d.EmpStatusId IN @StatusId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { StatusId = statusIds }, conn);
        return data;
    }

    public async Task<List<DeprecModel?>?> _02_ByPayrollgrpIds(int payrollgrpId, string schema, string conn)
    {
        var sql = $@"select  concat(trim(e.EmpLastNm),', ', trim(e.EmpFirstNm),' ', e.EmpMidNm) Empname, 
                            d.*, p.Name Positionname, s.Name Empstatusname, 
                            d2.Name DivName, d1.Name DepName, sec.Name Secname, l.Name LeavegrpName   
                        from {schema}.Deprec d 
                        left join {schema}.Empmas e         on e.Id     = d.EmpmasId 
                        left join {schema}.Position p       on p.Id     = d.Positionid 
                        left join {schema}.REmpstat s       on s.Id     = d.Empstatusid 
                        left join {schema}.RDepartment d1   on d1.Id    = d.DepId  
                        left join {schema}.RDivision   d2   on d2.Id    = d.Divid
                        left join {schema}.RSection    sec  on sec.Id   = d.SecId  
                        left join {schema}.Leavegrp l       on l.Id     = d.LeavegrpId  
                        where d.PayrollgrpId = @PayrollgrpId";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { PayrollgrpId = payrollgrpId }, conn);
        return data;
    }
    public async Task<List<DeprecModel?>?> _02ByField(string fieldName, string schema, string conn)
    {
        fieldName = fieldName.ToLower();
        var allowedFields = new HashSet<string>
    {
        "isondeviation",
        "isondiciplinary",
        "isoninvestigation"
    };

        if (!allowedFields.Contains(fieldName))
            throw new ArgumentException($"Invalid field name: {fieldName}");

        string sql = $@"SELECT d.*, 
                    CONCAT(TRIM(e.EmpLastNm), ', ', TRIM(e.EmpFirstNm), ' ', e.EmpMidNm) AS Empname,
                    e.Empnumber,   
                    s.Name AS Empstatusname, 
                    d3.Name AS DeploymentName
                FROM {schema}.Deprec d 
                LEFT JOIN {schema}.Empmas e       ON e.Id = d.EmpmasId 
                LEFT JOIN {schema}.REmpstat s     ON s.Id = d.Empstatusid 
                LEFT JOIN {schema}.RDeployment d3 ON d3.Id = d.IdDeployment  
                WHERE d.{fieldName} = 1";

        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { }, conn);
        return data;
    }

    public async Task<DeprecModel?> _02DeviationDtlsByEmpmasId(int empmasId , string schema, string conn)
    {
        string sql = $@"SELECT d.IdDeviation, d.IsOnDeviation , dv.Control_No TranNumber
                FROM {schema}.Deprec d 
                LEFT JOIN {schema}.Deviation dv ON dv.Id = d.IdDeviation  
                WHERE d.EmpmasId = @EmpmasId";

        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId  = empmasId }, conn);
        return data.FirstOrDefault();
    }



    public async Task<DeprecModel?> _03(DeprecModel deprec, string schema, string conn)
    {
        int empmasId = deprec.Empmasid;

        string sql = $@"Update {schema}.Deprec set 
                                EmpmasId            = @EmpmasId, 
                                DivId               = @DivId,  
                                DepId               = @DepId,  
                                SecId               = @SecId,  
                                LeavegrpId          = @LeavegrpId,  
                                PayrollgrpId        = @PayrollgrpId, 
                                Positionid          = @Positionid,
                                EmploymentTypeId    = @EmploymentTypeId,  
                                EmpStatusId         = @EmpStatusId,  
                                DHired              = @DHired,  
                                DRegularization     = @DRegularization,  
                                DTraineeStart       = @DTraineeStart,  
                                DTraineeEnd         = @DTraineeEnd,  
                                DContractualStart   = @DContractualStart,  
                                DContractualEnd     = @DContractualEnd,  
                                DProbationaryStart  = @DProbationaryStart,  
                                DProbationaryEnd    = @DProbationaryEnd,  
                                DRegularizationStart = @DRegularizationStart,  
                                DRegularizationEnd  = @DRegularizationEnd,  
                                DPermanentStart     = @DPermanentStart,  
                                DResigned           = @DResigned,  
                                DTerminated         = @DTerminated,  
                                DSeparated          = @DSeparated,  
                                Remarks             = @Remarks,
                                IsOnDeviation       = @IsOnDeviation, 
                                IdDeviation         = @IdDeviation,
                                IsOnDiciplinary     = @IsOnDiciplinary,
                                IsOnInvestigation   = @IsOnInvestigation,
                                IdInvestigate       = @IdInvestigate,
                                TranNumber          = @TranNumber,
                                IdDeployment        = @IdDeployment, 
                                DepDate             = @DepDate
                                where EmpmasId = @EmpmasId;


                        select  d.*, p.Name Positionname, s.Name Empstatusname  
                        from {schema}.Deprec d 
                        left join {schema}.Position p on p.Id = d.Positionid 
                        left join {schema}.REmpstat s on s.Id = d.Empstatusid 
                        where d.EmpmasId = @EmpmasId;";

        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, deprec, conn);
        return data?.FirstOrDefault();
    }

    public async Task<DeprecModel?> _04(int empmasid, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Deprec where EmpmasId = @Empmasid;
                        Select  * from {schema}.Deprec x where x.EmpmasId = @EmpmasId;";
        var data = await _sql.FetchData<DeprecModel?, dynamic>(sql, new { EmpmasId = empmasid }, conn);
        return data?.FirstOrDefault();
    }
}
