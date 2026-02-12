using System;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.Vars;

public class V12_102BodyPunch
{
    public OEmpmasModel?    empmas              {get; set;} = new OEmpmasModel();
    public string?          currIn              {get; set;} = "-"; 
    public string?          currOut             {get; set;} = "-"; 
    public string?          prevIn              {get; set;} = "00:00"; 
    public string?          prevOut             {get; set;} = "00:00"; 
    public bool             currInDisabled      {get; set;} = false;
    public  bool            currOutDisabled     {get; set;} = false;
    public  bool            prevInDisabled      {get; set;} = false; 
    public  bool            prevOutDisabled     {get; set;} = false;
    public  string?         msg                 {get; set;} = "Punch In for  : ";
    public  string?         msgVal              {get; set;} = DateTime.Now.ToString("yyyy-MM-dd");

}
