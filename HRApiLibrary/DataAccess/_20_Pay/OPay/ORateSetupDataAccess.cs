using HRApiLibrary.DataAccess._90_Utils.Interface;
using HRApiLibrary.Models._20_Pay;

namespace HRApiLibrary.DataAccess._20_Pay.OPay;

public class ORateSetupDataAccess : IORateSetupDataAccess
{
    private readonly I_90_001_MySqlDataAccess _sql;

    public ORateSetupDataAccess(I_90_001_MySqlDataAccess sql)
    {
        _sql = sql;
    }

    public async Task<List<RateSetupSalgradeModel>> _02Salgrades(string paySchema, string conn)
    {
        var data = await _sql.FetchLegacy<RateSetupSalgradeModel, dynamic>(
            $"select trim(sgcode) SgCode, trim(sgrade) SGrade, dailyRate DailyRate from {paySchema}.salgrade order by sgrade", new { }, conn) ?? new();
        data.Insert(0, new RateSetupSalgradeModel { SgCode = string.Empty, SGrade = string.Empty });
        return data;
    }

    public async Task<List<RateSetupGroupModel>> _02Groups(string paySchema, string conn)
    {
        var sql = $@"select p.Id, trim(p.Code) Code, trim(p.ClNumber) ClNumber, trim(p.Name) Name, p.Status,
                            ifnull(p.RatePerDay,0) Rate, ifnull(p.MinDailyRate,0) MinRate,
                            ifnull(p.EcolaRate,0) Ecola, ifnull(p.CtpaRate,0) Ctpa, ifnull(p.SeaRate,0) Sea,
                            ifnull(p.EcolaRevised,0) EcolaRevised, ifnull(p.WithCtpa,0) WithCtpa, ifnull(p.WithSea,0) WithSea,
                            trim(ifnull(p.SgCode,'')) SgCode, ifnull(g.dailyRate,0) SgDailyRate
                     from {paySchema}.payrollgrp p
                     left join {paySchema}.salgrade g on trim(g.sgcode) = trim(p.SgCode) and trim(p.SgCode) <> ''
                     order by p.Name";
        return await _sql.FetchLegacy<RateSetupGroupModel, dynamic>(sql, new { }, conn) ?? new();
    }

    public async Task<List<RateSetupEmployeeModel>> _02Employees(int payrollgrpId, string paySchema, string pisSchema, string conn)
    {
        var sql = $@"select e.EMPNUMBER EmpNumber,
                            concat(trim(ifnull(e.EMPLASTNM,'')), ', ', trim(ifnull(e.EMPFIRSTNM,'')), ' ', left(trim(ifnull(e.EMPMIDNM,'')),1)) Name,
                            ifnull(po.NAME,'') Position, ifnull(st.NAME,'') Status,
                            ifnull(e.empBasicRate,0) BasicRate, ifnull(e.rateID,2) RateId, ifnull(e.ALLOWRATE,0) AllowRate,
                            ifnull(e.ismaxsss,0) IsMaxSss, trim(ifnull(e.sgcode,'')) SgCode, ifnull(g.dailyRate,0) SgDailyRate,
                            ifnull(e.isTaxable,0) IsTaxable, ifnull(e.iswithSSS,0) WithSss, ifnull(e.iswithGSIS,0) WithGsis,
                            ifnull(e.iswithPagibig,0) WithPagibig, ifnull(e.iswithPHIC,0) WithPhic
                     from {pisSchema}.empmas e
                     join {pisSchema}.deprec d on d.EmpNumber = e.EMPNUMBER and d.PayrollgrpId = @Id
                     left join {pisSchema}.position po on po.CODE = e.POSITION_
                     left join {pisSchema}.empstat st on st.CODE = e.EMPSTAT_
                     left join {paySchema}.salgrade g on trim(g.sgcode) = trim(e.sgcode) and trim(e.sgcode) <> ''
                     order by e.EMPLASTNM, e.EMPFIRSTNM";
        return await _sql.FetchLegacy<RateSetupEmployeeModel, dynamic>(sql, new { Id = payrollgrpId }, conn) ?? new();
    }

    public async Task<string?> _03Groups(List<RateSetupGroupModel> changed, string paySchema, string conn)
    {
        foreach (var g in changed)
        {
            if (g.Rate < 0 || g.MinRate < 0 || g.Ecola < 0 || g.Ctpa < 0 || g.Sea < 0)
                return $"A rate of {g.Name} is not valid.";
        }

        foreach (var g in changed)
        {
            await _sql.ExecuteLegacyCmd($@"update {paySchema}.payrollgrp set RatePerDay = @Rate, MinDailyRate = @MinRate, EcolaRate = @Ecola, CtpaRate = @Ctpa,
                                                  SeaRate = @Sea, EcolaRevised = @EcolaRevised, WithCtpa = @WithCtpa, WithSea = @WithSea, SgCode = @SgCode
                                           where Id = @Id",
                                        new { g.Id, g.Rate, g.MinRate, g.Ecola, g.Ctpa, g.Sea, g.EcolaRevised, g.WithCtpa, g.WithSea, SgCode = (g.SgCode ?? string.Empty).Trim() }, conn);
        }
        return null;
    }

    public async Task<string?> _03Employees(List<RateSetupEmployeeModel> changed, string pisSchema, string conn)
    {
        foreach (var e in changed)
        {
            if (e.BasicRate < 0 || e.AllowRate < 0) return $"A rate of {e.Name} is not valid.";
            if (e.RateId is < 1 or > 3)             return $"The rate type of {e.Name} is not valid.";
        }

        foreach (var e in changed)
        {
            await _sql.ExecuteLegacyCmd($@"update {pisSchema}.empmas set empBasicRate = @BasicRate, rateID = @RateId, ALLOWRATE = @AllowRate, ismaxsss = @IsMaxSss,
                                                  sgcode = @SgCode, isTaxable = @IsTaxable, iswithSSS = @WithSss, iswithGSIS = @WithGsis,
                                                  iswithPagibig = @WithPagibig, iswithPHIC = @WithPhic
                                           where EMPNUMBER = @EmpNumber",
                                        new { e.EmpNumber, e.BasicRate, e.RateId, e.AllowRate, e.IsMaxSss, SgCode = (e.SgCode ?? string.Empty).Trim(), e.IsTaxable, e.WithSss, e.WithGsis, e.WithPagibig, e.WithPhic }, conn);
        }
        return null;
    }

    public async Task<RatesAmtModel> _02RatesAmt(string clNumber, string paySchema, string conn)
    {
        var data = await _sql.FetchLegacy<RatesAmtModel, dynamic>(
            $@"select clnumber ClNumber, ifnull(rot,0) Rot, ifnull(lh,0) Lh, ifnull(lhot,0) Lhot, ifnull(sh,0) Sh, ifnull(shot,0) Shot, ifnull(dh,0) Dh, ifnull(dhot,0) Dhot,
                      ifnull(rd,0) Rd, ifnull(rdot,0) Rdot, ifnull(rdlh,0) Rdlh, ifnull(rdlhot,0) Rdlhot, ifnull(rdsh,0) Rdsh, ifnull(rdshot,0) Rdshot, ifnull(rddh,0) Rddh, ifnull(rddhot,0) Rddhot
               from {paySchema}.ratesamt where clnumber = @ClNumber", new { ClNumber = clNumber }, conn);
        return data.FirstOrDefault() ?? new RatesAmtModel { ClNumber = clNumber };
    }

    public async Task<string?> _03RatesAmt(RatesAmtModel m, string paySchema, string conn)
    {
        if (string.IsNullOrWhiteSpace(m.ClNumber)) return "This payroll group has no client number.";
        var all = new[] { m.Rot, m.Lh, m.Lhot, m.Sh, m.Shot, m.Dh, m.Dhot, m.Rd, m.Rdot, m.Rdlh, m.Rdlhot, m.Rdsh, m.Rdshot, m.Rddh, m.Rddhot };
        if (all.Any(v => v < 0)) return "An amount is not valid.";

        await _sql.ExecuteLegacyCmd($@"insert into {paySchema}.ratesamt (clnumber, rot, lh, lhot, sh, shot, dh, dhot, rd, rdot, rdlh, rdlhot, rdsh, rdshot, rddh, rddhot)
                                       values (@ClNumber, @Rot, @Lh, @Lhot, @Sh, @Shot, @Dh, @Dhot, @Rd, @Rdot, @Rdlh, @Rdlhot, @Rdsh, @Rdshot, @Rddh, @Rddhot)
                                       on duplicate key update rot = @Rot, lh = @Lh, lhot = @Lhot, sh = @Sh, shot = @Shot, dh = @Dh, dhot = @Dhot, rd = @Rd, rdot = @Rdot,
                                              rdlh = @Rdlh, rdlhot = @Rdlhot, rdsh = @Rdsh, rdshot = @Rdshot, rddh = @Rddh, rddhot = @Rddhot",
                                    new { ClNumber = m.ClNumber!.Trim(), m.Rot, m.Lh, m.Lhot, m.Sh, m.Shot, m.Dh, m.Dhot, m.Rd, m.Rdot, m.Rdlh, m.Rdlhot, m.Rdsh, m.Rdshot, m.Rddh, m.Rddhot }, conn);
        return null;
    }

    public async Task _04RatesAmt(string clNumber, string paySchema, string conn)
    {
        await _sql.ExecuteLegacyCmd($"delete from {paySchema}.ratesamt where clnumber = @ClNumber", new { ClNumber = clNumber.Trim() }, conn);
    }
}

public interface IORateSetupDataAccess
{
    Task<List<RateSetupSalgradeModel>>  _02Salgrades(string paySchema, string conn);
    Task<List<RateSetupGroupModel>>     _02Groups(string paySchema, string conn);
    Task<List<RateSetupEmployeeModel>>  _02Employees(int payrollgrpId, string paySchema, string pisSchema, string conn);
    Task<string?>                       _03Groups(List<RateSetupGroupModel> changed, string paySchema, string conn);
    Task<string?>                       _03Employees(List<RateSetupEmployeeModel> changed, string pisSchema, string conn);
    Task<RatesAmtModel>                 _02RatesAmt(string clNumber, string paySchema, string conn);
    Task<string?>                       _03RatesAmt(RatesAmtModel m, string paySchema, string conn);
    Task                                _04RatesAmt(string clNumber, string paySchema, string conn);
}
