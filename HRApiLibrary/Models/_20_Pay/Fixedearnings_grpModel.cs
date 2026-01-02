namespace HRApiLibrary.Models._20_Pay;

public class Fixedearnings_grpModel
{
    public int          Id              { get; set; }
    public int          PayrollgrpId    { get; set; } = 0;
    public DateTime     DStart          { get; set; }
    public DateTime     DEnd            { get; set; }
    public string?      AcctNumber      { get; set; }
    public double       Amount          { get; set; }
    public int          CreatedbyId     { get; set; }
    public int          TerminatedbyId  { get; set; }
    public double       DaysPara        { get; set; }
    public string       Status          { get; set; } = "A"; 
    public int          P1              { get; set; }
    public int          P2              { get; set; }
    public int          P3              { get; set; }
    public int          P4              { get; set; }
    public int          P5              { get; set; }

    public int          PerdayEarnings  { get; set; }
    public string?      TRNPosted       { get; set; }
    //-----------------------------------------------------
    
    public bool             PerdayEarningsB  { get => PerdayEarnings == 1; set => PerdayEarnings = value ? 1 : 0; }
    public bool             P1B              { get => P1 == 1; set => P1 = value ? 1 : 0; }
    public bool             P2B              { get => P2 == 1; set => P2 = value ? 1 : 0; }
    public bool             P3B              { get => P3 == 1; set => P3 = value ? 1 : 0; }
    public bool             P4B              { get => P4 == 1; set => P4 = value ? 1 : 0; }
    public bool             P5B              { get => P5 == 1; set => P5 = value ? 1 : 0; }
    
    //-----------------------------------------------------------------
    public bool             Sel              { get; set; } = false;
    public string           AcctName         { get; set; } = string.Empty;
}
