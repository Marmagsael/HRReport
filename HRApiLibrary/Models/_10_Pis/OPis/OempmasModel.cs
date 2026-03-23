namespace HRApiLibrary.Models._10_Pis.OPis;

public class OEmpmasModel
{
    public string? EmpNumber                { get; set; }
    public string? EmpLastNm                { get; set; }
    public string? EmpFirstNm               { get; set; }
    public string? EmpMidNm                 { get; set; }
    public string? Suffix                   { get; set; }
    public string? EmpAlias                 { get; set; }
    public string? Client_                  { get; set; }
    public string? ClientCode               { get; set; }
    public string? BasicRate                { get; set; }
    public string? PayType                  { get; set; }
    public string? Admin                    { get; set; }
    public string? CashBond                 { get; set; }
    public string? WorkDays                 { get; set; }
    public string? AllowRate                { get; set; }
    public string? AllowType                { get; set; }
    public string? AllowFix                 { get; set; }
    public string? Allow2Rate               { get; set; }
    public string? Allow2Type               { get; set; }
    public string? Allow2Fix                { get; set; }
    public string? Allow3Rate               { get; set; }
    public string? Allow3Type               { get; set; }
    public string? Allow3Fix                { get; set; }
    public string? Allow4Rate               { get; set; }
    public string? Allow4Type               { get; set; }
    public string? Allow4Fix                { get; set; }
    public string? MovementNumber           { get; set; }
    public string? MovementMode             { get; set; }
    public DateTime? MovDate                { get; set; }
    public DateTime? MovEnd                 { get; set; }
    public string? DutyDate                 { get; set; }
    public string? Address1                 { get; set; }
    public string? Address2                 { get; set; }
    public string? Tel1                     { get; set; }
    public string? Tel2                     { get; set; }
    public DateTime? EmpBirth               { get; set; }
    public string? BirthPlace               { get; set; }
    public string? Sex_                     { get; set; }
    public string? CivStat_                 { get; set; }
    public string? Citizenship              { get; set; }
    public string? Citizen                  { get; set; }
    public int? Height                      { get; set; }
    public int? HeightInches                { get; set; }
    public string? Weight                   { get; set; }
    public string? Tin                      { get; set; }
    public string? Sss                      { get; set; }
    public string? PagIbigNo                { get; set; }
    public string? Phic                     { get; set; }
    public string? Religion                 { get; set; }
    public string? Hair                     { get; set; }
    public string? Eyes                     { get; set; }
    public string? Complexion               { get; set; }
    public string? Marks                    { get; set; }
    public string? BloodType                { get; set; }
    public string? Spouse                   { get; set; }
    public string? Occupation               { get; set; }
    public string? NumberOfChildren         { get; set; }
    public string? NoChildren               { get; set; }
    public DateTime? DateHired              { get; set; }
    public DateTime? Separate               { get; set; }
    public string? Position_                { get; set; }
    public string? EmpStat_                 { get; set; }
    public DateTime? StatusDate               { get; set; }
    public string? SecLicense               { get; set; }
    public DateTime? Licexpire              { get; set; }
    public string? TrainingAt               { get; set; }
    public string? TrainingDate             { get; set; }
    public string? Insurance                { get; set; }
    public string? PolicyNo                 { get; set; }
    public double? FaceValue                { get; set; } = 0.00;
    public double? Premium                  { get; set; } = 0.00;
    public DateTime? InsExpire              { get; set; }
    public string? EmergencyContactName     { get; set; }
    public string? EmergencyContactTel      { get; set; }
    public string? EmergencyContactAddress  { get; set; }
    public string? Age                      { get; set; }
    public string? Remarks                  { get; set; }
    public string? BadgeNo                  { get; set; }
    public string? Bank                     { get; set; }
    public string? Email                    { get; set; }
    public string? Password                 { get; set; }
    public string? Drv_License              { get; set; }
    public DateTime? Drv_Exp                { get; set; }
    public string? IsTaxable                { get; set; }
    public string? IsConfidential           { get; set; }
    public string? IsWithSss                { get; set; }
    public string? IsWithGsis               { get; set; }
    public string? IsWithPhilHealth         { get; set; }
    public string? IsWithPagIbig            { get; set; }
    public string? MlaCode_                 { get; set; }
    public string? ProCode_                 { get; set; }

    public string? GuardNoYrs               { get; set; }
    public DateTime? RegRef                 { get; set; }
    public string? MilitaryNoYr             { get; set; }
    public string? AcctCode                 { get; set; }
    public string? TaxCode                  { get; set; }
    public DateTime? Exp_Nbi                { get; set; }
    public DateTime? Exp_Police             { get; set; }
    public DateTime? Exp_Pnp                { get; set; }
    public DateTime? Exp_Brgy               { get; set; }
    public DateTime? Exp_Neuro              { get; set; }
    public DateTime? Exp_Drug               { get; set; }
    public DateTime? Exp_Court              { get; set; }
    public DateTime? ExpMed                 { get; set; }

    //-------------------------------------------------------
    public string? EmpName                  { get; set; } = string.Empty;
    public string? PositionName             { get; set; } = string.Empty;
    public string? EmpStatus                { get; set; } = string.Empty;
    public string? ClName                   { get; set; } = string.Empty;
    
    public string? Addr1                    { get; set; }
    public string? Addr2                    { get; set; }
}
