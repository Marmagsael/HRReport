namespace HRApiLibrary.Models._10_Pis;

public class LeavegrpapproverModel
{
    public int      Id                      { get; set; }
    public int      LeaveGrpId              { get; set; }
    public int      ApproverId              { get; set; }
    public int      ApproverLevel           { get; set; }

    //---------------------------------------------
    public string?  LeavegrpName            { get; set; } = string.Empty;
    public string?  ApproverName            { get; set; } = string.Empty;
    public string?  ApproverDesignation     { get; set; } = string.Empty;
    public bool     IsVisible               { get; set; } = true;
}