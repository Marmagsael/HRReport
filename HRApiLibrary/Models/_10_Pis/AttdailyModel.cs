using System;

namespace HRApiLibrary.Models._10_Pis;

public class AttdailyModel
{
    public int          Empmasid        { get; set; } = 0;
    public int          Dutytypeid      { get; set; } = 0;
    public string?      Empnumber       { get; set; } = string.Empty;
    public DateTime     Punchdate       { get; set; }
    public int          Dayno           { get; set; } = 0;
    public DateTime     Timein          { get; set; }
    public int          Timeint         { get; set; } = 0;
    public DateTime     Timeout         { get; set; }
    public int          Timeoutt        { get; set; } = 0;
    public int          Inbyid          { get; set; } = 0;
    public int          Outbyid         { get; set; } = 0;


    
}
