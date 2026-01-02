using Google.Protobuf.WellKnownTypes;

namespace HRApiLibrary.Models._20_Pay;

public class TmptbltranemplistModel
{
    public string?  Trn             { get; set; }

    public int      EmpmasId        { get; set; }

    public double   Emprate         { get; set; }

    public int      PayrateId       { get; set; }
    public int      PayrollgrpId    { get; set; }

    public double   TotEarnings     { get; set; } = 0; 
    public double   TotDeductions   { get; set; } = 0; 
    public double   NetPay          { get; set; } = 0; 
    public double   TotHours        { get; set; } = 0; 
    
    //--------------------------------------------------
    public string? EmpNumber        { get; set; } = string.Empty; 
    public string? EmpName          { get; set; } = string.Empty; 
    public string? PayrateName      { get; set; } = string.Empty; 
    
    
}