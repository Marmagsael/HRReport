namespace HRApiLibrary.Models._00_Main;

public class SystemuserModel
{

    public int              SystemId            { get; set; } = 0;
    public string?          Status              { get; set; } = "FA";
    public DateTime         DateInvited         { get; set; }
    public DateTime         DateAccepted        { get; set; }
    public int              IsApproved          { get; set; } = 0;
    
    //----------------------------------------------------------
    public string?          UserName            { get; set; } = string.Empty; 
    public bool             BIsApproved         { get; set; } = false;


}
