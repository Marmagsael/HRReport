using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_PayGeneric;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class OLoansDataAccess : IOLoansDataAccess
{

    private readonly I_90_001_MySqlDataAccess _sql;

    public OLoansDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<GLoansModel?> _01(GLoansModel loans, string? schema, string? conn)
    {
        string? sql = $@"Insert into {schema}.Loans 
							( NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, AMORT, Principal, MoToPay, InterestRate, InterestAmt, BALANCE, STATUS, ENCODEBY, ENCODEDT, CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, 
							  REMARKS, payMode, payStart, payRes, cvno, p1, p2, p3, p4, p5, TRNLastPosted) values 
							( @NUMBER, @EMPNUMBER, @DATE, @DEDNCODE, @DEDNDESC, @AMOUNT, @AMORT, @Principal, @MoToPay, @InterestRate, @InterestAmt, @BALANCE, @STATUS, @ENCODEBY, @ENCODEDT, @CHANGEBY, @CHANGEDT, @POSTED, @EMPLASTNM, @POSTFLAG, 
							  @REMARKS, @payMode, @payStart, @payRes, @cvno, @p1, @p2, @p3, @p4, @p5, @TRNLastPosted); 
							SELECT * FROM {schema}.Loans WHERE Number = @Number";
        var res = await _sql.FetchData<GLoansModel?, dynamic>(sql, loans, conn);
        return res.FirstOrDefault();
    }

    public async Task<GLoansModel?> _02(int? id, string? schema, string? conn)
    {
        string? sql = $@"select  NUMBER, EMPNUMBER, DATE, DEDNCODE, DEDNDESC, AMOUNT, AMORT, Principal, MoToPay, InterestRate, InterestAmt, BALANCE, STATUS, ENCODEBY, 
								ENCODEDT, CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, payStart, payRes, cvno, p1, p2, p3, p4, p5, TRNLastPosted 
						from {schema}.Loans where Number = @Number";
        var data = await _sql.FetchData<GLoansModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }


    public async Task<List<GLoansModel?>?> _02ByEmpNumbers(string? empnumber, string? schema, string? conn)
    {
        string? sql = $@"select  NUMBER, EMPNUMBER, 
                                IF(`DATE` IN ('0000-00-00','0000-00-00 00:00:00'), NULL, `DATE`) AS `date`,
                                IF(payStart IN ('0000-00-00','0000-00-00 00:00:00'), NULL, payStart) AS payStart,
                                IF(payRes   IN ('0000-00-00','0000-00-00 00:00:00'), NULL, payRes)   AS payRes,
                                DEDNCODE, DEDNDESC, AMOUNT, AMORT, Principal, MoToPay, 
                                InterestRate, InterestAmt, BALANCE, STATUS, 
                                ENCODEBY, ENCODEDT, CHANGEBY, CHANGEDT, POSTED, EMPLASTNM, POSTFLAG, REMARKS, payMode, 
                                cvno, p1, p2, p3, p4, p5, TRNLastPosted, 
                                c.AcctName  
						from {schema}.Loans l
						left join {schema}.ChartOfAcct c on c.AcctNumber = l.DedNCode   
						where Empnumber = @Empnumber and l.Balance > 0";
        var data = await _sql.FetchData<GLoansModel?, dynamic>(sql, new { EmpNumber = empnumber }, conn);

        return data;
    }

    public async Task<GLoansModel?> _03(int? id, GLoansModel loans, string? schema, string? conn)
    {
        string? sql = $@"Update {schema}.Loans set 
							NUMBER 		= @NUMBER, 
							EMPNUMBER 	= @EMPNUMBER, 
							DATE 		= @DATE, 
							DEDNCODE 	= @DEDNCODE, 
							DEDNDESC 	= @DEDNDESC, 
							AMOUNT 		= @AMOUNT, 
							AMORT 		= @AMORT, 
							Principal 	= @Principal, 
							MoToPay 	= @MoToPay, 
							InterestRate 	= @InterestRate, 
							InterestAmt 	= @InterestAmt, 
							BALANCE 	= @BALANCE, 
							STATUS 		= @STATUS, 
							ENCODEBY 	= @ENCODEBY, 
							ENCODEDT 	= @ENCODEDT, 
							CHANGEBY 	= @CHANGEBY, 
							CHANGEDT 	= @CHANGEDT, 
							POSTED 		= @POSTED, 
							EMPLASTNM 	= @EMPLASTNM, 
							POSTFLAG 	= @POSTFLAG, 
							REMARKS 	= @REMARKS, 
							payMode 	= @payMode, 
							payStart 	= @payStart, 
							payRes 		= @payRes, 
							cvno 		= @cvno, 
							p1 			= @p1, p2 = @p2, p3 = @p3, p4 = @p4, p5 = @p5, 
							TRNLastPosted = @TRNLastPosted 
						where Number = @Number;
						select  * from {schema}.Loans x where Number = @Number ;";
        var data = await _sql.FetchData<GLoansModel?, dynamic>(sql, new { Id = id }, conn);
        return data?.FirstOrDefault();
    }

    public async Task<GLoansModel?> _04(string? number, string? schema, string? conn)
    {
        string? sql = $@"Delete from {schema}.Loans where Number = @Number;";
        await _sql.ExecuteCmd<dynamic>(sql, new { Number = number }, conn);

        sql = $@" select  * from {schema}.Loans  where Number = @Number ;";
        var data = await _sql.FetchData<GLoansModel?, dynamic>(sql, new { Number = number }, conn);
        return data?.FirstOrDefault();
    }
}

public interface IOLoansDataAccess
{
    Task<GLoansModel?> _01(GLoansModel loans, string? schema, string? conn);
    Task<GLoansModel?> _02(int? id, string? schema, string? conn);
    Task<List<GLoansModel?>?> _02ByEmpNumbers(string? empnumber, string? schema, string? conn);
    Task<GLoansModel?> _03(int? id, GLoansModel loans, string? schema, string? conn);
    Task<GLoansModel?> _04(string? number, string? schema, string? conn);
}
