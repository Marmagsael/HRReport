using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications._03Payroll.Views.BlazorPages._06._0600;

public class V0601
{
    public  DateTime?                    Dstart             { get; set; } = null; 
    public  DateTime?                    Dend               { get; set; } = null;
    public  List<PayrollgrpModel?>?     Payrollgrps         { get; set; } = []; 
    public  PayrollgrpModel?            Payrollgrp          { get; set; } = new();
    public  PayrollgrpEntryPattern?     Payrollentry        { get; set; } = new();
    public  PaymaindtlsetupModel        Paymain             { get; set; } = new();
    
}

public class PayrollgrpEntryPattern
{
    public int          PayrollgrpId        { get; set; } = 0;
    public DateTime     PIndate             { get; set; }
    public int          PIn                 { get; set; }
    public DateTime     POutdate            { get; set; }
    public int          POut                { get; set; }
    
}

public class T601EmpList
{
    public int           EmpmasId     { get; set; } = 0;
    public string?       EmpNumber    { get; set; } = string.Empty;
    public string?       EmpName      { get; set; } = string.Empty;
    
    
}