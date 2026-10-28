using HRApiLibrary.Models._20_Pay.M0605;

namespace HRApiLibrary.Models._20_Pay.OPay;

//--- Payroll process on the MBPay (FoxPro) tables, following forms/payroll4.scx ------------------------------------------
//--- TRN = yyMMpp-ccccc (period + client number)

public class OPayEmp
{
    public string?  ClNumber        { get; set; }
    public string?  EmpNumber       { get; set; }
    public string?  Name            { get; set; }
    public double   AllowRate       { get; set; }
    public double   BasicRate       { get; set; }
    public int      RateId          { get; set; } = 2;                  //--- FoxPro: 1 monthly, 2 daily, 3 semi-monthly
    public string?  TaxCode         { get; set; }
    public int      IsTaxable       { get; set; }
    public string?  StatCode        { get; set; }
    public int      IsMaxSss        { get; set; }
    public int      IsWithSss       { get; set; } = 1;
    public int      IsWithPhic      { get; set; } = 1;
    public int      IsWithPagibig   { get; set; } = 1;
    public string?  SgCode          { get; set; }
    public double   SgDailyRate     { get; set; }
}

public class OPayClient
{
    public string?  ClNumber    { get; set; }
    public string?  ClName      { get; set; }
    public double   Rate        { get; set; }
    public double   MinRate     { get; set; }
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
}

public class OPayTran
{
    public string?  Trn         { get; set; }
    public string?  AcctNumber  { get; set; }
    public string?  EmpNumber   { get; set; }
    public double   Amount      { get; set; }
}

public class OPayEr
{
    public string?  EmpNumber   { get; set; }
    public double   SssEr       { get; set; }
    public double   PhicEr      { get; set; }
    public double   PagibigEr   { get; set; }
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

public class OPayDtl
{
    public string?  DtlCd   { get; set; }
    public double   NVal    { get; set; }
    public string?  Uom     { get; set; }
}

//--- One employee in the payroll: manhour inputs (paymaindtl columns) and computed amounts per account -------------------
public class OPayEmpResult
{
    public string?                      EmpNumber   { get; set; }
    public string?                      Name        { get; set; }
    public bool                         IsSelected  { get; set; }
    public Dictionary<string, double>   Hours       { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, double>   Amounts     { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, double>   ExtraEarnings { get; set; } = new(StringComparer.OrdinalIgnoreCase);   //--- posted before computing (fixed/other earnings)
    public List<OPayDtl>                Dtls        { get; set; } = [];
    public List<OPayLoan>               Loans       { get; set; } = [];     //--- loans deducted, amort = amount deducted
    public List<OPayOtherDed>           OtherDeds   { get; set; } = [];
    public double                       SssEr       { get; set; }
    public double                       PhicEr      { get; set; }
    public double                       PagibigEr   { get; set; }

    public double Earnings      => Amounts.Where(a => a.Key.StartsWith("E")).Sum(a => a.Value);
    public double Deductions    => Amounts.Where(a => a.Key.StartsWith("D")).Sum(a => a.Value);
    public double Net           => Math.Round(Earnings - Deductions, 2);
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
    public Dictionary<string, string>   Setup       { get; set; } = new(StringComparer.OrdinalIgnoreCase);   //--- paymaindtl column -> account
    public List<OPayCoa>                Coas        { get; set; } = [];
    public List<PremiumBracket>         SssTbl      { get; set; } = [];
    public List<PremiumBracket>         PhicTbl     { get; set; } = [];
    public List<PremiumBracket>         PagibigTbl  { get; set; } = [];
    public List<TaxBracket>             TaxTbl      { get; set; } = [];
    public List<OPayEmp>                Employees   { get; set; } = [];
    public List<OPayTran>               SavedMonth  { get; set; } = [];     //--- locked tbltran this month, other TRNs
    public List<OPayEr>                 SavedEr     { get; set; } = [];     //--- ER shares in paymaindtl this month, other TRNs
    public List<OPayLoan>               Loans       { get; set; } = [];
    public List<OPayOtherDed>           OtherDeds   { get; set; } = [];
    public List<OPayEmpResult>          Results     { get; set; } = [];

    public bool IsLocked => Status == "Lock";
}
