using HRApiLibrary.Models._20_Pay.M0605;

namespace HRApiLibrary.Models._20_Pay.OPay;

//--- Payroll process on the MBPay (FoxPro) tables, following forms/payroll4.scx and forms/payrollnew4.scx ----------------
//--- TRN = yyMMpp-ccccc (period + payroll group code)

public class OPayEmp
{
    public string?  ClNumber        { get; set; }
    public string?  EmpNumber       { get; set; }
    public string?  Name            { get; set; }
    public double   AllowRate       { get; set; }
    public double   BasicRate       { get; set; }
    public int      RateId          { get; set; } = 1;                  //--- FoxPro: 1 monthly, 2 daily, 3 per payroll (NVL(rateid,1))
    public string?  TaxCode         { get; set; }
    public int      IsTaxable       { get; set; }
    public string?  StatCode        { get; set; }
    public int      IsMaxSss        { get; set; }
    public int      IsWithSss       { get; set; } = 1;
    public int      IsWithPhic      { get; set; } = 1;
    public int      IsWithPagibig   { get; set; } = 1;
    public string?  SgCode          { get; set; }
    public double   SgDailyRate     { get; set; }
    public string?  Desig           { get; set; }                       //--- empmas.desig_ : fixed_earnings_grp key
    public string?  StatName        { get; set; }                       //--- empstat.name (Add Employee list)

    public OPayEmp Clone() => (OPayEmp)MemberwiseClone();
}

//--- {OldPay}.payrollgrp left join {OldPis}.client on ClNumber (one client row per ClNumber) --------------------------
public class OPayClient
{
    public int      Id          { get; set; }   // -- {OldPay}.payrollgrp.Id : {OldPis}.deprec.PayrollgrpId points here --
    public string?  Code        { get; set; }   // -- payrollgrp.Code : unique per group, the key and the TRN suffix --
    public string?  ClNumber    { get; set; }
    public string?  ClName      { get; set; }
    public double   Rate        { get; set; }   // -- payrollgrp.RatePerDay (client.rate) --
    public double   MinRate     { get; set; }   // -- payrollgrp.MinDailyRate (client.minRate) --
    public string?  PayPrd      { get; set; }   // -- client.payprd --
    public double   ColaRate    { get; set; }   // -- client.colarate --
    public int      EcolaRevised{ get; set; }   // -- client.ecolaRevised --
    public double   CtpaRate    { get; set; }   // -- client.ctpaRate --
    public int      WithCtpa    { get; set; }   // -- client.withCTPA --
    public double   SeaRate     { get; set; }   // -- client.seaRate --
    public int      WithSea     { get; set; }   // -- client.withSEA --
    public double   MealAllow   { get; set; }   // -- client.MealAllow --
    public int      WithSil     { get; set; }   // -- client.withSIL --
    public double   BillRate    { get; set; }   // -- client.billRate --
}

// -- {OldPay}.period : PdNumber 01-05 (00 = every payday, not a payroll period) --
public class OPayPeriod
{
    public string?  PdNumber    { get; set; }
    public string?  PdName      { get; set; }
}

public class OPayCoa
{
    public string?  AcctNumber  { get; set; }
    public string?  AcctName    { get; set; }
    public string?  AcctType    { get; set; }
    public string?  IsTaxable   { get; set; }
    public int      WithSss     { get; set; }
    public int      WithPhic    { get; set; }
    public int      WithPagibig { get; set; }
    public string?  IsLock      { get; set; }
    public int      DedSort     { get; set; }
    public double   OtRate      { get; set; }
    public int      IsTh        { get; set; }
    public int      Show01      { get; set; }
    public int      Show02      { get; set; }
    public string?  ShortDesc   { get; set; }
}

public class OPayTran
{
    public string?  Trn         { get; set; }
    public string?  AcctNumber  { get; set; }
    public string?  EmpNumber   { get; set; }
    public double   Amount      { get; set; }
    public string?  Stat        { get; set; } = "saved";    //--- saved (tbltran) or tmp (tmptbltran)
}

//--- paymaindtl rows of the other TRNs in the same period (prevPayMainDtl : ER shares already posted) ---
public class OPayEr
{
    public string?  EmpNumber   { get; set; }
    public double   SssEr       { get; set; }
    public double   PhicEr      { get; set; }
    public double   PagibigEr   { get; set; }
    public double   Phic        { get; set; }       //--- phicpremer_get_saved sums the PHIC (EE) column
}

public class OPayLoan
{
    public int      Number      { get; set; }
    public string?  EmpNumber   { get; set; }
    public string?  DedNCode    { get; set; }
    public double   Amort       { get; set; }
    public double   Balance     { get; set; }
}

public class OPayOtherDed
{
    public int      Year        { get; set; }
    public int      Month       { get; set; }
    public string?  PeriodId    { get; set; }
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   Amount      { get; set; }
    public string?  Remarks     { get; set; }
}

public class OPayFixedEarning
{
    public int      Id          { get; set; }
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   Amount      { get; set; }
    public double   DaysPara    { get; set; }
}

public class OPayFixedEarningGrp
{
    public int      Id              { get; set; }
    public string?  AcctNumber      { get; set; }
    public double   Amount          { get; set; }
    public double   DaysPara        { get; set; }
    public int      PerDayEarnings  { get; set; }
    public string?  Desig           { get; set; }
}

public class OPayOtherEarning
{
    public int      ControlNo   { get; set; }
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   Amount      { get; set; }
}

public class OPayRetro
{
    public int      Id          { get; set; }
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   Amt         { get; set; }
}

public class OPayMealOpt
{
    public string?  EmpNumber   { get; set; }
    public double   MealAllow   { get; set; }
    public int      IsMgr       { get; set; }
}

public class OPayEmpAcct
{
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
}

public class OPayDedMandatory
{
    public int      Id          { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   ContAmt     { get; set; }
    public double   MaxAmt      { get; set; }
}

public class OPayHospDtl
{
    public string?  EmpNumber   { get; set; }
    public string?  AcctNumber  { get; set; }
    public double   Ded         { get; set; }
}

//--- payroll4.hospitalization : htotsavings (hospitalizationgrp / default hospitalizationacct / hospitalizationsavings) ---
public class OPayHospSaving
{
    public string?  EmpNumber   { get; set; }
    public string?  SavingsAcct { get; set; }
    public double   Fund        { get; set; }
    public double   Amort       { get; set; }
    public double   Saving      { get; set; }
}

public class OPayDtl
{
    public string?  Acct    { get; set; }       //--- first 4 characters : the account the row belongs to
    public string?  DtlCd   { get; set; }       //--- account + "  " until a compute step renames it to _01 (hours) or _02 (days)
    public double   NVal    { get; set; }
    public string?  Uom     { get; set; }
}

public class OPaySource
{
    public string?  AcctNumber  { get; set; }
    public double   Amount      { get; set; }
    public string?  Source      { get; set; }
}

//--- One employee in the payroll: manhour inputs (paymaindtl columns) and computed amounts per account -------------------
public class OPayEmpResult
{
    public string?                      EmpNumber   { get; set; }
    public string?                      Name        { get; set; }
    public bool                         IsSelected  { get; set; }
    public Dictionary<string, double>   Hours       { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, double>   Amounts     { get; set; } = new(StringComparer.OrdinalIgnoreCase);    //--- tmpdata
    public List<OPayDtl>                Dtls        { get; set; } = [];                                       //--- tmpdatadtl
    public List<OPayLoan>               Loans       { get; set; } = [];     //--- LoansSaved : loans deducted this run
    public List<OPayOtherDed>           OtherDeds   { get; set; } = [];     //--- tmptblotherdeductions
    public List<OPaySource>             Sources     { get; set; } = [];     //--- tmptbltransource
    public Dictionary<string, double>   MandTrans   { get; set; } = new(StringComparer.OrdinalIgnoreCase);    //--- tmpdedmandatory_trans
    public string?                      PayStatus   { get; set; }
    public string                       CoId        { get; set; } = "0";
    public string                       BrId        { get; set; } = "0";
    public string                       DepId       { get; set; } = "0";
    public string                       SecId       { get; set; } = "0";
    public double                       Sss         { get; set; }           //--- xcurpaylst.sss / ssser ...
    public double                       SssEr       { get; set; }
    public double                       Phic        { get; set; }
    public double                       PhicEr      { get; set; }
    public double                       Pagibig     { get; set; }
    public double                       PagibigEr   { get; set; }

    public double Earnings      => Amounts.Where(a => a.Key.StartsWith("E")).Sum(a => a.Value);
    public double Deductions    => Amounts.Where(a => a.Key.StartsWith("D")).Sum(a => a.Value);
    public double Net           => Earnings - Deductions;
    public NetPayStatus NetStatus { get; set; } = NetPayStatus.Ok;
}

public class OPayrollSession
{
    public string?      Trn         { get; set; }
    public string?      Yy          { get; set; }       //--- 4-digit year
    public string?      Mm          { get; set; }
    public string?      Prd         { get; set; }
    public DateTime     AttStart    { get; set; }
    public DateTime     AttEnd      { get; set; }
    public string?      Status      { get; set; } = "New";     //--- New, Updated (draft saved), Lock
    public string?      Paydb       { get; set; }
    public string?      Pisdb       { get; set; }
    public string?      Conn        { get; set; }

    public OPayClient                   Client      { get; set; } = new();
    public Dictionary<string, string>   Setup       { get; set; } = new(StringComparer.OrdinalIgnoreCase);   //--- paymaindtlsetup : column -> account
    public Dictionary<string, string>   AcctConfig  { get; set; } = new(StringComparer.OrdinalIgnoreCase);   //--- acctconfig : ecola, ctpa, sea ... -> account
    public List<OPayCoa>                Coas        { get; set; } = [];
    public List<PremiumBracket>         SssTbl      { get; set; } = [];
    public List<PremiumBracket>         PhicTbl     { get; set; } = [];
    public List<PremiumBracket>         PagibigTbl  { get; set; } = [];
    public List<TaxBracket>             TaxTbl      { get; set; } = [];
    public List<OPayEmp>                Employees   { get; set; } = [];
    public List<OPayTran>               SavedMonth  { get; set; } = [];     //--- tmpTranDtl : locked tbltran of the month (left(trn,4)), every TRN
    public List<OPayEr>                 SavedEr     { get; set; } = [];     //--- prevPayMainDtl : locked paymaindtl of the period (left(trn,6)), other TRNs
    public List<OPayLoan>               Loans       { get; set; } = [];     //--- iLoans
    public List<OPayOtherDed>           OtherDeds   { get; set; } = [];     //--- itblOtherDeductions (exempted employee/account already removed)
    public List<OPayFixedEarning>       FixedEarnings    { get; set; } = [];
    public List<OPayFixedEarningGrp>    FixedEarningsGrp { get; set; } = [];
    public List<OPayOtherEarning>       OtherEarnings    { get; set; } = [];
    public List<OPayRetro>              Retros      { get; set; } = [];
    public List<OPayMealOpt>            MealOpts    { get; set; } = [];
    public HashSet<string>              RetirementOpts { get; set; } = [];  //--- employees with a retirementopt row (withretirement = 1)
    public HashSet<string>              UniformOpts    { get; set; } = [];  //--- employees with a uniformopt row
    public List<OPayEmpAcct>            DedExemptions  { get; set; } = [];  //--- empdedexemption of the group's client
    public HashSet<string>              MandExemptAccts{ get; set; } = [];  //--- dedmandatoryexemption of the group's client (whole account skipped)
    public List<OPayDedMandatory>       DedMandatories { get; set; } = [];
    public Dictionary<string, double>   MandLocked  { get; set; } = [];     //--- dedmandatory_trans (islock = 1) : "emp|acct" -> total
    public List<OPayHospDtl>            HospDtls    { get; set; } = [];     //--- hospitalizationdtl of the period, not yet posted
    public List<OPayHospSaving>         HospSavings { get; set; } = [];     //--- htotsavings
    public HashSet<string>              HospSavedThisPeriod { get; set; } = []; //--- employees whose savings were already deducted in the period
    public HashSet<string>              Thirteenth  { get; set; } = [];     //--- 13thmogrp of this TRN
    public Dictionary<string, double>   RatesAmt    { get; set; } = new(StringComparer.OrdinalIgnoreCase);   //--- ratesamt row of the client (rot ...)
    public List<OPayEmpResult>          Results     { get; set; } = [];

    //--- Records the compute step posted, so Lock can mark them ---
    public List<OPayFixedEarning>       PostedFixed   { get; set; } = [];
    public List<OPayOtherEarning>       PostedOther   { get; set; } = [];
    public List<OPayRetro>              PostedRetro   { get; set; } = [];
    public List<OPayHospDtl>            PostedHosp    { get; set; } = [];
    public List<OPayHospSaving>         PostedHospSavings { get; set; } = [];

    public bool IsLocked => Status == "Lock";
}
