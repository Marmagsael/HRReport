using System;
using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.Vars;

public class V12_102BodyPunch
{
    public OEmpmasModel?    Empmas              {get; set;} = new OEmpmasModel();
    public string?          CurrIn              {get; set;} = "-"; 
    public string?          CurrOut             {get; set;} = "-"; 
    public string?          PrevIn              {get; set;} = "00:00"; 
    public string?          PrevOut             {get; set;} = "00:00"; 
    public bool             CurrInDisabled      {get; set;} = false;
    public  bool            CurrOutDisabled     {get; set;} = false;
    public  bool            PrevInDisabled      {get; set;} = false; 
    public  bool            PrevOutDisabled     {get; set;} = false;
    public  string?         Msg                 {get; set;} = "Punch In for  : ";
    public  string?         MsgVal              {get; set;} = DateTime.Now.ToString("yyyy-MM-dd");

}
