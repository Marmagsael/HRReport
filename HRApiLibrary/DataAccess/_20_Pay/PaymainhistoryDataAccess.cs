using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class PaymainhistoryDataAccess : IPaymainhistoryDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public PaymainhistoryDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<PaymainhistoryModel?> _01(PaymainhistoryModel paymainhistory, string schema, string conn)
    {
        string sql = $@"Insert into {schema}.Paymainhistory 
                            (Trn, UserId, Posted, Action) values 
                            (@Trn, @UserId, @Posted, @Action); 
                        SELECT * FROM {schema}.Paymainhistory WHERE ID = (SELECT @@IDENTITY); ";
        var res = await _sql.FetchData<PaymainhistoryModel?, dynamic>(sql, new { }, conn);

        return res.FirstOrDefault();
    }

    public async Task<PaymainhistoryModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  Id, Trn, UserId, Posted, Action from {schema}.Paymainhistory where Id = @Id";
        var data = await _sql.FetchData<PaymainhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<List<PaymainhistoryModel?>?> _02ByTrn(string trn, string schema, string conn)
    {
        string sql = $@"select  Id, Trn, UserId, Posted, Action from {schema}.Paymainhistory where Trn = @Trn";
        var data = await _sql.FetchData<PaymainhistoryModel?, dynamic>(sql, new { Trn = trn }, conn);
        return data;
    }

    public async Task<PaymainhistoryModel?> _04(int id, string schema, string conn)
    {
        string sql = $@"Delete from {schema}.Paymainhistory where Id = @Id;
                        select  * from {schema}.Paymainhistory x where x.Id = @Id;";
        var data = await _sql.FetchData<PaymainhistoryModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }
}

