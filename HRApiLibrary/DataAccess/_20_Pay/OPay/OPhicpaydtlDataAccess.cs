using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OPhicpaydtlDataAccess
{

	private readonly I_90_001_MySqlDataAccess _sql;

	public OPhicpaydtlDataAccess(I_90_001_MySqlDataAccess sql)
	{
			_sql = sql;
	}

	public async Task _01(GPhicpaydtlModel phicpaydtl, string schema, string conn )
	{
		string sql = $@"Insert into {schema}.Phicpaydtl 
							(idPHICPayDtl,  orno,  empnumber,  trn,  EES,  ERS) values 
							(@IdPhicPayDtl, @OrNo, @Empnumber, @Trn, @Ees, @Ers) ; "; 

		var res = await _sql.FetchData<GPhicpaydtlModel?,dynamic>(sql, phicpaydtl,conn);

	}

	
	public async Task<GPhicpaydtlModel?> _02ByEmpNumber(string empnumber, string schema, string conn)
	{
		string sql = $@"select  p.*
						From {schema}.Phicpaydtl p
						Where Empnumber = @Empnumber " ; 
		var data = await _sql.FetchData<GPhicpaydtlModel?, dynamic>(sql, new { Empnumber = empnumber }, conn); 
		return data?.FirstOrDefault();
	}

	// public async Task<GPhicpaydtlModel?> _03(int id,GPhicpaydtlModel phicpaydtl, string schema, string conn)
	// {
	// 	string sql = $@"Update {schema}.Phicpaydtl set idPHICPayDtl = @idPHICPayDtl, orno = @orno, empnumber = @empnumber, trn = @trn, EES = @EES, ERS = @ERS where Id = @Id;"; 
	// 	await _sql.ExecuteCmd<dynamic>(sql, phicpaydtl, conn);
		
	// 	sql = $@" select  * from {schema}.Phicpaydtl x where x.Id = @Id ;";
	// 	var data = await _sql.FetchData<GPhicpaydtlModel?, dynamic>(sql, new { Id = id }, conn);
	// 	return data?.FirstOrDefault();
	// }

	// public async Task<GPhicpaydtlModel?> _04(int id, string schema, string conn)
	// {
	// 	string sql = $@"Delete from {schema}.Phicpaydtl where Id = @Id;";
	// 	await _sql.ExecuteCmd<dynamic>(sql, new {Id=id},conn);

	// 	sql = $@" select  * from {schema}.Phicpaydtl x where x.Id = @Id ;";
	// 	var data = await _sql.FetchData<GPhicpaydtlModel?, dynamic>(sql, new { Id = id }, conn);
	// 	return data?.FirstOrDefault();
	// }
}