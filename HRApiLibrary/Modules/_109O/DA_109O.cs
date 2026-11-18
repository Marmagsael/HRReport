using Dapper;
using HRApiLibrary.DataAccess._90_Utils.Interface;

namespace HRApiLibrary.Modules._109O;

public class DA_109O : IDA_109O
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public DA_109O(I_90_001_MySqlDataAccess sql) { _sql = sql; }

    public async Task<List<M109_Sss>> _02Sss(string opay, string conn)
        => await _sql.FetchLegacy<M109_Sss, dynamic>($"select FSTART Start, FEND End, ifnull(FSSSEE,0) Ee, ifnull(FSSSER,0) Er, ifnull(FECC,0) Ecc, compensation Compensation from {opay}.ssstbl order by FSTART", new { }, conn) ?? [];

    public async Task<List<M109_Phic>> _02Phic(string opay, string conn)
        => await _sql.FetchLegacy<M109_Phic, dynamic>($"select FSTART Start, ifnull(FEND,0) End, ifnull(FMEDEE,0) Ee, ifnull(FMEDER,0) Er, ifnull(`percent`,0) Percent from {opay}.medtbl order by FSTART", new { }, conn) ?? [];

    public async Task<List<M109_Pagibig>> _02Pagibig(string opay, string conn)
        => await _sql.FetchLegacy<M109_Pagibig, dynamic>($"select FSTART Start, FEND End, ifnull(EmpCont,0) Ee, ifnull(Emplyr,0) Er from {opay}.pagibigtbl order by FSTART", new { }, conn) ?? [];

    public async Task<List<M109_Tax>> _02Tax(string opay, string conn)
        => await _sql.FetchLegacy<M109_Tax, dynamic>($"select trim(TaxCode) TaxCode, SAmt, EAmt, Fix, Percentage from {opay}.semimonthlytax order by SAmt", new { }, conn) ?? [];

    // -- The table is replaced as a whole: all rows or none --
    private async Task Replace<T>(string table, string insert, IEnumerable<T> rows, string conn)
    {
        using var cn = _sql.GetConnString(conn);
        cn.Open();
        using var tx = cn.BeginTransaction();
        try
        {
            await cn.ExecuteAsync($"delete from {table}", null, tx);
            var list = rows.ToList();
            if (list.Count > 0) await cn.ExecuteAsync(insert, list, tx);
            tx.Commit();
        }
        catch { tx.Rollback(); throw; }
    }

    public Task _03Sss(List<M109_Sss> rows, string opay, string conn)
        => Replace($"{opay}.ssstbl", $"insert into {opay}.ssstbl (FSTART, FEND, FSSSEE, FSSSER, FECC, compensation) values (@Start, @End, @Ee, @Er, @Ecc, @Compensation)", rows, conn);

    public Task _03Phic(List<M109_Phic> rows, string opay, string conn)
        => Replace($"{opay}.medtbl", $"insert into {opay}.medtbl (FSTART, FEND, FMEDEE, FMEDER, `percent`) values (@Start, @End, @Ee, @Er, @Percent)", rows, conn);

    public Task _03Pagibig(List<M109_Pagibig> rows, string opay, string conn)
        => Replace($"{opay}.pagibigtbl", $"insert into {opay}.pagibigtbl (FSTART, FEND, EmpCont, Emplyr) values (@Start, @End, @Ee, @Er)", rows, conn);

    public Task _03Tax(List<M109_Tax> rows, string opay, string conn)
        => Replace($"{opay}.semimonthlytax", $"insert into {opay}.semimonthlytax (TaxCode, SAmt, EAmt, Fix, Percentage) values (@TaxCode, @SAmt, @EAmt, @Fix, @Percentage)", rows, conn);

    // -- The latest revision kept in mainpay (the system defaults) --
    private static string Latest(string t) => $"join (select DateStart ds, Revision rv from mainpay.{t} order by DateStart desc, Revision desc limit 1) l on l.ds = m.DateStart and l.rv = m.Revision";

    public async Task<List<M109_Sss>> _02DefaultSss(string conn)
        => await _sql.FetchLegacy<M109_Sss, dynamic>($"select m.FStart Start, m.FEnd End, m.Ee, m.Er, m.Ecc, m.Compensation from mainpay.matrixsss m {Latest("matrixsss")} order by m.FStart", new { }, conn) ?? [];

    public async Task<List<M109_Phic>> _02DefaultPhic(string conn)
        => await _sql.FetchLegacy<M109_Phic, dynamic>($"select m.FStart Start, m.FEnd End, m.Ee, m.Er, m.`Percent` Percent from mainpay.matrixphic m {Latest("matrixphic")} order by m.FStart", new { }, conn) ?? [];

    public async Task<List<M109_Pagibig>> _02DefaultPagibig(string conn)
        => await _sql.FetchLegacy<M109_Pagibig, dynamic>($"select m.FStart Start, m.FEnd End, m.Ee, m.Er from mainpay.matrixpagibig m {Latest("matrixpagibig")} order by m.FStart", new { }, conn) ?? [];

    // -- matrixwtax keeps the start of the bracket in Fix; semimonthlytax keeps the fixed tax, so it is computed from the brackets above --
    public async Task<List<M109_Tax>> _02DefaultTax(string conn)
    {
        var src = await _sql.FetchLegacy<M109_Tax, dynamic>(
            $@"select 'S' TaxCode, m.SAmt, m.EAmt, 0 Fix, m.Percentage from mainpay.matrixwtax m
               join (select From_ f, Revision r from mainpay.matrixwtax where PeriodCode = 'SM' order by From_ desc, Revision desc limit 1) l on l.f = m.From_ and l.r = m.Revision
               where m.PeriodCode = 'SM' order by m.SAmt", new { }, conn) ?? [];
        for (int i = 1; i < src.Count; i++)
            src[i].Fix = Math.Round(src[i - 1].Fix + src[i - 1].Percentage * (src[i].SAmt - src[i - 1].SAmt), 2);
        return src;
    }
}

public interface IDA_109O
{
    Task<List<M109_Sss>> _02Sss(string opay, string conn);
    Task<List<M109_Phic>> _02Phic(string opay, string conn);
    Task<List<M109_Pagibig>> _02Pagibig(string opay, string conn);
    Task<List<M109_Tax>> _02Tax(string opay, string conn);
    Task _03Sss(List<M109_Sss> rows, string opay, string conn);
    Task _03Phic(List<M109_Phic> rows, string opay, string conn);
    Task _03Pagibig(List<M109_Pagibig> rows, string opay, string conn);
    Task _03Tax(List<M109_Tax> rows, string opay, string conn);
    Task<List<M109_Sss>> _02DefaultSss(string conn);
    Task<List<M109_Phic>> _02DefaultPhic(string conn);
    Task<List<M109_Pagibig>> _02DefaultPagibig(string conn);
    Task<List<M109_Tax>> _02DefaultTax(string conn);
}

public class M109_Sss
{
    public double Start { get; set; }
    public double End { get; set; }
    public double Ee { get; set; }
    public double Er { get; set; }
    public double Ecc { get; set; }
    public double Compensation { get; set; }
}

public class M109_Phic
{
    public double Start { get; set; }
    public double End { get; set; }
    public double Ee { get; set; }
    public double Er { get; set; }
    public double Percent { get; set; }
}

public class M109_Pagibig
{
    public double Start { get; set; }
    public double End { get; set; }
    public double Ee { get; set; }
    public double Er { get; set; }
}

public class M109_Tax
{
    public string TaxCode { get; set; } = "S";
    public double SAmt { get; set; }
    public double EAmt { get; set; }
    public double Fix { get; set; }
    public double Percentage { get; set; }
}
