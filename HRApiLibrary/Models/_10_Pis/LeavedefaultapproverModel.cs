namespace HRApiLibrary.Models._10_Pis;

public class LeavedefaultapproverModel
{
    public int          Id              { get; set; } = 0;
    public int          Lvl             { get; set; } = 0;
    public int          EmpmasId        { get; set; } = 0;
    public string?      Designation     { get; set; } = string.Empty;

    //-----------------------------------------------------------------
    public string?      Fullname        { get; set; }   = string.Empty;
    public bool?        IsVisible       { get; set; }   = true;

}