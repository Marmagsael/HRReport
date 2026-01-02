namespace HRApiLibrary.Models._20_Pay;

public class DedmandatoryModel
{
    public int          Id              { get; set; } = 0; 
    public string?      AcctNumber      { get; set; } = string.Empty; 
    public double       ContAmt         { get; set; } = 0; 
    public double       MaxAmt          { get; set; } = 0; 
    public string?      Remarks         { get; set; } = string.Empty; 
    public string?      Status          { get; set; } = "N";
    public int          P1              { get; set; } = 0; 
    public int          P2              { get; set; } = 0; 
    public int          P3              { get; set; } = 0; 
    public int          P4              { get; set; } = 0; 
    public int          P5              { get; set; } = 0; 
    
    //------------------------------------------------------
    public bool             P1B              { get => P1 == 1; set => P1 = value ? 1 : 0; }
    public bool             P2B              { get => P2 == 1; set => P2 = value ? 1 : 0; }
    public bool             P3B              { get => P3 == 1; set => P3 = value ? 1 : 0; }
    public bool             P4B              { get => P4 == 1; set => P4 = value ? 1 : 0; }
    public bool             P5B              { get => P5 == 1; set => P5 = value ? 1 : 0; }
    
    //------------------------------------------------------
    public string?      AcctName       { get; set; } = string.Empty; 
}