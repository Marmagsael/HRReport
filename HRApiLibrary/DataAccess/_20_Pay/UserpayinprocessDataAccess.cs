using HRApiLibrary.DataAccess._20_Pay.Interface;
using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay;

public class UserpayinprocessDataAccess : IUserpayinprocessDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public UserpayinprocessDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<UserpayinprocessModel?> _01(UserpayinprocessModel userpayinprocess, string schema, string conn)
    {

        string sql = $@"Insert into {schema}.Userpayinprocess (UserId, Trn, PayrollgrpId, AttStart, AttEnd, Yr, Month, Period) 
							values (@UserId, @Trn, @PayrollgrpId, @AttStart, @AttEnd, @Yr, @Month, @Period) 
                        ON DUPLICATE KEY 
                        UPDATE  Trn 			= @Trn, 
							    PayrollgrpId 	= @PayrollgrpId, 
							    AttStart 		= @AttStart, 
							    AttEnd 			= @AttEnd, 
							    Yr 				= @Yr, 
							    Month 			= @Month   ; 
						Select * from {schema}.Userpayinprocess where UserId = @UserId;";
        var res = await _sql.FetchData<UserpayinprocessModel?, dynamic>(sql, userpayinprocess, conn);
        return res.FirstOrDefault();

    }

    public async Task<UserpayinprocessModel?> _02(int id, string schema, string conn)
    {
        string sql = $@"select  UserId, Trn, PayrollgrpId, AttStart, AttEnd, Yr, Month, Period 
							from {schema}.Userpayinprocess where Id = @Id";
        var data = await _sql.FetchData<UserpayinprocessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<UserpayinprocessModel?> _03(int id, UserpayinprocessModel userpayinprocess, string schema, string conn)
    {
        string sql = $@"Update {schema}.Userpayinprocess set 
							UserId 			= @UserId, 
							Trn 			= @Trn, 
							PayrollgrpId 	= @PayrollgrpId, 
							AttStart 		= @AttStart, 
							AttEnd 			= @AttEnd, 
							Yr 				= @Yr, 
							Month 			= @Month, 
							Period 			= @Period where UserId = @UserId;";
        await _sql.ExecuteCmd<dynamic>(sql, userpayinprocess, conn);

        sql = $@" select  * from {schema}.Userpayinprocess x where x.Id = @Id ;";
        var data = await _sql.FetchData<UserpayinprocessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<UserpayinprocessModel?> _04(int id, string schema, string conn)
    {

        string sql = $@"Delete from {schema}.Userpayinprocess where UserId = @UserId;
						 select  * from {schema}.Userpayinprocess where UserId = @UserId";
        var data = await _sql.FetchData<UserpayinprocessModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();

    }
}