namespace HRApiLibrary.Models._10_Pis;

public class EmploymenttypeModel
{
    public int      Id      { get; set; } = 0;
    public string?  Name    { get; set; } = string.Empty;
    public int      IsVisible           { get; set; } = 0;
    public int      ShowDeploymentEnd   { get; set; } = 0;
    public int      CanbeDeleted        { get; set; } = 0;

}

public class EmploymenttypeIsModel
{
    public int      Id                  { get; set; } = 0;
    public string?  Name                { get; set; } = string.Empty;
    public bool     IsVisible           { get; set; } = false;
    public bool     ShowDeploymentEnd   { get; set; } = false;
    public bool     CanbeDeleted        { get; set; } = false;

}