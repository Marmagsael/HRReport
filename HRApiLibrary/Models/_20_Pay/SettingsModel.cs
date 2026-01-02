namespace HRApiLibrary.Models._20_Pay;

public class SettingsModel
{
    
    public int          Id                      {get; set; } = 0; 
    public int          Yeartodays              {get; set; }  = 295;
    public int          Semiannualtodays        {get; set; } = 148;
    public int          Monthtodays             {get; set; } = 24;
    public int          SemiMonthtodays         {get; set; } = 12; 
    public int          DaysPerWeek             {get; set; } = 5; 
    public int          Daytohours              {get; set; } = 8; 
    public int          NdStart                 {get; set; } = 2200; 
    public int          NdEnd                   {get; set; } = 0600; 
    public string?      PayrollType             {get; set; } = "Bi-Monthly"; 
    public string?      TaxPeriodCode           {get; set; } = "SM"; 
    public int          AllowedMoPrd            {get; set; } = 3; 
    public string?      CoShortName             {get; set; } = string.Empty; 
    public string?      CoFullName              {get; set; } = string.Empty; 
    public string?      CoAddress               {get; set; } = string.Empty; 
    public string?      CoContactNos            {get; set; } = string.Empty; 
    public string?      RegNo                   {get; set; } = string.Empty; 
    public string?      Tin                     {get; set; } = string.Empty; 
    public string?      SssNo                   {get; set; } = string.Empty; 
    public string?      PhicNo                  {get; set; } = string.Empty; 
    public string?      PagibigNo               {get; set; } = string.Empty; 
    public string?      RevTax                  {get; set; } = string.Empty; 
    public string?      RevSss                  {get; set; } = string.Empty; 
    public string?      RevPhic                 {get; set; } = string.Empty; 
    public string?      RevPagibig              {get; set; } = string.Empty;
    public int          PremContSourceId        { get; set; } = 1; 
}