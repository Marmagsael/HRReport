namespace HRApiLibrary.Models._90_Utils;
public class MenusModel
{
    public int      Id              { get; set; } 
    public int      IdParent        { get; set; } 
    public int      Indent          { get; set; } 
    public string?  Type            { get; set; } 
    public string?  Code            { get; set; } 
    public string?  Icon1           { get; set; } 
    public string?  Icon2           { get; set; } 
    public string?  DispText        { get; set; }
    public int      IsWithDivider   { get; set; }
    public int      IsWithChild     { get; set; } 
    public int      IsSelected      { get; set; } 
    public string?  Controller      { get; set; } 
    public string?  Action          { get; set; } 
    public int      Odr             { get; set; }
    public int      IsPaid          { get; set; }
    public int      PaidType        { get; set; }
}