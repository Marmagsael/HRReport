namespace HRApiLibrary.Models._10_Pis;

public class LeaveapproverModel
{
    public int Id                   { get; set; } =0;

    public int EmpmasId             { get; set; } = 0;

    public int ApproverId           { get; set; } = 0;

    public int ApproverLevel        { get; set; } =0;

    //---------------------------------------------
    public string?  EmpmasName              { get; set; } = string.Empty;
    public string?  ApproverName            { get; set; } = string.Empty;
    public string?  ApproverDesignation     { get; set; } = string.Empty;
    public bool     IsVisible               { get; set; } = true;
}
