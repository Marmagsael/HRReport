using System;

namespace HRMvc.Applications.PisReport.Vars;

public class V2203Model
{
    public string? Action { get; set; } = "";
    public bool IsGroupByClient { get; set; } = false;
    public List<R2203Model> RepDtls { get; set; } = [];
}



public class R2203Model
{
    public string? EmpNumber                { get; set; } ="";
    public string? EmpName                  { get; set; } ="";
    public string? ClName                   { get; set; } ="";
    public string? Client_                  { get; set; } ="";
    public DateTime? Exp_Brgy               { get; set; } 
    public DateTime? Exp_Court              { get; set; }
    public DateTime? Exp_Drug               { get; set; }
    public DateTime? Exp_Nbi                { get; set; }
    public DateTime? Exp_Neuro              { get; set; }
    public DateTime? Exp_Pnp                { get; set; }
    public DateTime? Exp_Police             { get; set; }
    public DateTime? ExpMed                 { get; set; }

    //-------------------------------------------------------
    public string? EmpLastNm                { get; set; } = "";
    public string? EmpFirstNm               { get; set; } = "";




}
