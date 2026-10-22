namespace HRApiLibrary.Models._10_Pis;

public class ObapplicationhistModel
{
    public int?         Id          { get; set; } = 0;
    public int?         ObaId       { get; set; } = 0;
    public int?         EmpmasId    { get; set; } = 0;
    public DateTime?    Date        { get; set; } = DateTime.Now;
    public string?      Action      { get; set; } = string.Empty;

    //--------------------------------------------------------
    public string?      EmpmasName  { get; set; } = string.Empty;
}
