using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisReport.Vars;

public class V2202Model
{
    public string?              Msg             { get; set; } = "";
    public bool                 IsLoading       { get; set; } = true;
    public List<OClientModel>   Clients         { get; set; } = [];
    public List<R2202Model>     RepDtls         { get; set; } = [];
}

public class R2202Model
{
    public string?  ClNumber            { get; set; } = "";
    public string?  ClName              { get; set; } = "";
    public string?  EmpName             { get; set; } = "";
    public DateTime? DateStart          { get; set; }
    public DateTime? DateEnd            { get; set; }
    public int? LengthOfDuty            { get; set; } = 0;

}