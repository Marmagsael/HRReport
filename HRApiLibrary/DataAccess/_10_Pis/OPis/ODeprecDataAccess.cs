using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;


namespace HRApiLibrary.DataAccess._10_Pis.OPis;

public class ODeprecDataAccess : IODeprecDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public ODeprecDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<ODeprecModel?> _01(ODeprecModel deprec, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Deprec (EmpNumber, TranNumber, DivId, DepId, SecId, PositionId, LeavegrpId, PayrollgrpId, IdDeployment, EmploymentTypeId, EmpStatusId, DepDate, DHired, DRegularization, DTraineeStart, DTraineeEnd, DContractualStart, DContractualEnd, DProbationaryStart, DProbationaryEnd, DRegularizationStart, DRegularizationEnd, DPermanentStart, DResigned, DTerminated, DSeparated, Remarks, IsOnDeviation, IdDeviation, IsOnDiciplinary, IsOnInvestigation, IdInvestigate) values (@EmpNumber, @TranNumber, @DivId, @DepId, @SecId, @PositionId, @LeavegrpId, @PayrollgrpId, @IdDeployment, @EmploymentTypeId, @EmpStatusId, @DepDate, @DHired, @DRegularization, @DTraineeStart, @DTraineeEnd, @DContractualStart, @DContractualEnd, @DProbationaryStart, @DProbationaryEnd, @DRegularizationStart, @DRegularizationEnd, @DPermanentStart, @DResigned, @DTerminated, @DSeparated, @Remarks, @IsOnDeviation, @IdDeviation, @IsOnDiciplinary, @IsOnInvestigation, @IdInvestigate)";
        await _sql.ExecuteCmd<dynamic>(sql, deprec, conn);

        sql = $@"SELECT * FROM {schema}.Deprec WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<ODeprecModel?> _01PayrollGrp(ODeprecModel deprec, string? schema, string? conn)
    {
        string? insertSql = $@"INSERT INTO {schema}.Deprec 
                           (Empnumber, PayrollgrpId) 
                           VALUES (@Empnumber, @PayrollgrpId) 
                           ON DUPLICATE KEY UPDATE 
                           PayrollgrpId = @PayrollgrpId;";

        await _sql.ExecuteCmd<dynamic>(insertSql, deprec, conn);

        string? selectSql = $@"SELECT * FROM {schema}.Deprec   WHERE Empnumber = @Empnumber;";

        var result = await _sql.FetchData<ODeprecModel?, dynamic>(selectSql, new { Empnumber = deprec.EmpNumber }, conn);
        return result?.FirstOrDefault();
    }

    public async Task<ODeprecModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  EmpNumber, TranNumber, DivId, DepId, SecId, PositionId, LeavegrpId, PayrollgrpId, IdDeployment, EmploymentTypeId, EmpStatusId, DepDate, DHired, DRegularization, DTraineeStart, DTraineeEnd, DContractualStart, DContractualEnd, DProbationaryStart, DProbationaryEnd, DRegularizationStart, DRegularizationEnd, DPermanentStart, DResigned, DTerminated, DSeparated, Remarks, IsOnDeviation, IdDeviation, IsOnDiciplinary, IsOnInvestigation, IdInvestigate from {schema}.Deprec where Id = @Id";
        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ODeprecModel?> _02ByEmpnumber(string? empnumber, string? schema, string? conn)
    {
        string? sql = $@"select d.* from {schema}.Deprec d  where d.Empnumber = @Empnumber";
        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { Empnumber = empnumber }, conn);
        return data.FirstOrDefault();
    }

    public async Task<List<ODeprecModel?>?> _02ByFieldId(string? fieldName, int? fieldIdValue, string? schema, string? conn)
    {

        var allowedFields = new HashSet<string>
            {
                 "DivId", "DepId", "SecId", "PositionId", "LeavegrpId",
                "PayrollgrpId", "IdDeployment", "EmploymentTypeId", "EmpStatusId",
                "IsOnDeviation", "IdDeviation", "IsOnDiciplinary", "IsOnInvestigation", "IdInvestigate"
            };

        if (!allowedFields.Contains(fieldName!))
            throw new ArgumentException($"Invalid field name: {fieldName}");

        string? sql = $@"SELECT d.*, 
                                CONCAT(
                                    TRIM(COALESCE(e.EmpLastNm, '')), ', ', 
                                    TRIM(COALESCE(e.EmpFirstNm, '')), ' ', 
                                    TRIM(COALESCE(e.EmpMidNm, ''))
                                ) AS Empname
                            FROM {schema}.Deprec d 
                            LEFT JOIN {schema}.Empmas e ON e.Empnumber = d.Empnumber 
                            WHERE d.{fieldName} = @FieldIdValue ORDER BY EmpLastNm, EmpFirstNm, EmpMidNm ";

        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { FieldIdValue = fieldIdValue }, conn);
        return data;
    }



    public async Task<ODeprecModel?> _03(int id, ODeprecModel deprec, string schema, string conn)
    {
        string sql = $@"Update {schema}.Deprec set EmpNumber = @EmpNumber, TranNumber = @TranNumber, DivId = @DivId, DepId = @DepId, SecId = @SecId, PositionId = @PositionId, LeavegrpId = @LeavegrpId, PayrollgrpId = @PayrollgrpId, IdDeployment = @IdDeployment, EmploymentTypeId = @EmploymentTypeId, EmpStatusId = @EmpStatusId, DepDate = @DepDate, DHired = @DHired, DRegularization = @DRegularization, DTraineeStart = @DTraineeStart, DTraineeEnd = @DTraineeEnd, DContractualStart = @DContractualStart, DContractualEnd = @DContractualEnd, DProbationaryStart = @DProbationaryStart, DProbationaryEnd = @DProbationaryEnd, DRegularizationStart = @DRegularizationStart, DRegularizationEnd = @DRegularizationEnd, DPermanentStart = @DPermanentStart, DResigned = @DResigned, DTerminated = @DTerminated, DSeparated = @DSeparated, Remarks = @Remarks, IsOnDeviation = @IsOnDeviation, IdDeviation = @IdDeviation, IsOnDiciplinary = @IsOnDiciplinary, IsOnInvestigation = @IsOnInvestigation, IdInvestigate = @IdInvestigate where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, deprec, conn);

        sql = $@" select  * from {schema}.Deprec x where x.Id = @Id ;";
        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<ODeprecModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Deprec where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Deprec x where x.Id = @Id ;";
        var data = await _sql.FetchData<ODeprecModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}



public interface IODeprecDataAccess
{
    Task<ODeprecModel?> _01(ODeprecModel deprec, string schema, string conn);
    Task<ODeprecModel?> _01PayrollGrp(ODeprecModel deprec, string? schema, string? conn);
    Task<ODeprecModel?> _02(int id, string schema, string conn);
    Task<ODeprecModel?> _02ByEmpnumber(string? empnumber, string? schema, string? conn);
    Task<List<ODeprecModel?>?> _02ByFieldId(string? fieldName, int? fieldIdValue, string? schema, string? conn);
    Task<ODeprecModel?> _03(int id, ODeprecModel deprec, string schema, string conn);
    Task<ODeprecModel?> _04(int id, string schema, string conn);
}
