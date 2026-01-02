using HRApiLibrary.DataAccess._10_Pis.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._10_Pis;

namespace HRApiLibrary.DataAccess._10_Pis;

public class InvDataAccess : IInvDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public InvDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<InvModel?> _01(InvModel inv, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Inv (Name, TypeId, MakeId, ModelId, CategoryId, BrandId, Description, SerialNo, DatePurchased, DateWarrantyExpiration, UnitCost, Status, DeploymentId, EmpmasId, EmpNumber, DateAssignment, AssignmentNo, DateEncoded, EncodedbyId) values (@Name, @TypeId, @MakeId, @ModelId, @CategoryId, @BrandId, @Description, @SerialNo, @DatePurchased, @DateWarrantyExpiration, @UnitCost, @Status, @DeploymentId, @EmpmasId, @EmpNumber, @DateAssignment, @AssignmentNo, @DateEncoded, @EncodedbyId)";
        await _sql.ExecuteCmd<dynamic>(sql, inv, conn);

        sql = $@"SELECT * FROM {schema}.Inv WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<InvModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }


    public async Task<InvModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Name, TypeId, MakeId, ModelId, CategoryId, BrandId, Description, SerialNo, DatePurchased, DateWarrantyExpiration, UnitCost, Status, DeploymentId, EmpmasId, EmpNumber, DateAssignment, AssignmentNo, DateEncoded, EncodedbyId from {schema}.Inv where Id = @Id";
        var data = await _sql.FetchData<InvModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<InvModel?> _03(int id, InvModel inv, string schema, string conn)
    {
        string sql = $@"Update {schema}.Inv set Name = @Name, TypeId = @TypeId, MakeId = @MakeId, ModelId = @ModelId, CategoryId = @CategoryId, BrandId = @BrandId, Description = @Description, SerialNo = @SerialNo, DatePurchased = @DatePurchased, DateWarrantyExpiration = @DateWarrantyExpiration, UnitCost = @UnitCost, Status = @Status, DeploymentId = @DeploymentId, EmpmasId = @EmpmasId, EmpNumber = @EmpNumber, DateAssignment = @DateAssignment, AssignmentNo = @AssignmentNo, DateEncoded = @DateEncoded, EncodedbyId = @EncodedbyId where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, inv, conn);

        sql = $@" select  * from {schema}.Inv x where x.Id = @Id ;";
        var data = await _sql.FetchData<InvModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<InvModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Inv where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Inv x where x.Id = @Id ;";
        var data = await _sql.FetchData<InvModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}