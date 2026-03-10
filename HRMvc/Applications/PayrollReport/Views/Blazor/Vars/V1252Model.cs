using HRApiLibrary.Models._10_Pis.OPis;

namespace HRMvc.Applications.PisReport.Vars;

public class V1252Model
{
    public string           Msg         { get; set; } = ""; 

    public List<OEmpmasModel> Empmass   { get; set; } = []; 
    public List<R1252Model> RepDtls     { get; set; } = []; 
}

public class R1252Model
{
    public string       EmpNumber       { get; set; } = ""; 
    public string?  	EmpLastNm 		{ get; set; }
	public string?  	EmpFirstNm 		{ get; set; }
	public string?  	EmpMidNm 		{ get; set; }
	public string?  	Payrollgrp 		{ get; set; }
	public string?  	DateHired 		{ get; set; }
	public double?  	Ee 				{ get; set; }
	public double?  	Er 				{ get; set; }
	public double?  	Ec 				{ get; set; }
	public double?  	Compensation 	{ get; set; }   
    
}
