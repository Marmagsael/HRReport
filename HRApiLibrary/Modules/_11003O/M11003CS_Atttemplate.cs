namespace HRApiLibrary.Modules._11003O;

public class M11003CS_Atttemplate
{
    public int      Empmasid                        { get; set; }
    public string?  Empnumber                       { get; set; }
    public int      Attendancetypeid                { get; set; }
    public int      Attschedweeklyhdrid             { get; set; }
    public int      Attschedweeklyhdridadv          { get; set; }
    public DateTime Changeschedeffectivity          { get; set; }
    public DateTime Changeschedend                  { get; set; }
    
    //--- Others -------------------------------------------------
    public bool     IsSelected                      { get; set; } = false; 
    public string? CurrentScheduleName              { get; set; }
    public string? AdvanceScheduleName              { get; set; }
    public string? EmpName                          { get; set; }
    
}

