using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._06._0600;

public class V0600
{
    public List<TbltranModel?>?             Tbltrans             { get; set; } = [];
    public List<TmptbltranemplistModel?>?   Tmptbltranemplists   { get; set; } = []; 
    public TmptbltranemplistModel?          FooterTotal          { get; set; } = new(); 
}