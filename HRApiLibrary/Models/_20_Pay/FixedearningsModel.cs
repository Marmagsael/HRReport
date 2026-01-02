namespace HRApiLibrary.Models._20_Pay;

public class FixedearningsModel
{
    public int              Id               { get; set; }
    public int              Payrollgrpid     { get; set; }
    public string?          Empnumber        { get; set; }
    public DateTime         Dstart           { get; set; }
    public DateTime         Dend             { get; set; }
    public string?          Acctnumber       { get; set; } = "";
    public double           Amount           { get; set; }
    public int              IdSched          { get; set; }
    public string?          Createdby        { get; set; }
    public string?          Terminatedby     { get; set; }
    public double           Dayspara         { get; set; }
    public int              PerdayEarnings   { get; set; }
    public int              P1               { get; set; }
    public int              P2               { get; set; }
    public int              P3               { get; set; }
    public int              P4               { get; set; }
    public int              P5               { get; set; }
    public string?          Status           { get; set; } = "A";
    public string?          Trnposted        { get; set; }

    //-----------------------------------------------------
    public bool             PerdayEarningsB  { get => PerdayEarnings == 1; set => PerdayEarnings = value ? 1 : 0; }
    public bool             P1B              { get => P1 == 1; set => P1 = value ? 1 : 0; }
    public bool             P2B              { get => P2 == 1; set => P2 = value ? 1 : 0; }
    public bool             P3B              { get => P3 == 1; set => P3 = value ? 1 : 0; }
    public bool             P4B              { get => P4 == 1; set => P4 = value ? 1 : 0; }
    public bool             P5B              { get => P5 == 1; set => P5 = value ? 1 : 0; }

    public string           AcctName         { get; set; } = string.Empty; 
    public string           PayrollgrpName   { get; set; } = string.Empty; 
    

}

