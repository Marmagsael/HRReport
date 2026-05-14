namespace HRApiLibrary.Models._20_Pay;


public class FixedEarningsGrpModel
{
    public int              Id                               { get; set; } = 0;
    public string?          Clnumber                         { get; set; } = string.Empty;
    public DateTime         Dstart                           { get; set; }
    public DateTime         Dend                             { get; set; }
    public string?          Acctnumber                       { get; set; } = string.Empty;
    public double           Amount                           { get; set; } = 0;
    public string?          Createdby                        { get; set; } = string.Empty;
    public string?          Terminatedby                     { get; set; } = string.Empty;
    public double           Dayspara                         { get; set; } = 0;
    public int              P1                               { get; set; } = 0;
    public int              P2                               { get; set; } = 0;
    public int              P3                               { get; set; } = 0;
    public int              P4                               { get; set; } = 0;
    public int              P5                               { get; set; } = 0;
    public int              Perdayearnings                   { get; set; } = 0;
    public string?          Trnposted                        { get; set; } = string.Empty;
    
    public bool             PerdayearningsBol               { get => Perdayearnings == 1; set => Perdayearnings = value ? 1 : 0; } 
    public bool             P1Bool                          { get => P1 == 1; set => P1 = value ? 1 : 0;}
    public bool             P2Bool                          { get => P2 == 1; set => P2 = value ? 1 : 0;}
    public bool             P3Bool                          { get => P3 == 1; set => P3 = value ? 1 : 0;}
    public bool             P4Bool                          { get => P4 == 1; set => P4 = value ? 1 : 0;}
    public bool             P5Bool                          { get => P5 == 1; set => P5 = value ? 1 : 0;}

}