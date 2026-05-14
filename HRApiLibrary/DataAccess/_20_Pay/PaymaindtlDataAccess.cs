using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PaymaindtlDataAccess : IPaymaindtlDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PaymaindtlDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PaymaindtlModel?> _01(PaymaindtlModel paymaindtl, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Paymaindtl (Trn, Empnumber, EmpmasId, Sss, SssEr, SssEc, Pagibig, PagibigEr, Phic, PhicEr, PayStatus, PayrollGrpId, CompanyId, BranchId, DayWrk, Absent, Late, UTime, Rn, RnOt, Rot, Rd, RdOt, Lh, LhOt, RdLh, RdLhOt, Sh, ShOt, RdSh, RdShOt, Custom1, Custom2, Custom3, NdRd, NdRdOt, NdRdLh, NdRdLhOt, NdRdSh, NdRdShOt, Nd, NdOt, NdLh, NdLhOt, NdSh, NdShOt, Dh, DhOt, RdDh, RdDhOt) values (@Trn, @Empnumber, @EmpmasId, @Sss, @SssEr, @SssEc, @Pagibig, @PagibigEr, @Phic, @PhicEr, @PayStatus, @PayrollGrpId, @CompanyId, @BranchId, @DayWrk, @Absent, @Late, @UTime, @Rn, @RnOt, @Rot, @Rd, @RdOt, @Lh, @LhOt, @RdLh, @RdLhOt, @Sh, @ShOt, @RdSh, @RdShOt, @Custom1, @Custom2, @Custom3, @NdRd, @NdRdOt, @NdRdLh, @NdRdLhOt, @NdRdSh, @NdRdShOt, @Nd, @NdOt, @NdLh, @NdLhOt, @NdSh, @NdShOt, @Dh, @DhOt, @RdDh, @RdDhOt)";
        await _sql.ExecuteCmd<dynamic>(sql, paymaindtl, conn);

        sql = $@"SELECT * FROM {schema}.Paymaindtl WHERE ID = (SELECT @@IDENTITY)";

        var res = await _sql.FetchData<PaymaindtlModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }
    public async Task<List<PaymaindtlModel?>?> _01New(string trn, int payrollgrpId, string paydb, string pisdb, string conn)
    {
        string sql = $@"Insert into {paydb}.Paymaindtl 
                                  (Trn,  Empnumber,   EmpmasId) 
                            select @Trn, e.EmpNumber, d.EmpmasId from {pisdb}.deprec d 
                                left join {pisdb}.Empmas e on e.Id = d.EmpmasId 
                            where d.PayrollgrpId = @PayrollgrpId";
        await _sql.ExecuteCmd<dynamic>(sql, new {Trn=trn, PayrollgrpId = payrollgrpId}, conn);

        sql = $@"SELECT p.* FROM {paydb}.Paymaindtl p 
                 left join {pisdb}.Empmas e on e.Id = p.EmpmasId  WHERE p.Trn = @Trn";
        var res = await _sql.FetchData<PaymaindtlModel?, dynamic>(sql, new {Trn=trn }, conn);
        return res;
        
    }


    public async Task<PaymaindtlModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Trn, Empnumber, EmpmasId, Sss, SssEr, SssEc, Pagibig, PagibigEr, Phic, PhicEr, PayStatus, PayrollGrpId, CompanyId, BranchId, DayWrk, Absent, Late, UTime, Rn, RnOt, Rot, Rd, RdOt, Lh, LhOt, RdLh, RdLhOt, Sh, ShOt, RdSh, RdShOt, Custom1, Custom2, Custom3, NdRd, NdRdOt, NdRdLh, NdRdLhOt, NdRdSh, NdRdShOt, Nd, NdOt, NdLh, NdLhOt, NdSh, NdShOt, Dh, DhOt, RdDh, RdDhOt from {schema}.Paymaindtl where Id = @Id";
        var data = await _sql.FetchData<PaymaindtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<PaymaindtlModel?> _03(int id, PaymaindtlModel paymaindtl, string schema, string conn)
    {
        string sql = $@"Update {schema}.Paymaindtl set Trn = @Trn, Empnumber = @Empnumber, EmpmasId = @EmpmasId, Sss = @Sss, SssEr = @SssEr, SssEc = @SssEc, Pagibig = @Pagibig, PagibigEr = @PagibigEr, Phic = @Phic, PhicEr = @PhicEr, PayStatus = @PayStatus, PayrollGrpId = @PayrollGrpId, CompanyId = @CompanyId, BranchId = @BranchId, DayWrk = @DayWrk, Absent = @Absent, Late = @Late, UTime = @UTime, Rn = @Rn, RnOt = @RnOt, Rot = @Rot, Rd = @Rd, RdOt = @RdOt, Lh = @Lh, LhOt = @LhOt, RdLh = @RdLh, RdLhOt = @RdLhOt, Sh = @Sh, ShOt = @ShOt, RdSh = @RdSh, RdShOt = @RdShOt, Custom1 = @Custom1, Custom2 = @Custom2, Custom3 = @Custom3, NdRd = @NdRd, NdRdOt = @NdRdOt, NdRdLh = @NdRdLh, NdRdLhOt = @NdRdLhOt, NdRdSh = @NdRdSh, NdRdShOt = @NdRdShOt, Nd = @Nd, NdOt = @NdOt, NdLh = @NdLh, NdLhOt = @NdLhOt, NdSh = @NdSh, NdShOt = @NdShOt, Dh = @Dh, DhOt = @DhOt, RdDh = @RdDh, RdDhOt = @RdDhOt where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, paymaindtl, conn);

        sql = $@" select  * from {schema}.Paymaindtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<PaymaindtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<PaymaindtlModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paymaindtl where Id = @Id;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Id = id }, conn);

        sql = $@" select  * from {schema}.Paymaindtl x where x.Id = @Id ;";
        var data = await _sql.FetchData<PaymaindtlModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}