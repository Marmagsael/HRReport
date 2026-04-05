using HRApiLibrary.Models._10_Pis.OPis;
using System;

namespace HRMvc.Applications.PisReport.Vars;

public class V2104Model
{
    public string Action            { get; set; } = "";
    public bool IsGroupByClient     { get; set; } = false;
    public List<R2104Model> RepDtls { get; set; } = [];
}


public class R2104Model
{
    public string?      ClName      { get; set; }
    public string?      Client_      { get; set; }
    public string?      EmpNumber   { get; set; }
    public string?      EmpName     { get; set; }
    public DateTime?    EmpBirth    { get; set; }
    public string?      FullAge     { get; set; }
    public string?      EmpStatus   { get; set; }

    //------------------------------------
    public string? EmpFirstNm       { get; set; }
    public string? EmpLastNm        { get; set; }
    public Double? Age              { get; set; }


}
