namespace HRApiLibrary.Models._20_Pay;

public class PayrollprdModel
{
    public  int             Id              {get; set; } = 0; 
    public  int             Yr              {get; set; } = DateTime.Now.Year; 
    public  string?         Mo              {get; set; } = DateTime.Now.Month.ToString("00"); 
    public  string?         Prd             {get; set; } = string.Empty; 
    public  int             Openby          {get; set; } = 0; 
    public  DateTime?       DateOpened      {get; set; } = DateTime.Now; 
    public  int             Closedby        {get; set; } = 0; 
    public  DateTime        DateClosed      {get; set; }  
    public  string?         Status          {get; set; } = string.Empty; 
    
    //----------------------------------------------------------------
    public string?          MoName          { get; set; } = string.Empty; 
    
}