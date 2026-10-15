namespace HRApiLibrary.Models._20_Pay;

public class PaymaindtlModel
{
    public int?         Branchid            { get; set; } = 0; 
    public int?         Companyid           { get; set; } = 0;

    public int?         Empmasid            { get; set; } = 0;
    public string?      Empnumber           { get; set; } = string.Empty;
    public double?      Daywrk              { get; set; } = 0; 
    public double?      Ordinary            { get; set; } = 0; 
    public double?      Late                { get; set; } = 0; 
    public double?      Utime               { get; set; } = 0; 
    public double?      Absent              { get; set; } = 0; 
    public double?      Tardiness           { get; set; } = 0; 
    
    
    public double?      Rot                 { get; set; } = 0;
    public double?      Rd                  { get; set; } = 0;

    public double?      Custom1             { get; set; } = 0; 
    public double?      Custom2             { get; set; } = 0; 
    public double?      Custom3             { get; set; } = 0; 
    public double?      Dh                  { get; set; } = 0; 
    public double?      Dhot                { get; set; } = 0; 
    public double?      Lh                  { get; set; } = 0; 
    public double?      Lhot                { get; set; } = 0; 
    public double?      Nd                  { get; set; } = 0; 
    public double?      Ndlh                { get; set; } = 0; 
    public double?      Ndlhot              { get; set; } = 0; 
    public double?      Ndot                { get; set; } = 0; 
    public double?      Ndrd                { get; set; } = 0; 
    public double?      Ndrdlh              { get; set; } = 0; 
    public double?      Ndrdlhot            { get; set; } = 0; 
    public double?      Ndrdot              { get; set; } = 0; 
    public double?      Ndrdsh              { get; set; } = 0; 
    public double?      Ndrdshot            { get; set; } = 0; 
    public double?      Ndsh                { get; set; } = 0; 
    public double?      Ndshot              { get; set; } = 0; 
    public double?      Pagibig             { get; set; } = 0; 
    public double?      Pagibiger           { get; set; } = 0; 
    public string?      Paystatus           { get; set; } = string.Empty;
    public string?      Payrollgrpid        { get; set; }
    public double?      Phic                { get; set; } = 0;
    public double?      Phicer              { get; set; } = 0;
    public double?      Rddh                { get; set; } = 0;
    public double?      Rddhot              { get; set; } = 0;
    public double?      Rdlh                { get; set; } = 0;
    public double?      Rdlhot              { get; set; } = 0;
    public double?      Rdot                { get; set; } = 0;
    public double?      Rdsh                { get; set; } = 0;
    public double?      Rdshot              { get; set; } = 0;
    public double?      Rn                  { get; set; } = 0;
    public double?      Rnot                { get; set; } = 0;
    public double?      Sss                 { get; set; } = 0;
    public double?      Ssser               { get; set; } = 0;
    public double?      Sh                  { get; set; } = 0;
    public double?      Shot                { get; set; } = 0;
    public double?      Sssec               { get; set; } = 0;
    public string?      Trn                 { get; set; }
    public string?      Brid                { get; set; }
    public string?      Coid                { get; set; }
    public double?      Ctpa                { get; set; } = 0;
    public double?      Ctpalh              { get; set; } = 0;
    public double?      Ctpardlh            { get; set; } = 0;
    public double?      Sea                 { get; set; } = 0;
    public double?      Sealh               { get; set; } = 0;
    public double?      Seardlh             { get; set; } = 0;
    public string?      Secid               { get; set; }
    
    //----------------------------------------------------
    public string?      EmpName         { get; set; } = string.Empty;
    public bool?        IsSelected      { get; set; } = false;    
    
}