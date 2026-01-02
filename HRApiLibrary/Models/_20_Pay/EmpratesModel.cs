namespace HRApiLibrary.Models._20_Pay;

public class EmpratesModel
{
    public int          EmpmasId        { get; set; }
    public string?      EmpNumber       { get; set; }
    public int          PayrollgrpId    { get; set; }
    public bool         UsePaygrpRates  { get; set; } = true;
    public double       RatePerHr       { get; set; }
    public double       RatePerDay      { get; set; }
    public double       RatePerMonth    { get; set; }
    public double       RatePerYr       { get; set; }
    public double       EmpRate         { get; set; } = 0;
    public int          PayRateId       { get; set; } = 0;

    //-------------------------------------------------
    public int          SystemId        { get; set; } = 0;
    public string?      EmpLastNm       { get; set; } = string.Empty;
    public string?      EmpFirstNm      { get; set; } = string.Empty;
    public string?      EmpMidNm        { get; set; } = string.Empty;
    public string?      Suffix          { get; set; } = string.Empty;
    public string?      EmpAlias        { get; set; } = string.Empty;
    public string?      FullName        { get; set; } = string.Empty;
    public string?      EmpStatus       { get; set; } = string.Empty;

    //-------------------------------------------------
    public string?      PayrollGrpName  { get; set; } = string.Empty;
    public string?      Email           { get; set; } = string.Empty;


    //-------------------------------------------------
    public int          UserId          { get; set; } = 0;
    public int          EmpRateId       { get; set; } = 0;
    public string?      RateName        { get; set; } = string.Empty;

    //-------------------------------------
    public int          Show                { get; set; } = 0;
    public bool         IsSelected          { get; set; } = true;
    public bool         IsNewEntryVisible   { get; set; } = false;
    public string?      Css                 { get; set; } = string.Empty; 
}

public class EmpratesEmpCntPerPGModel
{
    public int      PayrollgrpId    { get; set; } = 0;
    public int      Count           { get; set; } = 0;
}
