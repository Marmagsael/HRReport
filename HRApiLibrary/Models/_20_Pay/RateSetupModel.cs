namespace HRApiLibrary.Models._20_Pay;

public class RateSetupGroupModel
{
    public int      Id              { get; set; }
    public string?  Code            { get; set; }
    public string?  ClNumber        { get; set; }
    public string?  Name            { get; set; }
    public string?  Status          { get; set; }
    public double   Rate            { get; set; }
    public double   MinRate         { get; set; }
    public double   Ecola           { get; set; }
    public double   Ctpa            { get; set; }
    public double   Sea             { get; set; }
    public int      EcolaRevised    { get; set; }
    public int      WithCtpa        { get; set; }
    public int      WithSea         { get; set; }
    public string?  SgCode          { get; set; }
    public double   SgDailyRate     { get; set; }
    public bool     Changed         { get; set; }
}

public class RateSetupEmployeeModel
{
    public string?  EmpNumber       { get; set; }
    public string?  Name            { get; set; }
    public string?  Position        { get; set; }
    public string?  Status          { get; set; }
    public double   BasicRate       { get; set; }
    public int      RateId          { get; set; } = 2;
    public double   AllowRate       { get; set; }
    public int      IsMaxSss        { get; set; }
    public string?  SgCode          { get; set; }
    public double   SgDailyRate     { get; set; }
    public int      IsTaxable       { get; set; }
    public int      WithSss         { get; set; }
    public int      WithGsis        { get; set; }
    public int      WithPagibig     { get; set; }
    public int      WithPhic        { get; set; }
    public bool     Changed         { get; set; }
}

public class RateSetupSalgradeModel
{
    public string?  SgCode          { get; set; }
    public string?  SGrade          { get; set; }
    public double   DailyRate       { get; set; }
}

public class RatesAmtModel
{
    public string?  ClNumber    { get; set; }
    public double   Rot         { get; set; }
    public double   Lh          { get; set; }
    public double   Lhot        { get; set; }
    public double   Sh          { get; set; }
    public double   Shot        { get; set; }
    public double   Dh          { get; set; }
    public double   Dhot        { get; set; }
    public double   Rd          { get; set; }
    public double   Rdot        { get; set; }
    public double   Rdlh        { get; set; }
    public double   Rdlhot      { get; set; }
    public double   Rdsh        { get; set; }
    public double   Rdshot      { get; set; }
    public double   Rddh        { get; set; }
    public double   Rddhot      { get; set; }
}
