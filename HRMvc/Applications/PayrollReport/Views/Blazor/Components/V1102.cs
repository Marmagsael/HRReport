using HRApiLibrary.Models._20_PayGeneric;

namespace HRMvc.Applications.PayrollReport.Views.Blazor.Components;

public class V1102
{
    public int?                         Yr              { get; set; } = DateTime.Now.Year;
    public int?                         Mo              { get; set; } = DateTime.Now.Month;
    public List<GChartofacctModel?>?    ChartOfAccts    { get; set; } = [];

}
