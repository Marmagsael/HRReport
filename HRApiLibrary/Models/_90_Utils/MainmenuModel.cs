namespace HRApiLibrary.Models._90_Utils;

public class MainmenuModel
{
    public int     Id         {get; set; } = 0; 
    public string? Type       {get; set; } = string.Empty;
    public int     IdParent   {get; set; } = 0; 
    public string? Indent     {get; set; } = string.Empty; 
    public string? Icon       {get; set; } = string.Empty; 
    public string? DispText   {get; set; } = string.Empty; 
    public string? Action     {get; set; } = string.Empty; 
    public int     Odr        {get; set; } = 0; 
}