namespace HRApiLibrary.Models._10_Pis;

public class DeviationModel
{
    public int           Id             { get; set; } = 0;
    public string?       Control_no     { get; set; } = string.Empty;
    public int           Prep_byid      { get; set; } = 0;
    public DateTime?     Prep_dt        { get; set; }
    public int           Coid           { get; set; } = 0;
    public string?       Empnumber      { get; set; } = string.Empty;
    public string?       Dev_no         { get; set; } = string.Empty;
    public DateTime      Occur_dt       { get; set; }
    public string?       Freq_no        { get; set; } = string.Empty;
    public string?       Penalty_no     { get; set; } = string.Empty;
    public int           Appr_byid      { get; set; } = 0;
    public DateTime      Appr_dt        { get; set; }
    public DateTime?      Devstart       { get; set; }
    public DateTime      Devend         { get; set; }

    //==============================================
    public string DevName { get; set; } = string.Empty;
    public string Penalty { get; set; } = string.Empty;

}