namespace HRApiLibrary.Models._10_Pis.OPis;

public class OEmpmasModel
{
    public string?   EmpNumber              { get; set; } = string.Empty;
    public string?   EmpLastNm              { get; set; }
    public string?   EmpFirstNm             { get; set; }
    public string?   EmpMidNm               { get; set; }
    public string?   Suffix                 { get; set; }
    public string?   EmpAlias               { get; set; }
    public string?   Client                 { get; set; }
    public string?   Client_                { get; set; }

    public double    BasicRate              { get; set; } = 0.0000;
    public int       PayType                { get; set; } = 0;
    public string?   Admin                  { get; set; }
    public double    CashBond               { get; set; } = 50.00;
    public double?   WorkDays               { get; set; }

    public double    AllowRate              { get; set; } = 0.00;
    public string?   AllowType              { get; set; }
    public string?   AllowFix               { get; set; }

    public double?   Allow2Rate             { get; set; }
    public string?   Allow2Type             { get; set; }
    public string?   Allow2Fix              { get; set; }

    public double?   Allow3Rate             { get; set; }
    public string?   Allow3Type             { get; set; }
    public string?   Allow3Fix              { get; set; }

    public double?   Allow4Rate             { get; set; }
    public string?   Allow4Type             { get; set; }
    public string?   Allow4Fix              { get; set; }

    public string?   MovNumber              { get; set; }
    public string?   MovMode                { get; set; }
    public DateTime? MovDate                { get; set; }
    public DateTime? MovEnd                 { get; set; }
    public DateTime? DutyDate               { get; set; }

    public string?   Addr1                  { get; set; }
    public string?   MlaCode_               { get; set; }
    public string?   Tel1                   { get; set; }

    public string?   Addr2                  { get; set; }
    public string?   ProCode_               { get; set; }
    public string?   Tel2                   { get; set; }

    public DateTime? EmpBirth               { get; set; }
    public string?   BirthPlace             { get; set; }
    public string?   Sex_                   { get; set; }
    public string?   CivStat_               { get; set; }
    public string?   Citizen                { get; set; }

    public double?   Height                 { get; set; }
    public double?   Weight                 { get; set; }

    public string?   Tin                    { get; set; }
    public string?   Sss                    { get; set; }
    public string?   Hdmf                   { get; set; }
    public string?   Religion               { get; set; }

    public string?   Hair                   { get; set; }
    public string?   Eyes                   { get; set; }
    public string?   Spouse                 { get; set; }
    public string?   Occupation             { get; set; }

    public double?   NoChildren             { get; set; }

    public DateTime? DateHired              { get; set; }
    public DateTime? Separate               { get; set; }

    public string?   Position_              { get; set; }
    public string?   EmpStat_               { get; set; }
    public DateTime? StatusDate             { get; set; }

    public string?   SecLicense             { get; set; }
    public DateTime? Licexpire              { get; set; }

    public string?   TrainAt                { get; set; }
    public DateTime? DateTrain              { get; set; }

    public string?   Insurance              { get; set; }
    public string?   PolicyNo               { get; set; }

    public double?   FaceValue              { get; set; }
    public double?   Premium                { get; set; }

    public DateTime? InsExpire              { get; set; }

    public string?   ExMilitary             { get; set; }
    public string?   Csp                    { get; set; }
    public string?   Cpp                    { get; set; }
    public string?   Rotc                   { get; set; }

    public string?   ElLevel                { get; set; }
    public string?   HsLevel                { get; set; }
    public string?   College_               { get; set; }

    public string?   Course                 { get; set; }
    public string?   VoLevel                { get; set; }
    public string?   VoCourse               { get; set; }

    public string?   Language               { get; set; }

    public string?   Skill1                 { get; set; }
    public string?   Skill2                 { get; set; }
    public string?   Skill3                 { get; set; }
    public string?   Skill4                 { get; set; }

    public string?   TaxCode                { get; set; }
    public string?   AcctCode               { get; set; } = string.Empty;

    public string?   Awol                   { get; set; }
    public string?   Dismiss                { get; set; }

    public DateTime? AStart                 { get; set; }
    public DateTime? AEnd                   { get; set; }
    public double?   ADays                  { get; set; }

    public DateTime? DStart                 { get; set; }
    public DateTime? DEnd                   { get; set; }
    public double?   DDays                  { get; set; }

    public string?   EmrName                { get; set; }
    public string?   EmrTel                 { get; set; }
    public string?   EmrAddr                { get; set; }

    public double?   GuardExp               { get; set; }

    public string?   ComTaxNo               { get; set; }
    public string?   ComTax_At              { get; set; }
    public DateTime? ComTaxDate             { get; set; }
    public string?   ComTaxAt               { get; set; }

    public string?   BloodType              { get; set; }
    public string?   Marks                  { get; set; }
    public string?   Complexion             { get; set; }

    public DateTime? Exp_Nbi                 { get; set; }
    public DateTime? Exp_Police              { get; set; }
    public DateTime? Exp_Pnp                 { get; set; }
    public DateTime? Exp_Brgy                { get; set; }
    public DateTime? Exp_Court               { get; set; }
    public DateTime? Exp_Neuro               { get; set; }
    public DateTime? Exp_Drug                { get; set; }

    public string?   W_BirthC                { get; set; }
    public string?   W_ClosingR              { get; set; }
    public string?   W_TrnCert               { get; set; }
    public string?   W_PreLic                { get; set; }
    public string?   W_CertEmp               { get; set; }
    public string?   W_MedExam               { get; set; }

    public double?   GkeRate                { get; set; }

    public string?   ClName                 { get; set; }
    public string?   MlaName                { get; set; }

    public string?   Age                    { get; set; }

    public string?   MBranch                { get; set; }
    public string?   MYear                  { get; set; }
    public string?   MNature                { get; set; }

    public string?   Remarks                { get; set; }
    public string?   BadgeNo                { get; set; }

    public string?   GuardNoYrs             { get; set; }
    public string?   MilitaryNoYr           { get; set; }

    public string?   PagIbigNo              { get; set; }
    public string?   Phic                   { get; set; }
    public string?   Bank                   { get; set; }

    public DateTime? ExpMed                 { get; set; }
    public DateTime? RegRef                 { get; set; }

    public double    EmpBasicRate           { get; set; } = 0.0000;
    public int       RateId                 { get; set; } = 2;
    public double    EmpEcola               { get; set; } = 0.0000;

    public int       XMark                  { get; set; } = 0;
    public double    SuretyBondQuota        { get; set; } = 0.00;

    public string?   Drv_License             { get; set; }
    public DateTime? Drv_Exp                 { get; set; }

    public int       IsTaxable              { get; set; } = 0;
    public int       IsConfi                { get; set; } = 0;

    public int?      IsWithSss              { get; set; } = 1;
    public int       IsWithGsis             { get; set; } = 0;
    public int?      IsWithPhic             { get; set; } = 1;
    public int       IsWithPagibig          { get; set; } = 1;

    public int?      IsMaxSss               { get; set; } = 0;

    public string?   Email                  { get; set; } = string.Empty;
    public string?   Passwd                 { get; set; } = string.Empty;

    public string?   CountryCode            { get; set; } = "PHL";
    public string?   SgCode                 { get; set; } = string.Empty;

    public DateTime? DpaDate                { get; set; }

    public string?   DpClient               { get; set; } = string.Empty;
    public string?   Desig_                 { get; set; } = "EMP";

    public int?      IsMigrated             { get; set; } = 0;
    
    
    //-------------------------------------------------------
    public int?     UserId                 { get; set; } = 0;
    public int?     EmpmasId                { get; set; } = 0;
    public string?  EmpName                 { get; set; } = string.Empty;
    public string?  Fullname                { get; set; } = string.Empty;
    public string?  PositionName            { get; set; } = string.Empty;
    public string?  EmpStatus               { get; set; } = string.Empty;
    public string?  Reason                  { get; set; }
    public int?     CntNotMigrated          { get; set; } = 0;
    public double?  HeightInches            { get; set; }
    public bool     Sel                     { get; set; } = false;


}
