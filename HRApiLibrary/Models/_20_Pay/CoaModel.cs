using System.Security.AccessControl;

namespace HRApiLibrary.Models._20_Pay;

public class CoaModel
{
    public string? AcctNumber            { get; set; } = "";
    public string? AcctName             { get; set; } = "";
    public string? AcctType             { get; set; } = ""; 
    public string?  ShortDesc           { get; set; } = string.Empty;
    public int      HasRateOverBasic    { get; set; }
    public double   RateOverBasic       { get; set; }
    public int      SortEarn            { get; set; }
    public int      SortDed             { get; set; }
    public int      IsLock              { get; set; }
    public int      IsSelected          { get; set; }

    //-------------------------------------
    public int      Show                { get; set; } = 0; 
    public bool     IsLocked            { get; set; } = false;
    public bool     Selected            { get; set; } = false;
    
    public bool     IsSelectedB         { get => IsSelected == 1; set => IsSelected = value ? 1 : 0; }
    public bool     IsLockB             { get => IsLock == 1; set => IsLock = value ? 1 : 0; }
}