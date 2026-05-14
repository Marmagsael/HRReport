namespace HRApiLibrary.Models._00_MainPis;

public class EmpmasModel
{
    public int Id               { get; set; }
    public int SystemId         { get; set; }
    public string? EmpLastNm    { get; set; } = string.Empty;
    public string? EmpFirstNm   { get; set; } = string.Empty;
    public string? EmpMidNm     { get; set; } = string.Empty;
    public string? Suffix       { get; set; } = string.Empty;
    public string? EmpAlias     { get; set; } = string.Empty;

    //----------------------------------------------------------------
    public string? EmpNumber    { get; set; } = string.Empty;

}
