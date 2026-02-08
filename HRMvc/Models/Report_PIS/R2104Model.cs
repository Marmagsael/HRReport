using System;

namespace HRMvc.Models.Report_PIS;

public class R2104Model
{
    public string?     EmpNumber    { get; set; }   = string.Empty; 
    public string?     EmpName      { get; set; }   = string.Empty; 
    public int?        ClName       { get; set; }   = 0; 
    public DateTime?   Dob          { get; set; }   = null; 
    public double?     Age          { get; set; }   = 0; 
}

