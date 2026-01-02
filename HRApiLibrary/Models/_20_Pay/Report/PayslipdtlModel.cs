namespace HRApiLibrary.Models._20_Pay.Report;

public class PayslipdtlModel
{
    public string?      Trn                    { get; set; } = string.Empty;
    public int          EmpmasId               { get; set; } = 0;
    public string?      EmpNumber              { get; set; } = string.Empty;
    public string?      EmpName                { get; set; } = string.Empty;
    
    public string?      SssNo                  { get; set; } = string.Empty;
    public string?      TaxNo                  { get; set; } = string.Empty;
    public double       EmpRate                { get; set; } = 0; 
    
    public string?      DepCode                { get; set; } = string.Empty;
    public string?      DepName                { get; set; } = string.Empty;
    
    public double       YtdEarnings           { get; set; } = 0;
    public double       YtdDeduction          { get; set; } = 0;
    public double       YtdTax                { get; set; } = 0;
    
    public int          RateTypeId             { get; set; } = 0;
                                                      
    /*--- Earnings ------------------------------------------------*/ 
    public string?      EAcctNumber            { get; set; } = string.Empty;
    public string?      EAcctName              { get; set; } = string.Empty;
    public double       EQty                   { get; set; } = 0; 
    public double       ERate                  { get; set; } = 0;
    public double       EAmount                { get; set; } = 0;
    public string?      ESource                { get; set; } = "-";
    public int          ERefId                 { get; set; } = 0;
    
    /*--- Deductions ------------------------------------------------*/
    public string?      DAcctNumber            { get; set; }  = string.Empty;
    public string?      DAcctName              { get; set; }  = string.Empty;
    public double       DQty                   { get; set; }  = 0;
    public double       DRate                  { get; set; }  = 0;
    public double       DAmount                { get; set; }  = 0;
    public string?      DSource                { get; set; }  = "-";
    public int          DRefId                 { get; set; }  = 0;
}

public class PayslipdtlQueryModel
{
    public string?      Trn                   { get; set; } = string.Empty;
    public int          EmpmasId              { get; set; } = 0;
    public string?      EmpNumber             { get; set; } = string.Empty;
    public string?      EmpName               { get; set; } = string.Empty;
    
    public string?      SssNo                 { get; set; } = string.Empty;
    public string?      TaxNo                 { get; set; } = string.Empty;
    public double       EmpRate               { get; set; } = 0; 
    public int          RateTypeId            { get; set; } = 0;

    public string?      DepCode               { get; set; } = string.Empty;
    public string?      DepName               { get; set; } = string.Empty;
    
    public double       YtdEarnings           { get; set; } = 0;
    public double       YtdDeduction          { get; set; } = 0;
    public double       YtdTax                { get; set; } = 0;
    
                                                      
    /*--- Earnings ------------------------------------------------*/ 
    public string?      AcctNumber            { get; set; } = string.Empty;
    public string?      AcctName              { get; set; } = string.Empty;
    public double       Qty                   { get; set; } = 0; 
    public double       Rate                  { get; set; } = 0;
    public double       Amount                { get; set; } = 0;
    public string?      Source                { get; set; } = "-";
    public int          RefId                 { get; set; } = 0;
    
}
