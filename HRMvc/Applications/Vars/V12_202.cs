using System;
using HRApiLibrary.Models._20_Pay;

namespace HRMvc.Applications.Vars;

public class V12_202
{
    public string TRN                       { get; set; } = string.Empty;
    public string SSS                       { get; set; } = string.Empty;
    public string TIN                       { get; set; } = string.Empty;  
    public string Payrollgrp                { get; set; } = string.Empty;  
    public string AttCoverage               { get; set; } = string.Empty;  
    public double Rate                      { get; set; } = 0.0;  
    public List<TbltranModel?>? Tbltrans    { get; set; } = [];
}
