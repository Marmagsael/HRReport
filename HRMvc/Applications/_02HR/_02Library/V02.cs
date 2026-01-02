using HRApiLibrary.Models._10_Pis;

namespace HRMvc.Applications._02HR._02Library;

public class V02
{
    public string?              Pisdb   = null;
    public string?              Paydb   = null;
    public string?              Conn    = null;

    public PissettingsModel?    Pissettings     { get; set; } = new(); 

    public string?              Action          { get; set; } = string.Empty;
    public int                  Selection       { get; set; } = 0;
    
}
