namespace HRApiLibrary.Models._00_MainPis;

public class MailModel
{
    public int             Id                   { get; set; } = 0;
    public int             Usercompanyid        { get; set; } = 0;
    public int             Senderid             { get; set; } = 0;
    public string?         module               { get; set; } = string.Empty;
    public string?         msg                  { get; set; } = string.Empty;
    public string?         Link                 { get; set; } = string.Empty;
    public int             Isread               { get; set; } = 0;
    public DateTime        Datesent             { get; set; }
    public DateTime        Dateread             { get; set; }

    //------------------------------------------------------------------
    public bool             Selected            { get; set; } = false;
    public string?          CompanyName         { get; set; } = string.Empty;
    public string?          SenderName          { get; set; } = string.Empty;
    public bool             BIsread             { get; set; } = false;

}