using System.Security.Permissions;

namespace HRApiLibrary.Models._20_Pay;

public class TbltranModel
{
    public string?      TRN             { get; set; }
    public int          EmpmasId        { get; set; }
    public string?      EmpNumber       { get; set; }
    public string?      AcctNumber      { get; set; }
    public double       Qty             { get; set; }
    public double       Rate            { get; set; }
    public int          RateTypeId      { get; set; }
    public double       Amount          { get; set; }
    public DateTime     DTimeStamp      { get; set; }
    public int          Postedby        { get; set; }
    public string?      Source          { get; set; } = "-";
    public int          RefId           { get; set; }
    
    //-----------------------------------------------------
    public string?      EmpName         { get; set; } = string.Empty;
    public string?      AcctName        { get; set; } = string.Empty;
    public string?      RateTypeName    { get; set; } = string.Empty;
    public string?      PostedByName    { get; set; } = string.Empty;
    public double       PrevQty         { get; set; }
    public string?      TrnType         { get; set; } = "-";
    public string?      TrnSource       { get; set; } = "-";
    public string?      Status          { get; set; } = "-";
    
}
