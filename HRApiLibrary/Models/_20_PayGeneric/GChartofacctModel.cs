namespace HRApiLibrary.Models._20_PayGeneric; 

public class GChartofacctModel
{
    public string?  AcctNumber              { get; set; }
    public string?  AcctName                { get; set; }
    public string?  AcctType                { get; set; }
    public string?  IsTaxable               { get; set; }
    public string?  IsYTDAcct               { get; set; }
    public string?  IsTaxExcl               { get; set; }
    public string?  IsLock                  { get; set; }
    public string?  IsChargeable            { get; set; }
    public string?  HasRateOverBasic        { get; set; }
    public string?  IsOthers                { get; set; }
    public string?  IsFixed                 { get; set; }
    public string?  TimedMode               { get; set; }
    public string?  ShortDesc               { get; set; }
    public string?  Show01                  { get; set; }
    public string?  Sort                    { get; set; }
    public string?  Special_                { get; set; }
    public string?  Show02                  { get; set; }
    public string?  DedSort                 { get; set; }
    public string?  IsTH                    { get; set; }
    public string?  Deferd                  { get; set; }
    public string?  IsOT                    { get; set; }
    public string?  IsMealAcct              { get; set; }
    public string?  Formula                 { get; set; }
    public string?  OTRate                  { get; set; }
    public string?  WithSSS                 { get; set; }
    public string?  WithPHIC                { get; set; }
    public string?  WithPagibig             { get; set; }
    public string?  IsGovAcct               { get; set; }
    public string?  IsLegalHoliday          { get; set; }
    public string?  IsExtLoan               { get; set; }
    public string?  ExtLoanPercentage       { get; set; }
    public string?  Status_                 { get; set; }
    public string?  CustomRate              { get; set; }
    public string?  Taxable_Type            { get; set; }
    public string?  MWE_Type                { get; set; }
    public string?  TaxExptAmt              { get; set; }
    public string?  Annualize               { get; set; }

    //-------------------------------------
    public bool IsSelectedB         { get; set; }
    public bool IsYTDAcctB          { get => IsYTDAcct    == "1"; set => IsYTDAcct    = value ? "1" : "0"; }
    public bool IsTaxExclB          { get => IsTaxExcl    == "1"; set => IsTaxExcl    = value ? "1" : "0"; }
    public bool IsLockB             { get => IsLock       == "1"; set => IsLock       = value ? "1" : "0";  }
    public bool IsChargeableB       { get => IsChargeable == "1"; set => IsChargeable = value ? "1" : "0";  }


}
