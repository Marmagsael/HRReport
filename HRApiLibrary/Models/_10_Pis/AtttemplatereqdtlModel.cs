using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRApiLibrary.Models._10_Pis
{
    public class AtttemplatereqdtlModel
    {
        public int?         Id                       { get; set; } = 0; 
        public int?         AtttemplateReqHdrId      { get; set; } = 0;
        public int?         EmpmasId                 { get; set; } = 0;
        public int?         AttendanceTypeId        { get; set; } = 1; 
        public int?         D1_In                   { get; set; } = 0; 
        public int?         D1_HrsLength            { get; set; } = 0; 
        public string?      D1_DutyType             { get; set; } = "RD";
        public int?         D2_In                   { get; set; } = 800; 
        public int?         D2_HrsLength            { get; set; } = 900; 
        public string?      D2_DutyType             { get; set; } = "R"; 
        public int?         D3_In                   { get; set; } = 800; 
        public int?         D3_HrsLength            { get; set; } = 900; 
        public string?      D3_DutyType             { get; set; } = "R"; 
        public int?         D4_In                   { get; set; } = 800; 
        public int?         D4_HrsLength            { get; set; } = 900; 
        public string?      D4_DutyType             { get; set; } = "R"; 
        public int?         D5_In                   { get; set; } = 800; 
        public int?         D5_HrsLength            { get; set; } = 900; 
        public string?      D5_DutyType             { get; set; } = "R"; 
        public int?         D6_In                   { get; set; } = 800; 
        public int?         D6_HrsLength            { get; set; } = 900; 
        public string?      D6_DutyType             { get; set; } = "R"; 
        public int?         D7_In                   { get; set; } = 0; 
        public int?         D7_HrsLength            { get; set; } = 0; 
        public string?      D7_DutyType             { get; set; } = "RN"; 

        //----------------------------------------------------------
        public DateTime?    Effectivity             { get; set; } = new DateTime(1901, 01, 01); 
        public DateTime?    EffectivityEnd          { get; set; } = new DateTime(9999, 12, 31, 23, 59, 29);
        public string?      AttendanceType          { get; set; } = string.Empty;
        public string?      DutyTypeName            { get; set; } = string.Empty;    
    }
    
}
